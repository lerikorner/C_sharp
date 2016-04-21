using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;  //подключаем нужные библиотеки

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
        //    try
       //     {
                FileStream inFile = new FileStream(inFileName, FileMode.Open);
                FileStream outFile = new FileStream(inFileName.Remove(inFileName.Length - 3), FileMode.Append);
                int _dataPortionSize;
                int compressedBlockLength;
                Thread[] tPool;
                Console.Write("Decompressing...");
                byte[] buffer = new byte[2];

                while (inFile.Position < inFile.Length)
                {
                    Console.Write(".");
                    tPool = new Thread[threadNumber];
                    for (int portionCount = 0; (portionCount < threadNumber) && (inFile.Position < inFile.Length); portionCount++)
                    {
                        inFile.Read(buffer, 0, 2);
                        compressedBlockLength = BitConverter.ToInt32(buffer, 2);
                        Console.WriteLine(compressedBlockLength);
                        Console.WriteLine(buffer.Length);
                        Console.WriteLine(portionCount);
                        byte[][] compressedDataArray = new byte[2][];
                        buffer.CopyTo(compressedDataArray[portionCount], 0);

                        inFile.Read(compressedDataArray[portionCount], 8, compressedBlockLength - 8);
                        _dataPortionSize = BitConverter.ToInt32(compressedDataArray[portionCount], compressedBlockLength - 4);
                        dataArray[portionCount] = new byte[_dataPortionSize];

                        tPool[portionCount] = new Thread(DecompressBlock);
                        tPool[portionCount].Start(portionCount);
                    }

                    for (int portionCount = 0; (portionCount < threadNumber) && (tPool[portionCount] != null);)
                    {
                        if (tPool[portionCount].ThreadState == ThreadState.Stopped)
                        {
                            outFile.Write(dataArray[portionCount], 0, dataArray[portionCount].Length);
                            portionCount++;
                        }
                    }
                }

                outFile.Close();
                inFile.Close();
         //   }
        /*    catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
            }*/
        }

        public static void DecompressBlock(object i)
        {
            using (MemoryStream input = new MemoryStream(compressedDataArray[(int)i]))
            {

                using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
                {
                    ds.Read(dataArray[(int)i], 0, dataArray[(int)i].Length);
                }

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
        //    string fileNameIN = "D:/metal.mkv";
            string fileNameOUT = "D:/test.jpg.gz";

            //  Compress(fileName);
            Decompress(fileNameOUT);
            Console.ReadKey();
        }
    }
}
