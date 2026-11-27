namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritInputMatrixResult
{
    int global::Quire.EngNotebook.Automation.IMathcadPrimeInputMatrixResult.ErrorCode
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

    global::Quire.EngNotebook.Automation.IMathcadPrimeMatrix global::Quire.EngNotebook.Automation.IMathcadPrimeInputMatrixResult.MatrixResult
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

    string global::Quire.EngNotebook.Automation.IMathcadPrimeInputMatrixResult.Units
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


    public AutomationSpiritInputMatrixResult(global::Quire.Controls.Core.IORegions.IUnitedMatrixState matrixState) : base(matrixState) { }
}
