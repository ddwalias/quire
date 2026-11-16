using Quire.Mirror;
using Quire.Calculation.Core.Trace2DResults;

namespace Quire.Calculation.Core;

public partial interface IEngineError
{
    string Message { get; set; }
}

public sealed partial class EngineError : IEngineError
{
    private string _message;

    [MirrorVerified("PtcCalculationCoreEngineErrorRelationsMechanicalDifferentialTests")]
    public string Message
    {
        get => _message;
        set
        {
            _message = value;
            if (_message != null)
            {
                if (!_message.IsValidXmlString())
                {
                    _message = _message.StripInvalidXmlChars();
                }
                StringCollection stringCollection = new StringCollection(_message.Split('\t'));
                _code = stringCollection[0];
                stringCollection.RemoveAt(0);
                _arguments = stringCollection;
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public EngineError() { }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    private string _code;

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public string ErrorCode => _code;

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    private StringCollection _arguments;

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public StringCollection ErrorMessageArguments => _arguments;

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public uint NodeHandle { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public uint Argument { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public string ResourceString { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public EngineError OriginalError { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public bool IsRuntime { get; set; }

    [MirrorVerified("PtcCalculationCoreEngineErrorMechanicalDifferentialTests")]
    public ICalculatable ErrorSource { get; set; }
}

public enum ResultValueType
{
    Empty = 0,
    Numeric = 1,
    UnitedValue = 1,
    String = 2,
    Mixed = 3,
    Matrix = 3,
    NaNs = 4,
    Function = 4,
    Trace2D = 5,
}

/// <summary>Display-ready result tree; values are already converted to their display units.</summary>
public partial interface IResultNode
{
}

public partial interface IEmptyResultNode : IResultNode;

public partial interface IStringResultNode : IResultNode
{
    string Value { get; set; }
}

public partial interface IMatrixResultNode : IResultNode
{
    IResultNode this[int r, int c] { get; }
    int Count { get; }
    TableSize Size { get; }
    int Columns { get; }
    int Rows { get; }
}

public partial interface IFunctionResultNode : IResultNode
{
    string Description { get; set; }
}

public partial interface ITrace2DResultNode : IResultNode
{
    ITraceAxisInfo RangeAxisInfo { get; }
    ITraceAxisInfo ResultAxisInfo { get; }
    ITraceDataPoints DataPoints { get; }
    Trace2DResultType TraceResultType { get; }
}
