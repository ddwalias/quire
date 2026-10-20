using System;

namespace System.Reflection.Emit.Quire.Mpl;

internal sealed partial class TypeNameBuilder
{
    private void EscapeAssemblyName(string name)
    {
        Append(name);
    }

    private void Append(string pStr)
    {
        int num = pStr.IndexOf('\0');
        if (num < 0)
        {
            _str.Append(pStr);
        }
        else if (num > 0)
        {
            _str.Append(pStr.AsSpan(0, num));
        }
    }

    private void Append(char c)
    {
        _str.Append(c);
    }
}
