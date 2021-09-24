using System;
using System.IO;
using System.Text;

namespace FS.Unpacker
{
    class Mp3Unpack
    {
        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            Mp3HashList.iLoadProject();

            using (FileStream TFileStream = File.OpenRead(m_Archive))
            {
                UInt32 dwSignature = TFileStream.ReadUInt32();
                UInt32 dwTableOffset = TFileStream.ReadUInt32();
                UInt32 dwTotalFiles = TFileStream.ReadUInt32();
                UInt32 dwTableZSize = TFileStream.ReadUInt32();
                UInt32 dwTableSize = dwTotalFiles * 40;
                var lpMD5 = TFileStream.ReadBytes(16);

                if (dwSignature != 0x1A435443)
                {
                    Utils.iSetError("[ERROR]: Invalid magic of MP3 archive file");
                    return;
                }

                Byte[] lpDstEntryBuffer = new Byte[dwTableSize];

                TFileStream.Seek(dwTableOffset, SeekOrigin.Begin);
                var lpSrcEntryBuffer = TFileStream.ReadBytes((Int32)dwTableZSize);
                LZO1X.iDecompress(lpSrcEntryBuffer, dwTableZSize, lpDstEntryBuffer, ref dwTableSize);

                using (var TMemoryReader = new MemoryStream(lpDstEntryBuffer))
                {
                    for (Int32 i = 0; i < dwTotalFiles; i++)
                    {
                        UInt32 dwFileNameCRC = TMemoryReader.ReadUInt32();
                        UInt32 dwFolderNameCRC = TMemoryReader.ReadUInt32();
                        UInt32 dwOffset = TMemoryReader.ReadUInt32();
                        UInt32 dwZSize = TMemoryReader.ReadUInt32();
                        UInt32 dwSize = TMemoryReader.ReadUInt32();
                        Int32 bFileFlag = TMemoryReader.ReadByte(); // 1 - file, 3 - folder
                        Int32 bCompressionFlag = TMemoryReader.ReadByte(); // 0 - (folders), 3 - lzo1x, 1 - not compressed
                        Int16 wFileNameLength = TMemoryReader.ReadInt16();
                        var lpEntryMD5 = TMemoryReader.ReadBytes(16);

                        if (bFileFlag == 3)
                        {
                            //skip
                        }
                        else if (bFileFlag == 1)
                        {
                            String m_FilePath = null;
                            String m_FileName = Mp3HashList.iGetNameFromArchive(TFileStream, dwOffset, dwZSize, bFileFlag, wFileNameLength);
                            String m_FolderName = Mp3HashList.iGetNameFromHashList(dwFolderNameCRC);

                            if (m_FolderName.Contains("__Unknown"))
                            {
                                m_FilePath = @"__Unknown\" + dwFolderNameCRC.ToString("X8") + @"\";
                            }

                            m_FilePath = m_FolderName.Replace("/", @"\") + @"\" + m_FileName;
                            String m_FullPath = m_DstFolder + m_FilePath;
                            Console.WriteLine("[UNPACKING]: {0}", m_FilePath);

                            Utils.iCreateDirectory(m_FullPath);

                            if (!File.Exists(m_FullPath))
                            {
                                TFileStream.Seek(dwOffset, SeekOrigin.Begin);
                                var lpSrcBuffer = TFileStream.ReadBytes((Int32)dwZSize);

                                if (bCompressionFlag == 1)
                                {
                                    File.WriteAllBytes(m_FullPath, lpSrcBuffer);
                                }
                                else if (bCompressionFlag == 3)
                                {
                                    Byte[] lpDstBuffer = new Byte[dwSize];
                                    LZO1X.iDecompress(lpSrcBuffer, dwZSize, lpDstBuffer, ref dwSize);

                                    File.WriteAllBytes(m_FullPath, lpDstBuffer);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
