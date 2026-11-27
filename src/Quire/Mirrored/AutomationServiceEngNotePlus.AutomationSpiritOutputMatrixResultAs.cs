namespace Quire.AutomationServiceEngNotePlus;

public partial class AutomationSpiritOutputMatrixResultAs
{
    int global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputMatrixResultAs.ErrorCode
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

    global::Quire.EngNotebookPlus.Automation.IMathcadPrimeMatrix global::Quire.EngNotebookPlus.Automation.IMathcadPrimeOutputMatrixResultAs.MatrixResult
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


    public AutomationSpiritOutputMatrixResultAs(global::Quire.Controls.Core.IORegions.IUnitedMatrixState matrixState) : base(matrixState) { }
}
