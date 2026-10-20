#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class InputsOutputsConflicts
{
    private readonly IMathcadPrimeInputsOutputsConflicts _realSubjectInstance;

    private IMathcadPrimeInputsOutputsConflicts RealSubjectInstance => _realSubjectInstance;

    private InputsOutputsConflicts()
    {
    }

    internal InputsOutputsConflicts(IMathcadPrimeInputsOutputsConflicts mathcadPrimeInputsOutputsConflicts)
    {
        _realSubjectInstance = mathcadPrimeInputsOutputsConflicts;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~InputsOutputsConflicts()
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

    public void AddGeneralWarning(string warningArg)
    {
        RealSubjectInstance.AddGeneralWarning(warningArg);
    }

    public void AddGeneralError(string errorArg)
    {
        RealSubjectInstance.AddGeneralError(errorArg);
    }

    public void AddItemWarning(string aliasArg, string warningArg)
    {
        RealSubjectInstance.AddItemWarning(aliasArg, warningArg);
    }

    public void AddItemError(string aliasArg, string errorArg)
    {
        RealSubjectInstance.AddItemError(aliasArg, errorArg);
    }
}
