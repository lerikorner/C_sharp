using System;
using System.IO;

namespace GZipTest
{
    class FileModify //класс в котором делим файл на фрагменты перед упаковкой и распаковкой для обхода лимита 4 GB
    {
        public static long GZipLimit = Properties.Settings.Default.gziplimit; // 2^32 (4 GB)

        //public static long GZipLimit = 40000000;
        public static void SplitFile(string infile, int count, bool pack, long[] bytes) //pack это флаг на сжатость файла
        {   //разбиваем файл на фрагменты равыне лимиту
            int read;
            var extension = pack ? Path.GetExtension(infile) : Path.GetExtension(infile.Remove(infile.Length - 3));
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            using (FileStream _from_stream = new FileStream(infile, FileMode.Open))
            {
                for (int i = 0; i < count; i++)
                {
                    string comp_string = !pack ? string.Format(directoryName + "boof1" + "_{0}" + extension + ".gz", i) :
                             string.Format(directoryName + "boof1" + "_{0}" + extension, i);

                    using (FileStream _to_stream = new FileStream(comp_string, FileMode.Create, FileAccess.Write)) //набираем файлы-фрагменты
                    {
                        Console.WriteLine("{0}й том для обработки создается...", i);
                        long offset = 0;

                        for (int k = 0; k < i; k++)
                        {
                            offset = offset + bytes[k];
                        }
                        _from_stream.Position = pack ? (i * (GZipLimit)) : offset;  // размер смещения

                        Console.WriteLine("смещение по файлу: {0}", _from_stream.Position);

                        long limit = pack ? GZipLimit : bytes[i];   //выбираем размер вырезаемого фрагмента

                        int fragsize, tail = Convert.ToInt32(limit % GZipTest.BufferSize);

                        while ((_to_stream.Length < limit) && (_from_stream.Position < _from_stream.Length))
                        {
                            if ((limit - _to_stream.Position < GZipTest.BufferSize))
                            {
                                fragsize = tail;
                            }
                            else {
                                fragsize = GZipTest.BufferSize;
                            }
                            read = _from_stream.Read(buffer, 0, fragsize);
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

        public static void MergeFile(string infile, int count, bool pack) //  соединяем фрагменты после операций
        {
            int read;
            var extension = !pack ? Path.GetExtension(infile) : Path.GetExtension(infile.Remove(infile.Length - 3));
            var directoryName = Path.GetDirectoryName(infile);
            byte[] buffer = new byte[GZipTest.BufferSize];
            using (FileStream _to_stream = new FileStream(infile, FileMode.Create, FileAccess.Write))
            {
                Console.WriteLine("{0} томов на склеивание...", count);
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine(extension);
                    string comp_string = !pack ? string.Format(directoryName + "boof1" + "_{0}" + extension, i) :
                                                 string.Format(directoryName + "boof1" + "_{0}" + extension + ".gz", i);

                    using (FileStream _from_stream = new FileStream(comp_string,
                                                                  FileMode.Open, FileAccess.Read))
                    {
                        Console.WriteLine("{0}й том соединен.", i);
                        while (((read = _from_stream.Read(buffer, 0, GZipTest.BufferSize)) != 0))
                        {
                            _to_stream.Write(buffer, 0, read);
                            Console.Write(" - {0} ", _to_stream.Position);
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
}
