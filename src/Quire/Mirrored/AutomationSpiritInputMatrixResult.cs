namespace Quire;

public partial class AutomationSpiritInputMatrixResult
{
    int global::Quire.Automation.IMathcadPrimeInputMatrixResult.ErrorCode
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

    global::Quire.Automation.IMathcadPrimeMatrix global::Quire.Automation.IMathcadPrimeInputMatrixResult.MatrixResult
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

    string global::Quire.Automation.IMathcadPrimeInputMatrixResult.Units
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
