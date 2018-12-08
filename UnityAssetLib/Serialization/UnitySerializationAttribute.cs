using System;
using System.Runtime.InteropServices;

namespace UnityAssetLib.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [ComVisible(true)]
    public sealed class UnitySerializableAttribute : Attribute
    {
        public UnitySerializableAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    [ComVisible(true)]
    public sealed class UnityDoNotAlignAttribute : Attribute
    {
        public UnityDoNotAlignAttribute() { }
    }
}
