#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using Quire.Mcd;

namespace Quire.Efi;

internal partial class VectorTrace
{
    public VectorTrace(Trace.Spec spec, Dim xDim, Dim yDim)
        : base(spec, xDim, yDim)
    {
        enumerator = PointEnumerator(spec.reserved);
    }

    public override void Process(Engine engine, int currentTrace)
    {
        engine.ProcessTrace(this, currentTrace);
    }

    private static void Fill(object[] array, object item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = item;
        }
    }
}
