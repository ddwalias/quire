using System;

namespace Quire;

public partial class AutomationSpiritOutputResultAs
{
    private readonly global::Quire.Controls.Core.IORegions.IOManagerErrorCode _errorCode;
    private readonly double _realResult;
    public global::Quire.Controls.Core.IORegions.IOManagerErrorCode ErrorCodeInternal => _errorCode;
    public AutomationSpiritOutputResultAs(global::Quire.Controls.Core.IORegions.IUnitedScalarState scalarState)
    {
        _errorCode = scalarState.ErrorCode;
        _realResult = scalarState.Value;
    }
}
