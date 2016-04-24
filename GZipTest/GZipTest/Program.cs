using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Compress_Decompress
{

    class GZipTest
    {
        static int BufferSize = 1024 * 1024;   //!!!!!!
        public static bool error;

        const int multithread = 4;//количество потоков
                                  /**       private static void WriteBlock(GZipStream inStream, int read, byte[] buffer) //запись в архив из входного блока
                                         {
                                             Console.Write('-');
                                             inStream.Write(buffer, 0, read);
                                         }*/
        private static void ProcessBlock(FileStream outStream, int read, byte[] buffer)//запись архивного блока в выходной файл
        {
            Console.Write('-');
            outStream.Write(buffer, 0, read);
        }

        private static void WriteBlockCompressed(FileStream inStream, int read, byte[] buffer)//запись архивного блока в выходной файл
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }

        public static void Compress(string inFileName, string outFileName)//путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))//инициализация входного файла
                {
                    using (FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))//инициализация выходного файла
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        using (GZipStream inGZip = new GZipStream(outFile, CompressionMode.Compress))//буферный поток сжатия
                        {
                            int i = 0, j = 0;
                            int[] read = new int[multithread + 1];
                            byte[] buffer = new byte[BufferSize];
                            Thread[] thread = new Thread[multithread + 1];
                            Thread[] thread_ = new Thread[multithread + 1];

                            while ((inFile.Length - inFile.Position) > BufferSize)
                            {
                                for (j = 0; (j < multithread) && ((inFile.Length - inFile.Position) > BufferSize); j++)
                                {
                                    thread[j] = new Thread(() =>
                                    {
                                        read[j] = inFile.Read(buffer, 0, BufferSize);
                                        Console.Write("- {0} -", j);
                                    });
                                    thread[j].Start();
                                    thread_[j] = new Thread(() =>
                                    {
                                        inGZip.Write(buffer, j*BufferSize, read[j]);
                                        Console.Write("|");
                                    });
                                    thread_[j].Start();
                                    thread_[j].Join();

                                }
                                for (i = 0; i < j; i++)
                                {
                                    thread[i].Join();
                                }

                            }
                            inGZip.Write(buffer, 0, inFile.Read(buffer, 0, BufferSize));
                            inGZip.Close();  //!!!!!!
                        }
                        outFile.Close();

                    }
                    inFile.Close();
                    error = false;
                    Console.WriteLine(" finished packing. ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
            }
        }

        public static void Decompress(string inFileName, string outFileName) //путь к входному файлу, путь к выходному файлу
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))//инициализация входного файла
                {
                    using (FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        using (GZipStream outGZip = new GZipStream(inFile, CompressionMode.Decompress))     //инициализация буфера распаковки
                        {
                            int i = 0, j = 0;
                            int[] read = new int[multithread + 1];
                            byte[] buffer = new byte[BufferSize];
                            Thread[] thread = new Thread[multithread + 1];
                            Thread[] thread_ = new Thread[multithread + 1];
                            while ((inFile.Length - inFile.Position) > BufferSize)
                            {
                                for (j = 0; (j < multithread) && ((inFile.Length - inFile.Position) > BufferSize); j++)
                                {
                                    thread[j] = new Thread(() =>
                                    {
                                        read[j] = outGZip.Read(buffer, 0, BufferSize);
                                        Console.Write("- {0} -", j);
                                    });
                                    thread[j].Start();
                                    thread[j].Join();
                                    thread_[j] = new Thread(() =>
                                    {
                                        outFile.Write(buffer, 0, read[j]);
                                        Console.Write("|");
                                    });
                                    thread_[j].Start();
                                    thread_[j].Join();

                                }
                       //         for (i = 0; i < j; i++)
                         //       {
                           //     }
                            }
                            outFile.Write(buffer, 0, outGZip.Read(buffer, 0, BufferSize));
                            outGZip.Close();
                        }
                        outFile.Close();
                    }
                    inFile.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
            }
        }
    }
    class Program
    {
        static string GZip, fIN, fOUT;
        static bool flag = true;
        public static void Main(string[] args)//параметры для командной строки должны быть закомментированы для отладки!!!!
        {
            DateTime dold = DateTime.Now;
            /*   try
               {
                   GZip = args[0]; //во время работы с консолью  раскомментировать GZip, fIN, fOUT!!!!
                   fIN = args[1];
                   fOUT = args[2];
               }
               catch (Exception ex)
               {
                   Console.WriteLine("ERROR: " + ex.Message+"|| Неверно введены параметры.");
                   error = true;
               }*/

            try
            {
                Console.CancelKeyPress += delegate //ловим ctrl+c
                {
                    GC.Collect(); //чистим мусор
                    flag = false;
                };
                while ((true) & (flag))
                {
                    GZip = "decompress";          //
                    fIN = "d:/test_comp.avi.gz";        //тестовые значения для отладки!!!!!
                    fOUT = "d:/test_decomp.avi";    //
                    if (GZip == "compress")
                    {
                        Console.WriteLine("packing: ");
                        GZipTest.Compress(fIN, fOUT);
                    }
                    else if (GZip == "decompress")
                    {
                        Console.Write("unpacking: ");
                        GZipTest.Decompress(fIN, fOUT);
                    }
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                GZipTest.error = true;
            }
            TimeSpan sp = DateTime.Now - dold;
            Console.WriteLine("press Enter to terminate! CODE {0}", GZipTest.error ? "1" : "0");
            Console.WriteLine("completed in {0} secs", sp);
            Console.ReadLine();

        }
     
    }
}
