// Mathcad IL-order whole-class partial of Spirit.CMathcadPrimeWorksheet (MathcadPrime 12.0.1.0).
// Unfinished members are unmarked throw placeholders in Mathcad order.
#nullable disable
#pragma warning disable

using Quire.Automation;
using System.Windows.Threading;

namespace Quire;

public partial class CMathcadPrimeWorksheet
{
    private readonly bool _isUiInteractiveModeDisabled;

    private WorksheetData worksheetData { get; set; }

    [global::Quire.Mirror.MirrorStub]
    public CMathcadPrimeWorksheet(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, bool isUiInteractiveModeDisabled) { }

    public string Name
    {
        get
        {
            try
            {
                return worksheetData.DocumentName;
            }
            catch
            {
            }

            return string.Empty;
        }
    }

    public string FullName
    {
        get
        {
            try
            {
                return worksheetData.DocumentFullName;
            }
            catch
            {
            }

            return string.Empty;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            try
            {
                DelegateFunction0<bool> getter = DoIsReadOnly;
                return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return true;
        }
    }

    private bool DoIsReadOnly() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool Modified
    {
        get
        {
            try
            {
                DelegateFunction0<bool> getter = DoGetModified;
                return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return false;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private bool DoGetModified() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetModified(bool isModified) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SetTitle(string titleArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetTitle(string title) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string GetTitle() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private string DoGetTitle() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Save() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool DoSave() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool DoSave(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void SaveAs(string fileName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool DoSaveAs(string newDocumentPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool TrySaveAsPdf(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, string newDocumentPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool TrySaveAsToPtcServer(string newDocumentPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool TrySaveAsInMathcadPrimeFormatLocally(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, string newDocumentPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool TrySaveAsInNonMathcadPrimeFormatLocally(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument, string newDocumentPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Synchronize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSynchronize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void PauseCalculation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void ResumeCalculation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoPauseCalculation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoResumeCalculation() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void UpdateCalculationStatus(bool status) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int SetRealValue(string aliasArg, double valueArg, string unitsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IOManagerErrorCode DoSetRealValue(string alias, double value, string units) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeInputs Inputs
    {
        get
        {
            try
            {
                DelegateFunction0<AutomationSpiritInputs> getter = DoGetInputs;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as IMathcadPrimeInputs;
            }
            catch
            {
            }

            return null;
        }
    }

    private global::Quire.AutomationSpiritInputs DoGetInputs() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeOutputs Outputs
    {
        get
        {
            try
            {
                DelegateFunction0<AutomationSpiritOutputs> getter = DoGetOutputs;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as IMathcadPrimeOutputs;
            }
            catch
            {
            }

            return null;
        }
    }

    private global::Quire.AutomationSpiritOutputs DoGetOutputs() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeInputResult InputGetRealValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeOutputResult OutputGetRealValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IScalarItem DoGetRealValue(string alias, bool isInput) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeOutputResultAs OutputGetRealValueAs(string aliasArg, string unitsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IScalarItem DoOutputGetRealValueAs(string alias, string units) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeMatrix CreateMatrix(int rowsArg, int columnsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int SetMatrixValue(string aliasArg, global::Quire.Automation.IMathcadPrimeMatrix valueArg, string unitsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IOManagerErrorCode DoSetMatrixValue(string alias, global::Quire.Automation.IMathcadPrimeMatrix value, string units) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeInputMatrixResult InputGetMatrixValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeOutputMatrixResult OutputGetMatrixValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IMatrixItem DoGetMatrixValue(string alias, bool isInput) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeOutputMatrixResultAs OutputGetMatrixValueAs(string aliasArg, string unitsArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IMatrixItem DoOutputGetMatrixValueAs(string alias, string units) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public bool IsOpen()
    {
        try
        {
            DelegateFunction0<bool> getter = DoIsDocumentOpen;
            return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return false;
    }

    private bool DoIsDocumentOpen() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Activate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoActivate() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void ActivateWorksheet(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public void Close(global::Quire.Automation.SaveOption saveOptionArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoClose(global::Quire.Automation.SaveOption saveOption) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IEngineeringDocument GetAssociatedEngineeringDocument() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool IsAssociatedEngineeringDocument(global::Quire.Controls.Core.IEngineeringDocument engineeringDocument) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string WorksheetTabIcon
    {
        get
        {
            try
            {
                DelegateFunction0<string> getter = DoGetWorksheetTabIcon;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private string DoGetWorksheetTabIcon() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetWorksheetTabIcon(string iconFullPath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string WorksheetTabName
    {
        get
        {
            try
            {
                DelegateFunction0<string> getter = DoGetWorksheetTabName;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private string DoGetWorksheetTabName() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetWorksheetTabName(string tabName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string WorksheetClosingPrompt
    {
        get
        {
            try
            {
                DelegateFunction0<string> getter = DoGetClosingPrompt;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private string DoGetClosingPrompt() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetClosingPrompt(string text) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string WorksheetDisplayedFilePath { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private string DoGetWorksheetDisplayedFilePath() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetWorksheetDisplayedFilePath(string filePath) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string WorksheetWorkingDirectory
    {
        get
        {
            try
            {
                DelegateFunction0<string> getter = DoGetWorksheetWorkingDirectory;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private string DoGetWorksheetWorkingDirectory() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetWorksheetWorkingDirectory(string workingDir) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public object GetWorksheetReadOnlyOptionValue(global::Quire.Automation.WorksheetReadonlyOptionNames optionNameArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private object DoGetWorksheetReadOnlyOptionValue(global::Quire.Automation.WorksheetReadonlyOptionNames optionName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeValuesSetter CreateValuesSetter() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IValuesSetter DoCreateScaledValueSettersCollection() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int SetStringValue(string aliasArg, string valueArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IOManagerErrorCode DoSetStringValue(string alias, string stringValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string InputGetStringValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string OutputGetStringValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private string DoGetStringValue(string alias, bool isInput) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int SetSExprValue(string aliasArg, string sexpressionArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.IOManagerErrorCode DoSetSExprValue(string alias, string sexpression) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public string InputGetSExprValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private string DoInputGetSExprValue(string alias) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeGetValueResult InputGetValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Automation.IMathcadPrimeGetValueResult OutputGetValue(string aliasArg) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private global::Quire.Controls.Core.IORegions.ICommonValueItem DoGetCommonValue(string alias, bool isInput) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public int DefaultCalculationTimeout
    {
        get
        {
            try
            {
                DelegateFunction0<int> getter = DoGetDefaultCalculationTimeout;
                return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(getter, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return 0;
        }
        set
        {
            throw global::Quire.Mirror.MirrorStub.NotImplemented();
        }
    }

    private int DoGetDefaultCalculationTimeout() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void DoSetDefaultCalculationTimeout(int defaultCalculationTimeout) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

}
