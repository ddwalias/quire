using System.Windows.Threading;

namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritValuesSetter
{
    private readonly global::Quire.Controls.Core.IORegions.IValuesSetter _collection;

    public AutomationSpiritValuesSetter(global::Quire.Controls.Core.IORegions.IValuesSetter collection)
    {
        _collection = collection;
    }

    public void AddScalarValue(string aliasArg, double valueArg, string unitsArg)
    {
        try
        {
            _collection.AddScalarValue(aliasArg, valueArg, unitsArg);
        }
        catch
        {
        }
    }

    public void AddMatrixValue(string aliasArg, object[,] valueArg, string unitsArg)
    {
        try
        {
            _collection.AddMatrixValue(aliasArg, valueArg, unitsArg);
        }
        catch
        {
        }
    }

    public void AddStringValue(string aliasArg, string valueArg)
    {
        try
        {
            _collection.AddStringValue(aliasArg, valueArg);
        }
        catch
        {
        }
    }

    public void AddSExprValue(string aliasArg, string sexpressionArg)
    {
        try
        {
            _collection.AddSExprValue(aliasArg, sexpressionArg);
        }
        catch
        {
        }
    }

    public global::Quire.EngNotebookPlus.Automation.IMathcadPrimeSetValueResults SetValues(int secondsArg)
    {
        try
        {
            DelegateFunction0<global::Quire.Controls.Core.IORegions.ISetValueResults> methodDelegate = () => DoSetValues(secondsArg);
            return (MathcadPrimeComServerManager.Instance.InvokeDelegate(methodDelegate, (DispatcherPriority)9) is global::Quire.Controls.Core.IORegions.ISetValueResults setValuesResults) ? new AutomationSpiritSetValueResults(setValuesResults) : null;
        }
        catch
        {
        }

        return null;
    }

    private global::Quire.Controls.Core.IORegions.ISetValueResults DoSetValues(int seconds)
    {
        try
        {
            return _collection.SetValues(seconds);
        }
        catch
        {
        }

        return null;
    }
}
