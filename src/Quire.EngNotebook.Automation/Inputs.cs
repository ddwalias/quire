#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class Inputs
{
    private readonly IMathcadPrimeInputs _realSubjectInstance;

    private IMathcadPrimeInputs RealSubjectInstance => _realSubjectInstance;

    public int Count => RealSubjectInstance.Count;

    private Inputs()
    {
    }

    internal Inputs(IMathcadPrimeInputs mathcadPrimeInputs)
    {
        _realSubjectInstance = mathcadPrimeInputs;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~Inputs()
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
