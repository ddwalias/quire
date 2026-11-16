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

    public abstract bool ShouldCommitAtLeastOnce { get; }

    public abstract bool ShouldCommitIfDirty { get; }

    protected abstract bool CheckOleActiveState { get; }

    protected OleDataExchangeContainerModel(global::Quire.OLE.IOleSiteManager oleSiteManager, bool autoResize)
        : base(oleSiteManager, autoResize)
    {
    }

    public abstract void SetValue(string place, global::Quire.IValue value, global::Quire.Calculation.IValueToIResultNodeTransform resultsTransform);

    public abstract global::Quire.IValue GetValue(string place);

    public abstract void Commit();

    protected static partial class EngineValueTypes
    {
        public const string Numeric = "Numeric";

        public const string String = "String";

        public const string Matrix = "Matrix";
    }
}
