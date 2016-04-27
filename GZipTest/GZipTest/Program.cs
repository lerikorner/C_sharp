using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Compress_Decompress
{

    class GZipTest
    {
        public static int BufferSize = 2048 * 2048;  //!!!!!!
        public static bool error;
        public static  int multithread = Environment.ProcessorCount;//количество потоков
     
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
                            int read, i = 0, j = 0;
                           // int[] read = new int[multithread];
                            byte[] buffer = new byte[BufferSize];
                            Thread[] thread = new Thread[multithread];
                            object block = new object();
                            while ((inFile.Length - inFile.Position) > BufferSize)
                            {                                   
                                for (j = 0; (j < multithread-1) && 
                                    (inFile.Length - inFile.Position > BufferSize)&&
                                    (inFile.Position<FileModify.GZipLimit); j++)
                                {
                                 //   read = inFile.Read(buffer, 0, BufferSize);

                                    thread[j] = new Thread(()=>
                                    {
                                        lock(block)
                                        {
                                            Console.Write("- {0} -", j);
                                            read = inFile.Read(buffer, 0, BufferSize);
                                            inGZip.Write(buffer, 0, read);
                                        }
                                    });                                  
                                    thread[j].Start();
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
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))//инициализация входного файла
                {
                    using (GZipStream outGZip = new GZipStream(inFile,CompressionMode.Decompress))
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        using (FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))     //инициализация буфера распаковки
                        {
                            int read = 0, i = 0, j = 0;
                            //  int[] read = new int[multithread];
                            byte[] buffer = new byte[BufferSize];
                            Thread[] thread = new Thread[multithread];
                            object block = new object();
                            while ((inFile.Length - inFile.Position) > BufferSize)
                            {
                                for (j = 0; (j < multithread-1) && ((inFile.Length - inFile.Position) > BufferSize); j++)
                                {
                                    read = outGZip.Read(buffer, 0, BufferSize);
                                    thread[j] = new Thread(() =>
                                    {
                                        lock (block)

                                        {
                                            Console.Write("- {0} -", j);
                                            outFile.Write(buffer, 0, read);
                                        }

                                    });
                                    thread[j].Start();

                                    //    thread[j].Join();
                                }
                                for (i = 0; i < j; i++)
                                {
                                    thread[i].Join();
                                }
                            }
                            outFile.Write(buffer, 0, outGZip.Read(buffer, 0, BufferSize));
                            outFile.Close();
                        }
                        outGZip.Close();
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
    class FileModify
    {
        // public static long GZipLimit = 4294967296; // 4*2^30
         public static long GZipLimit = 30000000;

        public static void SplitFile(string infile)
        {
            int read;
            var extension = Path.GetExtension(infile);
            var name = Path.GetFileNameWithoutExtension(infile);
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            int tail =Convert.ToInt32( GZipLimit % GZipTest.BufferSize);
            using (FileStream _from_stream = new FileStream(infile, FileMode.Open))
            {
                long _file_count = _from_stream.Length /GZipLimit;
                for (int i = 0; i < _file_count+1; i++)
                {
                    using (FileStream _to_stream = new FileStream(string.Format(directoryName+"boof1"+"_{0}"+extension, i), 
                                                                  FileMode.Create,FileAccess.Write))
                    {
                        Console.WriteLine("{0}й том для архивации создается...", i);
                        _from_stream.Position = i * GZipLimit;
                        while ((_to_stream.Length<GZipLimit)&&(_from_stream.Position<_from_stream.Length))
                        {
                            read = _from_stream.Read(buffer, 0, GZipTest.BufferSize);
                            if ((GZipLimit - _to_stream.Position < GZipTest.BufferSize)&&(i!=_file_count))
                            {
                                read = _from_stream.Read(buffer, 0, tail);
                            }
                            _to_stream.Write(buffer, 0, read);
                        }
                        _to_stream.Close();
                    }
                }
                _from_stream.Close();
            }          
        }
        public static void MergeFile(string infile)
        {
            int read;
            var extension = Path.GetExtension(infile);
            var name = Path.GetFileNameWithoutExtension(infile);
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            using (FileStream _to_stream = new FileStream(infile, FileMode.Append,FileAccess.Write))
            {
                for (int i = 0; i < 4; i++)
                {
                    using (FileStream _from_stream = new FileStream(string.Format(directoryName + "boof1" + "_{0}" + ".avi"+".gz", i),
                                                                  FileMode.Open, FileAccess.Read))
                    {
                        //_from_stream.Position = i * GZipLimit;
                     //   _to_stream.Position = i * GZipLimit;
                        while (((read = _from_stream.Read(buffer, 0, GZipTest.BufferSize)) != 0))
                        {
                            _to_stream.Write(buffer, 0, read);
                        }
                        _from_stream.Close();
                    }
                    Console.WriteLine("{0}й том соединен.", i);
                }
                 _to_stream.Close();
            }
        }
      
    }
    class FileProcess
    {
        static public void FileCompress(string infileName, string outfileName)
        {
            FileStream infile = new FileStream(infileName, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(infileName);
            var name = Path.GetFileNameWithoutExtension(infileName);
            var directoryName = Path.GetDirectoryName(infileName);
            int count = Convert.ToInt32(infile.Length / FileModify.GZipLimit);
            long length = infile.Length;
            infile.Close();
            if (length>FileModify.GZipLimit)
            {
                FileModify.SplitFile(infileName);
                for (int i=0;i< count+1; i++)
                {
                    string inFrag = string.Format(directoryName + "boof1" + "_{0}" + extension, i);
                    GZipTest.Compress(inFrag, inFrag + ".gz");
                }
                FileModify.MergeFile(outfileName);
        }
    }
        class Program
        {
            static string GZip, fIN, fOUT;
            static bool flag = true;
            public static void Main(string[] args)//параметры для командной строки должны быть закомментированы для отладки!!!!
            {
                DateTime dold = DateTime.Now;
                /*        try
                        {
                            GZip = args[0]; //во время работы с консолью  раскомментировать GZip, fIN, fOUT!!!!
                            fIN = args[1];
                            fOUT = args[2];
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR: " + ex.Message+"|| Неверно введены параметры.");
                            GZipTest.error = true;
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
                        GZip = "compress";
                        fIN = string.Format("d:/test_merge.avi");
                        fOUT = string.Format("d:/test_merge.avi.gz");
                        if (GZip == "compress")
                        {
                            FileProcess.FileCompress(fIN, fOUT);
                            Console.WriteLine("packing: ");

                        }
                        else if (GZip == "decompress")
                        {
                            Console.Write("unpacking: ");
                            GZipTest.Decompress(fIN, fOUT);
                        }
                        flag = false;
                        GC.Collect(); //чистим мусор
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    GZipTest.error = true;
                }
                //    FileModify.SplitFile("d:/test_decomp_3.avi");
                //   FileModify.MergeFile("d:/test_merge_2.avi.gz");
                TimeSpan sp = DateTime.Now - dold;
                // FileModify.Txt_Create(35000000000);
                // Console.WriteLine("press Enter to terminate! CODE {0}", GZipTest.error ? "1" : "0");
                Console.WriteLine("completed in {0} secs", sp);
                Console.ReadLine();
            }

        }
     
    }
}
