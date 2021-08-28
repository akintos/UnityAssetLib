using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityAssetLib.Compression
{
    public enum CompressionType : uint
    {
        None = 0u,
        LZMA = 1u,
        LZ4 = 2u,
        LZ4HC = 3u,
    }
}
