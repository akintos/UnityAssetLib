using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib.Types;
using UnityAssetLib.Util;

namespace UnityAssetLib.Texture
{
    internal class DDSExporter
    {
        private const uint DDSD_CAPS = 0x01;
        private const uint DDSD_HEIGHT = 0x02;
        private const uint DDSD_WIDTH = 0x04;
        private const uint DDSD_PITCH = 0x08;
        private const uint DDSD_PIXELFORMAT = 0x1000;
        private const uint DDSD_MIPMAPCOUNT = 0x20000;
        private const uint DDSD_LINEARSIZE = 0x80000;
        private const uint DDSD_DEPTH = 0x800000;

        private const uint DDSCAPS_COMPLEX = 0x08;
        private const uint DDSCAPS_MIPMAP = 0x400000;
        private const uint DDSCAPS_TEXTURE = 0x1000;

        private const uint DDPF_ALPHAPIXELS = 0x1;
        private const uint DDPF_ALPHA = 0x2;
        private const uint DDPF_FOURCC = 0x4;
        private const uint DDPF_RGB = 0x40;
        private const uint DDPF_YUV = 0x200;
        private const uint DDPF_LUMINANCE = 0x20000;

        private static readonly uint[] DDS_GENERATOR_DATA = 
            { 0x74696E55, 0x73734179, 0x694C7465, 0x53444462, 0, 0, 0, 0, 0, 0, 0};

        private struct DDS_HEADER
        {
            internal uint dwSize;
            internal uint dwFlags;
            internal uint dwHeight;
            internal uint dwWidth;
            internal uint dwPitchOrLinearSize;
            internal uint dwDepth;
            internal uint dwMipMapCount;
            internal uint[] dwReserved;
            internal DDS_PIXELFORMAT ddspf;
            internal uint dwCaps;
            internal uint dwCaps2;
            internal uint dwCaps3;
            internal uint dwCaps4;
            internal uint dwReserved2;
        }

        private struct DDS_PIXELFORMAT
        {
            internal uint dwSize;
            internal uint dwFlags;
            internal uint dwFourCC;
            internal uint dwRGBBitCount;
            internal uint dwRBitMask;
            internal uint dwGBitMask;
            internal uint dwBBitMask;
            internal uint dwABitMask;
        }

