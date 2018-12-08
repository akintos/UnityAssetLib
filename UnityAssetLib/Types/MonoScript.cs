using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class MonoScript : Object
    {
        public string m_Name;
        public int m_ExecutionOrder;
        public Hash128 m_PropertiesHash;
        public string m_ClassName;
        public string m_Namespace;
    }
}
