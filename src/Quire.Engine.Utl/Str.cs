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
                int index = 0;
                do
                {
                    if (!p(s[index]))
                    {
                        stringBuilder.Append(s[index]);
                    }

                    index++;
                }
                while (index < length);

                return stringBuilder.ToString();
            }
            while (num < length);
        }

        return s;
    }

    public static string RemoveWhite(string s) => remove_if(s_is_white, s);
}
