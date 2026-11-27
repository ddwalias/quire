namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritInputResult
{
    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputResult.ErrorCode
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

    double global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputResult.RealResult
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

    string global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputResult.Units
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


    public AutomationSpiritInputResult(global::Quire.Controls.Core.IORegions.IUnitedScalarState scalarState) : base(scalarState) { }
}
