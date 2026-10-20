#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class ValuesSetter
{
    private readonly IMathcadPrimeValuesSetter _realSubjectInstance;

    private IMathcadPrimeValuesSetter RealSubjectInstance => _realSubjectInstance;

    private ValuesSetter()
    {
    }

    internal ValuesSetter(IMathcadPrimeValuesSetter mathcadPrimeValuesSetter)
    {
        _realSubjectInstance = mathcadPrimeValuesSetter;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~ValuesSetter()
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

    public void AddScalarValue(string aliasArg, double valueArg, string unitsArg)
    {
        RealSubjectInstance.AddScalarValue(aliasArg, valueArg, unitsArg);
    }

    public void AddMatrixValue(string aliasArg, object[,] valueArg, string unitsArg)
    {
        RealSubjectInstance.AddMatrixValue(aliasArg, valueArg, unitsArg);
    }

    public void AddStringValue(string aliasArg, string valueArg)
    {
        RealSubjectInstance.AddStringValue(aliasArg, valueArg);
    }

    public void AddSExprValue(string aliasArg, string sexpressionArg)
    {
        RealSubjectInstance.AddSExprValue(aliasArg, sexpressionArg);
    }

    public IMathcadPrimeSetValueResults SetValues(int secondsArg)
    {
        IMathcadPrimeSetValueResults mathcadPrimeSetValueResults = RealSubjectInstance.SetValues(secondsArg);
        try
        {
            return new SetValueResults(mathcadPrimeSetValueResults);
        }
        catch
        {
        }
        return null;
    }
}
