using System;
using System.IO;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    private static void EmitValue(BinaryWriter writer, Type type, object? value)
    {
        if (type.IsEnum)
        {
            switch (Type.GetTypeCode(Enum.GetUnderlyingType(type)))
            {
                case TypeCode.SByte:
                    writer.Write((sbyte)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)value);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)value);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)value);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)value);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)value);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)value);
                    break;
            }
        }
        else if (type == typeof(string))
        {
            if (value == null)
            {
                writer.Write(byte.MaxValue);
            }
            else
            {
                EmitString(writer, (string)value);
            }
        }
        else if (type == typeof(Type))
        {
            if (value == null)
            {
                writer.Write(byte.MaxValue);
                return;
            }
            string text = TypeNameBuilder.ToString((Type)value, TypeNameBuilder.Format.AssemblyQualifiedName);
            if (text == null)
            {
                throw new ArgumentException("Invalid Type For CA");
            }
            EmitString(writer, text);
        }
        else if (type.IsArray)
        {
            if (value == null)
            {
                writer.Write(uint.MaxValue);
                return;
            }
            Array array = (Array)value;
            Type elementType = type.GetElementType();
            writer.Write(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                EmitValue(writer, elementType, array.GetValue(i));
            }
        }
        else if (type.IsPrimitive)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                    writer.Write((sbyte)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)value);
                    break;
                case TypeCode.Char:
                    writer.Write(Convert.ToUInt16((char)value));
                    break;
                case TypeCode.Boolean:
                    writer.Write(((bool)value) ? ((byte)1) : ((byte)0));
                    break;
                case TypeCode.Int16:
                    writer.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)value);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)value);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)value);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)value);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)value);
                    break;
                case TypeCode.Single:
                    writer.Write((float)value);
                    break;
                case TypeCode.Double:
                    writer.Write((double)value);
                    break;
            }
        }
        else
        {
            if (type == typeof(object))
            {
                Type valueType = value == null ? typeof(string) : value is Type ? typeof(Type) : value.GetType();
                if (valueType == typeof(object))
                    throw new ArgumentException("Bad Parameter Type For CAB");
                EmitType(writer, valueType);
                EmitValue(writer, valueType, value);
                return;
            }
            if (value != null)
            {
                value.GetType().ToString();
            }
            throw new ArgumentException("Bad Parameter Type For CAB");
        }
    }
}
