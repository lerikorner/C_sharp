using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Compress_Decompress
{
    class Program
   {
      /*  static void zip(string inFileName, string outFileName)
        {
            using (FileStream fileOpen = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fileCreate = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream fileGZip = new GZipStream(fileCreate, CompressionMode.Compress))
                    {
                        //     сжатие файла по одному байту
                        if (false)
                        {
                            int buf;
                            while ((buf = fileOpen.ReadByte()) > 0)
                            {
                                fileGZip.WriteByte((byte)buf);
                            }
                        }

                        //     сжатие файла целиком
                        if (true)
                        {
                            fileOpen.CopyTo(fileGZip, (int)fileOpen.Length);
                        }

                        //     сжатие файла с буффером по умолчанию(4096 байт)
                        if (false)
                        {
                            fileOpen.CopyTo(fileGZip);
                        }
                    }
                }
            }
        }
        static void unzip(string inFileName, string outFileName)
        {
            using (FileStream fileOpen = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (FileStream fileCreate = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    using (GZipStream fileGZip = new GZipStream(fileOpen, CompressionMode.Decompress))
                    {
                        //     сжатие файла по одному байту
                        if (false)
                        {
                            int buf;
                            while ((buf = fileGZip.ReadByte()) > 0)
                            {
                                fileCreate.WriteByte((byte)buf);
                            }
                        }

                        //     сжатие файла целиком
                        if (true)
                        {
                            fileGZip.CopyTo(fileCreate, (int)fileOpen.Length);
                        }

                        //     сжатие файла с буффером по умолчанию(4096 байт)
                        if (false)
                        {
                            fileGZip.CopyTo(fileCreate);
                        }
                    }
                }
            }
        }*/
        static void Main(string[] args)
        {

          //  zip("d:/muz.rar", "d:/test_rar.Gz");
         //   unzip("myFile.txt.Gz", "myNewFile.txt");
          //  string fileName = "D:\\test.jpg";
          //  Compress_me.Compress(fileName);
            Console.WriteLine("completed");
            Console.ReadKey();
        }
     /*   public static class Program
        {
            private static void Main()
            {
                const string outputFile = "d:/output.gz";
                  const string inputFile = "d:/metal.rar";

                  using (FileStream outFile = File.Create(outputFile))
                  {
                      using (GZipStream zipStream = new GZipStream(outFile, CompressionMode.Compress))
                      {
                          using (FileStream fileStream = File.Open(inputFile, FileMode.Open))
                          {
                              fileStream.CopyTo(zipStream);
                              Console.WriteLine("Compressed {0} from {1} to {2} bytes.",
                                                 Path.GetFileName(fileStream.Name), fileStream.Length, outFile.Length);
                          }
                      }
                  }*/
         /*  Console.WriteLine("введите символы :");
            ConsoleKeyInfo press = Console.ReadKey();

            do
            {
                Console.WriteLine("   Вы нажали  : " + " " + press.KeyChar);
                press = Console.ReadKey();
                // Проверить нажатие модифицирующих клавиш.
                if ((ConsoleModifiers.Alt & press.Modifiers) != 0)
                    Console.WriteLine("Нажата клавиша <Alt>.");
                if ((ConsoleModifiers.Control & press.Modifiers) != 0)
                    Console.WriteLine("Нажата клавиша <Control>.");
                if ((ConsoleModifiers.Shift & press.Modifiers) != 0)
                    Console.WriteLine("Нажата клавиша <Shift>.");
            } while (press.KeyChar!= 'Q');*/
        }
     //   }
//  }
}
