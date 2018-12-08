using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityAssetLib.Util;

namespace UnityAssetLib
{
    public class AssetReference
    {
        public readonly string assetPath;
        public readonly byte[] GUID;
        public readonly int type;
        public readonly string filePath;

        public AssetReference(string assetPath, byte[] GUID, int type, string filePath)
        {
            this.assetPath = assetPath;
            this.GUID = GUID;
            this.type = type;
            this.filePath = filePath;
        }

        public static AssetReference Read(BinaryReader reader)
        {
            string assetPath = reader.ReadStringToNull();
            byte[] GUID = reader.ReadBytes(16);
            int type = reader.ReadInt32();
            string filePath = reader.ReadStringToNull();

            return new AssetReference(assetPath: assetPath, GUID: GUID, type: type, filePath: filePath); 
        }
    }
}