        internal static string ExportDDSTexture(Texture2D texture, string directory, bool overwrite = false)
        {
            if (texture.Format != TextureFormat.Alpha8 &&
                texture.Format != TextureFormat.ARGB4444 &&
                texture.Format != TextureFormat.RGB24 &&
                texture.Format != TextureFormat.RGBA32 &&
                texture.Format != TextureFormat.ARGB32 &&
                texture.Format != TextureFormat.RGB565 &&
                texture.Format != TextureFormat.R16 &&
                texture.Format != TextureFormat.DXT1 &&
                texture.Format != TextureFormat.DXT5 &&
                texture.Format != TextureFormat.RGBA4444 &&
                texture.Format != TextureFormat.BGRA32 &&
                texture.Format != TextureFormat.RG16 &&
                texture.Format != TextureFormat.R8)
            {
                throw new UnsupportedFormatException($"Cannot export {texture.Format.ToString()} to DDS format.");
            }

            string exportPath = Path.Combine(directory, $"{texture.m_Name}.dds");

            if (File.Exists(exportPath) && !overwrite)
            {
                throw new IOException($"File {exportPath} already exist and overwrite argument is not set true.");
            }

            byte[] textureData = Texture2DExporter.GetTextureData(texture);

            DDS_HEADER ddsHeader = new DDS_HEADER()
            {
                dwSize              = 0x7C,
                dwFlags             = DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT,
                dwHeight            = (uint)texture.m_Height,
                dwWidth             = (uint)texture.m_Width,
                dwPitchOrLinearSize = (uint)texture.m_CompleteImageSize,
                dwDepth             = 0,
                dwMipMapCount       = (uint)texture.m_MipCount,
                dwReserved          = DDS_GENERATOR_DATA,
                dwCaps              = DDSCAPS_TEXTURE,
                dwCaps2             = 0,
                dwCaps3             = 0,
                dwCaps4             = 0,
                dwReserved2         = 0,
            };

            DDS_PIXELFORMAT ddspf = new DDS_PIXELFORMAT()
            {
                dwSize = 0x20,
                dwFlags = 0,
                dwFourCC = 0,
                dwRGBBitCount = 0,
                dwRBitMask = 0,
                dwGBitMask = 0,
                dwBBitMask = 0,
                dwABitMask = 0,
            };

            if (texture.HasMips)
            {
                ddsHeader.dwFlags |= DDSD_MIPMAPCOUNT;
                ddsHeader.dwCaps |= DDSCAPS_MIPMAP;
            }

            int blocksize = 0;

            switch (texture.Format)
            {
                case TextureFormat.Alpha8:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS;
                        ddspf.dwRGBBitCount = 0x8;
                        ddspf.dwABitMask = 0xF;
                        blocksize = 8;
                        break;
                    }
                case TextureFormat.ARGB4444:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS | DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x10;
                        ddspf.dwRBitMask = 0xFu << 8;
                        ddspf.dwGBitMask = 0xFu << 4;
                        ddspf.dwBBitMask = 0xFu;
                        ddspf.dwABitMask = 0xFu << 12;
                        blocksize = 16;
                        break;
                    }
                case TextureFormat.RGB24:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x18;
                        ddspf.dwRBitMask = 0xFFu << 16;
                        ddspf.dwGBitMask = 0xFFu << 8;
                        ddspf.dwBBitMask = 0xFFu;
                        blocksize = 24;
                        break;
                    }
                case TextureFormat.RGBA32:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS | DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x20;
                        ddspf.dwRBitMask = 0xFFu << 24;
                        ddspf.dwGBitMask = 0xFFu << 16;
                        ddspf.dwBBitMask = 0xFFu << 8;
                        ddspf.dwABitMask = 0xFFu;
                        blocksize = 32;
                        break;
                    }
                case TextureFormat.ARGB32:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS | DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x20;
                        ddspf.dwRBitMask = 0xFFu << 16;
                        ddspf.dwGBitMask = 0xFFu << 8;
                        ddspf.dwBBitMask = 0xFFu;
                        ddspf.dwABitMask = 0xFFu << 24;
                        blocksize = 32;
                        break;
                    }
                case TextureFormat.RGB565:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x10;
                        ddspf.dwRBitMask = 0b1111_1000_0000_0000;
                        ddspf.dwGBitMask = 0b0000_0111_1110_0000;
                        ddspf.dwBBitMask = 0b0000_0000_0001_1111;
                        blocksize = 16;
                        break;
                    }
                case TextureFormat.R16:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x10;
                        ddspf.dwRBitMask = 0xFFFF;
                        blocksize = 16;
                        break;
                    }
                case TextureFormat.DXT1:
                    {
                        ddsHeader.dwFlags |= DDSD_LINEARSIZE;
                        ddspf.dwFlags = DDPF_FOURCC;
                        ddspf.dwFourCC = 0x31545844; // DXT1
                        blocksize = 16;
                        break;
                    }
                case TextureFormat.DXT5:
                    {
                        ddsHeader.dwFlags |= DDSD_LINEARSIZE;
                        ddspf.dwFlags = DDPF_FOURCC;
                        ddspf.dwFourCC = 0x35545844; // DXT5
                        break;
                    }
                case TextureFormat.RGBA4444:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS | DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x10;
                        ddspf.dwRBitMask = 0xFu << 12;
                        ddspf.dwGBitMask = 0xFu << 8;
                        ddspf.dwBBitMask = 0xFu << 4;
                        ddspf.dwABitMask = 0xFu;
                        break;
                    }
                case TextureFormat.BGRA32:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_ALPHAPIXELS | DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x20;
                        ddspf.dwRBitMask = 0xFFu << 8;
                        ddspf.dwGBitMask = 0xFFu << 16;
                        ddspf.dwBBitMask = 0xFFu << 24;
                        ddspf.dwABitMask = 0xFFu;
                        break;
                    }
                case TextureFormat.RG16:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x10;
                        ddspf.dwRBitMask = 0xFF00;
                        ddspf.dwGBitMask = 0x00FF;
                        break;
                    }
                case TextureFormat.R8:
                    {
                        ddsHeader.dwFlags |= DDSD_PITCH;
                        ddspf.dwFlags = DDPF_RGB;
                        ddspf.dwRGBBitCount = 0x8;
                        ddspf.dwRBitMask = 0xF;
                        break;
                    }
                default:
                    throw new UnsupportedFormatException($"Cannot export {texture.Format} to DDS format.");
            }

            ddsHeader.ddspf = ddspf;

            using (var writer = new BinaryWriter(File.OpenWrite(exportPath)))
            {
                writer.Write(0x20534444u); // 'DDS '
                writer.Write(ddsHeader.dwSize);
                writer.Write(ddsHeader.dwFlags);
                writer.Write(ddsHeader.dwHeight);
                writer.Write(ddsHeader.dwWidth);
                writer.Write(ddsHeader.dwPitchOrLinearSize);
                writer.Write(ddsHeader.dwDepth);
                writer.Write(ddsHeader.dwMipMapCount);
                for (int i = 0; i < ddsHeader.dwReserved.Length; i++)
                    writer.Write(ddsHeader.dwReserved[i]);
                writer.Write(ddsHeader.ddspf.dwSize);
                writer.Write(ddsHeader.ddspf.dwFlags);
                writer.Write(ddsHeader.ddspf.dwFourCC);
                writer.Write(ddsHeader.ddspf.dwRGBBitCount);
                writer.Write(ddsHeader.ddspf.dwRBitMask);
                writer.Write(ddsHeader.ddspf.dwGBitMask);
                writer.Write(ddsHeader.ddspf.dwBBitMask);
                writer.Write(ddsHeader.ddspf.dwABitMask);
                writer.Write(ddsHeader.dwCaps);
                writer.Write(ddsHeader.dwCaps2);
                writer.Write(ddsHeader.dwCaps3);
                writer.Write(ddsHeader.dwCaps4);
                writer.Write(ddsHeader.dwReserved2);
                writer.Write(textureData);
            }

            return exportPath;
        }

        public static byte[] ReverseDataBlockArray(byte[] data, int blocksize)
        {
            int dataSize = data.Length;
            if ((dataSize % blocksize) != 0)
            {
                throw new ArgumentException($"Invalid blocksize : data size is not multiple of {blocksize}");
            }
            int blockCount = dataSize / blocksize;

            byte[] reversed = new byte[data.Length];

            for (int i = 0; i < blockCount; i++)
            {
                Buffer.BlockCopy(data, i * blocksize, reversed, (blockCount - i - 1) * blocksize, blocksize);
            }

            return reversed;
        }

        public class UnsupportedFormatException : Exception
        {
            public UnsupportedFormatException(string message) : base(message) { }
        }

    }
}
