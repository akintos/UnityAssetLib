using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityAssetLib.IO;

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

        public static AssetReference Read(ExtendedBinaryReader br)
        {
            string assetPath = br.ReadNullTerminatedString();
            byte[] GUID = br.ReadBytes(16);
            int type = br.ReadInt32();
            string filePath = br.ReadNullTerminatedString();

            return new AssetReference(assetPath: assetPath, GUID: GUID, type: type, filePath: filePath); 
        }
    }
}
