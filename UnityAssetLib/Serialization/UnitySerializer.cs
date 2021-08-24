using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityAssetLib.IO;
using UnityAssetLib.Types;

namespace UnityAssetLib.Serialization
{
    public class UnitySerializer
    {
        private readonly Version version;

        public UnitySerializer(AssetsFile assetsFile)
        {
            this.version = assetsFile.version;
        }

        public UnitySerializer(Version version)
        {
            this.version = version;
        }

        public T Deserialize<T>(byte[] data) where T : Types.Object
        {
            using (var ms = new MemoryStream(data, false))
            using (var reader = new ExtendedBinaryReader(ms))
            {
                return (T)Deserialize(typeof(T), reader);
            }
        }

        public T Deserialize<T>(AssetInfo assetInfo, bool fullDeserialize = true) where T : Types.Object
        {
            var ret = (T)Deserialize(typeof(T), assetInfo, fullDeserialize);
            ret.asset = assetInfo.asset;
            return ret;
        }

        public object Deserialize(Type classType, AssetInfo assetInfo, bool fullDeserialize = true)
        {
            var reader = assetInfo.InitReader();

            var startPos = reader.Position;

            var ret = Deserialize(classType, reader);
            reader.AlignStream();

            long readSize = (reader.Position - startPos);

            if (fullDeserialize && readSize != assetInfo.size)
            {
                throw new UnitySerializationException("Failed to fully deserialize " + classType.FullName);
            }

            return ret;
        }

        public object Deserialize(Type classType, ExtendedBinaryReader br, object obj = null)
        {
            if (!Attribute.IsDefined(classType, typeof(UnitySerializableAttribute)))
            {
                throw new UnitySerializationException("Not deserializable type : " + classType.FullName);
            }

            if (obj == null)
            {
                obj = Activator.CreateInstance(classType);
            }

            // Deserialize base type first because Type.GetFields() returns base fields last
            if (Attribute.IsDefined(classType.BaseType, typeof(UnitySerializableAttribute)))
            {
                Deserialize(classType.BaseType, br, obj);
            }

            foreach (var field in classType.GetFields())
            {
                if (!field.DeclaringType.Equals(classType))
                    continue;

                if (field.IsLiteral || field.IsInitOnly)
                    continue;

                Type fieldType = field.FieldType;

                if (Attribute.IsDefined(field, typeof(UnityMinVersionAttribute)))
                {
                    var minAttrib = Attribute.GetCustomAttribute(field, typeof(UnityMinVersionAttribute)) as UnityMinVersionAttribute;
                    if (minAttrib.version > version)
                    {
                        field.SetValue(obj, GetDefault(fieldType));
                        continue;
                    }
                }

                if (Attribute.IsDefined(field, typeof(UnityMaxVersionAttribute)))
                {
                    var maxAttrib = Attribute.GetCustomAttribute(field, typeof(UnityMaxVersionAttribute)) as UnityMaxVersionAttribute;
                    if (maxAttrib.version < version)
                    {
                        field.SetValue(obj, GetDefault(fieldType));
                        continue;
                    }
                }

                object value = null;

                if (fieldType.IsEnum)
                {
                    var enumType = Enum.GetUnderlyingType(fieldType);
                    value = ReadValueType(enumType, br, Attribute.IsDefined(field, typeof(UnityDoNotAlignAttribute)));
                }
                else if (fieldType.IsValueType)
                {
                    value = ReadValueType(fieldType, br, Attribute.IsDefined(field, typeof(UnityDoNotAlignAttribute)));
                }
                else if (fieldType.IsArray && fieldType.GetElementType().IsValueType) // Value type array
                {
                    value = ReadValueArray(fieldType.GetElementType(), br);
                }
                else if (fieldType == typeof(string))
                {
                    value = br.ReadAlignedString();
                }
                else if (fieldType.IsClass || Attribute.IsDefined(fieldType, typeof(UnitySerializableAttribute)))
                {
                    if (fieldType.IsArray)
                    {
                        var elementType = fieldType.GetElementType();

                        value = ReadArray(elementType, br);
                    }
                    else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elementType = fieldType.GetGenericArguments()[0];

                        value = Activator.CreateInstance(fieldType, (IEnumerable)ReadArray(elementType, br));
                    }
                    else
                    {
                        value = Deserialize(fieldType, br);
                    }
                }
                else
                {
                    throw new IOException("Failed to deserialize, unknown type : " + fieldType.ToString());
                }

                field.SetValue(obj, value);
            }

            return obj;
        }

        private Array ReadArray(Type elementType, ExtendedBinaryReader br)
        {
            br.AlignStream();
            int size = br.ReadInt32();

            if (size > 0x40000)
                throw new IOException("Size exceeds limit : " + size);

            var valueArray = Array.CreateInstance(elementType, size);

            if (elementType == typeof(string))
            {
                for (int i = 0; i < size; i++)
                {
                    valueArray.SetValue(br.ReadAlignedString(), i);
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    valueArray.SetValue(Deserialize(elementType, br), i);
                }
            }

            return valueArray;
        }


        private static object ReadValueArray(Type valueType, ExtendedBinaryReader br)
        {
            int size = br.ReadInt32();

            if (valueType == typeof(byte) || valueType == typeof(Byte))
            {
                var byteArray = br.ReadBytes(size);
                br.AlignStream();
                return byteArray;
            }

            var ret = Array.CreateInstance(valueType, size);

            for (int i = 0; i < size; i++)
            {
                ret.SetValue(ReadValueType(valueType, br, true), i);
            }

            br.AlignStream();

            return ret;
        }

        private static object ReadValueType(Type valueType, ExtendedBinaryReader br, bool noAlign = false)
        {
            if (!noAlign)
                br.AlignStream();

