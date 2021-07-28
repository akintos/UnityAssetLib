using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityAssetLib.Serialization;

namespace UnityAssetLib.Types
{
    [UnitySerializable]
    public class GameObject : Object
    {
        public PPtr[] m_Component;
        public uint m_Layer;
        public string m_Name;
        public ushort m_Tag;

        [UnityDoNotAlign]
        public bool m_IsActive;
    }
}
