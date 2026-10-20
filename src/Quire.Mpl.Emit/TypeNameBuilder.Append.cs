using System;

namespace System.Reflection.Emit.Quire.Mpl;

internal sealed partial class TypeNameBuilder
{
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
