﻿using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Collections;

namespace Compress_Decompress
{

    class GZipTest
    {
        public static int BufferSize = 2048*2048;  //!!!!!!
        public static bool error;
        public static int multithread = Environment.ProcessorCount;//количество потоков
        public static byte[][] block = new byte[multithread][];
        public static byte[][] brick = new byte[multithread][];

        public static void CompressBlock(object i)
        {
            using (MemoryStream output = new MemoryStream(block[(int)i].Length))
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                {
                    cs.Write(block[(int)i], 0, block[(int)i].Length);
                }
                brick[(int)i] = output.ToArray();
            }
        }
        public static void Compress(string inFileName, string outFileName)//путь к входному файлу, путь к выходному файлу, флаг
        {
            try
            {
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))//инициализация входного файла
                {
                    using (FileStream outFile = new FileStream(outFileName, FileMode.Create,FileAccess.Write))//инициализация выходного файла
                    {
                        //
                        // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                        // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                        //
                        int i = 0, j = 0;
                        int[] read = new int[multithread];
                        Thread[] thread;
                        int datasize;
                        while (inFile.Length > inFile.Position)
                        {
                            Console.Write("-");
                            thread = new Thread[multithread];
                            for (j = 0; (j < multithread) &&
                                (inFile.Length > inFile.Position); j++)
                            {
                                if (inFile.Length - inFile.Position <= BufferSize)
                                {
                                   datasize = (int)(inFile.Length - inFile.Position);
                                    Console.WriteLine(datasize);
                                }
                                else
                                {
                                    datasize=BufferSize;
                                }
                                block[j] = new byte[datasize];
                                inFile.Read(block[j], 0, datasize);
                                thread[j] = new Thread(CompressBlock);
                                thread[j].Start(j);
                            }
                            for (i = 0; (i < multithread)&&(thread[i]!=null);)
                            {
                                //if (thread[i].ThreadState == ThreadState.Stopped)
                                thread[i].Join();
                                {
                                    BitConverter.GetBytes(brick[i].Length+1)
                                    .CopyTo(brick[i],4);
                                    outFile.Write(brick[i], 0, brick[i].Length);
                                    i++;
                                }
                            }
                        }
                        Console.WriteLine();                     
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
                            byte[] buffer = new byte[BufferSize];
                            Thread[] thread = new Thread[multithread];
                            Thread[] thread_ = new Thread[multithread];
                            object block = new object();
                            while ((inFile.Length - inFile.Position) > BufferSize)
                            {
                                for (j = 0; (j < multithread) && ((inFile.Length - inFile.Position) > BufferSize); j++)
                                {
                                    //   thread[j] = new Thread(() =>
                                     //  {
                                      //     lock (block)
                                           {
                                               Console.Write("- {0} -", j);
                                               read = outGZip.Read(buffer, 0, BufferSize);
                                               outFile.Write(buffer, 0, read);
                                           }
                                  //     });
                                  //     thread[j].Start();
                                 //   thread[j].Priority = ThreadPriority.AboveNormal;

                                }
                                for (i = 0; i < j; i++)
                                {
                            //        thread[i].Join();
                                }
                            }
                            outFile.Write(buffer, 0, outGZip.Read(buffer, 0, BufferSize));
                            Console.WriteLine();
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
         public static long GZipLimit = 20000000;

        public static void SplitFile(string infile,int count,bool pack,long[]bytes)
        {
            int read;
            var extension = pack ? Path.GetExtension(infile):Path.GetExtension(infile.Remove(infile.Length - 3));
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            using (FileStream _from_stream = new FileStream(infile, FileMode.Open))
            {
                for (int i = 0; i < count; i++)
                {
                    string comp_string = !pack ? string.Format(directoryName + "boof1" + "_{0}" + extension+".gz", i) :
                             string.Format(directoryName + "boof1" + "_{0}"+extension, i);

                    using (FileStream _to_stream = new FileStream(comp_string, FileMode.Create, FileAccess.Write))
                    {
                        Console.WriteLine("{0}й том для обработки создается...", i);
                        long offset = 0;
                        for (int k = 0; k < i; k++)
                        {
                            offset =offset+ bytes[k];
                        }
                        _from_stream.Position = pack ? (i * (GZipLimit)) : offset;
                        Console.WriteLine("смещение по файлу: {0}",_from_stream.Position);
                        long limit = pack ? GZipLimit : bytes[i];
                        int tail = Convert.ToInt32(limit % GZipTest.BufferSize);
                        while ((_to_stream.Length<limit)&&(_from_stream.Position<_from_stream.Length))
                        {
                            if ((limit - _to_stream.Position < GZipTest.BufferSize))
                            {
                                read = _from_stream.Read(buffer, 0, tail);
                            }
                            else
                            {
                                read = _from_stream.Read(buffer, 0, GZipTest.BufferSize);
                            }
                            _to_stream.Write(buffer, 0, read);
                            Console.Write(" - ");
                        }
                        Console.WriteLine();
                        _to_stream.Close();
                    }
                }
                _from_stream.Close();
            }          
        }
        public static void MergeFile(string infile,int count,bool pack)
        {
            int read;
            var extension = !pack ? Path.GetExtension(infile) : Path.GetExtension(infile.Remove(infile.Length - 3));
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            using (FileStream _to_stream = new FileStream(infile, FileMode.Create,FileAccess.Write))
            {
                Console.WriteLine("{0} томов на склеивание...", count);
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine(extension);
                    string comp_string = !pack ? string.Format(directoryName + "boof1" + "_{0}" + extension, i) : 
                                                 string.Format(directoryName + "boof1" + "_{0}"+extension+".gz", i); 

                    using (FileStream _from_stream = new FileStream(comp_string,
                                                                  FileMode.Open, FileAccess.Read))
                    {
                        Console.WriteLine("{0}й том соединен.", i);
                        //_from_stream.Position = i * GZipLimit;
                        //   _to_stream.Position = i * GZipLimit;
                        while (((read = _from_stream.Read(buffer, 0, GZipTest.BufferSize)) != 0))
                        {
                            _to_stream.Write(buffer, 0, read);
                            Console.Write(" - ");
                        }
                        Console.WriteLine();
                        _from_stream.Close();
                        File.Delete(comp_string);
                    }
                }
                 _to_stream.Close();
            }
        }      
    }

