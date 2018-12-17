using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;
using UnityAssetLib.Types;

namespace UnityAssetLib.ExternalTypes
{
    [UnitySerializable]
    public class TMP_FontAsset : MonoBehaviour
    {
        public int hashCode;
        public PPtr material;
        public int materialHashCode;
        public int fontAssetType;

        public FaceInfo m_fontInfo;

        public PPtr atlas;

        public GlyphInfo[] m_glyphInfoList;

        public KerningPair[] m_kerningInfo;
        public KerningPair m_kerningPair;

        public PPtr[] fallbackFontAssets;

        public FontCreationSetting fontCreationSetting;

        public FontWeights[] fontWeights;

        public float normalStyle;
        public float normalSpacingOffset;

        public float boldStylef;
        public float boldSpacing;

        public byte italicStyle;

        public byte tabSize;


        [UnitySerializable]
        public class FaceInfo
        {
            public string Name;
            public float PointSize;
            public float Scale;
            public int CharacterCount;
            public float LineHeight;
            public float Baseline;
            public float Ascender;
            public float CapHeight;
            public float Descender;
            public float CenterLine;
            public float SuperscriptOffset;
            public float SubscriptOffset;
            public float SubSize;
            public float Underline;
            public float UnderlineThickness;
            public float strikethrough;
            public float strikethroughThickness;
            public float TabWidth;
            public float Padding;
            public float AtlasWidth;
            public float AtlasHeight;
        }

        [UnitySerializable]
        public class GlyphInfo
        {
            public int id;

            public float x;
            public float y;

            public float width;
            public float height;

            public float xOffset;
            public float yOffset;

            public float xAdvance;

            public float scale;
        }

        [UnitySerializable]
        public class KerningPair
        {
            public uint m_FirstGlyph;
            public Rectf m_FirstGlyphAdjustments;

            public uint m_SecondGlyph;
            public Rectf m_SecondGlyphAdjustments;

            public float xOffset;
        }

        [UnitySerializable]
        public class FontCreationSetting
        {
            public string fontSourcePath;
            public int fontSizingMode;
            public int fontSize;
            public int fontPadding;
            public int fontPackingMode;
            public int fontAtlasWidth;
            public int fontAtlasHeight;
            public int fontCharacterSet;
            public int fontStyle;
            public float fontStlyeModifier;
            public int fontRenderMode;
            public bool fontKerning;
        }

        [UnitySerializable]
        public class FontWeights
        {
            public PPtr regularTypeface;
            public PPtr italicTypeface;
        }
    }
}
