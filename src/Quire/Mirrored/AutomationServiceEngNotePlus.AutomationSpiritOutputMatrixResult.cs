namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritOutputMatrixResult
{
    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputMatrixResult.ErrorCode
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

    global::Quire.EngNotebookPlus.Automation.IMathcadPrimeMatrix global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputMatrixResult.MatrixResult
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

    string global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputMatrixResult.Units
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
