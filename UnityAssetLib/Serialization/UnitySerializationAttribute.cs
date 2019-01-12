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

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    [ComVisible(true)]
    public sealed class UnityMinVersionAttribute : Attribute
    {
        internal readonly Version version;

        public UnityMinVersionAttribute(int major) { this.version = new Version(major, 0); }
        public UnityMinVersionAttribute(int major, int minor) { this.version = new Version(major, minor); }
        public UnityMinVersionAttribute(int major, int minor, int build) { this.version = new Version(major, minor, build); }
        public UnityMinVersionAttribute(int major, int minor, int build, int revision) { this.version = new Version(major, minor, build, revision); }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    [ComVisible(true)]
    public sealed class UnityMaxVersionAttribute : Attribute
    {
        internal readonly Version version;

        public UnityMaxVersionAttribute(int major) { this.version = new Version(major, 0); }
        public UnityMaxVersionAttribute(int major, int minor) { this.version = new Version(major, minor); }
        public UnityMaxVersionAttribute(int major, int minor, int build) { this.version = new Version(major, minor, build); }
        public UnityMaxVersionAttribute(int major, int minor, int build, int revision) { this.version = new Version(major, minor, build, revision); }
    }
}
