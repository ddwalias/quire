#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class OutputMatrixResult
{
    private readonly IMathcadPrimeOutputMatrixResult _realSubjectInstance;

    private IMathcadPrimeOutputMatrixResult RealSubjectInstance => _realSubjectInstance;

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

    private OutputMatrixResult()
    {
    }

    internal OutputMatrixResult(IMathcadPrimeOutputMatrixResult mathcadPrimeOutputMatrixResult)
    {
        _realSubjectInstance = mathcadPrimeOutputMatrixResult;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~OutputMatrixResult()
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
