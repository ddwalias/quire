namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritOutputResult
{
    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputResult.ErrorCode
    {
        get
        {
            try
            {
                return ErrorCode;
            }
            catch
            {
            }

            return -1;
        }
    }

    double global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputResult.RealResult
    {
        get
        {
            try
            {
                return RealResult;
            }
            catch
            {
            }

            return double.NaN;
        }
    }

    string global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputResult.Units
    {
        get
        {
            try
            {
                return Units;
            }
            catch
            {
            }

            return null;
        }
    }


    public AutomationSpiritOutputResult(global::Quire.Controls.Core.IORegions.IUnitedScalarState scalarState) : base(scalarState) { }
}
