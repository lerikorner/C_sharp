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
   class Program
   {
        
        static int threadNumber = Environment.ProcessorCount;

        static byte[][] dataArray = new byte[threadNumber][];
        static byte[][] compressedDataArray = new byte[threadNumber][];

        static int dataPortionSize = 10000; // размер считываемого блока
        static int dataArraySize = dataPortionSize * threadNumber;
        static int BufferSize = 1024*1024;


        static public void Compress(string inFileName)
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
            { 
                using (FileStream comp = new FileStream(inFileName + ".gz", FileMode.Append))
                {
                    int read = 0;
                    byte[] buffer = new byte[BufferSize];
                    using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
                    {
                        Console.Write("packing: ");
                        while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                        {
                            Console.Write('-');
                            inStream.Write(buffer, 0, read);
                        }
                        inStream.Close();
                    }
                    comp.Close();
                }
                inFile.Close();
            }
        }
        //сжатие блока
     /*   static public void CompressBlock(object i)
        {
            using (MemoryStream output = new MemoryStream(dataArray[(int)i].Length)) 
            {
                using (GZipStream cs = new GZipStream(output, CompressionMode.Compress))
                {
                    cs.Write(dataArray[(int)i], 0, dataArray[(int)i].Length);
                }
                compressedDataArray[(int)i] = output.ToArray();
            }
        }
        public static void WriteZip(string zipFile, List<string> files, Action<int, string> progress,
            Func<bool> cancel, string comment)
        {
            using (FileStream fs = new FileStream(zipFile, FileMode.Create, FileAccess.Write))
            {
                using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress))
                {
                    // write the header 
                    WriteFieldToZipStream(Signature, ds);

                    // write comment 
                    WriteFieldToZipStream(comment, ds);

                    // write file count 
                    WriteFieldToZipStream(files.Count.ToString(), ds);

                    int index = 0;
                    foreach (string file in files)
                    {
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        using (FileStream sr = new FileStream(file, FileMode.Open, FileAccess.Read))
                        {
                            WriteFieldToZipStream(Path.GetFileName(file), ds);

                            // write the length of data coming from file 
                            ds.Write(BitConverter.GetBytes(sr.Length), 0, sizeof(Int32));
                            Debug.WriteLine("Length: " + sr.Length);

                            // write actual file data to zip stream reading a block at a time 
                            while ((read = sr.Read(buffer, 0, BufferSize)) != 0)
                            {
                                if (cancel()) break;
                                ds.Write(buffer, 0, read);
                            }
                        }

                        if (cancel()) break;

                        // report the ptogress back to the caller 
                        progress((++index * 100) / files.Count, "Zipping file: " + file);
                        Debug.WriteLine("Percent: " + (index * 100) / files.Count);
                    }
                }
            }
        }*/
        public static void Decompress(string inFileName)
        {
            using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))
                {
                    string dir = Path.GetDirectoryName(inFileName);
                    string decompressionFileName = dir + Path.GetFileNameWithoutExtension(inFileName) + "_decompressed";
                    Console.Write("unpacking: ");
                    using (FileStream outStream = new FileStream(decompressionFileName, FileMode.Create, FileAccess.Write))
                    {
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                        {
                            Console.Write('-');
                            outStream.Write(buffer, 0, read);
                        }
                        outStream.Close();
                    }
                    decomp.Close();
                }
                inFile.Close();
            }
        }
        public static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                string mode = args[0];
                string file1 = args[1];
                string file2 = args[2];
                        
            }
            Console.CancelKeyPress += delegate 
            {
                   Console.WriteLine("PRESS ANY KEY TO EXIT");
                   Console.ReadKey();
             };
             while (true)
             {
                string fileNameIN = "d:/7.avi";
                string fileNameOUT = "d:/acad.rar";

                //   Compress(fileNameIN);
                Decompress(fileNameOUT);
                Console.ReadKey();
            }           
        }
    }
}