            if (valueType == typeof(string))
            {
                return br.ReadAlignedString();
            }
            else if (valueType == typeof(Int32))
            {
                return br.ReadInt32();
            }
            else if (valueType == typeof(UInt32))
            {
                return br.ReadUInt32();
            }
            else if (valueType == typeof(Int64))
            {
                return br.ReadInt64();
            }
            else if (valueType == typeof(UInt64))
            {
                return br.ReadUInt64();
            }
            else if (valueType == typeof(Int16))
            {
                return br.ReadInt16();
            }
            else if (valueType == typeof(UInt16))
            {
                return br.ReadUInt16();
            }
            else if (valueType == typeof(Byte))
            {
                return br.ReadByte();
            }
            else if (valueType == typeof(SByte))
            {
                return br.ReadSByte();
            }
            else if (valueType == typeof(Boolean))
            {
                return br.ReadBoolean();
            }
            else if (valueType == typeof(Double))
            {
                return br.ReadDouble();
            }
            else if (valueType == typeof(Single))
            {
                return br.ReadSingle();
            }
            else
            {
                throw new ArgumentException($"{valueType} is not a value type");
            }

        }

        public byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            using (var writer = new ExtendedBinaryWriter(ms))
            {
                Serialize(obj, writer);

                return ms.ToArray();
            }
        }

        public void Serialize(object obj, ExtendedBinaryWriter writer, Type objType = null)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("valueObj cannot be null");
            }

            if (objType == null)
            {
                objType = obj.GetType();
            }

            if (objType.IsArray)
            {
                Type elemType = objType.GetElementType();

                var arrayObj = obj as Array;
                int length = arrayObj.Length;

                writer.AlignStream();
                writer.Write(length);

                if (elemType == typeof(Byte))
                {
                    writer.Write(obj as byte[]);
                    writer.AlignStream();
                }
                else if (elemType.IsValueType)
                {
                    foreach (object element in arrayObj)
                    {
                        WriteValueType(element, writer, objType, noAlign: true);
                    }

                    writer.AlignStream();
                }
                else
                {
                    foreach (object element in arrayObj)
                    {
                        Serialize(element, writer);
                    }
                }

            }
            else if (objType.IsEnum)
            {
                var enumType = Enum.GetUnderlyingType(objType);
                WriteValueType(obj, writer, enumType);
            }
            else if (objType.IsValueType)
            {
                WriteValueType(obj, writer, objType);
            }
            else if (objType == typeof(string))
            {
                writer.AlignStream();
                writer.WriteAlignedString(obj as string);
            }
            else if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elemType = objType.GetGenericArguments()[0];

                var listObj = obj as IList;
                int length = listObj.Count;

                writer.AlignStream();
                writer.Write(length);

                if (elemType.IsValueType)
                {
                    foreach (object element in listObj)
                    {
                        WriteValueType(element, writer, elemType, noAlign: true);
                    }
                }
                else
                {
                    foreach (object element in listObj)
                    {
                        Serialize(element, writer);
                    }
                }
            }
            else if (objType.IsClass)
            {
                if (!Attribute.IsDefined(objType, typeof(UnitySerializableAttribute)))
                {
                    throw new Exception("not serializable type : " + objType.ToString());
                }

                if (Attribute.IsDefined(objType.BaseType, typeof(UnitySerializableAttribute)))
                {
                    Serialize(obj, writer, objType.BaseType);
                }

                foreach (var field in objType.GetFields())
                {
                    if (!field.DeclaringType.Equals(objType))
                    {
                        continue;
                    }
                    var fieldValue = field.GetValue(obj);
                    var fieldType = field.FieldType;

                    if (fieldType.IsEnum)
                    {
                        var enumType = Enum.GetUnderlyingType(fieldType);
                        WriteValueType(fieldValue, writer, enumType);
                    }
                    else if (fieldType.IsValueType)
                    {
                        WriteValueType(fieldValue, writer, fieldType, Attribute.IsDefined(field, typeof(UnityDoNotAlignAttribute)));
                    }
                    else
                    {
                        Serialize(fieldValue, writer);
                    }

                }
            }

            writer.AlignStream();
        }

        private static void WriteValueType(object valueObj, ExtendedBinaryWriter writer, Type valueType, bool noAlign = false)
        {
            if (valueObj == null)
                throw new ArgumentNullException("valueObj cannot be null");

            if (valueType == null)
                valueType = valueObj.GetType();

            if (!noAlign)
                writer.AlignStream();

            if (valueType == typeof(Int32))
            {
                writer.Write((int)valueObj);
            }
            else if (valueType == typeof(UInt32))
            {
                writer.Write((uint)valueObj);
            }
            else if (valueType == typeof(Int64))
            {
                writer.Write((long)valueObj);
            }
            else if (valueType == typeof(UInt64))
            {
                writer.Write((ulong)valueObj);
            }
            else if (valueType == typeof(Int16))
            {
                writer.Write((short)valueObj);
            }
            else if (valueType == typeof(UInt16))
            {
                writer.Write((ushort)valueObj);
            }
            else if (valueType == typeof(Byte))
            {
                writer.Write((byte)valueObj);
            }
            else if (valueType == typeof(SByte))
            {
                writer.Write((sbyte)valueObj);
            }
            else if (valueType == typeof(Boolean))
            {
                writer.Write((bool)valueObj);
            }
            else if (valueType == typeof(Double))
            {
                writer.Write((double)valueObj);
            }
            else if (valueType == typeof(Single))
            {
                writer.Write((float)valueObj);
            }
        }

        public static object GetDefault(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }

    class UnitySerializationException : Exception
    {
        public UnitySerializationException(string message) : base(message) { }
    }
}
