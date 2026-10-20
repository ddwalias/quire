#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.Automation;

public partial class GetValueResult
{
    private readonly IMathcadPrimeGetValueResult _realSubjectInstance;

    private IMathcadPrimeGetValueResult RealSubjectInstance => _realSubjectInstance;

    public ValueResultTypes ResultType => RealSubjectInstance.ResultType;

    public int ErrorCode => RealSubjectInstance.ErrorCode;

    public string Units => RealSubjectInstance.Units;

    public double RealResult => RealSubjectInstance.RealResult;

    public string StringResult => RealSubjectInstance.StringResult;

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

    private GetValueResult()
    {
    }

    internal GetValueResult(IMathcadPrimeGetValueResult mathcadPrimeGetValueResult)
    {
        _realSubjectInstance = mathcadPrimeGetValueResult;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~GetValueResult()
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
