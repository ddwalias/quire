namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritOutputMatrixResultAs
{
    int global::Quire.EngNotebook.Automation.IMathcadPrimeOutputMatrixResultAs.ErrorCode
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

    global::Quire.EngNotebook.Automation.IMathcadPrimeMatrix global::Quire.EngNotebook.Automation.IMathcadPrimeOutputMatrixResultAs.MatrixResult
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
