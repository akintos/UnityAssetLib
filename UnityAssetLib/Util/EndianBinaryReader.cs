using System;
using System.IO;

namespace UnityAssetLib.Util
{
    public enum EndianType
    {
        BigEndian,
        LittleEndian
    }

    public class EndianBinaryReader : BinaryReader
    {
        public EndianType endian;

        public EndianBinaryReader(Stream stream, EndianType endian = EndianType.BigEndian)
            : base(stream)
        { this.endian = endian; }

        public long Position
        {
            get => BaseStream.Position;
            set => BaseStream.Position = value;
        }

        public override short ReadInt16()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(2);
                Array.Reverse(data);
                return BitConverter.ToInt16(data, 0);
            }
            return base.ReadInt16();
        }

        public override int ReadInt32()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToInt32(data, 0);
            }
            return base.ReadInt32();
        }

        public override long ReadInt64()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToInt64(data, 0);
            }
            return base.ReadInt64();
        }

        public override ushort ReadUInt16()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(2);
                Array.Reverse(data);
                return BitConverter.ToUInt16(data, 0);
            }
            return base.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToUInt32(data, 0);
            }
            return base.ReadUInt32();
        }

        public override ulong ReadUInt64()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToUInt64(data, 0);
            }
            return base.ReadUInt64();
        }

        public override float ReadSingle()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(4);
                Array.Reverse(data);
                return BitConverter.ToSingle(data, 0);
            }
            return base.ReadSingle();
        }

        public override double ReadDouble()
        {
            if (endian == EndianType.BigEndian)
            {
                var data = ReadBytes(8);
                Array.Reverse(data);
                return BitConverter.ToUInt64(data, 0);
            }
            return base.ReadDouble();
        }
    }
}
