using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityAssetLib.IO
{
    public class ExtendedBinaryReader : BinaryReader
    {
        private static readonly EndianType systemEndian = BitConverter.IsLittleEndian ? EndianType.LittleEndian : EndianType.BigEndian;

        public EndianType endian;

        public ExtendedBinaryReader(Stream stream, EndianType endian = EndianType.LittleEndian) : base(stream)
        {
            this.endian = endian;
        }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public long Seek(long offset, SeekOrigin origin = SeekOrigin.Begin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public long Tell()
        {
            return BaseStream.Position;
        }

        public override short ReadInt16()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(2);
                Array.Reverse(data);
                return BitConverter.ToInt16(data, 0);
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToInt32(data, 0);
            }
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToInt64(data, 0);
            }
            return base.ReadInt64();
        }

        public override ushort ReadUInt16()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(2);
                Array.Reverse(data);
                return BitConverter.ToUInt16(data, 0);
            }
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToUInt32(data, 0);
            }
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToUInt64(data, 0);
            }
            return base.ReadUInt64();
        }

        public override float ReadSingle()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToSingle(data, 0);
            }
            return base.ReadSingle();
        }

        public override double ReadDouble()
        {
            if (endian != systemEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToUInt64(data, 0);
            }
            return base.ReadDouble();
        }

        public object ReadValueArray<T>(int size) where T : struct
        {
            var type = typeof(T);
            if (!type.IsValueType)
                throw new ArgumentException($"{type} is not a value type");

            var byteArray = ReadBytes(size * Marshal.SizeOf(type));
            if (type == typeof(byte))
                return byteArray;

            var ret = Array.CreateInstance(type, size);
            Buffer.BlockCopy(byteArray, 0, ret, 0, byteArray.Length);
            return ret;
        }

        public void AlignStream(int alignment = 4)
        {
            var mod = Position % alignment;
            if (mod != 0)
            {
                Position += alignment - mod;
            }
        }

        public string ReadAlignedString()
        {
            return ReadAlignedString(ReadInt32());
        }

        public string ReadAlignedString(int length)
        {
            if (length > 0 && length < (BaseStream.Length - BaseStream.Position))
            {
                var stringData = ReadBytes(length);
                var result = Encoding.UTF8.GetString(stringData);
                AlignStream(4);
                return result;
            }
            return string.Empty;
        }

        public string ReadNullTerminatedString()
        {
            byte b;
            var bytes = new List<byte>();
            while (Position != BaseStream.Length && (b = ReadByte()) != 0)
                bytes.Add(b);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
