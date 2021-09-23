using System;
using System.IO;
using System.Reflection;

namespace FS.Unpacker
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("pd_fszhs_zhengshi Data Unpacker");
            Console.WriteLine("(c) 2021 Ekey (h4x0r) / v{0}\n", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.ResetColor();

            if (args.Length != 2)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[Usage]");
                Console.WriteLine("    FS.Unpacker <m_File> <m_Directory>");
                Console.WriteLine("    m_File - Source of MP3 archive file");
                Console.WriteLine("    m_Directory - Destination directory\n");
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[Examples]");
                Console.WriteLine("    FS.Unpacker D:\\data0.mp3 D:\\Unpacked_data0");
                Console.ResetColor();
                return;
            }

            String m_Input = args[0];
            String m_Output = Utils.iCheckArgumentsPath(args[1]);

            if (!File.Exists("lzo1x_32.dll") || !File.Exists("lzo1x_64.dll"))
            {
                Utils.iSetError("[ERROR]: Unable to find LZO modules!");
                return;
            }

            if (!File.Exists(m_Input))
            {
                Utils.iSetError("[ERROR]: Input file -> " + m_Input + " <- does not exist!");
                return;
            }

            Mp3Unpack.iDoIt(m_Input, m_Output);
        }
    }
}
