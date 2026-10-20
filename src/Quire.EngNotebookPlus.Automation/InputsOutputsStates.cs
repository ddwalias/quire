#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class InputsOutputsStates
{
    private readonly IMathcadPrimeInputsOutputsStates _realSubjectInstance;

    private IMathcadPrimeInputsOutputsStates RealSubjectInstance => _realSubjectInstance;

    public int ScalarInputsCount => RealSubjectInstance.ScalarInputsCount;

    public int ScalarOutputsCount => RealSubjectInstance.ScalarOutputsCount;

    private InputsOutputsStates()
    {
    }

    internal InputsOutputsStates(IMathcadPrimeInputsOutputsStates mathcadPrimeInputsOutputsStates)
    {
        _realSubjectInstance = mathcadPrimeInputsOutputsStates;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~InputsOutputsStates()
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

    public int GetInputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg)
    {
        return RealSubjectInstance.GetInputScalarStateByIndex(indexArg, out aliasArg, out valueArg, out unitsArg);
    }

    public int GetOutputScalarStateByIndex(int indexArg, out string aliasArg, out double valueArg, out string unitsArg)
    {
        return RealSubjectInstance.GetOutputScalarStateByIndex(indexArg, out aliasArg, out valueArg, out unitsArg);
    }

    public int GetInputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg)
    {
        return RealSubjectInstance.GetInputScalarStateByAlias(aliasArg, out valueArg, out unitsArg);
    }

    public int GetOutputScalarStateByAlias(string aliasArg, out double valueArg, out string unitsArg)
    {
        return RealSubjectInstance.GetOutputScalarStateByAlias(aliasArg, out valueArg, out unitsArg);
    }
}
