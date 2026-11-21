using Quire.Automation;
using Quire.Mcd;
using Quire.VSINTERFACESLib;

namespace Quire.Components;

internal partial class McxIO
{
    protected IVSDataType data;

    protected UnitAction action;

    public IVSDataType Data
    {
        get
        {
            if (action != UnitAction.None && data != null && data is VSDataValue vSDataValue)
            {
                data = new VSDataValue((MatVal)Utilities.ScaleToSI(vSDataValue.Core, action == UnitAction.Simplify));
                action = UnitAction.None;
            }
            return data;
        }
        set
        {
            data = value;
            if (action != UnitAction.None && data != null && data is VSDataValue vSDataValue)
            {
                data = new VSDataValue((MatVal)Utilities.ScaleFromSI(vSDataValue.Core, action == UnitAction.Simplify));
            }
        }
    }

    public bool IsValid => data != null;

    protected McxIO(UnitAction action)
    {
        this.action = action;
    }

    protected virtual void CopyOf(McxIO io)
    {
        data = null;
        if (io.IsValid)
        {
            data = io.data.Clone();
        }
    }
}
