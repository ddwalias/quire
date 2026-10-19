#nullable disable
using System.Linq;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class ListResultNode<T>
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests")]
    private int _meaningfulValuesCount = -1;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    private readonly IList<T> _data;

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
    public T this[int index] { get => _data[index]; set => _data[index] = value; }

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
        int count = data.Count();
        if (count >= minLength)
        {
            return data;
        }

        List<T> extended = new List<T>(minLength);
        extended.AddRange(data);
        IList<T> result = extended;
        for (int i = count; i < minLength; i++)
        {
            result.Add(paddingValue);
        }

        return result;
    }
}
