namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritOutputs
{
    public AutomationSpiritOutputs(global::System.Collections.Generic.List<string> aliasesList) : base(aliasesList) { }

    public AutomationSpiritOutputs(global::Quire.Controls.Core.IORegions.IIOItemCollection inputs) : base(inputs) { }

    int global::Quire.EngNotebook.Automation.IMathcadPrimeOutputs.Count
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

    string global::Quire.EngNotebook.Automation.IMathcadPrimeOutputs.GetAliasByIndex(int indexArg)
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
