#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class InputResult
{
    private readonly IMathcadPrimeInputResult _realSubjectInstance;

    private IMathcadPrimeInputResult RealSubjectInstance => _realSubjectInstance;

    public int ErrorCode => RealSubjectInstance.ErrorCode;

    public double RealResult => RealSubjectInstance.RealResult;

    public string Units => RealSubjectInstance.Units;

    private InputResult()
    {
    }

    internal InputResult(IMathcadPrimeInputResult mathcadPrimeInputResult)
    {
        _realSubjectInstance = mathcadPrimeInputResult;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~InputResult()
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
