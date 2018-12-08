using UnityAssetLib.Serialization;


namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class Font : Object
    {
        public string m_Name;
        public float m_LineSpacing;

        public PPtr m_DefaultMaterial;
        public float m_FontSize;
        public PPtr m_Texture;
        public int m_AsciiStartOffset;
        public float m_Tracking;
        public int m_CharacterSpacing;
        public int m_CharacterPadding;
        public int m_ConvertCase;

        public CharacterInfo[] m_CharacterRects;

        public KerningPair[] m_KerningValues;

        public float m_PixelScale;
        
        public byte[] m_FontData;

        public float m_Ascent;
        public float m_Descent;

        public uint m_DefaultStyle;

        public string[] m_FontNames;

        public PPtr[] m_FallbackFonts;

        public int m_FontRenderingMode;

        public bool m_UseLegacyBoundsCalculation;
        public bool m_ShouldRoundAdvanceValue;

        [UnitySerializable]
        public class CharacterInfo
        {
            public uint index;
            public Rectf uv;
            public Rectf vert;
            public float advance;
            public bool flipped;
        }

        [UnitySerializable]
        public class KerningPair
        {
            public ushort firstGlyph;
            public ushort secondGlyph;
            public float adjustment;
        }
    }
}
