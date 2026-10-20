#nullable disable
using System;
using System.Runtime.InteropServices;

namespace Quire.Automation;

public partial class Worksheet
{
    private readonly IMathcadPrimeWorksheet3 _realSubjectInstance;

    private IMathcadPrimeWorksheet3 RealSubjectInstance => _realSubjectInstance;

    public string Name => RealSubjectInstance.Name;

    public string FullName => RealSubjectInstance.FullName;

    public bool IsReadOnly => RealSubjectInstance.IsReadOnly;

    public bool Modified
    {
        get => RealSubjectInstance.Modified;
        set => RealSubjectInstance.Modified = value;
    }

    public IMathcadPrimeInputs Inputs
    {
        get
        {
            IMathcadPrimeInputs inputs = RealSubjectInstance.Inputs;
            try
            {
                return new Inputs(inputs);
            }
            catch
            {
            }
            return null;
        }
    }

    public IMathcadPrimeOutputs Outputs
    {
        get
        {
            IMathcadPrimeOutputs outputs = RealSubjectInstance.Outputs;
            try
            {
                return new Outputs(outputs);
            }
            catch
            {
            }
            return null;
        }
    }

    public string WorksheetTabIcon
    {
        get => RealSubjectInstance.WorksheetTabIcon;
        set => RealSubjectInstance.WorksheetTabIcon = value;
    }

    public string WorksheetTabName
    {
        get => RealSubjectInstance.WorksheetTabName;
        set => RealSubjectInstance.WorksheetTabName = value;
    }

    public string WorksheetClosingPrompt
    {
        get => RealSubjectInstance.WorksheetClosingPrompt;
        set => RealSubjectInstance.WorksheetClosingPrompt = value;
    }

    public string WorksheetDisplayedFilePath
    {
        get => RealSubjectInstance.WorksheetDisplayedFilePath;
        set => RealSubjectInstance.WorksheetDisplayedFilePath = value;
    }

    public string WorksheetWorkingDirectory
    {
        get => RealSubjectInstance.WorksheetWorkingDirectory;
        set => RealSubjectInstance.WorksheetWorkingDirectory = value;
    }

    public int DefaultCalculationTimeout
    {
        get => RealSubjectInstance.DefaultCalculationTimeout;
        set => RealSubjectInstance.DefaultCalculationTimeout = value;
    }

    private Worksheet()
    {
    }

