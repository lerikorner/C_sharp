using System;
using GZipTest;


namespace Compress_Decompress
{
    class Program
    {
        static string GZip, fIN, fOUT, fIN2;
        static bool flag = true;
        public static void Main(string[] args)//параметры для командной строки должны быть закомментированы для отладки!!!!
        {
            DateTime dold = DateTime.Now;
            try
            {
                GZip = args[0]; //во время работы с консолью  раскомментировать GZip, fIN, fOUT!!!!
                fIN = args[1];
                fOUT = args[2];
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + "|| Неверно введены параметры.");
                GZipTest.GZipTest.error = true;
            }
            try
            {
                Console.CancelKeyPress +=delegate  //ловим ctrl+c
                {
                    GC.Collect(); //чистим мусор
                    flag = false;
                };
                while ((true) & (flag))
                {
                    //    GZip = "decompress";
                    //    fIN = string.Format("d:/test2.avi");
                    //    fOUT = string.Format("d:/test2_magic.avi.gz");
                    //    fIN2 = string.Format("d:/test2_magic_GZ.avi");
                    if (GZip == "compress")
                    {
                        Console.WriteLine("сжимаем: ");
                        FileProcess.FileCompress(fIN, fOUT);
                    }
                    else if (GZip == "decompress")
                    {
                        Console.WriteLine("разжимаем: ");
                        FileProcess.FileDecompress(fIN, fOUT);
                    }
                    flag = false;
                    GC.Collect(); //чистим мусор
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                GZipTest.GZipTest.error = true;

            }
            TimeSpan sp = DateTime.Now - dold;
            Console.WriteLine("нажмите Enter для завершения! CODE {0}", GZipTest.GZipTest.error ? "1" : "0");
            Console.WriteLine("выполнено за {0} сек", sp);
            Console.ReadLine();
        }
    }
}

        
