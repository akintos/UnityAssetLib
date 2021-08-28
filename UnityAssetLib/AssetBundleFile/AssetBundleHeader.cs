using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityAssetLib.IO;
using UnityAssetLib.Compression;

namespace UnityAssetLib
{
    public class AssetBundleHeader
    {
        public string signature;
        public uint fileVersion;
        public string minPlayerVersion;
        public string fileEngineVersion;
        public long totalFileSize;
        public uint compressedSize;
        public uint decompressedSize;
        public BundleFlag flags;

        public void Read(ExtendedBinaryReader br)
        {
            var prevEndian = br.endian;
            br.endian = EndianType.BigEndian;

            signature = br.ReadNullTerminatedString();
            fileVersion = br.ReadUInt32();
            minPlayerVersion = br.ReadNullTerminatedString();
            fileEngineVersion = br.ReadNullTerminatedString();
            totalFileSize = br.ReadInt64();
            compressedSize = br.ReadUInt32();
            decompressedSize = br.ReadUInt32();
            flags = (BundleFlag)br.ReadUInt32();

            br.endian = prevEndian;
        }

        public void Write(ExtendedBinaryWriter bw)
        {
            var prevEndian = bw.endian;
            bw.endian = EndianType.BigEndian;

            bw.WriteNullTerminatedString(signature);
            bw.Write(fileVersion);
            bw.WriteNullTerminatedString(minPlayerVersion);
            bw.WriteNullTerminatedString(fileEngineVersion);
            bw.Write(totalFileSize);
            bw.Write(compressedSize);
            bw.Write(decompressedSize);
            bw.Write((uint)flags);

            bw.endian = prevEndian;
        }

        public bool IsCompressed()
        {
            return GetCompressionType() == CompressionType.None;
        }

        public CompressionType GetCompressionType()
        {
            return (CompressionType)(flags & BundleFlag.kArchiveCompressionTypeMask);
        }

        [Flags]
        public enum BundleFlag : uint
        {
            kArchiveBlocksInfoAtTheEnd = 0x80u,
            kArchiveBlocksAndDirectoryInfoCombined = 0x40u,
            kArchiveCompressionTypeMask = 0x3Fu,
        }
    }
}
