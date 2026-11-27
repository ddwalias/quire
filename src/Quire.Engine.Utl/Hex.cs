using System.Text;

namespace Quire.utl_mgd;

public partial class _Hex
{
    private static byte hexdig(int hd)
    {
        if ((uint)(hd - 48) <= 9u)
        {
            return (byte)(hd - 48);
        }

        return (byte)(hd - 55);
    }

    private static int dighex(byte dh)
    {
        if ((uint)dh < 10u)
        {
            return dh + 48;
        }

        return dh + 55;
    }

    public static string ToHex(byte[] b)
    {
        StringBuilder stringBuilder = new StringBuilder((int)(b.LongLength * 2));
        int num = 0;
        if (0 < b.LongLength)
        {
            do
            {
                byte highNibble = (byte)(b[num] >> 4);
                int highDigit = (((uint)highNibble >= 10u) ? (highNibble + 55) : (highNibble + 48));
                stringBuilder.Append((char)highDigit);
                byte lowNibble = (byte)(b[num] % 16);
                int lowDigit = (((uint)lowNibble >= 10u) ? (lowNibble + 55) : (lowNibble + 48));
                stringBuilder.Append((char)lowDigit);
                num++;
            }
            while (num < b.LongLength);
        }

        return stringBuilder.ToString();
    }

    public static byte[] FromHex(string s)
    {
        int num = s.Length / 2;
        byte[] array = new byte[num];
        int index = 0;
        if (0 < num)
        {
            do
            {
                int highCharacter = s[index * 2];
                int highNibble = (((uint)(highCharacter - 48) > 9u) ? (highCharacter - 55) : (highCharacter - 48));
                int lowCharacter = s[index * 2 + 1];
                int lowNibble = (((uint)(lowCharacter - 48) > 9u) ? (lowCharacter - 55) : (lowCharacter - 48));
                array[index] = (byte)(highNibble * 16 + lowNibble);
                index++;
            }
            while (index < num);
        }

        return array;
    }
}
