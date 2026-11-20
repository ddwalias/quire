using System;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Quire.Components;

internal partial class BitStream
{
    private IStream stream;

    public static readonly Exception SerializeException = new Exception("serialize exception");

    public byte[] ReadSome(int cb)
    {
        nint zero = IntPtr.Zero;
        byte[] array = new byte[cb];
        stream.Read(array, cb, zero);
        if (zero != IntPtr.Zero)
        {
            throw SerializeException;
        }
        return array;
    }

    public void WriteSome(byte[] buf)
    {
        nint zero = IntPtr.Zero;
        stream.Write(buf, buf.Length, zero);
        if (zero != IntPtr.Zero)
        {
            throw SerializeException;
        }
    }

    public void WriteStringLength(uint nLength)
    {
        if (nLength < 255)
        {
            WriteSome(new byte[1] { (byte)nLength });
            return;
        }
        if (nLength < 65534)
        {
            WriteSome(new byte[1] { 255 });
            WriteSome(BitConverter.GetBytes((ushort)nLength));
            return;
        }
        if (nLength < uint.MaxValue)
        {
            WriteSome(new byte[1] { 255 });
            WriteSome(BitConverter.GetBytes(ushort.MaxValue));
            WriteSome(BitConverter.GetBytes(nLength));
            return;
        }
        throw SerializeException;
    }

    private uint ReadStringLength()
    {
        byte b = ReadSome(1)[0];
        if (b != byte.MaxValue)
        {
            return b;
        }
        ushort num = BitConverter.ToUInt16(ReadSome(2), 0);
        if (num < ushort.MaxValue)
        {
            return num;
        }
        return BitConverter.ToUInt32(ReadSome(4), 0);
    }

    public BitStream(IStream stm)
    {
        stream = stm;
    }

    public double ReadDouble()
    {
        return BitConverter.ToDouble(ReadSome(8), 0);
    }

    public void WriteDouble(double val)
    {
        WriteSome(BitConverter.GetBytes(val));
    }

    public uint ReadUInt()
    {
        return BitConverter.ToUInt32(ReadSome(4), 0);
    }

    public void WriteUInt(uint val)
    {
        WriteSome(BitConverter.GetBytes(val));
    }

    public string BytesToString(byte[] buf)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < buf.Length; i++)
        {
            stringBuilder.Append((char)buf[i]);
        }
        return stringBuilder.ToString();
    }

    public byte[] StringToBytes(string str)
    {
        byte[] array = new byte[str.Length];
        for (int i = 0; i < str.Length; i++)
        {
            array[i] = (byte)str[i];
        }
        return array;
    }

    public string ReadString(bool oldStyle)
    {
        nint zero = IntPtr.Zero;
        uint num = ReadStringLength();
        if (num >= uint.MaxValue)
        {
            return null;
        }
        byte[] array = new byte[oldStyle ? num : (num * 2)];
        stream.Read(array, array.Length, zero);
        if (!oldStyle)
        {
            return Encoding.Unicode.GetString(array);
        }
        return BytesToString(array);
    }

    public void WriteString(string val, bool oldStyle)
    {
        nint zero = IntPtr.Zero;
        WriteStringLength((uint)val.Length);
        byte[] array = (oldStyle ? StringToBytes(val) : Encoding.Unicode.GetBytes(val));
        stream.Write(array, array.Length, zero);
    }
}
