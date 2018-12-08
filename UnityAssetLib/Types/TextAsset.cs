using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class TextAsset : Object
    {
        public string m_Name;
        public byte m_Data;
    }
}
