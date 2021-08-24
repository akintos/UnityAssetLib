using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib.IO
{
    public class ExtendedBinaryWriter : BinaryWriter
    {
        private static readonly EndianType systemEndian = BitConverter.IsLittleEndian ? EndianType.LittleEndian : EndianType.BigEndian;
        private static readonly byte[] zerobytes = new byte[16];

        public EndianType endian;

        public ExtendedBinaryWriter(Stream stream, EndianType endian = EndianType.LittleEndian) : base(stream)
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

        public override void Write(short v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(int v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(long v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(ushort v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(uint v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(ulong v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(float v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public override void Write(double v)
        {
            if (endian != systemEndian)
            {
                var data = BitConverter.GetBytes(v);
                Array.Reverse(data);
                Write(data);
            }
            base.Write(v);
        }

        public void AlignStream(int alignment = 4)
        {
            var mod = Position % alignment;
            if (mod != 0)
            {
                Write(zerobytes, 0, (int)(alignment - mod));
            }
        }

        public void WriteAlignedString(string value)
        {
            WriteString(value);
            AlignStream();
        }

        public void WriteNullTerminatedString(string value)
        {
            Write(Encoding.UTF8.GetBytes(value));
            Write(false);
        }

        public void WriteString(string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            Write(data.Length);
            Write(data);
        }
    }
}
