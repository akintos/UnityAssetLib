using System;
using System.Text;
using UnityAssetLib.Util;

namespace UnityAssetLib
{
    public class AssetInfo
    {
        public AssetsFile asset;

        public long pathID;
        public uint dataOffset;
        public uint size;

        public int typeID;
        public int classID;

        public bool isDestroyed;
        public byte stripped;

        public override string ToString()
        {
            return pathID + ":" + TypeString;
        }

        public string TypeString
        {
            get
            {
                if (IsKnownType)
                {
                    return ((ClassIDType)classID).ToString();
                }
                else
                {
                    return "UnknownType(" + classID + ")";
                }
            }
        }

        public uint RealOffset
        {
            get => dataOffset + asset.data_offset;
        }

        public ClassIDType ClassIDType
        {
            get
            {
                if (IsKnownType)
                {
                    return ((ClassIDType)classID);
                }
                else
                {
                    return ClassIDType.UnknownType;
                }
            }
        }

        public bool IsKnownType
        {
            get => Enum.IsDefined(typeof(ClassIDType), classID);
        }

        public EndianBinaryReader InitReader()
        {
            var buf = asset.buf;
            buf.Position = RealOffset;
            return buf;
        }

        public string TryGetName()
        {
            if (IsKnownType)
            {
                var id = (ClassIDType)classID;
                int namePosition = 0;

                var buf = InitReader();

                switch (id)
                {
                    case ClassIDType.GameObject:
                        int componentCount = buf.ReadInt32();
                        buf.Position += componentCount * 0xC + 4;
                        return buf.ReadAlignedString();

                    case ClassIDType.MonoBehaviour:
                        namePosition = 0x1C;
                        break;

                    case ClassIDType.TextAsset:
                    case ClassIDType.MonoScript:
                    case ClassIDType.Font:
                    case ClassIDType.Texture2D:
                        namePosition = 0x00;
                        break;

                    default:
                        return null;
                }

                buf.Position += namePosition;
                int nameLength = buf.ReadInt32();

                if (nameLength > 128 || nameLength < 0)
                {
                    return null;
                }
                else if (nameLength == 0)
                {
                    return "";
                }
                
                try
                {
                    var nameBytes = buf.ReadBytes(nameLength);
                    return Encoding.UTF8.GetString(nameBytes);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
