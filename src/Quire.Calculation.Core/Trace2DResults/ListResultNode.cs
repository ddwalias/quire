#nullable disable
using System.Collections.Generic;
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public abstract partial class ListResultNode<T> : IListResultNode<T>, IListResultNode
{
    private readonly IList<T> _data;

    private int _meaningfulValuesCount = -1;

    public abstract T NeutralValue { get; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests")]
    public int MeaningfulValuesCount
    {
        get
        {
            if (_meaningfulValuesCount == -1)
            {
                _meaningfulValuesCount = _data.Count(IsMeaningfulValue);
            }
            return _meaningfulValuesCount;
        }
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public int Count => _data.Count;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public T this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public IList<T> List => _data;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests")]
    protected ListResultNode(IList<T> data, int meaningfulValuesCount = -1)
    {
        _data = data;
        _meaningfulValuesCount = meaningfulValuesCount;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests")]
    protected ListResultNode(IList<T> data, T paddingValue, int minLength)
    {
        _data = ExtendList(data, minLength, paddingValue);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests")]
    public static IList<T> ExtendList(IList<T> data, int minLength, T paddingValue)
    {
        int num = data.Count();
        if (num >= minLength)
        {
            return data;
        }
        List<T> list = new List<T>(minLength);
        list.AddRange(data);
        IList<T> list2 = list;
        for (int i = num; i < minLength; i++)
        {
            list2.Add(paddingValue);
        }
        return list2;
    }

    public abstract bool IsMeaningfulValue(T valueIndex);
}
