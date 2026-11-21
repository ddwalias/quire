using Quire.Automation;
using Quire.VSINTERFACESLib;

namespace Quire.Components;

internal partial class McxOutput : McxIO, IVSQueueOutput
{
    public McxOutput(UnitAction action)
        : base(action)
    {
    }

    protected void Set(McxOutput output)
    {
        CopyOf(output);
    }

    public void Send(IVSDataType lppVSData)
    {
        data = lppVSData;
    }

    public void IsReady()
    {
        if (IsValid)
        {
            throw HRESULT.S_FALSE;
        }
    }
}
