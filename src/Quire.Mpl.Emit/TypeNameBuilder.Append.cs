using System;

namespace System.Reflection.Emit.Quire.Mpl;

internal sealed partial class TypeNameBuilder
{
    private void EscapeName(string name)
    {
        if (ContainsReservedChar(name))
        {
            foreach (char c in name)
            {
                if (c != '\0')
                {
                    if (IsTypeNameReservedChar(c))
                    {
                        _str.Append('\\');
                    }
                    _str.Append(c);
                    continue;
                }
                break;
            }
        }
        else
        {
            Append(name);
        }
    }

    private void EscapeEmbeddedAssemblyName(string name)
    {
        if (name.Contains(']'))
        {
            foreach (char c in name)
            {
                if (c == ']')
                {
                    Append('\\');
                }
                Append(c);
            }
        }
        else
        {
            Append(name);
        }
    }

    private void AddArray(int rank)
    {
        if (rank == 1)
        {
            Append("[*]");
            return;
        }
        if (rank > 64)
        {
            _str.Append('[').Append(rank).Append(']');
            return;
        }
        Append('[');
        for (int i = 1; i < rank; i++)
        {
            Append(',');
        }
        Append(']');
    }

    private void AddAssemblySpec(string assemblySpec)
    {
        if (assemblySpec != null && !assemblySpec.Equals(""))
        {
            Append(", ");
            if (_instNesting > 0)
            {
                EscapeEmbeddedAssemblyName(assemblySpec);
            }
            else
            {
                EscapeAssemblyName(assemblySpec);
            }
            _hasAssemblySpec = true;
        }
    }

    private void AddName(string name)
    {
        if (_nestedName)
        {
            Append('+');
        }
        _nestedName = true;
        EscapeName(name);
    }

    private static bool ContainsReservedChar(string name)
    {
        foreach (char c in name)
        {
            if (c == '\0')
            {
                break;
            }
            if (IsTypeNameReservedChar(c))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsTypeNameReservedChar(char ch)
    {
        switch (ch)
        {
            case '&':
            case '*':
            case '+':
            case ',':
            case '[':
            case '\\':
            case ']':
                return true;
            default:
                return false;
        }
    }

    private void OpenGenericArguments()
    {
        _instNesting++;
        _firstInstArg = true;
        if (_useAngleBracketsForGenerics)
        {
            Append('<');
        }
        else
        {
            Append('[');
        }
    }

    private void CloseGenericArguments()
    {
        _instNesting--;
        if (_firstInstArg)
        {
            _str.Remove(_str.Length - 1, 1);
        }
        else if (_useAngleBracketsForGenerics)
        {
            Append('>');
        }
        else
        {
            Append(']');
        }
    }

    private void OpenGenericArgument()
    {
        _nestedName = false;
        if (!_firstInstArg)
        {
            Append(',');
        }
        _firstInstArg = false;
        if (_useAngleBracketsForGenerics)
        {
            Append('<');
        }
        else
        {
            Append('[');
        }
        PushOpenGenericArgument();
    }

    private void CloseGenericArgument()
    {
        if (_hasAssemblySpec)
        {
            if (_useAngleBracketsForGenerics)
            {
                Append('>');
            }
            else
            {
                Append(']');
            }
        }
        PopOpenGenericArgument();
    }

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

    private void PushOpenGenericArgument()
    {
        _stack.Add(_str.Length);
        _stackIdx++;
    }

    private void PopOpenGenericArgument()
    {
        int num = _stack[--_stackIdx];
        _stack.RemoveAt(_stackIdx);
        if (!_hasAssemblySpec)
        {
            _str.Remove(num - 1, 1);
        }
        _hasAssemblySpec = false;
    }

    private void SetUseAngleBracketsForGenerics(bool value)
    {
        _useAngleBracketsForGenerics = value;
    }
}
