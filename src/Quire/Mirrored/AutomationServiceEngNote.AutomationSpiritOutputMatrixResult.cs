namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritOutputMatrixResult
{
    int global::Quire.EngNotebook.Automation.IMathcadPrimeOutputMatrixResult.ErrorCode
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

    global::Quire.EngNotebook.Automation.IMathcadPrimeMatrix global::Quire.EngNotebook.Automation.IMathcadPrimeOutputMatrixResult.MatrixResult
    {
        get
        {
            try
            {
                return MatrixResult;
            }
            catch
            {
            }

            return null;
        }
    }

    string global::Quire.EngNotebook.Automation.IMathcadPrimeOutputMatrixResult.Units
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


    public AutomationSpiritOutputMatrixResult(global::Quire.Controls.Core.IORegions.IUnitedMatrixState matrixState) : base(matrixState) { }
}
