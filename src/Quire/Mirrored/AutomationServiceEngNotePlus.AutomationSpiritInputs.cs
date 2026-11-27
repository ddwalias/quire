namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritInputs
{
    public AutomationSpiritInputs(global::System.Collections.Generic.List<string> aliasesList) : base(aliasesList) { }

    public AutomationSpiritInputs(global::Quire.Controls.Core.IORegions.IIOItemCollection inputs) : base(inputs) { }

    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputs.Count
    {
        get
        {
            try
            {
                return Count;
            }
            catch
            {
            }

            return 0;
        }
    }

    string global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputs.GetAliasByIndex(int indexArg)
    {
        try
        {
            return GetAliasByIndex(indexArg);
        }
        catch
        {
        }

        return null;
    }
}