    internal Worksheet(IMathcadPrimeWorksheet mathcadPrimeWorksheet)
    {
        _realSubjectInstance = mathcadPrimeWorksheet as IMathcadPrimeWorksheet3;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    internal Worksheet(IMathcadPrimeWorksheet2 mathcadPrimeWorksheet2)
    {
        _realSubjectInstance = mathcadPrimeWorksheet2 as IMathcadPrimeWorksheet3;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    internal Worksheet(IMathcadPrimeWorksheet3 mathcadPrimeWorksheet3)
    {
        _realSubjectInstance = mathcadPrimeWorksheet3;
        if (_realSubjectInstance == null)
        {
            throw new Exception("The real subject cannot be null.");
        }
    }

    ~Worksheet()
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

    public void SetTitle(string titleArg)
    {
        RealSubjectInstance.SetTitle(titleArg);
    }

    public void Save()
    {
        RealSubjectInstance.Save();
    }

    public void SaveAs(string fileName)
    {
        RealSubjectInstance.SaveAs(fileName);
    }

    public void Synchronize()
    {
        RealSubjectInstance.Synchronize();
    }

    public void PauseCalculation()
    {
        RealSubjectInstance.PauseCalculation();
    }

    public void ResumeCalculation()
    {
        RealSubjectInstance.ResumeCalculation();
    }

    public int SetRealValue(string aliasArg, double valueArg, string unitsArg)
    {
        return RealSubjectInstance.SetRealValue(aliasArg, valueArg, unitsArg);
    }

    public IMathcadPrimeInputResult InputGetRealValue(string aliasArg)
    {
        IMathcadPrimeInputResult mathcadPrimeInputResult = RealSubjectInstance.InputGetRealValue(aliasArg);
        try
        {
            return new InputResult(mathcadPrimeInputResult);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeOutputResult OutputGetRealValue(string aliasArg)
    {
        IMathcadPrimeOutputResult mathcadPrimeOutputResult = RealSubjectInstance.OutputGetRealValue(aliasArg);
        try
        {
            return new OutputResult(mathcadPrimeOutputResult);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeOutputResultAs OutputGetRealValueAs(string aliasArg, string unitsArg)
    {
        IMathcadPrimeOutputResultAs mathcadPrimeOutputResultAs = RealSubjectInstance.OutputGetRealValueAs(aliasArg, unitsArg);
        try
        {
            return new OutputResultAs(mathcadPrimeOutputResultAs);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeMatrix CreateMatrix(int rowsArg, int columnsArg)
    {
        IMathcadPrimeMatrix mathcadPrimeMatrix = RealSubjectInstance.CreateMatrix(rowsArg, columnsArg);
        try
        {
            return new Matrix(mathcadPrimeMatrix);
        }
        catch
        {
        }
        return null;
    }

    public int SetMatrixValue(string aliasArg, IMathcadPrimeMatrix valueArg, string unitsArg)
    {
        return RealSubjectInstance.SetMatrixValue(aliasArg, ((IProxyComRealSubject<IMathcadPrimeMatrix>)valueArg).RealSubjectInstance, unitsArg);
    }

    public IMathcadPrimeInputMatrixResult InputGetMatrixValue(string aliasArg)
    {
        IMathcadPrimeInputMatrixResult mathcadPrimeInputMatrixResult = RealSubjectInstance.InputGetMatrixValue(aliasArg);
        try
        {
            return new InputMatrixResult(mathcadPrimeInputMatrixResult);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeOutputMatrixResult OutputGetMatrixValue(string aliasArg)
    {
        IMathcadPrimeOutputMatrixResult mathcadPrimeOutputMatrixResult = RealSubjectInstance.OutputGetMatrixValue(aliasArg);
        try
        {
            return new OutputMatrixResult(mathcadPrimeOutputMatrixResult);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeOutputMatrixResultAs OutputGetMatrixValueAs(string aliasArg, string unitsArg)
    {
        IMathcadPrimeOutputMatrixResultAs mathcadPrimeOutputMatrixResultAs = RealSubjectInstance.OutputGetMatrixValueAs(aliasArg, unitsArg);
        try
        {
            return new OutputMatrixResultAs(mathcadPrimeOutputMatrixResultAs);
        }
        catch
        {
        }
        return null;
    }

    public bool IsOpen()
    {
        return RealSubjectInstance.IsOpen();
    }

    public void Activate()
    {
        RealSubjectInstance.Activate();
    }

    public void Close(SaveOption saveOptionArg)
    {
        RealSubjectInstance.Close(saveOptionArg);
    }

    public string GetTitle()
    {
        return RealSubjectInstance.GetTitle();
    }

    public object GetWorksheetReadOnlyOptionValue(WorksheetReadonlyOptionNames optionNameArg)
    {
        return RealSubjectInstance.GetWorksheetReadOnlyOptionValue(optionNameArg);
    }

    public IMathcadPrimeValuesSetter CreateValuesSetter()
    {
        IMathcadPrimeValuesSetter mathcadPrimeValuesSetter = RealSubjectInstance.CreateValuesSetter();
        try
        {
            return new ValuesSetter(mathcadPrimeValuesSetter);
        }
        catch
        {
        }
        return null;
    }

    public int SetStringValue(string aliasArg, string valueArg)
    {
        return RealSubjectInstance.SetStringValue(aliasArg, valueArg);
    }

    public string InputGetStringValue(string aliasArg)
    {
        return RealSubjectInstance.InputGetStringValue(aliasArg);
    }

    public string OutputGetStringValue(string aliasArg)
    {
        return RealSubjectInstance.OutputGetStringValue(aliasArg);
    }

    public int SetSExprValue(string aliasArg, string sexpressionArg)
    {
        return RealSubjectInstance.SetSExprValue(aliasArg, sexpressionArg);
    }

    public string InputGetSExprValue(string aliasArg)
    {
        return RealSubjectInstance.InputGetSExprValue(aliasArg);
    }

    public IMathcadPrimeGetValueResult InputGetValue(string aliasArg)
    {
        IMathcadPrimeGetValueResult mathcadPrimeGetValueResult = RealSubjectInstance.InputGetValue(aliasArg);
        try
        {
            return new GetValueResult(mathcadPrimeGetValueResult);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeGetValueResult OutputGetValue(string aliasArg)
    {
        IMathcadPrimeGetValueResult mathcadPrimeGetValueResult = RealSubjectInstance.OutputGetValue(aliasArg);
        try
        {
            return new GetValueResult(mathcadPrimeGetValueResult);
        }
        catch
        {
        }
        return null;
    }
}
