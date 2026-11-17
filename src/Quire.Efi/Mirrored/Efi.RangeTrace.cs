#nullable disable
#pragma warning disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Quire.Mcd;
using Quire.Mpl.Run;

namespace Quire.Efi;

internal partial class RangeTrace
{
    private partial class RangeEnumerator : IEnumerator<double>, IEnumerator, IDisposable
    {
        private readonly RngVal m_range;

        private readonly int count;

        private int step;

        public double Current => m_range.get(step);

        object IEnumerator.Current => Current;

        public RangeEnumerator(RngVal range)
        {
            m_range = range;
            count = range.Rsize;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            return ++step < count;
        }

        public void Reset()
        {
            step = 0;
        }
    }

    public RangeTrace(Spec spec, Dim xDim, Dim yDim)
        : base(spec, xDim, yDim)
    {
        enumerator = PointEnumerator(spec.reserved as object[]);
    }

    public override void Process(Engine engine, int currentTrace)
    {
        engine.ProcessTrace(this, currentTrace);
    }

    private object[] BuildArguments(IEnumerator<double>[] arguments)
    {
        object[] array = new object[arguments.Length];
        for (int i = 0; i < arguments.Length; i++)
        {
            array[i] = new NumVal(arguments[i].Current);
        }
        return array;
    }

    private IEnumerator<object[]> ArgumentEnumerator(object[] ranges)
    {
        System.Diagnostics.Trace.Assert(ranges != null, "ranges != null");
        IEnumerator<double>[] array = new IEnumerator<double>[ranges.Length];
        int num = 1;
        for (int i = 0; i < ranges.Length; i++)
        {
            RngVal rngVal = ranges[i] as RngVal;
            num *= rngVal.Rsize;
            array[i] = new RangeEnumerator(rngVal);
        }
        Size = num;
        return MultiRangeEnumerator(array);
    }
}
