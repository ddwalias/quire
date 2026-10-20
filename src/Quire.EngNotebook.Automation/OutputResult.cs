#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class OutputResult
{
    private readonly IMathcadPrimeOutputResult _realSubjectInstance;

    private IMathcadPrimeOutputResult RealSubjectInstance => _realSubjectInstance;

    public int ErrorCode => RealSubjectInstance.ErrorCode;

    public double RealResult => RealSubjectInstance.RealResult;

    public string Units => RealSubjectInstance.Units;

    private OutputResult()
    {
    }

    internal OutputResult(IMathcadPrimeOutputResult mathcadPrimeOutputResult)
    {
        _realSubjectInstance = mathcadPrimeOutputResult;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~OutputResult()
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
