using System;

namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritOutputResultAs
{
    private readonly global::Quire.Controls.Core.IORegions.IOManagerErrorCode _errorCode;
    private readonly double _realResult;
    public global::Quire.Controls.Core.IORegions.IOManagerErrorCode ErrorCodeInternal => _errorCode;
    public int ErrorCode
    {
        get
        {
            try
            {
                return (int)_errorCode;
            }
            catch
            {
            }

            return -1;
        }
    }

    public double RealResult
    {
        get
        {
            try
            {
                return _realResult;
            }
            catch
            {
            }

            return double.NaN;
        }
    }

    public AutomationSpiritOutputResultAs(global::Quire.Controls.Core.IORegions.IUnitedScalarState scalarState)
    {
        _errorCode = scalarState.ErrorCode;
        _realResult = scalarState.Value;
    }
}