    class FileProcess
    {
        static public long[] FileCompress(string infileName, string outfileName)
        {
            FileStream infile = new FileStream(infileName, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(infileName);
            var directoryName = Path.GetDirectoryName(infileName);
            int count = Convert.ToInt32(infile.Length / FileModify.GZipLimit)+1;
            long length = infile.Length;
          //  Console.WriteLine("размер файла: {0}", length);
            long[] bytes = new long[count];
            bytes[0] = 0;
            infile.Close();
            if (length > FileModify.GZipLimit)
            {
                FileModify.SplitFile(infileName, count, true, bytes);
                for (int i = 0; i < count; i++)
                {
                    string inFrag = string.Format(directoryName + "boof1" + "_{0}" + extension, i);
                    GZipTest.Compress(inFrag, inFrag + ".gz");
                    FileStream buf = new FileStream(inFrag+".gz", FileMode.Open, FileAccess.Read);
                    Console.WriteLine(" fragment length: {0}", buf.Length);
                    if (i< count+1) bytes[i]=buf.Length;
                    Console.WriteLine(" length {0}, offset {1}",i,bytes[i]);
                    buf.Close();
                    File.Delete(inFrag);
                }
                FileModify.MergeFile(outfileName,count, true);
            }
            else
            {
                GZipTest.Compress(infileName, outfileName);
            }
            return bytes;
        }
        static public void FileDecompress(string infileName, string outfileName, long[] bytes)
        {
            FileStream infile = new FileStream(infileName, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(infileName.Remove(infileName.Length-3));
            var directoryName = Path.GetDirectoryName(infileName);
            int count = bytes.Length;
            Console.WriteLine("количество фрагментов: {0}", count);
            long length = infile.Length-1;
            infile.Close();
            if (length > FileModify.GZipLimit)
            {
                FileModify.SplitFile(infileName,count,false,bytes);
                for (int i = 0; i < count; i++)
                {
                    string outFrag = string.Format(directoryName + "boof1" + "_{0}"+extension, i);
                    GZipTest.Decompress(outFrag+".gz", outFrag);
                    FileStream buf = new FileStream(outFrag + ".gz", FileMode.Open, FileAccess.Read);
                    if (i < count) bytes[i] = buf.Length;
                    Console.WriteLine(" length_unpack {0}, offset {1}", i, bytes[i]);
                    buf.Close();
                    File.Delete(outFrag + ".gz");

                }
                FileModify.MergeFile(outfileName,count,false);
            }
            else
            {
                GZipTest.Decompress(infileName, outfileName);
            }

        }
        class Program
        {
            static long[] bytes;
            static string GZip, fIN, fOUT, fIN2;
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
                        fIN = string.Format("d:/test2_M.avi");
                        fOUT = string.Format("d:/test3_S.avi.gz");
                        fIN2 = string.Format("d:/test3_MS.avi");
                        if (GZip == "compress")
                        {
                            Console.WriteLine("packing: ");
                            bytes = FileProcess.FileCompress(fIN, fOUT);
                            TimeSpan sp1 = DateTime.Now - dold;
                            Console.WriteLine("completed in {0} secs", sp1);
                            dold = DateTime.Now;
                            FileProcess.FileDecompress(fOUT, fIN2, bytes);
                          /*  StreamWriter legend = new StreamWriter("d:\\legend.txt");
                            for (int i = 0; i < bytes.Length; i++)
                            {
                                legend.WriteLine(bytes[i]);
                            }
                            legend.Close();  */                       
                        }
                        else if (GZip == "decompress")
                        {
                            Console.WriteLine("unpacking: ");
                      //      FileProcess.FileDecompress(fIN, fOUT);
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
                TimeSpan sp = DateTime.Now - dold;
                Console.WriteLine("press Enter to terminate! CODE {0}", GZipTest.error ? "1" : "0");
                Console.WriteLine("completed in {0} secs", sp);
                Console.ReadLine();
            }

        }
     
    }
}
