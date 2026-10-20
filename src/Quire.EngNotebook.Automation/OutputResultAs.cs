#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class OutputResultAs
{
    private readonly IMathcadPrimeOutputResultAs _realSubjectInstance;

    private IMathcadPrimeOutputResultAs RealSubjectInstance => _realSubjectInstance;

    public int ErrorCode => RealSubjectInstance.ErrorCode;

    public double RealResult => RealSubjectInstance.RealResult;

    private OutputResultAs()
    {
    }

    internal OutputResultAs(IMathcadPrimeOutputResultAs mathcadPrimeOutputResultAs)
    {
        _realSubjectInstance = mathcadPrimeOutputResultAs;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~OutputResultAs()
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
