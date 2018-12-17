﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAssetLib.Types;

namespace UnityAssetLib.Util
{
    class TextureExporter
    {
        const uint DDSD_CAPS        = 0x01;
        const uint DDSD_HEIGHT      = 0x02;
        const uint DDSD_WIDTH       = 0x04;
        const uint DDSD_PITCH       = 0x08;
        const uint DDSD_PIXELFORMAT = 0x1000;
        const uint DDSD_MIPMAPCOUNT = 0x20000;
        const uint DDSD_LINEARSIZE  = 0x80000;
        const uint DDSD_DEPTH       = 0x800000;



        public static string ExportTexture(Texture2D texture, string folder)
        {
            switch (texture.Format)
            {
                case TextureFormat.Alpha8:
                case TextureFormat.ARGB4444:
                case TextureFormat.RGB24:
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                case TextureFormat.RGB565:
                case TextureFormat.R16:
                case TextureFormat.DXT1:
                case TextureFormat.DXT5:
                case TextureFormat.RGBA4444:
                case TextureFormat.BGRA32:
                case TextureFormat.RG16:
                case TextureFormat.R8:
                    return ExportDDS(texture, folder);
                case TextureFormat.DXT1Crunched:
                case TextureFormat.DXT5Crunched:
                case TextureFormat.ETC_RGB4Crunched:
                case TextureFormat.ETC2_RGBA8Crunched:
                    return ".crn";
                case TextureFormat.YUY2:
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.ETC_RGB4:
                case TextureFormat.ETC2_RGB:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.ASTC_RGB_4x4:
                case TextureFormat.ASTC_RGB_5x5:
                case TextureFormat.ASTC_RGB_6x6:
                case TextureFormat.ASTC_RGB_8x8:
                case TextureFormat.ASTC_RGB_10x10:
                case TextureFormat.ASTC_RGB_12x12:
                case TextureFormat.ASTC_RGBA_4x4:
                case TextureFormat.ASTC_RGBA_5x5:
                case TextureFormat.ASTC_RGBA_6x6:
                case TextureFormat.ASTC_RGBA_8x8:
                case TextureFormat.ASTC_RGBA_10x10:
                case TextureFormat.ASTC_RGBA_12x12:
                case TextureFormat.ETC_RGB4_3DS:
                case TextureFormat.ETC_RGBA8_3DS:
                    return ".pvr";
                case TextureFormat.RHalf:
                case TextureFormat.RGHalf:
                case TextureFormat.RGBAHalf:
                case TextureFormat.RFloat:
                case TextureFormat.RGFloat:
                case TextureFormat.RGBAFloat:
                case TextureFormat.BC4:
                case TextureFormat.BC5:
                case TextureFormat.BC6H:
                case TextureFormat.BC7:
                case TextureFormat.EAC_R:
                case TextureFormat.EAC_R_SIGNED:
                case TextureFormat.EAC_RG:
                case TextureFormat.EAC_RG_SIGNED:
                    return ".ktx";
                default:
                    return ".tex";
            }
        }

        private static string ExportDDS(Texture2D texture, string folder)
        {
            throw new NotImplementedException(); // TODO: Implement DDS Export

            byte[] data = GetTextureData(texture);

            string texturePath = Path.Combine(folder, texture.m_Name + ".dds");

            uint dwFlags = DDSD_CAPS | DDSD_WIDTH | DDSD_HEIGHT | DDSD_PIXELFORMAT;

            if (texture.m_MipCount > 0)
            {
                dwFlags |= DDSD_MIPMAPCOUNT;
            }


            using (var writer = new BinaryWriter(File.OpenWrite(texturePath)))
            {
                writer.Write(new byte[] { 0x44, 0x44, 0x53, 0x20, 0x7c }); // DDS Magic = "DDS "

            }


        }

        private static byte[] GetTextureData(Texture2D texture)
        {
            if (texture.imageData.Length != 0)
            {
                return texture.imageData;
            }
            else if (!String.IsNullOrEmpty(texture.m_StreamData.path))
            {
                byte[] buffer = new byte[texture.m_StreamData.size];

                string texturePath = Path.Combine(texture.asset.basepath, texture.m_StreamData.path);

                using (var reader = File.OpenRead(texturePath))
                {
                    reader.Read(buffer, (int)texture.m_StreamData.offset, (int)texture.m_StreamData.size);
                }

                return buffer;
            }

            throw new ArgumentException("Texture2D object has no data information");
        }

    }
}
