using Quire.FunctionalitiesLimitation;

using System;
using Quire.Controls.Core.AttachedBehavior;
using System.Runtime.InteropServices;
using Quire.Microsoft.Office.Interop.Excel;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Ole;
using Quire.Core;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.OLE;
namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelContainerModel
{
    public override string ProgId => "Excel.Sheet";

    public override bool ShouldCommitAtLeastOnce => true;

    public override bool ShouldCommitIfDirty => true;

    protected override bool CheckOleActiveState => true;

    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)797;

    public global::System.Nullable<bool> IsAsIcon { get; set; }

    public bool NeedRecalculateOnClose { get; set; }

    public QueryAttachedInputsOrOutputsHandler QueryAttachedInputsOrOutputs { get; set; }

    private bool IsDeactivateInitiatedFromMathcad { get; set; }

    private void OnWorkbookOpen(global::Quire.Microsoft.Office.Interop.Excel.Workbook wb)
    {
    }

    internal global::Quire.Microsoft.Office.Interop.Excel.Workbook GetSafeWorkBook(bool forceRun)
    {
        try
        {
            return GetWorkbook(forceRun);
        }
        catch
        {
            return null;
        }
    }

    private void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation();
        }
    }

    private global::Quire.Microsoft.Office.Interop.Excel.Sheets GetWorksheets(bool forceRun)
    {
        global::Quire.Microsoft.Office.Interop.Excel.Workbook workbook = GetWorkbook(forceRun);
        if (workbook == null)
        {
            return null;
        }
        return workbook.Worksheets;
    }

    protected override void CheckInstalledAndAvailable(bool forceRun)
    {
        if (GetExcelVersionInstalled(forceRun) == false)
        {
            throw new Exception(_excelVersionNotInstalledErrorFormat);
        }
        if (!CheckServerAvailable(forceRun))
        {
            throw new Exception(_comExceptionError);
        }
        base.CheckInstalledAndAvailable(forceRun);
    }

    public override void Commit()
    {
        AssertDispatcherAccess();
        CheckInstalledAndAvailable(forceRun: true);
        CalculateWorkbook(forceRun: true);
        ClearMessage(MessageCategory.UnsynchronizedResults);
    }

    private Workbook GetWorkbook(bool forceRun)
    {
        object oleObject = GetOleObject(forceRun);
        try
        {
            return (Workbook)oleObject;
        }
        catch (InvalidCastException innerException)
        {
            throw new Exception(_comExceptionError, innerException);
        }
        catch (COMException innerException2)
        {
            throw new Exception(_comExceptionError, innerException2);
        }
    }

    private Application GetSafeApplication(bool forceRun)
    {
        try
        {
            return GetSafeWorkBook(forceRun).Application;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsExcelErrorValue(object obj)
    {
        if (obj is int num)
        {
            return Enum.IsDefined(typeof(CVErrEnum), num);
        }
        return false;
    }

    private static object ConvertFromIValue(IValue value)
    {
        switch (value.Type)
        {
            case "String":
                return ((Quire.Automation.StringValue)value).Value;
            case "Numeric":
                {
                    double real = ((Quire.Automation.NumericValue)value).Real;
                    if (double.IsNaN(real))
                    {
                        return null;
                    }
                    return real;
                }
            case "Matrix":
                {
                    Quire.Automation.MatrixValue matrixValue = (Quire.Automation.MatrixValue)value;
                    object[,] array = new object[matrixValue.Rows, matrixValue.Cols];
                    for (int i = 0; i < matrixValue.Rows; i++)
                    {
                        for (int j = 0; j < matrixValue.Cols; j++)
                        {
                            array[i, j] = ConvertFromIValue(matrixValue.GetElement(i, j) as IValue);
                        }
                    }
                    return array;
                }
            default:
                throw new Exception(string.Format(_excelNotSupportedValueTypeFormat, value.Type));
        }
    }

    private static string GetRangeFullName(Microsoft.Office.Interop.Excel.Range range)
    {
        return string.Format("{0}!{1}", range.Worksheet.Name, range[(object)false, (object)false, XlReferenceStyle.xlA1, Type.Missing, Type.Missing]);
    }

    private static bool HasVisibleWindow(Microsoft.Office.Interop.Excel.Windows windows)
    {
        if (windows == null)
        {
            return false;
        }
        int count = windows.Count;
        for (int i = 1; i <= count; i++)
        {
            if (windows[i].Visible)
            {
                return true;
            }
        }
        return false;
    }

    public bool HasAttachedInputsOrOutputs(bool checkOutputOnly)
    {
        if (QueryAttachedInputsOrOutputs != null)
        {
            return QueryAttachedInputsOrOutputs(checkOutputOnly);
        }
        return false;
    }

    protected override bool BeforeActivate()
    {
        if (base.BeforeActivate())
        {
            DisableWorkbookCalculation(disable: false);
            ClearMessage(MessageCategory.UnsynchronizedResults);
            return true;
        }
        return false;
    }

    protected override void AfterActivate()
    {
        DebugTools.DevAssert(NeedRecalculateOnClose, null);
        base.AfterActivate();
    }

    public void HideOleObject()
    {
        Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
        if (safeWorkBook != null)
        {
            Microsoft.Office.Interop.Excel.Windows windows = safeWorkBook.Windows;
            int count = windows.Count;
            for (int i = 1; i <= count; i++)
            {
                windows[i].Visible = false;
            }
        }
    }

    private void OnOleShownSync()
    {
        NeedRecalculateOnClose = true;
        RegisterOnActivateHandler();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        DisableWorkbookCalculation(disable: true);
        RegisterOnActivateHandler();
    }
}
