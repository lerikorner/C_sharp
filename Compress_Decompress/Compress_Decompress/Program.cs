using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading; 

namespace Compress_Decompress
{
   class Program
   {
        
        static int threadNumber = Environment.ProcessorCount;

        static byte[][] dataArray = new byte[threadNumber][];
        static byte[][] compressedDataArray = new byte[threadNumber][];

        static int dataPortionSize = 1000000; // размер считываемого блока
        static int dataArraySize = dataPortionSize * threadNumber;

        static public void Compress(string inFileName)
        {

            FileStream inFile = new FileStream(inFileName, FileMode.Open); //конструктор открытия входного файла
            FileStream outFile = new FileStream(inFileName + ".gz", FileMode.Append); //конструктор модификации выходного файла
            int _dataPortionSize;
            Thread[] tPool;
            Console.Write("Compressing...");
            while (inFile.Position < inFile.Length)
            {
                Console.Write(".");
                tPool = new Thread[threadNumber];

                //читаем файл по блокам
                for (int portionCount = 0; (portionCount < threadNumber) && (inFile.Position < inFile.Length); portionCount++)
                {
                    //запись считываем остаток целочисленного деления размера файла на размер блока в конце цикла
                    if (inFile.Length - inFile.Position <= dataPortionSize)
                    {
                        _dataPortionSize = (int)(inFile.Length - inFile.Position); 
                    }
                    else
                    {
                        _dataPortionSize = dataPortionSize;
                    }
                    dataArray[portionCount] = new byte[_dataPortionSize];
                    inFile.Read(dataArray[portionCount], 0, _dataPortionSize);
                    //раскидываем действия на потоки, пишем в них блоки
                    tPool[portionCount] = new Thread(CompressBlock);
                    tPool[portionCount].Start(portionCount);
                }
                //пишем блоки в выходной файл
                for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);)
                {
                    if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                    {
                        outFile.Write(compressedDataArray[portionCount], 0, compressedDataArray[portionCount].Length);
                        portionCount++;
                    }
                }
            }
            //...
            outFile.Close();
            inFile.Close();
        }
        //сжатие блока
        static public void CompressBlock(object i)
        {
            using (MemoryStream output = new MemoryStream(dataArray[(int)i].Length)) //конструктор вывода данных из памяти
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress)) //конструктор сжатия
                {
                    cs.Write(dataArray[(int)i], 0, dataArray[(int)i].Length); //пишем все выведенное в блок
                }
                compressedDataArray[(int)i] = output.ToArray();
            }
        }
        public static void Decompress(string inFileName)
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    string dir = Path.GetDirectoryName(inFileName);
                    string decompressionFileName = dir + Path.GetFileNameWithoutExtension(inFileName) + "_decompressed";
                    Console.Write("processing...");
                    int BufferSize = 8192;
                    using (FileStream outStream = new FileStream(inFileName, FileMode.Create, FileAccess.Write))
                    {
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                        {
                            outStream.Write(buffer, 0, read);
                        }
                        outStream.Close();
                    }
                    decomp.Close();
                }
                inFile.Close();
            }
        }
        public static void Main(string[] args)
        {
         /*   Console.CancelKeyPress += delegate {
                // call methods to clean up
                Console.WriteLine("PRESS ANY KEY TO EXIT");
                Console.ReadKey();
            };

            while (true)
            {

            }*/
            string fileNameIN = "D:/test.jpg";
            string fileNameOUT = "D:/myfile.txt.gz";

            // Compress(fileNameIN);
            // Extract(fileNameOUT);
            Decompress(fileNameOUT);
            Console.ReadKey();
        }
    }
}
