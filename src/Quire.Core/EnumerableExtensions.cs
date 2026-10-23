#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using System.Collections.Specialized;
using Quire.Wpf;

namespace Quire;

public static partial class EnumerableExtensions
{
    public static bool HasCommonValue<T>(this IEnumerable<T> values, out T commonValue)
    {
        commonValue = default;
        if (values.IsNullOrEmpty())
        {
            return false;
        }
        bool flag = false;
        foreach (T value in values)
        {
            if (flag)
            {
                if (!object.Equals(commonValue, value))
                {
                    return false;
                }
            }
            else
            {
                flag = true;
                commonValue = value;
            }
        }
        return true;
    }

}
