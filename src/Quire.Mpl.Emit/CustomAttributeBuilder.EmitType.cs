using System;
using System.IO;
using System.Reflection;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    private static void EmitType(BinaryWriter writer, Type type)
    {
        if (type.IsPrimitive)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.SByte:
                    writer.Write((byte)4);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)5);
                    break;
                case TypeCode.Char:
                    writer.Write((byte)3);
                    break;
                case TypeCode.Boolean:
                    writer.Write((byte)2);
                    break;
                case TypeCode.Int16:
                    writer.Write((byte)6);
                    break;
                case TypeCode.UInt16:
                    writer.Write((byte)7);
                    break;
                case TypeCode.Int32:
                    writer.Write((byte)8);
                    break;
                case TypeCode.UInt32:
                    writer.Write((byte)9);
                    break;
                case TypeCode.Int64:
                    writer.Write((byte)10);
                    break;
                case TypeCode.UInt64:
                    writer.Write((byte)11);
                    break;
                case TypeCode.Single:
                    writer.Write((byte)12);
                    break;
                case TypeCode.Double:
                    writer.Write((byte)13);
                    break;
            }
        }
        else if (type.IsEnum)
        {
            writer.Write((byte)85);
            EmitString(writer, type.AssemblyQualifiedName);
        }
        else if (type == typeof(string))
        {
            writer.Write((byte)14);
        }
        else if (type == typeof(Type))
        {
            writer.Write((byte)80);
        }
        else if (type.IsArray)
        {
            writer.Write((byte)29);
            EmitType(writer, type.GetElementType());
        }
        else
        {
            writer.Write((byte)81);
        }
    }
}
