#nullable disable
#pragma warning disable

using System.Collections;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal partial class BoolMap
{
    private int myNcols;

    private int myNrows;

    private BitArray myBoolMap;

    public BoolMap(int ncols, int nrows)
    {
        myNcols = ncols;
        myNrows = nrows;
        myBoolMap = new BitArray(ncols * nrows);
        zero();
    }

    public void zero()
    {
        myBoolMap.SetAll(value: false);
    }

    public void setBit(int col, int row)
    {
        if (col >= myNcols || row >= myNrows)
        {
            AppException.Raise("index out of bounds");
        }
        int index = col * myNrows + row;
        myBoolMap.Set(index, value: true);
    }

    public bool bitSet(int col, int row)
    {
        if (col >= myNcols || row >= myNrows)
        {
            AppException.Raise("index out of bounds");
        }
        int index = col * myNrows + row;
        return myBoolMap.Get(index);
    }
}
