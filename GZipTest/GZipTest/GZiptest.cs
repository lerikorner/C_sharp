using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.IO.Compression;
namespace GZipTest
{
    class GZipTest //класс, в котором работаем с GZipStream
    {
        public static int BufferSize = Properties.Settings.Default.buffersize;   //!!!!!!

        public static bool error;
        public static int multithread = Environment.ProcessorCount;//количество потоков

        public static byte[][] block = new byte[multithread][];
        public static byte[][] gzipdata = new byte[multithread][];

        public static void CompressBlock(object i) //работаем с буфером
        {
            using (MemoryStream output = new MemoryStream(block[(int)i].Length))
            {
                try
                {
                    using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                    {
                        cs.Write(block[(int)i], 0, block[(int)i].Length);
                        //    cs.Close();
                    }
                    gzipdata[(int)i] = output.ToArray();
                    //    output.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    error = true;
                }
            }
        }
        public static void DecompressBlock(object i) //работаем с буфером
        {
            using (MemoryStream input = new MemoryStream(gzipdata[(int)i]))
            {
                try
                {
                    using (GZipStream ds = new GZipStream(input, CompressionMode.Decompress))
                    {
                        ds.Read(block[(int)i], 0, block[(int)i].Length);
                        //      ds.Close();                  
                    }
                    //   input.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                    error = true;
                }
            }
        }
        public static long[] MagicNumbers(string inFileName) //ищем ключи фрагментов архивов для корректной распаковки
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                int bricksize, datasize, i = 1;
                byte[] gzipdata;
                byte[] flag = new byte[8];
                List<long> magicbyte = new List<long>();
                long source = 0, pack = 0;
                while (inFile.Position < inFile.Length)
                {
                    Console.Write("*");
                    inFile.Read(flag, 0, 8);
                    bricksize = BitConverter.ToInt32(flag, 4) - 1;   //байт с реальным размером
                    gzipdata = new byte[bricksize + 1];
                    flag.CopyTo(gzipdata, 0);
                    inFile.Read(gzipdata, 8, bricksize - 8);
                    datasize = BitConverter.ToInt32(gzipdata, bricksize - 4);  //байт с несжатым размером
                    double compression = (double)bricksize / datasize;
                    source += datasize;
                    pack += bricksize;
                    long current_source = i * FileModify.GZipLimit;
                    if ((source == current_source))
                    {
                        Console.WriteLine("степень сжатия = {0}...", compression);
                        magicbyte.Add(pack);
                        pack = 0;
                        i++;
                    }
                }
                magicbyte.Add(pack);
                long[] magic = magicbyte.ToArray();
                return magic;
            }
        }
        public static void Compress(string inFileName, string outFileName)//путь к входному файлу, путь к выходному файлу
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
                        Thread[] thread;
                        int datasize;
                        while (inFile.Length > inFile.Position)
                        {
                            Console.Write("-");
                            thread = new Thread[multithread];
                            for (int j = 0; (j < multithread) &&
                                (inFile.Length > inFile.Position); j++)
                            {
                                if (inFile.Length - inFile.Position <= BufferSize)
                                {
                                    datasize = (int)(inFile.Length - inFile.Position);
                                }
                                else
                                {
                                    datasize = BufferSize;
                                }
                                block[j] = new byte[datasize];
                                inFile.Read(block[j], 0, datasize);
                                thread[j] = new Thread(CompressBlock);
                                thread[j].Start(j);   //стартуем потоки
                            }
                            for (int i = 0; (i < multithread) && (thread[i] != null);)
                            {
                           //     if (thread[i].ThreadState == ThreadState.Stopped)
                                {
                                    thread[i].Join();
                                    BitConverter.GetBytes(gzipdata[i].Length + 1)
                                        .CopyTo(gzipdata[i], 4);
                                    outFile.Write(gzipdata[i], 0, gzipdata[i].Length); //пишем в цикле обработанные блоки
                                    i++;
                                }
                            }
                        }
                        Console.WriteLine();
                        outFile.Close();
                    }
                    inFile.Close();
                    error = false;
                    Console.WriteLine(" упаковка завершена. ");
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
                    //
                    // ПРИ РАБОТЕ С КОНСОЛЬЮ WINDOWS 
                    // ОБЯЗАТЕЛЬНО УКАЗЫВАЕМ ПОЛНЫЙ ПУТЬ И РАСШИРЕНИЕ ДЛЯ ВХОДНЫХ И ВЫХОДНЫХ ФАЙЛОВ!!!! 
                    //
                    using (FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))     //инициализация буфера распаковки
                    {
                        Thread[] thread;
                        int datasize, bricksize;
                        byte[] flag = new byte[8];
                        while (inFile.Position < inFile.Length)
                        {
                            Console.Write("|");
                            thread = new Thread[multithread];
                            for (int j = 0; (j < multithread) && (inFile.Position < inFile.Length); j++)
                            {  //внутри этого цикла бежим по ключам в архиве
                                inFile.Read(flag, 0, 8);
                                bricksize = BitConverter.ToInt32(flag, 4) - 1;
                                gzipdata[j] = new byte[bricksize + 1];
                                flag.CopyTo(gzipdata[j], 0);

                                inFile.Read(gzipdata[j], 8, bricksize - 8);
                                datasize = BitConverter.ToInt32(gzipdata[j], bricksize - 4);
                                block[j] = new byte[datasize];

                                thread[j] = new Thread(DecompressBlock);
                                thread[j].Start(j);   //стартуем потоки
                            }
                            for (int i = 0; (i < multithread) && (thread[i] != null);)//пишем на диск, завершаем потоки
                            {
                            //    if (thread[i].ThreadState == ThreadState.Stopped)
                                {
                                    thread[i].Join();
                                    outFile.Write(block[i], 0, block[i].Length);
                                    i++;
                                }
                            }
                        }
                        outFile.Close();

                    }
                    inFile.Close();
                    Console.WriteLine(" распаковка завершена. ");
                    error = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
            }
        }
    }
}
