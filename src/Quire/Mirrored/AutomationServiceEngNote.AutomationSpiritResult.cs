using System;

namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritResult
{
    private readonly global::Quire.Controls.Core.IORegions.IOManagerErrorCode _errorCode;
    private readonly double _realResult;
    private readonly string _units;
    public global::Quire.Controls.Core.IORegions.IOManagerErrorCode ErrorCodeInternal => _errorCode;
    public int ErrorCode => (int)_errorCode;
    public double RealResult => _realResult;
    public string Units => _units;
    public AutomationSpiritResult(global::Quire.Controls.Core.IORegions.IUnitedScalarState scalarState)
    {
        if (scalarState != null)
        {
            _errorCode = scalarState.ErrorCode;
            _realResult = scalarState.Value;
            _units = scalarState.Units;
        }
        else
        {
            _errorCode = (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)(-1);
        }
    }
}
