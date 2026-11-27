using Quire.FunctionalitiesLimitation;

using System;
using System.Linq;
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
using IValue = Mathcad.IValue;
using System.Collections.Generic;
namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelContainerModel
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.Model.ExcelContainerModel+DisconnectedHResult, Ptc.Controls.ExcelComponent")]
    private enum DisconnectedHResult : uint
    {
        Excel_RPC_E_DISCONNECTED = 2147746216u,
        CO_E_OBJNOTCONNECTED = 2147746301u,
        RPC_E_DISCONNECTED = 2147549448u,
    }

    private global::System.Nullable<bool> _excelVersionInstalled;

    private static readonly string _excelSheetNotFoundFormat = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("excel_sheet_not_found", 1);

    private static readonly string _excelNotSupportedValueTypeFormat = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("excel_not_supported_value_type", 1);

    private static readonly string _excelInvalidRangeFormat = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("excel_range_address", 1);

    private static readonly string _nonContiguousRangeErrorFormat = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("non_contiguous_range", 0);

    private static readonly string _comExceptionError = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("com_exception", 0);

    private static readonly string _excelVersionNotInstalledErrorFormat = OleDataExchangeContainerModel<excelContainerType>.ConvertToEngineError("excel_version_not_installed", 0);

    internal const string DefaultRangeFirstCell = "A1";

    internal const string DefaultRangeLastCell = "G6";

    private readonly global::Quire.Services.IApplicationBehaviorOptions _appBehaviorOptions;

    private readonly global::System.Windows.Threading.DispatcherTimer _postOleClosedTimer;

    private global::Quire.Microsoft.Office.Interop.Excel.AppEvents_Event _appEvents;

    protected override bool CheckOleActiveState => true;

    public event global::System.Action RequestEvaluation;

    public QueryAttachedInputsOrOutputsHandler QueryAttachedInputsOrOutputs { get; set; }

    public bool? GetExcelVersionInstalled(bool forceRun)
    {
        if (!_excelVersionInstalled.HasValue && (forceRun || IsOleSiteManagerRunning))
        {
            _excelVersionInstalled = OleUtils.IsVersionInstalled(GetOleObject(forceRun));
        }
        return _excelVersionInstalled;
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

    public override global::Mathcad.IValue GetValue(string place)
    {
        CheckInstalledAndAvailable(forceRun: true);
        try
        {
            global::Quire.Microsoft.Office.Interop.Excel.Range range = GetRange(place, forceRun: true);
            if (range.Areas.Count > 1)
            {
                throw new AddressErrorException(_nonContiguousRangeErrorFormat);
            }
            if (range.Count > 1)
            {
                global::Quire.Automation.MatrixValue matrixValue = new global::Quire.Automation.MatrixValue();
                object[,] array = (dynamic)range.get_Value(Type.Missing);
                int count = range.Rows.Count;
                int columnCount = range.Columns.Count;
                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        int excelRow = i + 1;
                        int excelCol = j + 1;
                        matrixValue.SetElement(i, j, ConvertToIValue(array[i + 1, j + 1], () => (dynamic)((global::Quire.Microsoft.Office.Interop.Excel.Range)(dynamic)range.get__Default(excelRow, excelCol)).Text));
                    }
                }
                return (global::Mathcad.IValue)matrixValue;
            }
            return GetCellValue(range);
        }
        catch (COMException innerException)
        {
            throw new Exception(_comExceptionError, innerException);
        }
    }

    private static global::Mathcad.IValue GetCellValue(global::Quire.Microsoft.Office.Interop.Excel.Range range)
    {
        return ConvertToIValue(range.get_Value(Type.Missing), () => (string)(dynamic)range.Text);
    }


    public override void Commit()
    {
        AssertDispatcherAccess();
        CheckInstalledAndAvailable(forceRun: true);
        CalculateWorkbook(forceRun: true);
        ClearMessage(MessageCategory.UnsynchronizedResults);
    }

    public override bool ShouldCommitAtLeastOnce => true;

    public override bool ShouldCommitIfDirty => true;

    private global::Quire.Microsoft.Office.Interop.Excel.Sheets GetWorksheets(bool forceRun)
    {
        global::Quire.Microsoft.Office.Interop.Excel.Workbook workbook = GetWorkbook(forceRun);
        if (workbook == null)
        {
            return null;
        }
        return workbook.Worksheets;
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
        catch (COMException comException)
        {
            throw new Exception(_comExceptionError, comException);
        }
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

    public global::Quire.Microsoft.Office.Interop.Excel.Range GetRange(string rangeAddress, bool forceRun)
    {
        // Mathcad allocates the worksheet locals before the split array in Release IL.
        string address;
        global::Quire.Microsoft.Office.Interop.Excel.Worksheet worksheet;
        string[] array = rangeAddress.Split('!');
        string sheetName;
        int rangePartCount = array.Length;
        switch (rangePartCount)
        {
            case 1:
                address = array[0];
                try
                {
                    global::Quire.Microsoft.Office.Interop.Excel.Sheets worksheets = GetWorksheets(forceRun);
                    worksheet = (global::Quire.Microsoft.Office.Interop.Excel.Worksheet)(dynamic)worksheets[1];
                }
                catch (COMException innerException)
                {
                    throw new Exception(_comExceptionError, innerException);
                }
                break;
            case 2:
            {
                sheetName = array[0];
                address = array[1];
                try
                {
                    global::Quire.Microsoft.Office.Interop.Excel.Sheets worksheets = GetWorksheets(forceRun);
                    worksheet = (global::Quire.Microsoft.Office.Interop.Excel.Worksheet)(dynamic)worksheets[sheetName];
                }
                catch (COMException)
                {
                    throw new AddressErrorException(string.Format(_excelSheetNotFoundFormat, sheetName));
                }
                break;
            }
            default:
                throw new AddressErrorException(string.Format(_excelSheetNotFoundFormat, rangeAddress));
        }
        try
        {
            return ((global::Quire.Microsoft.Office.Interop.Excel._Worksheet)worksheet)[(object)address, Type.Missing];
        }
        catch (COMException)
        {
            throw new AddressErrorException(string.Format(_excelInvalidRangeFormat, address));
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

    private static global::Mathcad.IValue ConvertToIValue(object value, global::System.Func<string> getDefaultText) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public global::System.Nullable<bool> IsAsIcon { get; set; }

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

    public string GetSelection()
    {
        if (!IsOleSiteManagerRunning)
        {
            return null;
        }
        global::Quire.Microsoft.Office.Interop.Excel.Workbook safeWorkBook = GetSafeWorkBook(forceRun: false);
        global::Quire.Microsoft.Office.Interop.Excel.Application safeApplication = GetSafeApplication(forceRun: false);
        if (safeWorkBook != null && safeApplication != null)
        {
            global::Quire.Microsoft.Office.Interop.Excel.Range range = safeApplication.Selection as global::Quire.Microsoft.Office.Interop.Excel.Range;
            if (range != null && (dynamic)range.Worksheet.Parent == safeWorkBook)
            {
                return GetRangeFullName(range);
            }
        }
        return null;
    }

    private static string GetRangeFullName(Microsoft.Office.Interop.Excel.Range range)
    {
        return string.Format("{0}!{1}", range.Worksheet.Name, range[(object)false, (object)false, XlReferenceStyle.xlA1, Type.Missing, Type.Missing]);
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

    public bool NeedRecalculateOnClose { get; set; }

    private void OnOleShownSync()
    {
        NeedRecalculateOnClose = true;
        RegisterOnActivateHandler();
    }

    private void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation();
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

    public override string ProgId => "Excel.Sheet";

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        DisableWorkbookCalculation(disable: true);
        RegisterOnActivateHandler();
    }

    private global::Quire.Microsoft.Office.Interop.Excel.AppEvents_Event AppEvents { set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }

    private void RegisterOnActivateHandler()
    {
        Application safeApplication = GetSafeApplication(forceRun: false);
        if (DebugTools.AssertNotNull(safeApplication, null).Passed)
        {
            AppEvents = safeApplication;
        }
    }

    private void OnWorkbookOpen(global::Quire.Microsoft.Office.Interop.Excel.Workbook wb)
    {
    }

    private void OnWorkbookActivate(global::Quire.Microsoft.Office.Interop.Excel.Workbook activeWb) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public bool HasAttachedInputsOrOutputs(bool checkOutputOnly)
    {
        if (QueryAttachedInputsOrOutputs != null)
        {
            return QueryAttachedInputsOrOutputs(checkOutputOnly);
        }
        return false;
    }

    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)797;

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

    private bool IsDeactivateInitiatedFromMathcad { get; set; }

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


    [global::Quire.Mirror.MirrorOf("Ptc.Controls.ExcelComponent.Model.ExcelContainerModel+CVErrEnum, Ptc.Controls.ExcelComponent")]
    private enum CVErrEnum
    {
        ErrDiv0 = -2146826281,
        ErrNA = -2146826246,
        ErrName = -2146826259,
        ErrNull = -2146826288,
        ErrNum = -2146826252,
        ErrRef = -2146826265,
        ErrValue = -2146826273,
    }

    protected override bool IsDisconnected
    {
        get
        {
            object oleObject = GetOleObject(forceRun: false, ignoreRunningState: true);
            try
            {
                _ = ((Workbook)oleObject).Application;
            }
            catch (Exception ex)
            {
                int value;
                if (ex is InvalidCastException)
                {
                    IntPtr interfacePointer;
                    Guid iid = typeof(Workbook).GUID;
                    value = Marshal.QueryInterface(Marshal.GetIUnknownForObject(oleObject), in iid, out interfacePointer);
                }
                else
                {
                    value = Marshal.GetHRForException(ex);
                }
                return (from hRes in Enum.GetValues(typeof(DisconnectedHResult)).OfType<DisconnectedHResult>()
                    select (uint)hRes).Contains((uint)value);
            }
            return base.IsDisconnected;
        }
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

    private void CalculateWorkbook(bool forceRun)
    {
        throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public override void Dispose()
    {
        AppEvents = null;
        base.Dispose();
    }

    void IDataExchangeModel.Unlock()
    {
        Unlock();
    }

}
