namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritInputResult
{
    int global::Quire.EngNotebook.Automation.IMathcadPrimeInputResult.ErrorCode
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

    double global::Quire.EngNotebook.Automation.IMathcadPrimeInputResult.RealResult
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

    string global::Quire.EngNotebook.Automation.IMathcadPrimeInputResult.Units
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
