using System;
using System.Text;
using System.Windows.Threading;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public abstract partial class OleDataExchangeContainerModel<TPersistentData>
{
    public new Dispatcher Dispatcher => base.Dispatcher;

    public bool Lock(bool wait)
    {
        return Lock(wait, CheckOleActiveState);
    }

    public bool Lock()
    {
        return Lock(wait: true);
    }

    public virtual bool ShouldSkipUnchangedValueMultiThreaded(string place)
    {
        return false;
    }

    protected virtual void CheckInstalledAndAvailable(bool forceRun)
    {
        SetVisibleSize(forceRun);
    }

    public bool DoActionWithLock(Action action)
    {
        return TryDoingActionWithLock(action, CheckOleActiveState);
    }

    protected internal static string ConvertToEngineError(string error, int numArguments)
    {
        StringBuilder stringBuilder = new StringBuilder(error.Replace('_', ' '));
        for (int i = 0; i < numArguments; i++)
        {
            stringBuilder.Append(string.Format("\t{{{0}}}", i));
        }
        return stringBuilder.ToString();
    }

    void IDataExchangeModel.Unlock()
    {
        Unlock();
    }
}
