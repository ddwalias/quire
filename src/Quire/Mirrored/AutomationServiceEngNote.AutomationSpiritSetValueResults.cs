namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritSetValueResults
{
    private readonly global::Quire.Controls.Core.IORegions.ISetValueResults _setValuesResults;

    public int Count
    {
        get
        {
            try
            {
                return _setValuesResults.Count;
            }
            catch
            {
            }

            return 0;
        }
    }

    public AutomationSpiritSetValueResults(global::Quire.Controls.Core.IORegions.ISetValueResults setValuesResults) { _setValuesResults = setValuesResults; }

    public int GetResultByIndex(int indexArg)
    {
        try
        {
            return (int)_setValuesResults.GetResult(indexArg);
        }
        catch
        {
        }

        return -1;
    }

    public int GetResultByAlias(string aliasArg)
    {
        try
        {
            return (int)_setValuesResults.GetResult(aliasArg);
        }
        catch
        {
        }

        return -1;
    }
}
