#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class WorksheetReadonlyOptions
{
    private readonly IMathcadPrimeWorksheetReadonlyOptions _realSubjectInstance;

    public IMathcadPrimeWorksheetReadonlyOptions RealSubjectInstance => _realSubjectInstance;

    private WorksheetReadonlyOptions()
    {
    }

    internal WorksheetReadonlyOptions(IMathcadPrimeWorksheetReadonlyOptions mathcadPrimeWorksheetReadonlyOptions)
    {
        _realSubjectInstance = mathcadPrimeWorksheetReadonlyOptions;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~WorksheetReadonlyOptions()
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

    public int SetOptionValue(WorksheetReadonlyOptionNames optionNameArg, object optionValueArg)
    {
        return RealSubjectInstance.SetOptionValue(optionNameArg, optionValueArg);
    }
}
