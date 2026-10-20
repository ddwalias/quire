#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.Automation;

public partial class Outputs
{
    private readonly IMathcadPrimeOutputs _realSubjectInstance;

    private IMathcadPrimeOutputs RealSubjectInstance => _realSubjectInstance;

    public int Count => RealSubjectInstance.Count;

    private Outputs()
    {
    }

    internal Outputs(IMathcadPrimeOutputs mathcadPrimeOutputs)
    {
        _realSubjectInstance = mathcadPrimeOutputs;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~Outputs()
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

    public string GetAliasByIndex(int indexArg)
    {
        return RealSubjectInstance.GetAliasByIndex(indexArg);
    }
}
