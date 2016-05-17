using System;
using System.IO;
namespace GZipTest
{
    class FileProcess //в этом классе производим операции над всем файлом
    {
        static public void FileCompress(string infileName, string outfileName)
        {
            FileStream infile = new FileStream(infileName, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(infileName);
            var directoryName = Path.GetDirectoryName(infileName);
            int count = Convert.ToInt32(infile.Length / FileModify.GZipLimit) + 1;
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
                    string inFrag = string.Format(directoryName + "boof1" + "_{0}" + extension, i); //сжимаем через разбиение
                    GZipTest.Compress(inFrag, inFrag + ".gz");
                    File.Delete(inFrag);
                }
                FileModify.MergeFile(outfileName, count, true);
            }
            else
            {
                GZipTest.Compress(infileName, outfileName);
            }
        }
        static public void FileDecompress(string infileName, string outfileName)
        {
            FileStream infile = new FileStream(infileName, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(infileName.Remove(infileName.Length - 3));
            var directoryName = Path.GetDirectoryName(infileName);
            long length = infile.Length - 1;
            infile.Close();
            long[] bytes = GZipTest.MagicNumbers(infileName);
            int count = bytes.Length;
            Console.WriteLine("количество фрагментов: {0}", count);
            if (length > FileModify.GZipLimit)
            {
                FileModify.SplitFile(infileName, count, false, bytes);
                for (int i = 0; i < count; i++)
                {
                    string outFrag = string.Format(directoryName + "boof1" + "_{0}" + extension, i);
                    GZipTest.Decompress(outFrag + ".gz", outFrag);
                    Console.WriteLine("распаковка {0},  {1}", i, bytes[i]);
                    File.Delete(outFrag + ".gz");

                }
                FileModify.MergeFile(outfileName, count, false);
            }
            else
            {
                GZipTest.Decompress(infileName, outfileName);
            }
        }
    }
}
        
    
