using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib.Compression
{
    public static class CompressionHelper
    {
        public static byte[] DecompressLZ4(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var dec = new Lz4.Lz4DecoderStream(ms, data.Length))
            using (var ms2 = new MemoryStream())
            {
                dec.CopyTo(ms2);
                return ms2.ToArray();
            }
        }

        public static byte[] Decompress(CompressionType type, byte[] data)
        {
            switch (type)
            {
                case CompressionType.None:
                    return data;

                case CompressionType.LZMA:
                    return SevenZip.SevenZipHelper.Decompress(data); // TODO: LZMA decompression untested!

                case CompressionType.LZ4:
                case CompressionType.LZ4HC:
                    return DecompressLZ4(data);
            }

            throw new ArgumentException($"Unsupported CompressionType {type}");
        }
    }
}
