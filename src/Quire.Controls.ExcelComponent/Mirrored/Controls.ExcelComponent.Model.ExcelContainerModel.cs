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
using System.Collections.Generic;
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

    public bool? GetExcelVersionInstalled(bool forceRun)
    {
        if (!_excelVersionInstalled.HasValue && (forceRun || IsOleSiteManagerRunning))
        {
            _excelVersionInstalled = OleUtils.IsVersionInstalled(GetOleObject(forceRun));
        }
        return _excelVersionInstalled;
    }

    public void SetValue(string place, object value, bool forceRun)
    {
        Microsoft.Office.Interop.Excel.Range range = GetRange(place, forceRun);
        try
        {
            if (value is Array)
            {
                if (range.Areas.Count > 1)
                {
                    throw new AddressErrorException(_nonContiguousRangeErrorFormat);
                }
                object[,] array = (object[,])value;
                range.get_Resize((object)array.GetLength(0), (object)array.GetLength(1)).set_Value(Type.Missing, value);
            }
            else
            {
                range.set_Value(Type.Missing, value);
            }
        }
        catch (COMException innerException)
        {
            throw new Exception(_comExceptionError, innerException);
        }
    }

    public override void SetValue(string place, IValue value, IValueToIResultNodeTransform resultsTransform)
    {
        CheckInstalledAndAvailable(forceRun: true);
        SetValue(place, ConvertFromIValue(value), forceRun: true);
    }

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, excelContainerType excelContainer)
    {
        Unpack(d11NProvider, excelContainer.itemidref);
        IsAsIcon = excelContainer.IsIcon;
        return null;
    }

    public override excelContainerType GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy)
    {
        DebugTools.DevAssert(IsAsIcon != true, null);
        return new excelContainerType(SizeData)
        {
            itemidref = Pack(s11NProvider, uniqueId, ExternalObjectType.Excel, onServerBusy)
        };
    }

    public bool Select(string rangeAddress, bool forceRun)
    {
        try
        {
            GetRange(rangeAddress, forceRun).Select();
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected override OleResult CheckServerStatus(bool forceRun)
    {
        OleResult oleResult = base.CheckServerStatus(forceRun);
        if (oleResult != OleResult.Success)
        {
            return oleResult;
        }
        try
        {
            Application safeApplication = GetSafeApplication(forceRun);
            safeApplication.Interactive = safeApplication.Interactive;
            return OleResult.Success;
        }
        catch (COMException)
        {
            return OleResult.UnknownFailure;
        }
        catch
        {
            return OleResult.Success;
        }
    }

    protected override void OnOleClosedSync()
    {
        base.OnOleClosedSync();
        if (IsDeactivateInitiatedFromMathcad)
        {
            _postOleClosedTimer.Interval = TimeSpan.Zero;
        }
        else
        {
            _postOleClosedTimer.Interval = TimeSpan.FromMilliseconds(_appBehaviorOptions.DelayRecalculationInMsecAfterCloseExcel);
        }
        _postOleClosedTimer.Start();
    }

    protected override void DoDeactivate()
    {
        try
        {
            IsDeactivateInitiatedFromMathcad = true;
            Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
            Application safeApplication = GetSafeApplication(forceRun: false);
            safeWorkBook.Close(Type.Missing, Type.Missing, Type.Missing);
            if (safeApplication != null && safeApplication.Visible && !safeApplication.UserControl && !HasVisibleWindow(safeApplication.Windows))
            {
                safeApplication.Visible = false;
            }
        }
        catch
        {
            base.DoDeactivate();
        }
        finally
        {
            IsDeactivateInitiatedFromMathcad = false;
        }
    }

    private void PostOleClosed(object sender, EventArgs args)
    {
        _postOleClosedTimer.Stop();
        if (IsOutOfPlaceActive)
        {
            return;
        }
        if (NeedRecalculateOnClose)
        {
            NeedRecalculateOnClose = false;
            FireRequestEvaluation();
        }
        ClearMessage(MessageCategory.UnsynchronizedResults);
    }

    private void RegisterOnActivateHandler()
    {
        Application safeApplication = GetSafeApplication(forceRun: false);
        if (DebugTools.AssertNotNull(safeApplication, null).Passed)
        {
            AppEvents = safeApplication;
        }
    }

    private void OnWorkbookActivateAsync(string name)
    {
        if (IsOutOfPlaceActive)
        {
            return;
        }
        try
        {
            Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
            Application safeApplication = GetSafeApplication(forceRun: false);
            if (safeApplication != null && safeApplication.Visible && safeWorkBook != null && safeWorkBook.Name == name && HasVisibleWindow(safeWorkBook.Windows))
            {
                NeedRecalculateOnClose = true;
                SetMessage(MessageCategory.UnsynchronizedResults, NodeMessageAdornerType.Warning, "excel_workbook_unhidden");
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
    }

    private void DisableWorkbookCalculation(bool disable)
    {
        Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
        try
        {
            foreach (Worksheet worksheet in safeWorkBook.Worksheets)
            {
                worksheet.EnableCalculation = !disable;
            }
        }
        catch (Exception ex)
        {
            if (IsOleSiteManagerRunning)
            {
                DebugTools.FailAssertion("unable to switch calculation mode due to {0}", ex.Message);
            }
        }
    }

    private void EnableWorksheetsCalculation(Sheets worksheets, bool enable)
    {
        try
        {
            foreach (Worksheet worksheet in worksheets)
            {
                worksheet.EnableCalculation = enable;
            }
        }
        catch (Exception ex)
        {
            if (IsOleSiteManagerRunning)
            {
                DebugTools.FailAssertion("unable to switch calculation mode due to {0}", ex.Message);
            }
        }
    }

    public bool? IsExcelCalculationManualModel()
    {
        bool? result = null;
        Application safeApplication = GetSafeApplication(forceRun: false);
        if (safeApplication != null)
        {
            bool visible = safeApplication.Visible;
            if (visible && safeApplication.ActiveWorkbook != null)
            {
                result = IsExcelCalculationManualModel(safeApplication);
            }
            else
            {
                Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
                if (safeWorkBook != null)
                {
                    Window window = safeWorkBook.Windows[1];
                    if (window != null)
                    {
                        bool visible2 = window.Visible;
                        try
                        {
                            window.Visible = true;
                            result = IsExcelCalculationManualModel(safeApplication);
                        }
                        finally
                        {
                            window.Visible = visible2;
                            safeApplication.Visible = visible;
                        }
                    }
                }
            }
        }
        return result;
    }

    private static bool? IsExcelCalculationManualModel(Application application)
    {
        try
        {
            return application.Calculation == XlCalculation.xlCalculationManual;
        }
        catch
        {
            return null;
        }
    }

    void IDataExchangeModel.Unlock()
    {
        Unlock();
    }
}
