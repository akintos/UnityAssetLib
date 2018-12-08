using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityAssetLib.Util;

namespace UnityAssetLib
{
    public class TypeTree
    {
        public uint format;

        public int version;
        public bool isArray;
        public int size;
        public int index;
        public int flags;
        public string type;
        public string name;

        public List<TypeTree> children = new List<TypeTree>();

        /*
        public TypeTree(TypeTree parent, uint format, int version, bool isArray, int size, int index, int flags, string type, string name)
        {
            this.parent = parent;
            this.format = format;

            this.version = version;
            this.isArray = isArray;
            this.size = size;
            this.index = index;
            this.flags = flags;
            this.type = type;
            this.name = name;
        }
        */

        public static TypeTree ReadTypeTree(uint format, BinaryReader buf)
        {
            TypeTree root = new TypeTree();

            if (format == 10 || format >= 12)
            {
                int nodesCount = buf.ReadInt32();
                int stringBufferBytes = buf.ReadInt32();

                buf.BaseStream.Seek(24 * nodesCount, SeekOrigin.Current);
                byte[] stringData = buf.ReadBytes(stringBufferBytes);
                buf.BaseStream.Seek(-(24 * nodesCount + stringBufferBytes), SeekOrigin.Current);

                Stack<TypeTree> stack = new Stack<TypeTree>();

                stack.Push(root);

                using (var stringReader = new BinaryReader(new MemoryStream(stringData)))
                {
                    for (int i = 0; i < nodesCount; i++)
                    {
                        short version = buf.ReadInt16();
                        byte depth = buf.ReadByte();

                        bool isArray = buf.ReadBoolean();

                        ushort typeIndex = buf.ReadUInt16();
                        string typeStr;
                        if (buf.ReadUInt16() == 0)
                        {
                            stringReader.BaseStream.Position = typeIndex;
                            typeStr = stringReader.ReadStringToNull();
                        }
                        else
                        {
                            typeStr = baseStrings.ContainsKey(typeIndex) ? baseStrings[typeIndex] : typeIndex.ToString();
                        }

                        ushort nameIndex = buf.ReadUInt16();
                        string nameStr;
                        if (buf.ReadUInt16() == 0)
                        {
                            stringReader.BaseStream.Position = nameIndex;
                            nameStr = stringReader.ReadStringToNull();
                        }
                        else
                        {
                            nameStr = baseStrings.ContainsKey(nameIndex) ? baseStrings[nameIndex] : nameIndex.ToString();
                        }

                        int size = buf.ReadInt32();
                        int index = buf.ReadInt32();
                        int flags = buf.ReadInt32();

                        TypeTree t;
                        if (depth == 0)
                        {
                            t = root;
                        }
                        else
                        {
                            while (stack.Count > depth)
                                stack.Pop();
                            t = new TypeTree();
                            stack.Peek().children.Add(t);
                            stack.Push(t);
                        }

                        t.version = version;
                        t.isArray = isArray;
                        t.type = typeStr;
                        t.name = nameStr;
                        t.size = size;
                        t.index = index;
                        t.flags = flags;
                    }
                }

                buf.BaseStream.Seek(stringBufferBytes, SeekOrigin.Current);
            }
            else
            {
                root.type = buf.ReadStringToNull();
                root.name = buf.ReadStringToNull();
                root.size = buf.ReadInt32();
                root.index = buf.ReadInt32();
                root.isArray = buf.ReadBoolean();
                root.version = buf.ReadInt32();
                root.flags = buf.ReadInt32();

                int childCount = buf.ReadInt32();
                for (int i = 0; i < childCount; i++)
                {
                    root.children.Add(TypeTree.ReadTypeTree(format, buf));
                }
            }

            return root;
        }

        #region base strings
        private static readonly Dictionary<int, string> baseStrings = new Dictionary<int, string>
        {
            {0, "AABB"},
            {5, "AnimationClip"},
            {19, "AnimationCurve"},
            {34, "AnimationState"},
            {49, "Array"},
            {55, "Base"},
            {60, "BitField"},
            {69, "bitset"},
            {76, "bool"},
            {81, "char"},
            {86, "ColorRGBA"},
            {96, "Component"},
            {106, "data"},
            {111, "deque"},
            {117, "double"},
            {124, "dynamic_array"},
            {138, "FastPropertyName"},
            {155, "first"},
            {161, "float"},
            {167, "Font"},
            {172, "GameObject"},
            {183, "Generic Mono"},
            {196, "GradientNEW"},
            {208, "GUID"},
            {213, "GUIStyle"},
            {222, "int"},
            {226, "list"},
            {231, "long long"},
            {241, "map"},
            {245, "Matrix4x4f"},
            {256, "MdFour"},
            {263, "MonoBehaviour"},
            {277, "MonoScript"},
            {288, "m_ByteSize"},
            {299, "m_Curve"},
            {307, "m_EditorClassIdentifier"},
            {331, "m_EditorHideFlags"},
            {349, "m_Enabled"},
            {359, "m_ExtensionPtr"},
            {374, "m_GameObject"},
            {387, "m_Index"},
            {395, "m_IsArray"},
            {405, "m_IsStatic"},
            {416, "m_MetaFlag"},
            {427, "m_Name"},
            {434, "m_ObjectHideFlags"},
            {452, "m_PrefabInternal"},
            {469, "m_PrefabParentObject"},
            {490, "m_Script"},
            {499, "m_StaticEditorFlags"},
            {519, "m_Type"},
            {526, "m_Version"},
            {536, "Object"},
            {543, "pair"},
            {548, "PPtr<Component>"},
            {564, "PPtr<GameObject>"},
            {581, "PPtr<Material>"},
            {596, "PPtr<MonoBehaviour>"},
            {616, "PPtr<MonoScript>"},
            {633, "PPtr<Object>"},
            {646, "PPtr<Prefab>"},
            {659, "PPtr<Sprite>"},
            {672, "PPtr<TextAsset>"},
            {688, "PPtr<Texture>"},
            {702, "PPtr<Texture2D>"},
            {718, "PPtr<Transform>"},
            {734, "Prefab"},
            {741, "Quaternionf"},
            {753, "Rectf"},
            {759, "RectInt"},
            {767, "RectOffset"},
            {778, "second"},
            {785, "set"},
            {789, "short"},
            {795, "size"},
            {800, "SInt16"},
            {807, "SInt32"},
            {814, "SInt64"},
            {821, "SInt8"},
            {827, "staticvector"},
            {840, "string"},
            {847, "TextAsset"},
            {857, "TextMesh"},
            {866, "Texture"},
            {874, "Texture2D"},
            {884, "Transform"},
            {894, "TypelessData"},
            {907, "UInt16"},
            {914, "UInt32"},
            {921, "UInt64"},
            {928, "UInt8"},
            {934, "unsigned int"},
            {947, "unsigned long long"},
            {966, "unsigned short"},
            {981, "vector"},
            {988, "Vector2f"},
            {997, "Vector3f"},
            {1006, "Vector4f"},
            {1015, "m_ScriptingClassIdentifier"},
            {1042, "Gradient"},
            {1051, "Type*"}
        };
        #endregion
    }
}
