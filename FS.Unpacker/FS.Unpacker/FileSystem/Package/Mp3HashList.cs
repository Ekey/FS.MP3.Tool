using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

namespace FS.Unpacker
{
    class Mp3HashList
    {
        static String m_Path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        static String m_ProjectFile = @"\Projects\FolderNames.list";
        static String m_ProjectFilePath = m_Path + m_ProjectFile;

        public static Dictionary<String, String> m_HashList = new Dictionary<String, String>();

        public static void iLoadProject()
        {
            String m_Line = null;

            if (!File.Exists(m_ProjectFilePath))
            {
                Utils.iSetError("[ERROR]: Unable to load project file " + m_ProjectFile);
            }
            else
            {
                Int32 i = 0;
                m_HashList.Clear();

                StreamReader TProjectFile = new StreamReader(m_ProjectFilePath);
                while ((m_Line = TProjectFile.ReadLine()) != null)
                {
                    UInt32 dwHash = CRC32.iGetHash(m_Line);
                    String m_Hash = dwHash.ToString("X8");

                    if (m_HashList.ContainsKey(m_Hash))
                    {
                        String m_Collision = null;
                        m_HashList.TryGetValue(m_Hash, out m_Collision);
                        Console.WriteLine("[COLLISION]: {0} <-> {1}", m_Collision, m_Line);
                    }

                    m_HashList.Add(m_Hash, m_Line);
                    i++;
                }

                TProjectFile.Close();
                Console.WriteLine("[INFO]: Project File Loaded: {0}", i);
                Console.WriteLine();
            }
        }

        public static String iGetNameFromHashList(UInt32 dwHash)
        {
            String m_FileName = null;
            String m_Hash = dwHash.ToString("X8");

            if (m_HashList.ContainsKey(m_Hash))
            {
                m_HashList.TryGetValue(m_Hash, out m_FileName);
            }
            else
            {
                m_FileName = @"__Unknown\" + m_Hash;
            }

            return m_FileName;
        }

        public static String iGetNameFromArchive(FileStream TFileStream, UInt32 dwOffset, UInt32 dwZSize, Int32 bFlag, Int16 wStringLength)
        {
            if (bFlag == 3)
            {
                TFileStream.Seek(dwOffset, SeekOrigin.Begin);
            }
            else if (bFlag == 1)
            {
                TFileStream.Seek(dwOffset + dwZSize, SeekOrigin.Begin);
            }

            var lpHintData = TFileStream.ReadBytes(wStringLength - 1);
            String lpHintName = Encoding.ASCII.GetString(lpHintData);
            return lpHintName;
        }
    }
}
