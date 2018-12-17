using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnityAssetLib.Util
{
    static class BinaryWriterExtensions
    {
        public static void AlignStream(this BinaryWriter writer, int alignment = 4)
        {
            var pos = writer.BaseStream.Position;
            var mod = pos % alignment;
            if (mod != 0)
            {
                writer.Write(new byte[alignment - mod]);
            }
        }

        public static void WriteAlignedString(this BinaryWriter writer, string value)
        {
            var data = Encoding.UTF8.GetBytes(value);
            writer.Write(data.Length);
            writer.Write(data);
            writer.AlignStream();
        }
    }
}
