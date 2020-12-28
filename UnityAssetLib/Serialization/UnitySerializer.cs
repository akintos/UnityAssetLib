using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityAssetLib.Util;
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
            using (var reader = new BinaryReader(ms))
            {
                return (T)Deserialize(typeof(T), reader);
            }
        }

        public T Deserialize<T>(AssetInfo assetInfo, bool fullDeserialize=true) where T : Types.Object
        {
            var ret = (T)Deserialize(typeof(T), assetInfo, fullDeserialize);
            ret.asset = assetInfo.asset;
            return ret;
        }

        public object Deserialize(Type classType, AssetInfo assetInfo, bool fullDeserialize=true)
        {
            var reader = assetInfo.InitReader();

            var startPos = reader.Position;

            var ret = Deserialize(classType, reader);

            long readSize = (reader.Position - startPos);

            if (fullDeserialize && readSize != assetInfo.size)
            {
                throw new UnitySerializationException("Failed to fully deserialize " + classType.FullName);
            }

            return ret;
        }

        public object Deserialize(Type classType, BinaryReader reader, object obj=null)
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
                Deserialize(classType.BaseType, reader, obj);
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
                    value = ReadValueType(enumType, reader, Attribute.IsDefined(field, typeof(UnityDoNotAlignAttribute)));
                }
                else if (fieldType.IsValueType)
                {
                    value = ReadValueType(fieldType, reader, Attribute.IsDefined(field, typeof(UnityDoNotAlignAttribute)));
                }
                else if (fieldType.IsArray && fieldType.GetElementType().IsValueType) // Value type array
                {
                    value = ReadValueArray(fieldType.GetElementType(), reader);
                }
                else if (fieldType == typeof(string))
                {
                    value = reader.ReadAlignedString();
                }
                else if(fieldType.IsClass || Attribute.IsDefined(fieldType, typeof(UnitySerializableAttribute)))
                {
                    if (fieldType.IsArray)
                    {
                        var elementType = fieldType.GetElementType();

                        value = ReadArray(elementType, reader);
                    }
                    else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elementType = fieldType.GetGenericArguments()[0];

                        value = Activator.CreateInstance(fieldType, (IEnumerable)ReadArray(elementType, reader));
                    }
                    else
                    {
                        value = Deserialize(fieldType, reader);
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

        private Array ReadArray(Type elementType, BinaryReader reader)
        {
            int size = reader.ReadInt32();

            if (size > 0x40000)
                throw new IOException("Size exceeds limit : " + size);

            var valueArray = Array.CreateInstance(elementType, size);

            if (elementType == typeof(string))
            {
                for (int i = 0; i < size; i++)
                {
                    valueArray.SetValue(reader.ReadAlignedString(), i);
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    valueArray.SetValue(Deserialize(elementType, reader), i);
                }
            }

            return valueArray;
        }


        private static object ReadValueArray(Type valueType, BinaryReader reader)
        {
            int size = reader.ReadInt32();

            if (valueType == typeof(byte) || valueType == typeof(Byte))
            {
                var byteArray = reader.ReadBytes(size);
                reader.AlignStream();
                return byteArray;
            }

            var ret = Array.CreateInstance(valueType, size);

            for (int i = 0; i < size; i++)
            {
                ret.SetValue(ReadValueType(valueType, reader, true), i);
            }

            reader.AlignStream();

            return ret;
        }

        private static object ReadValueType(Type valueType, BinaryReader reader, bool noAlign=false)
        {
            object ret = null;

            if (valueType == typeof(string))
            {
                ret = reader.ReadAlignedString();
            }
            else if (valueType == typeof(Int32))
            {
                ret = reader.ReadInt32();
            }
            else if (valueType == typeof(UInt32))
            {
                ret = reader.ReadUInt32();
            }
            else if (valueType == typeof(Int64))
            {
                ret = reader.ReadInt64();
            }
            else if (valueType == typeof(UInt64))
            {
                ret = reader.ReadUInt64();
            }
            else if (valueType == typeof(Int16))
            {
                ret = reader.ReadInt16();
                if(!noAlign)
                    reader.AlignStream();
            }
            else if (valueType == typeof(UInt16))
            {
                ret = reader.ReadUInt16();
                if (!noAlign)
                    reader.AlignStream();
            }
            else if (valueType == typeof(Byte))
            {
                ret = reader.ReadByte();
                if (!noAlign)
                    reader.AlignStream();
            }
            else if (valueType == typeof(SByte))
            {
                ret = reader.ReadSByte();
                if (!noAlign)
                    reader.AlignStream();
            }
            else if (valueType == typeof(Boolean))
            {
                ret = reader.ReadBoolean();
                if (!noAlign)
                    reader.AlignStream();
            }
            else if (valueType == typeof(Double))
            {
                ret = reader.ReadDouble();
            }
            else if (valueType == typeof(Single))
            {
                ret = reader.ReadSingle();
            }

            return ret;
        }

        public byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                Serialize(obj, writer);

                return ms.ToArray();
            }
        }

        public void Serialize(object obj, BinaryWriter writer, Type objType = null)
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
                        WriteValueType(element, writer, objType, noAlign:true);
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
                writer.WriteAlignedString(obj as string);
            }
            else if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elemType = objType.GetGenericArguments()[0];

                var listObj = obj as IList;
                int length = listObj.Count;

                writer.Write(length);

                if (elemType.IsValueType)
                {
                    foreach (object element in listObj)
                    {
                        WriteValueType(element, writer, elemType, noAlign: true);
                    }

                    writer.AlignStream();
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
        }

        private static void WriteValueType(object valueObj, BinaryWriter writer, Type valueType, bool noAlign = false)
        {
            if (valueObj == null)
            {
                throw new ArgumentNullException("valueObj cannot be null");
            }
            if (valueType == null)
                valueType = valueObj.GetType();

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
                if (!noAlign)
                    writer.AlignStream();
            }
            else if (valueType == typeof(UInt16))
            {
                writer.Write((ushort)valueObj);
                if (!noAlign)
                    writer.AlignStream();
            }
            else if (valueType == typeof(Byte))
            {
                writer.Write((byte)valueObj);
                if (!noAlign)
                    writer.AlignStream();
            }
            else if (valueType == typeof(SByte))
            {
                writer.Write((sbyte)valueObj);
                if (!noAlign)
                    writer.AlignStream();
            }
            else if (valueType == typeof(Boolean))
            {
                writer.Write((bool)valueObj);
                if (!noAlign)
                    writer.AlignStream();
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
