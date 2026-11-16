#nullable disable
#pragma warning disable

using System;

namespace Quire.Efi;

internal partial class Options
{
    private int bits;

    public Options(int bits)
    {
        this.bits = bits;
    }

    private void CheckIndex(int index)
    {
        if (index < 0 || index > 32)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public bool GetBit(int index)
    {
        CheckIndex(index);
        return (bits & (1 << index)) != 0;
    }

    public void SetBit(int index, bool b)
    {
        CheckIndex(index);
        if (b)
        {
            bits ^= 1 << index;
        }
        else
        {
            bits |= 1 << index;
        }
    }
}
