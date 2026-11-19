namespace Quire;

public partial class AutomationGetValueResult
{
    private readonly global::Quire.Controls.Core.IORegions.ICommonValueItem _valueItem;

    public AutomationGetValueResult(global::Quire.Controls.Core.IORegions.ICommonValueItem valueItem)
    {
        _valueItem = valueItem;
    }

    public global::Quire.Controls.Core.IORegions.IOManagerErrorCode GetErrorCode()
    {
        return _valueItem.ErrorState.ErrorCode;
    }

    public global::Quire.Automation.ValueResultTypes ResultType
    {
        get
        {
            if (_valueItem.ScalarState != null)
            {
                return global::Quire.Automation.ValueResultTypes.Real;
            }

            if (_valueItem.MatrixState != null)
            {
                return global::Quire.Automation.ValueResultTypes.Matrix;
            }

            if (_valueItem.StringState != null)
            {
                return global::Quire.Automation.ValueResultTypes.String;
            }

            global::Quire.DebugTools.DevAssert(_valueItem.ErrorState != null && _valueItem.ErrorState.ErrorCode != 0, null);
            return global::Quire.Automation.ValueResultTypes.None;
        }
    }

    public int ErrorCode
    {
        get
        {
            if (global::Quire.DebugTools.AssertNotNull(_valueItem.ErrorState, null).Failed)
            {
                return -1;
            }

            return (int)_valueItem.ErrorState.ErrorCode;
        }
    }

    public string Units
    {
        get
        {
            if (_valueItem.ScalarState != null)
            {
                return _valueItem.ScalarState.Units;
            }

            if (_valueItem.MatrixState != null)
            {
                return _valueItem.MatrixState.Units;
            }

            return null;
        }
    }

    public double RealResult
    {
        get
        {
            if (_valueItem.ScalarState != null)
            {
                return _valueItem.ScalarState.Value;
            }

            return double.NaN;
        }
    }

    public string StringResult
    {
        get
        {
            if (_valueItem.StringState != null)
            {
                return _valueItem.StringState.Value;
            }

            return null;
        }
    }

    public global::Quire.Automation.IMathcadPrimeMatrix MatrixResult
    {
        get
        {
            if (_valueItem.MatrixState != null)
            {
                return new AutomationSpiritMatrix(_valueItem.MatrixState.Value);
            }

            return null;
        }
    }
}
