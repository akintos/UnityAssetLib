using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityAssetLib.Types;

namespace UnityAssetLib.Util
{
    public static class BinaryReaderExtensions
    {
        public static void AlignStream(this BinaryReader reader, int alignment = 4)
        {
            var pos = reader.BaseStream.Position;
            var mod = pos % alignment;
            if (mod != 0)
            {
                reader.BaseStream.Position += alignment - mod;
            }
        }

        public static string ReadAlignedString(this BinaryReader reader)
        {
            return ReadAlignedString(reader, reader.ReadInt32());
        }

        public static string ReadAlignedString(this BinaryReader reader, int length)
        {
            if (length > 0 && length < (reader.BaseStream.Length - reader.BaseStream.Position))
            {
                var stringData = reader.ReadBytes(length);
                var result = Encoding.UTF8.GetString(stringData);
                reader.AlignStream(4);
                return result;
            }
            return string.Empty;
        }

        public static string ReadStringToNull(this BinaryReader reader)
        {
            var bytes = new List<byte>();
            byte b;
            while (reader.BaseStream.Position != reader.BaseStream.Length && (b = reader.ReadByte()) != 0)
                bytes.Add(b);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        private static T[] ReadArray<T>(Func<T> del, int length)
        {
            var array = new T[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = del();
            }
            return array;
        }

        public static int[] ReadInt32Array(this BinaryReader reader, int length)
        {
            return ReadArray(reader.ReadInt32, length);
        }

        public static uint[] ReadUInt32Array(this BinaryReader reader, int length)
        {
            return ReadArray(reader.ReadUInt32, length);
        }

        public static float[] ReadSingleArray(this BinaryReader reader, int length)
        {
            return ReadArray(reader.ReadSingle, length);
        }

        public static PPtr ReadPPtr(this BinaryReader reader)
        {
            var result = new PPtr();

            result.m_FileID = reader.ReadInt32();
            result.m_PathID = reader.ReadInt64();

            return result;
        }
    }
}
