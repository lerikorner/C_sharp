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
        static int BufferSize = 1024 * 1024;
        const  int multithread= 2;
        public static void WriteBlock(GZipStream inStream, int read, byte[] buffer)
        {
            Console.Write('-');
            inStream.Write(buffer, 0, read);
        }

        public static void Compress(string inFileName, string outFileName, bool error)
        {
            try
            {
                if (File.Exists(outFileName))
                {
                    File.Delete(outFileName);
                }
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open))
                {
                    using (FileStream comp = new FileStream(outFileName + ".gz", FileMode.Create,FileAccess.Write))
                    {
                        int read = 0;
                        byte[] buffer = new byte[BufferSize];
                        using (GZipStream inStream = new GZipStream(comp, CompressionMode.Compress))
                        {
                            Console.WriteLine("packing: ");
                            while ((read = inFile.Read(buffer, 0, BufferSize)) != 0)
                            {
                                WriteBlock(inStream, read, buffer);
                                for (int i=1; i<multithread; i++)
                                {
                                    Thread thread=new Thread(delegate() { WriteBlock(inStream, read, buffer); });
                                    thread.Start();
                                }
                            }
                            error = false;
                            inStream.Close();
                        }
                        comp.Close();
                    }
                    inFile.Close();
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
            }
        }

        public static void Decompress(string inFileName, string outFileName, bool error)
        {
            try
            {
                if (File.Exists(outFileName))
                {
                    File.Delete(outFileName);
                }
                using (FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream decomp = new GZipStream(inFile, CompressionMode.Decompress))
                    {
                        string dir = Path.GetDirectoryName(inFileName);
                        // string decompressionFileName = dir + outFileName
                        // Path.GetFileNameWithoutExtension(inFileName) 
                        // + "_decompressed";
                        Console.Write("unpacking: ");
                        using (FileStream outStream = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                        {
                            int read = 0;
                            byte[] buffer = new byte[BufferSize];
                            while ((read = decomp.Read(buffer, 0, BufferSize)) != 0)
                            {
                                Console.Write('-');
                                WriteBlock(decomp, read, buffer);
                                for (int i = 1; i < multithread; i++)
                                {
                                    Thread thread = new Thread(delegate () { WriteBlock(decomp, read, buffer); });
                                    thread.Start();
                                }

                                error = false;
                            }
                            outStream.Close();
                        }
                        decomp.Close();
                    }
                    inFile.Close();
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                error = true;
            }
        }
    }

    class Program
    {

        static string GZip, fIN, fOUT;
        static bool error;

        public static void Main(string[] args)
        {
            try
            {
                GZip = args[0];
                fIN = args[1];
                fOUT = args[2];
                Console.CancelKeyPress += delegate
                {
                    GC.Collect();
                };
                while (true)
                {
                    //string fIN = "d:/7.avi";
                    //string fOUT = "d:/myfile.txt.gz";
                    if (GZip == "compress")
                    {
                        GZipTest.Compress(fIN, fOUT, error);
                    }
                    else if (GZip == "decompress")
                    {
                        GZipTest.Decompress(fIN, fOUT, error);
                    }
                    Console.Write("press CTRL+C to terminate. CODE {0}", error ? "1" : "0");
                    Console.ReadLine();
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
