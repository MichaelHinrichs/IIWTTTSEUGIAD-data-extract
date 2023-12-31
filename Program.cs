﻿//Written for "Is It Wrong to Try to Shoot 'em Up Girls in a Dungeon?" https://store.steampowered.com/app/1446720/Is_It_Wrong_to_Try_to_Shoot_em_Up_Girls_in_a_Dungeon/
using System;
using System.IO;

namespace IIWTTTSEUGIAD_data_extract
{
    class Program
    {
        public static BinaryReader br;
        static void Main(string[] args)
        {
            br = new BinaryReader(File.OpenRead(args[0]));
            br.ReadInt32();
            int subFileCount = br.ReadInt32();
            System.Collections.Generic.List<SUBFILE> subfiles = new();

            for (int i = 0; i < subFileCount; i++)
                subfiles.Add(ReadSubfileMetadata());

            for (int i = 0; i < subFileCount; i++)
                WriteFiles(subfiles[i], args[0]);
        }
        
        static SUBFILE ReadSubfileMetadata()
        {
            long nameOffset = br.BaseStream.Position;
            string name = ReadString1();
            br.BaseStream.Position = nameOffset + 0x40;

            int UnknownA = br.ReadInt32();
            int UnknownB = br.ReadInt32();
            return new SUBFILE
            {
                name = name,
                offset = br.ReadUInt32(),
                size = br.ReadUInt32()
            };
        }

        static void WriteFiles(SUBFILE sub, string file)
        {
            string newFolder = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file);

            if (sub.name.Contains(@"\"))
            {
                newFolder += "\\" + Path.GetDirectoryName(sub.name);
                sub.name = Path.GetFileName(sub.name);
            }

            Directory.CreateDirectory(newFolder);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(newFolder + "\\" + sub.name));
            br.BaseStream.Position = sub.offset;
            bw.Write(br.ReadBytes((int)sub.size));
            bw.Close();
        }
        
        public static string ReadString1()
        {
            char[] fileName = Array.Empty<char>();
            char readchar = (char)1;
            while (readchar > 0)
            {
                readchar = br.ReadChar();
                Array.Resize(ref fileName, fileName.Length + 1);
                fileName[^1] = readchar;
            }
            Array.Resize(ref fileName, fileName.Length - 1);
            return new(fileName);
        }

        public struct SUBFILE
        {
            public string name;
            public uint offset;
            public uint size;
        }
    }
}
