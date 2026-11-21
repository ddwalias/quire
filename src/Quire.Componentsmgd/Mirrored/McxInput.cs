using Quire.Automation;
using Quire.VSINTERFACESLib;

namespace Quire.Components;

internal partial class McxInput : McxIO, IVSQueueInput
{
    public McxInput(UnitAction action)
        : base(action)
    {
    }

    protected void Set(McxInput input)
    {
        CopyOf(input);
    }

    public void IsReady()
    {
        if (!IsValid)
        {
            throw HRESULT.S_FALSE;
        }
    }

    public IVSDataType Recv()
    {
        IVSDataType result = Data;
        Data = null;
        return result;
    }
}
