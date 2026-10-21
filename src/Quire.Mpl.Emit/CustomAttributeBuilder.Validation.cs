using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace System.Reflection.Emit.Quire.Mpl;

public partial class CustomAttributeBuilder
{
    private static void EmitString(BinaryWriter writer, string str)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        uint num = (uint)bytes.Length;
        if (num <= 127)
        {
            writer.Write((byte)num);
        }
        else if (num <= 16383)
        {
            writer.Write(BinaryPrimitives.ReverseEndianness((short)(num | 0x8000)));
        }
        else
        {
            writer.Write(BinaryPrimitives.ReverseEndianness(num | 0xC0000000u));
        }
        writer.Write(bytes);
    }
}
