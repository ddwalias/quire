#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class Worksheets
{
    private readonly IMathcadPrimeWorksheets _realSubjectInstance;

    private IMathcadPrimeWorksheets RealSubjectInstance => _realSubjectInstance;

    public int Count => RealSubjectInstance.Count;

    private Worksheets()
    {
    }

    internal Worksheets(IMathcadPrimeWorksheets mathcadPrimeWorksheets)
    {
        _realSubjectInstance = mathcadPrimeWorksheets;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~Worksheets()
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

    public IMathcadPrimeWorksheet2 Item(int indexArg)
    {
        IMathcadPrimeWorksheet2 mathcadPrimeWorksheet = RealSubjectInstance.Item(indexArg);
        try
        {
            return new Worksheet(mathcadPrimeWorksheet);
        }
        catch
        {
        }
        return null;
    }
}
