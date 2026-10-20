#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class OutputMatrixResultAs
{
    private readonly IMathcadPrimeOutputMatrixResultAs _realSubjectInstance;

    private IMathcadPrimeOutputMatrixResultAs RealSubjectInstance => _realSubjectInstance;

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

    private OutputMatrixResultAs()
    {
    }

    internal OutputMatrixResultAs(IMathcadPrimeOutputMatrixResultAs mathcadPrimeOutputMatrixResultAs)
    {
        _realSubjectInstance = mathcadPrimeOutputMatrixResultAs;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~OutputMatrixResultAs()
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
