namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritInputMatrixResult
{
    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputMatrixResult.ErrorCode
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

    global::Quire.EngNotebookPlus.Automation.IMathcadPrimeMatrix global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputMatrixResult.MatrixResult
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

    string global::Quire.EngNotebookPlus.Automation.IMathcadPrimeInputMatrixResult.Units
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
