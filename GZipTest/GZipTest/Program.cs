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
    class GZipTest
    {
        static int BufferSize = 32768;
        const  int multithread= 2;//количество потоков
        static public void WriteBlock(GZipStream inStream,int read, byte[] buffer)
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }

        public static void WriteBlockCompressed(FileStream inStream, int read, byte[] buffer)//запись блока
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }


        public static void Compress(string inFileName, string outFileName, bool error)//путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))//инициализация входного файла
                {
                    using (FileStream comp = new FileStream(outFileName, FileMode.Create,FileAccess.Write))//инициализация выходного файла
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                    //    MemoryStream mstream = new MemoryStream(buffer);
                        GZipStream inStream = new GZipStream(comp, CompressionMode.Compress);//буферный поток сжатия
                        {
                            while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                            {
                                Thread[] thread = new Thread[multithread];
                                for (int td=0; td < multithread; td++)
                                {
                                    thread[td] = new Thread(delegate () { WriteBlock(inStream, read, buffer); });
                                    thread[td].Start();
                                    thread[td].Join();
                                }

                                //  WriteBlock(inStream, read, buffer);
                            }
                            inStream.Close();
                        }
                        comp.Close();
                    }
                    inFile.Close(); 
                }
                error = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
                Console.Write("press CTRL+C to terminate. CODE {0}", error ? "1" : "0");

            }
        }

        public static void Decompress(string inFileName, string outFileName, bool error) //путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))//инициализация входного файла
                {
                    using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))//инициализация буфера распаковки
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        using (FileStream outStream = new FileStream(outFileName, FileMode.Create, FileAccess.Write))//инициализация выходного файла
                        {
                            int read = 0;
                            byte[] buffer = new byte[BufferSize];
                            while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                            {
                                WriteBlockCompressed(outStream, read, buffer);
                            }
                            outStream.Close();
                        }
                        decomp.Close();
                    }
                    inFile.Close();
                }
                error = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
                Console.Write("press CTRL+C to terminate. CODE {0}", error ? "1" : "0");
            }
        }
    
    }

    class Program
    {

        static string GZip, fIN, fOUT;
        static bool error;

        public static void Main(string[] args)//параметры для командной строки закомментированы для отладки!!!!
        {
            try
            {
                //GZip = args[0]; во время работы с консолью и раскомментировать fIN, fOUT!!!!
                //fIN = args[1];
                //fOUT = args[2];
               /* Console.CancelKeyPress += delegate //ловим ctrl+c
                {
                    GC.Collect(); //чистим мусор
                };
                Console.Write("press CTRL+C to terminate!!!! CODE {0}", error ? "1" : "0");
                Console.ReadLine();*/

             //   while (true)
                {
                    GZip = "compress";              //
                    fIN = "d:/test.avi"; //тестовые значения для отладки!!!!!
                    fOUT = "d:/test.avi.gz";    //
                    if (GZip == "compress")   
                    {
                        Console.WriteLine("packing: ");
                        GZipTest.Compress(fIN, fOUT, error);
                        Console.WriteLine(" finished packing. ");

                    }
                    else if (GZip == "decompress")
                    {
                        Console.Write("unpacking: ");
                        GZipTest.Decompress(fIN, fOUT, error);
                        Console.WriteLine(" finished unpacking. ");

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            Console.ReadLine();
        }
    }
}
/*
namespace Compress_Decompress
{
    class GZipTest
    {
        static int BufferSize = 32768;
        const int multithread = 2;//количество потоков
        static public void WriteBlock(GZipStream inStream, int read, byte[] buffer)
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }

        public static void WriteBlockCompressed(FileStream inStream, int read, byte[] buffer)//запись блока
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }


        public static void Compress(string inFileName, string outFileName, bool error)//путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))//инициализация входного файла
                {
                    using (FileStream comp = new FileStream(outFileName, FileMode.Create, FileAccess.Write))//инициализация выходного файла
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        //    MemoryStream mstream = new MemoryStream(buffer);
                        GZipStream inStream = new GZipStream(comp, CompressionMode.Compress);//буферный поток сжатия
                        {
                            while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                            {
                                WriteBlock(inStream, read, buffer);
                            }
                            inStream.Close();
                        }
                        comp.Close();
                    }
                    inFile.Close();
                }
                error = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
                Console.Write("press CTRL+C to terminate. CODE {0}", error ? "1" : "0");

            }
        }

        public static void Decompress(string inFileName, string outFileName, bool error) //путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))//инициализация входного файла
                {
                    using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))//инициализация буфера распаковки
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        using (FileStream outStream = new FileStream(outFileName, FileMode.Create, FileAccess.Write))//инициализация выходного файла
                        {
                            int read = 0;
                            byte[] buffer = new byte[BufferSize];
                            while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                            {
                                WriteBlockCompressed(outStream, read, buffer);
                            }
                            outStream.Close();
                        }
                        decomp.Close();
                    }
                    inFile.Close();
                }
                error = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
                Console.Write("press CTRL+C to terminate. CODE {0}", error ? "1" : "0");
            }
        }

    }

    class Program
    {

        static string GZip, fIN, fOUT;
        static bool error;

        public static void Main(string[] args)//параметры для командной строки закомментированы для отладки!!!!
        {
            try
            {
                //GZip = args[0]; во время работы с консолью и раскомментировать fIN, fOUT!!!!
                //fIN = args[1];
                //fOUT = args[2];
                /* Console.CancelKeyPress += delegate //ловим ctrl+c
                 {
                     GC.Collect(); //чистим мусор
                 };
                 Console.Write("press CTRL+C to terminate!!!! CODE {0}", error ? "1" : "0");
                 Console.ReadLine();

                //   while (true)
                {
                    GZip = "compress";              //
                    fIN = "d:/rails/movie.rar"; //тестовые значения для отладки!!!!!
                    fOUT = "d:/movie.rar.gz";    //
                    if (GZip == "compress")
                    {
                        Console.WriteLine("packing: ");
                        GZipTest.Compress(fIN, fOUT, error);
                        Console.WriteLine(" finished packing. ");

                    }
                    else if (GZip == "decompress")
                    {
                        Console.Write("unpacking: ");
                        GZipTest.Decompress(fIN, fOUT, error);
                        Console.WriteLine(" finished unpacking. ");

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            Console.ReadLine();
        }
    }
}
*/