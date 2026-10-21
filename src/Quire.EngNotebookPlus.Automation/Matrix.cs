#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebookPlus.Automation;

public partial class Matrix
{
    private readonly IMathcadPrimeMatrix _realSubjectInstance;

    public IMathcadPrimeMatrix RealSubjectInstance => _realSubjectInstance;

    public double Rows => RealSubjectInstance.Rows;

    public double Columns => RealSubjectInstance.Columns;

    private Matrix()
    {
    }

    internal Matrix(IMathcadPrimeMatrix mathcadPrimeMatrix)
    {
        _realSubjectInstance = mathcadPrimeMatrix;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~Matrix()
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

    public int SetMatrixElement(int rowIndexArg, int columnIndexArg, double valueArg)
    {
        return RealSubjectInstance.SetMatrixElement(rowIndexArg, columnIndexArg, valueArg);
    }

    public int GetMatrixElement(int rowIndexArg, int columnIndexArg, out double valueArg)
    {
        return RealSubjectInstance.GetMatrixElement(rowIndexArg, columnIndexArg, out valueArg);
    }

    double[][] IMathcadPrimeMatrix.Matrix => RealSubjectInstance.Matrix;
}
