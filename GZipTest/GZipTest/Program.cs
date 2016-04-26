using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Compress_Decompress
{

    class GZipTest
    {
        static int BufferSize = 2048 * 2048;  //!!!!!!
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
                                for (j = 0; (j < multithread-1) && ((inFile.Length - inFile.Position) > BufferSize); j++)
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
    class FileCreation
    {
        public static void Txt_Create (ulong sz)
        {
            char[] simb = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1' };


            ulong f = 0; //счетчик кол-ва байт

            StreamWriter FW = new StreamWriter("D:\\file.txt", false); //создаем файл

            Random rnd = new Random(); //объект случайного числа    
            Random smb = new Random(); //объект случайного символа 

            //запишем в файл слова до заданного размера в байтах
            for (ulong j = 0; j < sz; j++)
            {
                //записываем строку слов (в каждой строке по 10 слов)
                for (int i = 0; i < 10; i++)
                {
                    uint sh = (uint)rnd.Next(7, 20); //случайное число, кол-во символов в слове 

                    //генерируем слово из случайных символов
                    for (int s = 0; s < sh; s++)
                    {
                        int b = smb.Next(0, 53); //случайный символ 
                        FW.Write(simb[b]);
                        f++; //увеличиваем счетчик записанных байтов
                        if (f == sz) //если счетчик равен заданному размеру файла - выход
                            goto M;
                    }
                    FW.Write(" "); //разделитель между словами  
                    f++;
                    if (f == sz)
                        goto M;
                }
                FW.WriteLine(); //переход на новую строку                   
                f++;
                if (f == sz)
                    goto M;
                f++;
                if (f == sz)
                    goto M;
            }
        M:
            FW.Close();
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
                    GZip = "compress";          //
                    fIN = "d:/file.txt";        //тестовые значения для отладки!!!!!
                    fOUT = "d:/file.txt.gz";    //
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
                    GC.Collect(); //чистим мусор
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                GZipTest.error = true;
            }
            TimeSpan sp = DateTime.Now - dold;
           // FileCreation.Txt_Create(35000000000);
           // Console.WriteLine("press Enter to terminate! CODE {0}", GZipTest.error ? "1" : "0");
            Console.WriteLine("completed in {0} secs", sp);
            Console.ReadLine();

        }
     
    }
}
