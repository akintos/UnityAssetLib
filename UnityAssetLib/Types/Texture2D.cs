using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class Texture2D : Object
    {
        public TextureFormat Format
        {
            get => (TextureFormat)m_TextureFormat;
            set => m_TextureFormat = (int)value;
        }

        public string m_Name;
        public int m_ForcedFallbackFormat;
        public bool m_DownscaleFallback;
        public int m_Width;
        public int m_Height;
        public int m_CompleteImageSize;
        public int m_TextureFormat;
        public int m_MipCount;
        public bool m_IsReadable;
        public int m_ImageCount;
        public int m_TextureDiemnsion;

        public GLTextureSettings m_TextureSettings;

        public int m_LightmapFormat;
        public int m_ColorSpace;

        public byte[] imageData;

        public StreamingInfo m_StreamData;

        [UnitySerializable]
        public class GLTextureSettings
        {
            public int m_FilterMode;
            public int m_Aniso;
            public float m_MipBias;
            public int m_WrapU;
            public int m_WrapV;
            public int m_WrapW;
        }

        public enum TextureFormat
        {
            /// <summary>
            ///   <para>Alpha-only texture format.</para>
            /// </summary>
            Alpha8 = 1,
            /// <summary>
            ///   <para>A 16 bits/pixel texture format. Texture stores color with an alpha channel.</para>
            /// </summary>
            ARGB4444,
            /// <summary>
            ///   <para>Color texture format, 8-bits per channel.</para>
            /// </summary>
            RGB24,
            /// <summary>
            ///   <para>Color with alpha texture format, 8-bits per channel.</para>
            /// </summary>
            RGBA32,
            /// <summary>
            ///   <para>Color with alpha texture format, 8-bits per channel.</para>
            /// </summary>
            ARGB32,
            /// <summary>
            ///   <para>A 16 bit color texture format.</para>
            /// </summary>
            RGB565 = 7,
            /// <summary>
            ///   <para>A 16 bit color texture format that only has a red channel.</para>
            /// </summary>
            R16 = 9,
            /// <summary>
            ///   <para>Compressed color texture format.</para>
            /// </summary>
            DXT1,
            /// <summary>
            ///   <para>Compressed color with alpha channel texture format.</para>
            /// </summary>
            DXT5 = 12,
            /// <summary>
            ///   <para>Color and alpha  texture format, 4 bit per channel.</para>
            /// </summary>
            RGBA4444,
            /// <summary>
            ///   <para>Color with alpha texture format, 8-bits per channel.</para>
            /// </summary>
            BGRA32,
            /// <summary>
            ///   <para>Scalar (R)  texture format, 16 bit floating point.</para>
            /// </summary>
            RHalf,
            /// <summary>
            ///   <para>Two color (RG)  texture format, 16 bit floating point per channel.</para>
            /// </summary>
            RGHalf,
            /// <summary>
            ///   <para>RGB color and alpha texture format, 16 bit floating point per channel.</para>
            /// </summary>
            RGBAHalf,
            /// <summary>
            ///   <para>Scalar (R) texture format, 32 bit floating point.</para>
            /// </summary>
            RFloat,
            /// <summary>
            ///   <para>Two color (RG)  texture format, 32 bit floating point per channel.</para>
            /// </summary>
            RGFloat,
            /// <summary>
            ///   <para>RGB color and alpha texture format,  32-bit floats per channel.</para>
            /// </summary>
            RGBAFloat,
            /// <summary>
            ///   <para>A format that uses the YUV color space and is often used for video encoding or playback.</para>
            /// </summary>
            YUY2,
            /// <summary>
            ///   <para>RGB HDR format, with 9 bit mantissa per channel and a 5 bit shared exponent.</para>
            /// </summary>
            RGB9e5Float,
            /// <summary>
            ///   <para>Compressed one channel (R) texture format.</para>
            /// </summary>
            BC4 = 26,
            /// <summary>
            ///   <para>Compressed two-channel (RG) texture format.</para>
            /// </summary>
            BC5,
            /// <summary>
            ///   <para>HDR compressed color texture format.</para>
            /// </summary>
            BC6H = 24,
            /// <summary>
            ///   <para>High quality compressed color texture format.</para>
            /// </summary>
            BC7,
            /// <summary>
            ///   <para>Compressed color texture format with Crunch compression for smaller storage sizes.</para>
            /// </summary>
            DXT1Crunched = 28,
            /// <summary>
            ///   <para>Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.</para>
            /// </summary>
            DXT5Crunched,
            /// <summary>
            ///   <para>PowerVR (iOS) 2 bits/pixel compressed color texture format.</para>
            /// </summary>
            PVRTC_RGB2,
            /// <summary>
            ///   <para>PowerVR (iOS) 2 bits/pixel compressed with alpha channel texture format.</para>
            /// </summary>
            PVRTC_RGBA2,
            /// <summary>
            ///   <para>PowerVR (iOS) 4 bits/pixel compressed color texture format.</para>
            /// </summary>
            PVRTC_RGB4,
            /// <summary>
            ///   <para>PowerVR (iOS) 4 bits/pixel compressed with alpha channel texture format.</para>
            /// </summary>
            PVRTC_RGBA4,
            /// <summary>
            ///   <para>ETC (GLES2.0) 4 bits/pixel compressed RGB texture format.</para>
            /// </summary>
            ETC_RGB4,
            /// <summary>
            ///   <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed unsigned single-channel texture format.</para>
            /// </summary>
            EAC_R = 41,
            /// <summary>
            ///   <para>ETC2  EAC (GL ES 3.0) 4 bitspixel compressed signed single-channel texture format.</para>
            /// </summary>
            EAC_R_SIGNED,
            /// <summary>
            ///   <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed unsigned dual-channel (RG) texture format.</para>
            /// </summary>
            EAC_RG,
            /// <summary>
            ///   <para>ETC2  EAC (GL ES 3.0) 8 bitspixel compressed signed dual-channel (RG) texture format.</para>
            /// </summary>
            EAC_RG_SIGNED,
            /// <summary>
            ///   <para>ETC2 (GL ES 3.0) 4 bits/pixel compressed RGB texture format.</para>
            /// </summary>
            ETC2_RGB,
            /// <summary>
            ///   <para>ETC2 (GL ES 3.0) 4 bits/pixel RGB+1-bit alpha texture format.</para>
            /// </summary>
            ETC2_RGBA1,
            /// <summary>
            ///   <para>ETC2 (GL ES 3.0) 8 bits/pixel compressed RGBA texture format.</para>
            /// </summary>
            ETC2_RGBA8,
            /// <summary>
            ///   <para>ASTC (4x4 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_4x4,
            /// <summary>
            ///   <para>ASTC (5x5 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_5x5,
            /// <summary>
            ///   <para>ASTC (6x6 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_6x6,
            /// <summary>
            ///   <para>ASTC (8x8 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_8x8,
            /// <summary>
            ///   <para>ASTC (10x10 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_10x10,
            /// <summary>
            ///   <para>ASTC (12x12 pixel block in 128 bits) compressed RGB texture format.</para>
            /// </summary>
            ASTC_RGB_12x12,
            /// <summary>
            ///   <para>ASTC (4x4 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_4x4,
            /// <summary>
            ///   <para>ASTC (5x5 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_5x5,
            /// <summary>
            ///   <para>ASTC (6x6 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_6x6,
            /// <summary>
            ///   <para>ASTC (8x8 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_8x8,
            /// <summary>
            ///   <para>ASTC (10x10 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_10x10,
            /// <summary>
            ///   <para>ASTC (12x12 pixel block in 128 bits) compressed RGBA texture format.</para>
            /// </summary>
            ASTC_RGBA_12x12,
            /// <summary>
            ///   <para>ETC 4 bits/pixel compressed RGB texture format.</para>
            /// </summary>
            ETC_RGB4_3DS,
            /// <summary>
            ///   <para>ETC 4 bitspixel RGB + 4 bitspixel Alpha compressed texture format.</para>
            /// </summary>
            ETC_RGBA8_3DS,
            /// <summary>
            ///   <para>Two color (RG) texture format, 8-bits per channel.</para>
            /// </summary>
            RG16,
            /// <summary>
            ///   <para>Scalar (R) render texture format, 8 bit fixed point.</para>
            /// </summary>
            R8,
            /// <summary>
            ///   <para>Compressed color texture format with Crunch compression for smaller storage sizes.</para>
            /// </summary>
            ETC_RGB4Crunched,
            /// <summary>
            ///   <para>Compressed color with alpha channel texture format with Crunch compression for smaller storage sizes.</para>
            /// </summary>
            ETC2_RGBA8Crunched
        }

        public enum TextureWrapMode
        {
            /// <summary>
            ///   <para>Tiles the texture, creating a repeating pattern.</para>
            /// </summary>
            Repeat,
            /// <summary>
            ///   <para>Clamps the texture to the last pixel at the edge.</para>
            /// </summary>
            Clamp,
            /// <summary>
            ///   <para>Tiles the texture, creating a repeating pattern by mirroring it at every integer boundary.</para>
            /// </summary>
            Mirror,
            /// <summary>
            ///   <para>Mirrors the texture once, then clamps to edge pixels.</para>
            /// </summary>
            MirrorOnce
        }
    }
}
