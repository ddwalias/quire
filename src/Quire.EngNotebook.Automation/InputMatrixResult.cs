#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class InputMatrixResult
{
    private readonly IMathcadPrimeInputMatrixResult _realSubjectInstance;

    private IMathcadPrimeInputMatrixResult RealSubjectInstance => _realSubjectInstance;

    public int ErrorCode => RealSubjectInstance.ErrorCode;

    public IMathcadPrimeMatrix MatrixResult
    {
        get
        {
            IMathcadPrimeMatrix matrixResult = RealSubjectInstance.MatrixResult;
            try
            {
                return new Matrix(matrixResult);
            }
            catch
            {
            }
            return null;
        }
    }

    public string Units => RealSubjectInstance.Units;

    private InputMatrixResult()
    {
    }

    internal InputMatrixResult(IMathcadPrimeInputMatrixResult mathcadPrimeInputMatrixResult)
    {
        _realSubjectInstance = mathcadPrimeInputMatrixResult;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~InputMatrixResult()
    {
        try
        {
            if (RealSubjectInstance != null && Marshal.IsComObject(RealSubjectInstance))
            {
                Marshal.FinalReleaseComObject(RealSubjectInstance);
            }
        }
        catch
        {
        }
    }
}
