using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class PPtr
    {
        //m_FileID 0 means current file
        public int m_FileID;
        //m_PathID acts more like a hash in some games
        public long m_PathID;
    }

    [UnitySerializable]
    public class Rectf
    {
        public float x;
        public float y;
        public float width;
        public float height;
    }

    [UnitySerializable]
    public class StreamingInfo
    {
        public uint offset;
        public uint size;
        public string path;
    }

    [UnitySerializable]
    public class Hash128
    {
        public uint m_u32_0;
        public uint m_u32_1;
        public uint m_u32_2;
        public uint m_u32_3;

        public byte[] GetBytes()
        {
            byte[] result = new byte[0x10];

            BitConverter.GetBytes(m_u32_0).CopyTo(result, 0x00);
            BitConverter.GetBytes(m_u32_1).CopyTo(result, 0x04);
            BitConverter.GetBytes(m_u32_2).CopyTo(result, 0x08);
            BitConverter.GetBytes(m_u32_3).CopyTo(result, 0x0C);

            return result;
        }

        public static Hash128 FromBytes(byte[] data)
        {
            if (data?.Length != 16)
            {
                throw new ArgumentException("data must be 16byte array");
            }

            var ret = new Hash128
            {
                m_u32_0 = BitConverter.ToUInt32(data, 0x00),
                m_u32_1 = BitConverter.ToUInt32(data, 0x04),
                m_u32_2 = BitConverter.ToUInt32(data, 0x08),
                m_u32_3 = BitConverter.ToUInt32(data, 0x0C)
            };

            return ret;
        }
    }
}
