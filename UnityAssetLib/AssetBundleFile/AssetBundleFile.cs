using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityAssetLib.IO;
using UnityAssetLib.Compression;

namespace UnityAssetLib
{
    public class AssetBundleFile : IDisposable
    {
        public const int BLOCKSIZE = 0x2000;

        private readonly ExtendedBinaryReader br;

        private readonly AssetBundleHeader Header;

        private readonly List<BlockInfo> Blocks = new List<BlockInfo>();
        private readonly List<FileInfo> FileInfoList = new List<FileInfo>();

        public List<string> FileList => FileInfoList.Select(x => x.path).ToList();

        public static AssetBundleFile Open(string path)
        {
            var file = File.OpenRead(path);
            return new AssetBundleFile(file);
        }

        public bool IsCompressed => Header.IsCompressed();

        public CompressionType CompressionType => Header.GetCompressionType();

        private AssetBundleFile(Stream stream)
        {
            br = new ExtendedBinaryReader(stream, EndianType.BigEndian);

            var sig = br.ReadNullTerminatedString();
            if (sig != "UnityFS")
                throw new IOException($"Unsupported AssetBundle signature {sig}");

            var ver = br.ReadInt32();
            if (ver < 6 || ver > 7)
                throw new IOException($"Unsupported AssetBundle format version {ver}, only version 6 and 7 are supported");

            br.Position = 0;
            this.Header = new AssetBundleHeader();
            this.Header.Read(br);

            if (Header.fileVersion >= 7)
                br.AlignStream(16);

            byte[] blocksInfoBytes;
            if ((Header.flags & AssetBundleHeader.BundleFlag.kArchiveBlocksInfoAtTheEnd) != 0)
            {
                var position = br.Position;
                br.Position = br.BaseStream.Length - Header.compressedSize;
                blocksInfoBytes = br.ReadBytes((int)Header.compressedSize);
                br.Position = position;
            }
            else // 0x40 kArchiveBlocksAndDirectoryInfoCombined
            {
                blocksInfoBytes = br.ReadBytes((int)Header.compressedSize);
            }

            blocksInfoBytes = CompressionHelper.Decompress(Header.GetCompressionType(), blocksInfoBytes);

            ReadBlocksInfo(blocksInfoBytes);
        }

        private void ReadBlocksInfo(byte[] blocksInfoBytes)
        {
            using var ms = new MemoryStream(blocksInfoBytes);
            using var br = new ExtendedBinaryReader(ms, EndianType.BigEndian);

            var hash = br.ReadBytes(16);

            int blockCount = br.ReadInt32();
            for (int i = 0; i < blockCount; i++)
            {
                var block = new BlockInfo()
                {
                    decompressedSize = br.ReadInt32(),
                    compressedSize = br.ReadInt32(),
                    flags = br.ReadUInt16(),
                };
                Blocks.Add(block);
            }

            int fileCount = br.ReadInt32();
            for (int i = 0; i < fileCount; i++)
            {
                var file = new FileInfo()
                {
                    offset = br.ReadInt64(),
                    size = br.ReadInt64(),
                    flags = br.ReadUInt32(),
                    path = br.ReadNullTerminatedString(),
                };
                FileInfoList.Add(file);
            }
        }

        public IList<string> ExtractAll(string outputDirectory)
        {
            var result = new List<string>();

            for (int i = 0; i < FileList.Count; i++)
            {
                ExtractFile(outputDirectory, FileList[i]);
            }
        }

        public void ExtractFile(string outputPath, string fileName)
        {
            FileInfo fileinfo = FileInfoList.FirstOrDefault(x => x.path == fileName);
            if (fileinfo == default(FileInfo))
                throw new ArgumentException($"File {fileName} does not exist");

            using (var file = File.Create(outputPath))
                ExtractFile(file, fileinfo);

            throw new NotImplementedException();
        }

        private void ExtractFile(Stream outputStream, FileInfo file)
        {

        }

        private byte[] GetBlockData(BlockInfo block)
        {

        }

        public void Dispose()
        {
            br.Dispose();
        }

        private class BlockInfo
        {
            internal int decompressedSize;
            internal int compressedSize;
            internal ushort flags;
        }

        private class FileInfo
        {
            internal long offset;
            internal long size;
            internal uint flags;
            internal string path;
        }
    }
}
