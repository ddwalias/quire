namespace Quire;

public partial class AutomationSpiritOutputMatrixResultAs
{
    int global::Quire.Automation.IMathcadPrimeOutputMatrixResultAs.ErrorCode
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

    global::Quire.Automation.IMathcadPrimeMatrix global::Quire.Automation.IMathcadPrimeOutputMatrixResultAs.MatrixResult
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
