using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class MonoBehaviour : Object
    {
        public PPtr m_GameObject;
        public byte m_Enabled;
        public PPtr m_Script;
        public string m_Name;
    }
}
