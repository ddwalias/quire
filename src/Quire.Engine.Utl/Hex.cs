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
                byte b2 = (byte)(b[num] >> 4);
                int num2 = (((uint)b2 >= 10u) ? (b2 + 55) : (b2 + 48));
                stringBuilder.Append((char)num2);
                byte b3 = (byte)(b[num] % 16);
                int num3 = (((uint)b3 >= 10u) ? (b3 + 55) : (b3 + 48));
                stringBuilder.Append((char)num3);
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
        int num2 = 0;
        if (0 < num)
        {
            do
            {
                int num3 = s[num2 * 2];
                int num4 = (((uint)(num3 - 48) > 9u) ? (num3 - 55) : (num3 - 48));
                int num5 = s[num2 * 2 + 1];
                int num6 = (((uint)(num5 - 48) > 9u) ? (num5 - 55) : (num5 - 48));
                array[num2] = (byte)(num4 * 16 + num6);
                num2++;
            }
            while (num2 < num);
        }

        return array;
    }
}
