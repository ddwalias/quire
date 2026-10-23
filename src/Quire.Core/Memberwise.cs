#nullable disable
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Quire;

public static partial class Memberwise
{
    public static bool Equal(object left, object right)
    {
        bool flag;
        if (left == right)
        {
            flag = true;
        }
        else if (left == null || right == null)
        {
            flag = false;
        }
        else
        {
            Type type = left.GetType();
            if (type != right.GetType())
            {
                flag = false;
            }
            else if (left is ValueType)
            {
                flag = left.Equals(right);
            }
            else if (left is IEnumerable)
            {
                IEnumerable enumerable = right as IEnumerable;
                flag = enumerable != null;
                if (flag)
                {
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    enumerator.Reset();
                    foreach (object item in left as IEnumerable)
                    {
                        if (!enumerator.MoveNext())
                        {
                            flag = false;
                            break;
                        }
                        if (!Equal(item, enumerator.Current))
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty).All((PropertyInfo info) => Equal(info.GetValue(left, null), info.GetValue(right, null))) && type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField).All((FieldInfo info) => Equal(info.GetValue(left), info.GetValue(right)));
            }
        }
        return flag;
    }
}
