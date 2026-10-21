using System.Text;

namespace Quire.utl_mgd;

public partial class Str
{
    private static Predicate<char> s_is_white = char.IsWhiteSpace;

    private static string remove_if(Predicate<char> p, string s)
    {
        int length = s.Length;
        int num = 0;
        if (0 < length)
        {
            do
            {
                if (!p(s[num]))
                {
                    num++;
                    continue;
                }

                StringBuilder stringBuilder = new StringBuilder(length);
                int num2 = 0;
                do
                {
                    if (!p(s[num2]))
                    {
                        stringBuilder.Append(s[num2]);
                    }

                    num2++;
                }
                while (num2 < length);

                return stringBuilder.ToString();
            }
            while (num < length);
        }

        return s;
    }

    public static string RemoveWhite(string s) => remove_if(s_is_white, s);
}
