namespace Quire.AutomationServiceEngNote;

public partial class AutomationSpiritMatrixResult
{
    private readonly global::Quire.Controls.Core.IORegions.IOManagerErrorCode _errorCode;
    private readonly AutomationSpiritMatrix _matrixResult;
    private readonly string _units;

    public global::Quire.Controls.Core.IORegions.IOManagerErrorCode ErrorCodeInternal => _errorCode;

    public int ErrorCode => (int)_errorCode;

    public global::Quire.EngNotebook.Automation.IMathcadPrimeMatrix MatrixResult => _matrixResult;

    public string Units => _units;

    public AutomationSpiritMatrixResult(global::Quire.Controls.Core.IORegions.IUnitedMatrixState matrixState)
    {
        if (matrixState != null)
        {
            _errorCode = matrixState.ErrorCode;
            _matrixResult = new AutomationSpiritMatrix(matrixState.Value);
            _units = matrixState.Units;
        }
        else
        {
            _errorCode = (global::Quire.Controls.Core.IORegions.IOManagerErrorCode)(-1);
        }
    }
}
