#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public partial class ListResultNode<T>
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    private readonly IList<T> _data;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public int Count => _data.Count;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public T this[int index] { get => _data[index]; set => _data[index] = value; }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests")]
    public IList<T> List => _data;
}
