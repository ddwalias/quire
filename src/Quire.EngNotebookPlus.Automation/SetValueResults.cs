#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class SetValueResults
{
    private readonly IMathcadPrimeSetValueResults _realSubjectInstance;

    private IMathcadPrimeSetValueResults RealSubjectInstance => _realSubjectInstance;

    public int Count => RealSubjectInstance.Count;

    private SetValueResults()
    {
    }

    internal SetValueResults(IMathcadPrimeSetValueResults mathcadPrimeSetValueResults)
    {
        _realSubjectInstance = mathcadPrimeSetValueResults;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~SetValueResults()
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

    public int GetResultByIndex(int indexArg)
    {
        return RealSubjectInstance.GetResultByIndex(indexArg);
    }

    public int GetResultByAlias(string aliasArg)
    {
        return RealSubjectInstance.GetResultByAlias(aliasArg);
    }
}
