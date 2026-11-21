#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Printing;
using System.Runtime;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.InputControls;
using Quire.Controls.Worksheet;
using Quire.Core;
using Quire.FunctionalitiesLimitation;
using Quire.IO;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;
using Quire.Wpf.Printing;
using Quire.Xml;

namespace Quire.Controls;

public partial class EngineeringDocument
{
    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public void CloseOpenedComboBoxEditingMode() => WpfUtils.ExecuteCommandIfFocusedElementIsDescendantOf<EngineeringDocument>(this, EditableComboBoxCommands.CloseEditingMode);

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private readonly ManualResetEvent _waitForLoad;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public void WaitForLoad(int seconds) => FileManager.WaitFor(seconds, _waitForLoad);

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private void FireCalculationOptionUpdated(string name, object value) => IsDirty = true;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private static void TraceWarning(string format, params object[] args) => Trace.TraceWarning(format, args);

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public IWorksheetControl Worksheet => WorksheetControl;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool NeedsDictionaryRefresh { get; set; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public double Zoom { get => DocumentAttachedProperties.GetZoom(this); set => DocumentAttachedProperties.SetZoom<EngineeringDocument>(this, value); }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public string FilePath { get => DocumentAttachedProperties.GetFilePath(this); set => DocumentAttachedProperties.SetFilePath(this, value); }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public long MaxUpdateLayoutTimeMsec { get; set; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public IWorksheetPersistentData WorksheetData { get; private set; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private bool _isNetworkDriveDisconnected;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsNetworkDriveDisconnected => _isNetworkDriveDisconnected;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public ISerializationHelper DocumentSerializationHelper { get; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private static bool _isDocumentFromPtcPlaces;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsDocumentFromPTCPlaces { get => _isDocumentFromPtcPlaces; set => _isDocumentFromPtcPlaces = value; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private readonly ITextStylesManager _textStylesManager;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public ITextStylesManager TextStylesManager => _textStylesManager;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private bool _isNew;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsNew => _isNew;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnPrepareSnapshotStart;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnPrepareSnapshotEnd;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument> IsDirtyChanged;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnDispose;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<string> CwdUpdated;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument> FilePathChanged;

    [MirrorOf("Ptc.Controls.EngineeringDocument+PropertyNames, Ptc.Controls")]
    public static class PropertyNames
    {
        public const string IsDocumentATemplate = "IsDocumentATemplate";
        public const string ReadonlyOptions = "ReadonlyOptions";
        public const string Zoom = "Zoom";
        public const string Worksheet = "Worksheet";
        public const string DocumentTabIcon = "DocumentTabIcon";
        public const string DisplayFilePath = "DisplayFilePath";
        public const string DisplayFileName = "DisplayFileName";
        public const string DisplaySavedDate = "DisplaySavedDate";
        public const string DisplayDocumentName = "DisplayDocumentName";
        public const string DisplayWindowTitle = "DisplayWindowTitle";
    }

    [MirrorOf("Ptc.Controls.EngineeringDocument+CustomizedValues, Ptc.Controls")]
    private struct CustomizedValues
    {
        public DateTime? SavedDate;
        public string FilePath;
        public string DocumentName;
        public string WindowTitle;
        public string TabIcon;
        public string ClosePrompt;
    }

    private CustomizedValues _customizedValues;
    private DocumentReadonlyOptions _readonlyOptions;
    private string _unsavedDocumentName;

    [MirrorOf("Ptc.Controls.EngineeringDocument+DisplayPropertyChangeNotifier, Ptc.Controls")]
    private class DisplayPropertyChangeNotifier : IDisposable
    {
        private readonly EngineeringDocument _doc;
        private readonly string _displayDocumentName;
        private readonly string _displayFileName;
        private readonly string _displayFilePath;
        private readonly string _displayWindowTitle;
        private readonly DateTime? _displaySavedDate;

        public DisplayPropertyChangeNotifier(EngineeringDocument doc)
        {
            _doc = doc;
            _displayDocumentName = doc.DisplayDocumentName;
            _displayFileName = doc.DisplayFileName;
            _displayFilePath = doc.DisplayFilePath;
            _displaySavedDate = doc.DisplaySavedDate;
            _displayWindowTitle = doc.DisplayWindowTitle;
        }

        public void Dispose()
        {
            if (_displayDocumentName != _doc.DisplayDocumentName)
            {
                _doc.FirePropertyChanged("DisplayDocumentName");
            }
            if (_displayFileName != _doc.DisplayFileName)
            {
                _doc.FirePropertyChanged("DisplayFileName");
            }
            if (_displayFilePath != _doc.DisplayFilePath)
            {
                _doc.FirePropertyChanged("DisplayFilePath");
            }
            if (_displaySavedDate != _doc.DisplaySavedDate)
            {
                _doc.FirePropertyChanged("DisplaySavedDate");
            }
            if (_displayWindowTitle != _doc.DisplayWindowTitle)
            {
                _doc.FirePropertyChanged("DisplayWindowTitle");
            }
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (propertyName == "DisplayFilePath")
        {
            WorksheetControl.DisplayFilePath = DisplayFilePath;
        }
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public IIORegionManager InputOutputRegionManager => Worksheet.InputOutputRegionManager;

    public string DocumentTabIcon
    {
        get => _customizedValues.TabIcon;
        set
        {
            if (_customizedValues.TabIcon == value)
            {
                return;
            }
            if (value.IsNotEmpty() && !File.Exists(value))
            {
                return;
            }
            _customizedValues.TabIcon = value;
            FirePropertyChanged("DocumentTabIcon");
        }
    }

    public DocumentReadonlyOptions ReadonlyOptions
    {
        get => _readonlyOptions;
        private set
        {
            DocumentReadonlyOptions documentReadonlyOptions = value ?? DocumentReadonlyOptions.Default;
            if (_readonlyOptions == documentReadonlyOptions)
            {
                return;
            }
            _readonlyOptions = documentReadonlyOptions;
            InputOutputRegionManager.GetAndSetOperationsCompareAliasesCaseInsensitively = _readonlyOptions.CaseInsensitiveAliasComparisonEnabled;
            FirePropertyChanged("ReadonlyOptions");
        }
    }

    public string CustomizedClosePrompt
    {
        get => _customizedValues.ClosePrompt;
        set => _customizedValues.ClosePrompt = value;
    }

    public string CustomizedFilePath
    {
        get => _customizedValues.FilePath;
        set
        {
            if (_customizedValues.FilePath == value)
            {
                return;
            }
            using (new DisplayPropertyChangeNotifier(this))
            {
                _customizedValues.FilePath = value;
            }
        }
    }

    public string DisplayFilePath => GetFirstNonEmptyString(CustomizedFilePath, FilePath);

    public string FileName => Path.GetFileName(FilePath);

    public string CustomizedFileName => Path.GetFileName(CustomizedFilePath);

    public string DisplayFileName => GetFirstNonEmptyString(CustomizedFileName, FileName);

    public DateTime? CustomizedSavedDate
    {
        get => _customizedValues.SavedDate;
        set
        {
            if (_customizedValues.SavedDate == value)
            {
                return;
            }
            using (new DisplayPropertyChangeNotifier(this))
            {
                _customizedValues.SavedDate = value;
            }
        }
    }

    public DateTime? DisplaySavedDate
    {
        get
        {
            if (CustomizedFilePath.IsNullOrEmpty())
            {
                return LastSavedDate;
            }
            if (CustomizedSavedDate.HasValue)
            {
                return CustomizedSavedDate;
            }
            try
            {
                if (Path.IsPathRooted(CustomizedFilePath) && File.Exists(CustomizedFilePath))
                {
                    return File.GetLastWriteTime(CustomizedFilePath);
                }
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertion(string.Format("{0} : {1}", ex.Message, CustomizedFilePath));
            }
            return null;
        }
    }

    public string DocumentName
    {
        get => FilePathToNameConverter.Convert(FilePath, _unsavedDocumentName);
        set
        {
            if (_unsavedDocumentName == value)
            {
                return;
            }
            DebugTools.DevAssert(string.IsNullOrEmpty(_unsavedDocumentName) && string.IsNullOrEmpty(FilePath), null);
            using (new DisplayPropertyChangeNotifier(this))
            {
                _unsavedDocumentName = value;
            }
        }
    }

    public string CustomizedDocumentName
    {
        get => _customizedValues.DocumentName;
        set
        {
            if (_customizedValues.DocumentName == value)
            {
                return;
            }
            using (new DisplayPropertyChangeNotifier(this))
            {
                _customizedValues.DocumentName = value;
            }
        }
    }

    public string DisplayDocumentName => GetFirstNonEmptyString(CustomizedDocumentName, FilePathToNameConverter.Convert(DisplayFileName, DocumentName));

    public string CustomizedWindowTitle
    {
        get => _customizedValues.WindowTitle;
        set
        {
            if (_customizedValues.WindowTitle == value)
            {
                return;
            }
            using (new DisplayPropertyChangeNotifier(this))
            {
                _customizedValues.WindowTitle = value;
            }
        }
    }

    public string DisplayWindowTitle => GetFirstNonEmptyString(CustomizedWindowTitle, DisplayFilePath, DisplayDocumentName);

    public void ActivateHeader() => Worksheet.ActivateHeader();

    public void ActivateFooter() => Worksheet.ActivateFooter();

    public void ActivateMain() => Worksheet.ActivateContent();

    public void ActivateWorksheet() => WorksheetControl.Activate();

    public MatrixFormat MathResultMatrixFormat
    {
        get => Worksheet.ResultMatrixFormat;
        set => Worksheet.ResultMatrixFormat = value;
    }

    public NumberFormat MathDisplayNumberFormat
    {
        get => Worksheet.DisplayNumberFormat;
        set => Worksheet.DisplayNumberFormat = value;
    }

    public IRegionFactory RegionFactory
    {
        get => Quire.Controls.Worksheet.RegionFactory.Instance;
        set => DebugTools.FailAssertion("RegionFactory: setter not imp");
    }

    public IDictionary<IdLabels, ExtendedFont> LabeledIdFormat
    {
        get => Worksheet.LabeledIdFormat;
        set => Worksheet.LabeledIdFormat = value;
    }

    public MathFormat MathFormat => Worksheet.MathFormat;

    IWorksheetProperties IEngineeringDocument.Worksheet => Worksheet;

    public bool IsPrinting => _runningPrintJobs.Count > 0;

    private IMetadataModel _metadataModel;
    private IFileOperationsProvider _fileOpsProvider;
    private IPackageOperationsProvider _packageOpsProvider;
    private FileStream _fileStream;
    private bool _isDocumentATemplate;
    private bool _showEngineChangesBanner;

    public string UnitSystemHeader
    {
        get
        {
            switch (DefaultUnitSystem)
            {
                case UnitSystems.Custom:
                    return Resources[UnitSystemHeaderCustom].ToString();
                case UnitSystems.CGS:
                    return UnitSystems.CGS.ToString();
                case UnitSystems.SI:
                    return UnitSystems.SI.ToString();
                case UnitSystems.US:
                    return UnitSystems.US.ToString() + "CS";
                default:
                    return UnitSystems.SI.ToString();
            }
        }
    }

    public IMetadataModel MetadataModel
    {
        get => _metadataModel = _metadataModel ?? new MetadataModel();
        set => _metadataModel = value;
    }

    public IFileOperationsProvider FileOpsProvider
    {
        get => _fileOpsProvider ?? new FileOperationsProvider();
        set => _fileOpsProvider = value;
    }

    public IPackageOperationsProvider PackageOpsProvider
    {
        get => _packageOpsProvider ?? PackageOperationsProvider.Instance;
        set => _packageOpsProvider = value;
    }

    public UnitSystems DefaultUnitSystem
    {
        get => WorksheetControl.DefaultUnitSystem;
        set => WorksheetControl.DefaultUnitSystem = value;
    }

    public UnitSystems CustomBaseUnitSystem
    {
        get => WorksheetControl.CustomBaseUnitSystem;
        set => WorksheetControl.CustomBaseUnitSystem = value;
    }

    public bool SimplifyUnitSystem
    {
        get => WorksheetControl.SimplifyUnitSystem;
        set => WorksheetControl.SimplifyUnitSystem = value;
    }

    public List<UnitDescriptor> CustomBaseUnitDescriptors
    {
        get => WorksheetControl.CustomBaseUnitDescriptors;
        set => WorksheetControl.CustomBaseUnitDescriptors = value;
    }

    public List<UnitDescriptor> CustomDerivedUnitDescriptors
    {
        get => WorksheetControl.CustomDerivedUnitDescriptors;
        set => WorksheetControl.CustomDerivedUnitDescriptors = value;
    }

    public CalculationOptions DocCalculationOptions => Worksheet.DocCalculationOptions;

    public CalculationVariables DocCalculationVariables => Worksheet.DocCalculationVariables;

    public int RegionCount => Regions.Count();

    public IEnumerable<UIElement> Regions => Worksheet.WorksheetItems.Cast<UIElement>();

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        set => SetValue(IsDirtyProperty, value);
    }

    public bool IsDocumentATemplate
    {
        get => _isDocumentATemplate;
        set
        {
            if (_isDocumentATemplate == value)
            {
                return;
            }
            _isDocumentATemplate = value;
            FirePropertyChanged("IsDocumentATemplate");
        }
    }

    public IIOItemCollection Inputs => Worksheet.Inputs;

    public IIOItemCollection Outputs => Worksheet.Outputs;

    public bool IsCalculationsEnabled
    {
        get => Worksheet.IsCalculationsEnabled;
        set => Worksheet.IsCalculationsEnabled = value;
    }

    public bool IsAutoCalcOn
    {
        get => Worksheet.IsAutoCalcOn;
        set => Worksheet.IsAutoCalcOn = value;
    }

    public DateTime? LastSavedDate
    {
        get => (DateTime?)GetValue(LastSavedDateProperty);
        set => SetValue(LastSavedDateProperty, value);
    }

    public bool FileIsSaved
    {
        get => (bool)GetValue(FileIsSavedProperty);
        set => SetValue(FileIsSavedProperty, value);
    }

    public string FileExtension
    {
        get => Path.GetExtension(FilePath);
        set => FilePath = Path.ChangeExtension(FilePath, value);
    }

    public int PageCount => Worksheet.PageCount;

    public int SelectedRegionCount => Worksheet.SelectedItems.Count();

    public bool IsHeaderFocused => Worksheet.IsHeaderActive;

    public bool IsFooterFocused => Worksheet.IsFooterActive;

    public bool IsMainActive => Worksheet.IsContentActive;

    public Point InsertionPoint => Worksheet.InsertionPoint;

    public string CWD
    {
        get => DocCalculationVariables.CurrentWorkingDirectory;
        set
        {
            string objectToCheck;
            try
            {
                objectToCheck = Path.GetFullPath(value);
            }
            catch
            {
                objectToCheck = null;
            }
            if (DebugTools.AssertNotNull(objectToCheck, null).Failed)
            {
                return;
            }
            if (DocCalculationVariables.CurrentWorkingDirectory == value)
            {
                return;
            }
            DocCalculationVariables.CurrentWorkingDirectory = value;
        }
    }

    public bool CanBeSavedWithOriginalName
    {
        get
        {
            if (_fileStream != null)
            {
                return _fileStream.CanWrite;
            }
            return false;
        }
    }

    public bool CacheWorksheet
    {
        get => Worksheet.CacheWorksheet;
        set => Worksheet.CacheWorksheet = value;
    }

    public string HelpServerPath
    {
        get => WorksheetControl.HelpServerPath;
        set => WorksheetControl.HelpServerPath = value;
    }

    public IEnumerable<UIElement> ActiveSectionRegions => Worksheet.ActiveSectionItems.OfType<UIElement>();

    public bool ShowEngineChangesBanner
    {
        get => _showEngineChangesBanner;
        set
        {
            if (_showEngineChangesBanner == value)
            {
                return;
            }
            _showEngineChangesBanner = value;
            FirePropertyChanged("ShowEngineChangesBanner");
        }
    }

    private static void OnLastSavedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EngineeringDocument)
        {
            (d as EngineeringDocument).FirePropertyChanged("DisplaySavedDate");
        }
    }

    private static void OnFilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is EngineeringDocument)
        {
            (d as EngineeringDocument).OnFilePathChanged();
        }
    }

    private void OnFilePathChanged()
    {
        FilePathChanged(this);
        FirePropertyChanged("DisplayFilePath");
        FirePropertyChanged("DisplayFileName");
        FirePropertyChanged("DisplayDocumentName");
        FirePropertyChanged("DisplayWindowTitle");
    }

    private static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EngineeringDocument engineeringDocument)
        {
            engineeringDocument.IsDirtyChanged(engineeringDocument);
        }
    }

    private void FireSystemVariableUpdated(string name, object value)
    {
        if (!name.Equals("mc_CWD"))
        {
            IsDirty = true;
        }
    }

    private void BindingCalculationOptionsAndSystemVariables()
    {
        Worksheet.DocCalculationOptions.CalculationOptionChanged += FireCalculationOptionUpdated;
        IsAutoCalcOn = Worksheet.DocCalculationOptions.IsAutoCalcOn;
        Worksheet.DocCalculationVariables.CalculationVariableChanged += FireSystemVariableUpdated;
        Worksheet.DocCalculationOptions.BinOctHexModeOptionChanged += FireCalculationOptionUpdated;
    }

    private void WorksheetPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "SimplifyUnitSystem":
                IsDirty = true;
                break;
            case "DefaultUnitSystem":
                IsDirty = true;
                FirePropertyChanged("UnitSystemHeader");
                break;
        }
    }

    public void SetAutoCalculationEnabledWithoutTriggeringIsDirty(bool value)
    {
        bool isDirty = IsDirty;
        IsAutoCalcOn = value;
        IsDirty = isDirty;
    }

    public TextFormat GetContentTextFormat() => Worksheet.ContentTextFormat;

    public TextFormat GetHeaderTextFormat() => Worksheet.HeaderTextFormat;

    public TextFormat GetFooterTextFormat() => Worksheet.FooterTextFormat;

    private void InvokeOnDispose()
    {
        try
        {
            Worksheet.Dispose();
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.ToString());
        }
        finally
        {
            OnDispose?.Invoke(this);
        }
    }

    public void ResetDocumentCustomization()
    {
        ReadonlyOptions = null;
        DocumentTabIcon = null;
        CustomizedDocumentName = null;
        CustomizedFilePath = null;
        CustomizedSavedDate = null;
        CustomizedWindowTitle = null;
        CustomizedClosePrompt = null;
        DebugTools.DevAssert(ReadonlyOptions == DocumentReadonlyOptions.Default, null);
        DebugTools.DevAssert(_customizedValues.Equals(default(CustomizedValues)), null);
    }

    private void UpdateCWDState(string fileName)
    {
        string prevCwd = CWD;
        string newCwd = CWD = Path.GetDirectoryName(Path.GetFullPath(fileName));
        if (ReadonlyOptions.FileLocationHistoryEnabled && prevCwd != newCwd && CwdUpdated != null)
        {
            CwdUpdated(newCwd);
        }
    }

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        (d as EngineeringDocument)?.OnZoomChanged((double)args.OldValue, (double)args.NewValue);
    }

    private void OnZoomChanged(double oldValue, double newValue)
    {
        AdjustScrollBeforeZoomChange(oldValue, newValue);
        LayoutTransform = new ScaleTransform(newValue, newValue);
        FirePropertyChanged("Zoom");
    }

    private void AdjustScrollBeforeZoomChange(double zoomOldValue, double zoomNewValue)
    {
        ScrollViewer scrollViewer = this.TryFindAncestorIncludingSelf<ScrollViewer>();
        if (scrollViewer != null)
        {
            double num = scrollViewer.ViewportWidth / 2.0;
            double num2 = scrollViewer.ViewportHeight / 2.0;
            double num3 = scrollViewer.HorizontalOffset + num;
            double num4 = scrollViewer.VerticalOffset + num2;
            double num5 = zoomNewValue / zoomOldValue;
            double value = scrollViewer.ExtentWidth * num5;
            if (scrollViewer.ExtentWidth.IsLessThan(scrollViewer.ViewportWidth) && value.IsGreaterThan(scrollViewer.ViewportWidth))
            {
                Point val = TranslatePoint(default, scrollViewer);
                num3 -= val.X;
            }
            double num6 = num3 * num5;
            double num7 = num4 * num5;
            double offset = num6 - num;
            double offset2 = num7 - num2;
            scrollViewer.ScrollToHorizontalOffset(offset);
            scrollViewer.ScrollToVerticalOffset(offset2);
        }
    }

    internal void HandleCtrlMouseWheel(int ticks)
    {
        EngineeringDocCommands.ZoomIn.Execute(ticks, this);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            int num = Math.Abs(e.Delta) < WpfUtils.MouseWheelScrollDelta ? WpfUtils.MouseScrollDeltaForTouchPad : WpfUtils.MouseWheelScrollDelta;
            int num2 = e.Delta / num;
            if (num2 == 0)
            {
                return;
            }
            HandleCtrlMouseWheel(num2);
            e.Handled = true;
        }
        base.OnMouseWheel(e);
    }

    private WorksheetControl _worksheet;
    private bool _isDoneOpeningFile;

    internal WorksheetControl WorksheetControl
    {
        get => _worksheet;
        set
        {
            if (_worksheet == value)
            {
                return;
            }
            if (_worksheet != null)
            {
                _worksheet.PropertyChanged -= WorksheetPropertyChangedHandler;
            }
            _worksheet = value;
            FirePropertyChanged("Worksheet");
            if (_worksheet != null)
            {
                _worksheet.PropertyChanged += WorksheetPropertyChangedHandler;
                BindingCalculationOptionsAndSystemVariables();
            }
        }
    }

    public XElementSerializable GetMcdxPageModel(XName name) => new McdxPageModel(WorksheetData, name);

    public FileLoadResult NewFromDocument(string fullFilePath)
    {
        string text = Path.GetExtension(fullFilePath) ?? string.Empty;
        _isDoneOpeningFile = false;
        FileLoadResult result = new FileLoadResult();
        try
        {
            _waitForLoad.Reset();
            ResultManager.Instance.Clear();
            if (FileManager.IsPackage(fullFilePath, text))
            {
                OpenPackage(ref result, fullFilePath, true);
                ResultManager.Instance.PushResultsToOwners();
                CommandBuffer.Clear();
                _isNew = true;
                IsDirty = false;
            }
            else if (FileManager.IsWorksheet(fullFilePath))
            {
                result.LoadException = new PreviousVersionWorksheetException();
                result.LoadException.Data.Add("fileName", fullFilePath);
                result.LoadException.Data.Add("extension", text);
            }
            else
            {
                result.LoadException = new FormatException("Incorrect file format, file should be from Prime");
            }
        }
        catch (Exception ex)
        {
            ExceptionOccured("opening file {0} as {1}: {2}", fullFilePath, text, ex);
            result.LoadException = ex;
        }
        finally
        {
            if (_fileStream != null)
            {
                EnsureStreamRelease();
                _fileStream = null;
            }
            _waitForLoad.Set();
        }
        _isDoneOpeningFile = true;
        return result;
    }

    internal MarginLineAdorner GetAnnotationMarginLineOfElement(FrameworkElement element) => GetTopLevelAdornersForElement(element).OfType<MarginLineAdorner>().SingleOrDefault();

    private IEnumerable<VisualAdornerBase> GetTopLevelAdornersForElement(FrameworkElement element)
    {
        if (DebugTools.AssertNotNull(element, null).Failed)
        {
            return new List<VisualAdornerBase>();
        }
        AdornerDecorator errorMessageAdornerDecorator = GetErrorMessageAdornerDecorator();
        if (DebugTools.AssertNotNull(errorMessageAdornerDecorator, null).Failed)
        {
            return new List<VisualAdornerBase>();
        }
        AdornerLayer adornerLayer = errorMessageAdornerDecorator.AdornerLayer;
        if (DebugTools.AssertNotNull(adornerLayer, null).Failed)
        {
            return new List<VisualAdornerBase>();
        }
        Adorner[] adorners = adornerLayer.GetAdorners(element);
        if (adorners == null)
        {
            return new List<VisualAdornerBase>();
        }
        return adorners.OfType<VisualAdornerBase>();
    }

    private AdornerDecorator GetErrorMessageAdornerDecorator() => GetTemplateChild("PART_ErrorMessageAdornerDecorator") as AdornerDecorator;

    private void HandleAdornerAdded(object sender, RoutedEventArgs e)
    {
        VisualAdornerBase visualAdornerBase = e.OriginalSource as VisualAdornerBase;
        if (DebugTools.AssertNotNull(visualAdornerBase, null).Failed)
        {
            return;
        }
        AdornerDecorator errorMessageAdornerDecorator = GetErrorMessageAdornerDecorator();
        if (DebugTools.AssertNotNull(errorMessageAdornerDecorator, null).Failed)
        {
            return;
        }
        VisualAdornerBase.AddToAdornerLayer(errorMessageAdornerDecorator, visualAdornerBase);
    }

    private void HandleCopySingleWhiteboardItemNoImage(object sender, CustomRoutedEventArgs<IWhiteboardItem> e)
    {
        e.Handled = HandleCopyWhiteboardItems(e.Parameter.Yield().ToList(), false);
    }

    private void HandleCopyWhiteboardItems(object sender, CustomRoutedEventArgs<IEnumerable<IWhiteboardItem>> e)
    {
        IList<IWhiteboardItem> list;
        if (DebugTools.DevAssert(e.Parameter.IsNotEmpty(out list), null).Failed)
        {
            return;
        }
        e.Handled = HandleCopyWhiteboardItems(list, true);
    }

    public CopyWidgetBase CreateCopyWidget()
    {
        return new CopyWidget
        {
            TotalPages = PageCount,
            DisplaySavedDate = DisplaySavedDate,
            DisplayFilePath = DisplayFilePath,
            DisplayFileName = DisplayFileName,
            DisplayDocumentName = DisplayDocumentName,
            PageNumber = Worksheet.CurrentPageNumber,
            Width = 0.0,
            Height = 0.0
        };
    }

    private void HandleWorksheetControlChanged(object sender, RoutedEventArgs e)
    {
        if (IsDoneOpeningFile())
        {
            IsDirty = true;
        }
    }

    private bool IsDoneOpeningFile() => IsLoaded && _isDoneOpeningFile;

    private void HandleReplace(object parameter)
    {
        if (parameter is FindReplaceQuery findReplaceQuery)
        {
            DebugTools.DevAssert(!findReplaceQuery.Status.HasValue, null);
            findReplaceQuery.Status = FindReplaceStatus.Empty;
        }
        else
        {
            DebugTools.FailAssertion(parameter?.GetType().Name);
        }
    }

    public void FireWaitForCalculationIdle() => Worksheet.WaitForCalculation();

    public void FireWaitForCalculationIdle(int seconds, DispatcherPriority priority) => Worksheet.WaitForCalculation(seconds, priority);

    public void Recalculate() => WorksheetCommands.RecalculateWorksheet.Execute(null, WorksheetControl);

    public Exception PreSave(IDocumentSaveOptions saveOptions)
    {
        CloseOpenedComboBoxEditingMode();
        Worksheet.PreSave(saveOptions);
        Exception exception;
        if (saveOptions.WaitForCalculationForSaveAsApi && !FireWaitForCalculationForSaveAsApi(out exception))
        {
            return exception;
        }
        return null;
    }

    public string GetEngineVersion() => Worksheet.GetEngineVersion();

    public void WaitForSave(int seconds)
    {
        DebugTools.LogData("EngineeringDocument.WaitForSave: Enter");
        // Storing the negated WaitOne result materializes ldc.i4.0 / ceq / brfalse.s matching Mathcad's IL.
        bool timedOut = !_waitForSaveXps.WaitOne(seconds * 1000, false);
        if (timedOut)
        {
            throw new TimeoutException("The wait for save has timed out");
        }
    }

    private void EnsureStreamRelease()
    {
        FileManager.EnsureStreamRelease(_fileStream);
        _fileStream = null;
    }

    protected void NetworkDriveEnsureStreamRelease(string filePath, ref IOException e)
    {
        DriveInfo[] drives = DriveInfo.GetDrives();
        string text = filePath.Substring(0, 3);
        foreach (DriveInfo driveInfo in drives)
        {
            DirectoryInfo rootDirectory = driveInfo.RootDirectory;
            if (text.Contains(rootDirectory.FullName))
            {
                return;
            }
        }
        EnsureStreamRelease();
        _isNetworkDriveDisconnected = true;
        e = new IOException(e.Message + Environment.NewLine + string.Format((string)_errorWarningMessageResourceDictionary["WorksheetMessages.NetworkDriveDisconnected"], text));
    }

    private void HandleTraverse(object sender, ExecutedRoutedEventArgs e)
    {
        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    public bool Find(string text, LogicalDirection direction, bool startFromCaret, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        if ((options & FindOptions.HighlightAll) != 0)
        {
            throw new NotImplementedException();
        }
        return Worksheet.FindOnce(text, startFromCaret, direction, options, subscriptPositions);
    }

    private static void ThrowNotSupported(string message, params object[] args)
    {
        throw new NotSupportedException(string.Format(message, args));
    }

    private static void ExceptionOccured(string action, params object[] args)
    {
        // Mathcad calls string.Format rather than string.Concat for this interpolated format string.
        TraceWarning(string.Format("Exception occurred while {0}", action), args);
    }

    public FileLoadResult OpenFileAndUpdateCWDState(string fileName)
    {
        string text = Path.GetExtension(fileName) ?? string.Empty;
        _isDoneOpeningFile = false;
        FilePath = fileName;
        FileLoadResult result = new FileLoadResult();
        try
        {
            _waitForLoad.Reset();
            ResultManager.Instance.Clear();
            if (FileManager.IsPackage(fileName, text))
            {
                OpenPackage(ref result, fileName, false);
                OnFileForWorksheetOpened(fileName);
            }
            else if (FileManager.IsWorksheet(fileName))
            {
                result.LoadException = new PreviousVersionWorksheetException();
                result.LoadException.Data.Add("fileName", fileName);
                result.LoadException.Data.Add("extension", text);
            }
            else
            {
                result.LoadException = new FormatException("Incorrect file format, file should be from PTC Mathcad Prime");
            }
        }
        catch (Exception ex)
        {
            ExceptionOccured("opening file {0} as {1}: {2}", fileName, text, ex);
            result.LoadException = ex;
        }
        finally
        {
            _waitForLoad.Set();
        }
        _isDoneOpeningFile = true;
        return result;
    }

    private void OnFileForWorksheetOpened(string fileName)
    {
        ResultManager.Instance.PushResultsToOwners(Worksheet.IsScriptingRunWorksheetOpen & Worksheet.HasScriptingControls);
        ShowEngineChangesBanner = DocCalculationOptions.EngineTypeAutoCorrected && Worksheet.ContainsSymbolicEvaluation;
        FilePath = fileName;
        LastSavedDate = File.GetLastWriteTime(FilePath);
        _isNew = false;
        IsDirty = false;
        CommandBuffer.Clear();
        PostSaveOpenCommonOperations();
        if (Path.IsPathRooted(fileName))
        {
            UpdateCWDState(fileName);
        }
    }

    public Exception SaveFile(string fileName, IDocumentSaveOptions options, out string extensionOut)
    {
        extensionOut = "";
        if (string.IsNullOrEmpty(fileName))
        {
            throw new InvalidOperationException("Logic Error: File name not initialized");
        }
        Exception result = null;
        try
        {
            string text = (Path.GetExtension(fileName) ?? string.Empty).ToLower();
            if (IsDocumentFromPTCPlaces)
            {
                text = ".mcdx";
            }
            extensionOut = text;
            switch (text)
            {
            case ".xps":
                SaveXpsDocument(fileName, true);
                break;
            case ".pdf":
                if (FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SaveEngineeringDocumentAsPdfCommand))
                {
                    SavePdfDocument(fileName);
                }
                else
                {
                    result = new FileFormatBlockedByFreemiumException();
                }
                break;
            case ".rtf":
                if (FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SaveEngineeringDocumentAsRtfCommand))
                {
                    SaveRtfDocument(fileName);
                }
                else
                {
                    result = new FileFormatBlockedByFreemiumException();
                }
                break;
            case ".dita":
                SaveDita(fileName);
                break;
            case ".mcdx":
                result = SavePrimeDocFormat(fileName, options);
                break;
            case ".mctx":
                result = FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SaveEngineeringDocumentAsMCTXCommand) ? SavePrimeDocFormat(fileName, options) : new FileFormatBlockedByFreemiumException();
                break;
            default:
                ThrowNotSupported("Save File: \"{0}\" extension is not supported.", text);
                break;
            }
        }
        catch (Exception ex)
        {
            if (ex is OutOfMemoryException)
            {
                GC.Collect();
            }
            ExceptionOccured("saving file {0}: {1}", fileName, ex);
            result = ex;
        }
        return result;
    }

    private Exception SavePrimeDocFormat(string fileName, IDocumentSaveOptions options)
    {
        Exception ex = SavePackage(fileName, options);
        if (ex == null && !options.IsDetachedCopy)
        {
            LastSavedDate = DateTime.Today;
            PostSaveOpenCommonOperations();
        }
        return ex;
    }

    private void PostSaveOpenCommonOperations()
    {
        MetadataModel.Version++;
        FileIsSaved = true;
        Worksheet.IsSavedWorksheet = true;
    }

    private Package OpenPackageStream(string fullFilePath)
    {
        Package result = null;
        OpenPackageStreamForWriteAndSharedRead(fullFilePath, true);
        if (_fileStream.Length > 0)
        {
            result = Package.Open(_fileStream);
        }
        return result;
    }

    private void OpenPackageStreamForWriteAndSharedRead(string fullFilePath, bool allowReadOnly)
    {
        FileStream fileStream;
        try
        {
            fileStream = File.Open(fullFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }
        catch
        {
            if (allowReadOnly)
            {
                fileStream = PackageOperationsProvider.OpenFileStreamForReadWithFullShare(fullFilePath);
            }
            else
            {
                throw;
            }
        }
        EnsureStreamRelease();
        _fileStream = fileStream;
    }

    private void OpenPackage(ref FileLoadResult result, string fileName, bool newFromDocument)
    {
        try
        {
            using (Package package = OpenPackageStream(fileName))
            {
                if (package != null)
                {
                    Read(package, ref result, fileName, newFromDocument);
                }
            }
        }
        catch (Exception)
        {
            EnsureStreamRelease();
            throw;
        }
    }

    private void DeserializeWorksheetAndHeaderFooter(Package package, string fullFilePath)
    {
        worksheetRegionCollectionSerializer worksheetRegionCollectionSerializer = new worksheetRegionCollectionSerializer();
        DocumentSerializationHelper.MainRegions = DeserializeWorksheetSection(PackageOpsProvider.GetMainPartFromPackage(package), WorksheetData.WorksheetContent, worksheetRegionCollectionSerializer, fullFilePath);
        WorksheetData.WorksheetContent.ConvertedFromMC14 = worksheetRegionCollectionSerializer.ConvertedFromMC14;
        WorksheetData.WorksheetContent.MsgId = worksheetRegionCollectionSerializer.MsgId;
        PackagePart headersContainerPartFromPackage = PackageOpsProvider.GetHeadersContainerPartFromPackage(package);
        if (headersContainerPartFromPackage != null)
        {
            DeserializeHeadersFromContainer(headersContainerPartFromPackage, fullFilePath);
        }
        else
        {
            headerRegionCollectionSerializer headerRegionCollectionSerializer = new headerRegionCollectionSerializer();
            DocumentSerializationHelper.HeaderRegions = DeserializeWorksheetSection(PackageOpsProvider.GetHeaderPartFromPackage(package), WorksheetData.Header, headerRegionCollectionSerializer, fullFilePath);
            WorksheetData.Header.ConvertedFromMC14 = headerRegionCollectionSerializer.ConvertedFromMC14;
        }
        PackagePart footersContainerPartFromPackage = PackageOpsProvider.GetFootersContainerPartFromPackage(package);
        if (footersContainerPartFromPackage != null)
        {
            DeserializeFootersFromContainer(footersContainerPartFromPackage, fullFilePath);
        }
        else
        {
            footerRegionCollectionSerializer footerRegionCollectionSerializer = new footerRegionCollectionSerializer();
            DocumentSerializationHelper.FooterRegions = DeserializeWorksheetSection(PackageOpsProvider.GetFooterPartFromPackage(package), WorksheetData.Footer, footerRegionCollectionSerializer, fullFilePath);
            WorksheetData.Footer.ConvertedFromMC14 = footerRegionCollectionSerializer.ConvertedFromMC14;
        }
    }

    private void DeserializeHeadersFromContainer(PackagePart containerPart, string fullFilePath)
    {
        headersContainerRegionCollectionSerializer headersContainerRegionCollectionSerializer = new headersContainerRegionCollectionSerializer();
        XmlElement xmlElement = LoadAndGetRootElement(containerPart, "//ws:headers");
        if (xmlElement != null)
        {
            headersContainerRegionCollectionSerializer.Deserialize(xmlElement, true);
            headersContainerWrapper containerWrapper = headersContainerRegionCollectionSerializer.ContainerWrapper;
            if (containerWrapper.Header?.Regions != null && containerWrapper.Header.Regions.Any())
            {
                DocumentSerializationHelper.HeaderRegions = DeserializeRegionsFromCollection(containerWrapper.Header, WorksheetData.Header, fullFilePath, containerPart);
                WorksheetData.Header.ConvertedFromMC14 = containerWrapper.HeaderConvertedFromMC14;
            }
            if (containerWrapper.FirstPageHeader?.Regions != null && containerWrapper.FirstPageHeader.Regions.Any())
            {
                DocumentSerializationHelper.FirstPageHeaderRegions = DeserializeRegionsFromCollection(containerWrapper.FirstPageHeader, WorksheetData.FirstPageHeader, fullFilePath, containerPart);
                WorksheetData.FirstPageHeader.ConvertedFromMC14 = containerWrapper.FirstPageHeaderConvertedFromMC14;
            }
        }
    }

    private void DeserializeFootersFromContainer(PackagePart containerPart, string fullFilePath)
    {
        footersContainerRegionCollectionSerializer footersContainerRegionCollectionSerializer = new footersContainerRegionCollectionSerializer();
        XmlElement xmlElement = LoadAndGetRootElement(containerPart, "//ws:footers");
        if (xmlElement != null)
        {
            footersContainerRegionCollectionSerializer.Deserialize(xmlElement, true);
            footersContainerWrapper containerWrapper = footersContainerRegionCollectionSerializer.ContainerWrapper;
            if (containerWrapper.Footer?.Regions != null && containerWrapper.Footer.Regions.Any())
            {
                DocumentSerializationHelper.FooterRegions = DeserializeRegionsFromCollection(containerWrapper.Footer, WorksheetData.Footer, fullFilePath, containerPart);
                WorksheetData.Footer.ConvertedFromMC14 = containerWrapper.FooterConvertedFromMC14;
            }
            if (containerWrapper.FirstPageFooter?.Regions != null && containerWrapper.FirstPageFooter.Regions.Any())
            {
                DocumentSerializationHelper.FirstPageFooterRegions = DeserializeRegionsFromCollection(containerWrapper.FirstPageFooter, WorksheetData.FirstPageFooter, fullFilePath, containerPart);
                WorksheetData.FirstPageFooter.ConvertedFromMC14 = containerWrapper.FirstPageFooterConvertedFromMC14;
            }
        }
    }

    private XmlElement LoadAndGetRootElement(PackagePart containerPart, string xpath)
    {
        using (Stream inStream = containerPart.GetStream(FileMode.Open))
        {
            XmlDocument xmlDocument = new XmlDocument();
            SpiritResolver.InitializeDocument(xmlDocument, SchemasManager.Worksheet50);
            xmlDocument.Load(inStream);
            xmlDocument.Validate(null);
            XmlNamespaceManager mcdxNameSpaceManager = XmlWorksheetData.McdxNameSpaceManager;
            return xmlDocument.SelectSingleNode(xpath, mcdxNameSpaceManager) as XmlElement;
        }
    }

    private IList<IRegionPersistentData> DeserializeWorksheetSection(PackagePart part, IWorksheetSectionPersistentData sectionData, IRegionCollectionSerializer regionCollectionSerializer, string fullFilePath)
    {
        IList<IRegionPersistentData> result;
        if (part == null)
        {
            result = null;
        }
        else
        {
            using (Stream stream = part.GetStream(FileMode.Open))
            using (McdxDeserializer mcdxDeserializer = new McdxDeserializer(fullFilePath, part, new WorksheetSectionDeserializationStrategy(sectionData, MathFormat, LabeledIdFormat), DocumentSerializationHelper, regionCollectionSerializer, true, _textStylesManager.TextStyleChangedHandler))
            {
                mcdxDeserializer.Deserialize(stream);
                result = mcdxDeserializer.DeserializedRegions;
            }
        }
        return result;
    }

    protected Exception SavePackage(string newFileName, IDocumentSaveOptions options)
    {
        Exception result = null;
        string text = null;
        string fullPath = Path.GetFullPath(newFileName);
        try
        {
            text = Path.GetTempFileName();
            using (FileStream fileStream = File.Open(text, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                using (Package package = PackageOpsProvider.GetPackageForWrite(fileStream))
                {
                    ResultManager.Instance.Clear();
                    DocumentSerializationHelper.InitializeForWrite("MathcadPrime");
                    DocumentSerializationHelper.EngineVersion = GetEngineVersion();
                    SerializeWorksheet(package);
                    package.Flush();
                    using (Stream stream = PackageOpsProvider.AddCoreMetadataPartToPackage(package).GetStream(FileMode.Create))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        new SpiritMetadataSerializer().Serialize(MetadataModel, writer);
                    }
                    PackageSerializable.GetApplicationPropertiesSerializer(PackageOpsProvider, DocumentSerializationHelper).Write(package);
                    PackageSerializable.GetPresentationSettingsSerializable(this).Write(package);
                    PackageSerializable.GetCalculationSettingsSerializable(this).Write(package);
                    new ResultDataSerializer(PackageOpsProvider).Write(package);
                    new IntegrationDataSerializer(this, Worksheet).Write(package);
                    DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.WorksheetContent.RegionsToSerialize.Keys.ToList());
                    DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.Header.RegionsToSerialize.Keys.ToList());
                    DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.Footer.RegionsToSerialize.Keys.ToList());
                    DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.FirstPageHeader.RegionsToSerialize.Keys.ToList());
                    DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.FirstPageFooter.RegionsToSerialize.Keys.ToList());
                }
                try
                {
                    if (!options.IsDetachedCopy)
                    {
                        DebugTools.DevAssert((_fileStream == null && (FilePath == null || _isNetworkDriveDisconnected)) || (_fileStream != null && _fileStream.Name == FilePath), null);
                        if (_fileStream == null || !_fileStream.CanWrite || !string.Equals(_fileStream.Name, fullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            OpenPackageStreamForWriteAndSharedRead(fullPath, false);
                        }
                        _fileStream.SetLength(0L);
                        fileStream.Position = 0L;
                        fileStream.CopyTo(_fileStream);
                        _fileStream.Position = 0L;
                        try
                        {
                            File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite).Dispose();
                        }
                        catch (Exception ex)
                        {
                            DebugTools.FailAssertion(ex);
                        }
                        FilePath = fullPath;
                        _isNew = false;
                        IsDirty = false;
                        _isNetworkDriveDisconnected = false;
                    }
                }
                catch (IOException innerException)
                {
                    NetworkDriveEnsureStreamRelease(FilePath, ref innerException);
                    throw new FileIsUsedByAnotherProcessException(innerException);
                }
            }
            try
            {
                if (options.IsDetachedCopy)
                {
                    FileAttributes? fileAttributes;
                    try
                    {
                        fileAttributes = File.GetAttributes(fullPath);
                    }
                    catch (Exception)
                    {
                        fileAttributes = null;
                    }
                    if (fileAttributes.HasValue && fileAttributes.Value.HasFlag(FileAttributes.Hidden))
                    {
                        File.SetAttributes(fullPath, fileAttributes.Value & ~FileAttributes.Hidden);
                    }
                    File.Copy(text, fullPath, true);
                    try
                    {
                        if (fileAttributes.HasValue)
                        {
                            File.SetAttributes(fullPath, fileAttributes.Value);
                        }
                    }
                    catch (Exception ex3)
                    {
                        DebugTools.FailAssertion(ex3);
                    }
                }
            }
            catch (IOException innerException2)
            {
                NetworkDriveEnsureStreamRelease(FilePath, ref innerException2);
                throw new FileIsUsedByAnotherProcessException(innerException2);
            }
        }
        catch (Exception ex4)
        {
            if (ex4 is OutOfMemoryException)
            {
                GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            ExceptionOccured("saving package {0}", ex4);
            result = ex4;
        }
        finally
        {
            try
            {
                if (text != null)
                {
                    File.Delete(text);
                }
            }
            catch (Exception ex5)
            {
                DebugTools.FailAssertion(ex5);
            }
            WorksheetData = null;
            DocumentSerializationHelper.Reset();
        }
        return result;
    }

    protected void SaveDita(string fileName)
    {
        using (DitaWriter ditaWriter = new DitaWriter(fileName, DocumentSerializationHelper))
        {
            foreach (UIElement region in Regions)
            {
                if (region is ContentControl)
                {
                    ditaWriter.WriteContentControl(region as ContentControl);
                }
                else if (region is RichTextBox)
                {
                    ditaWriter.WriteRichTextBox(region as RichTextBox);
                }
                else if (region is ImageControl)
                {
                    ditaWriter.WriteImageControl(region as ImageControl);
                }
            }
            using (new DitaWriter.Worksheet(ditaWriter))
            {
                foreach (UIElement region2 in Regions)
                {
                    if (region2 is ImageControl)
                    {
                        ditaWriter.WriteImageControlML(region2 as ImageControl);
                    }
                    if (region2 is IDitaRegion)
                    {
                        ditaWriter.WriteRegionML(region2);
                    }
                }
            }
            ditaWriter.Flush();
            ditaWriter.Close();
        }
    }

    private void OnPrintingStarted(object sender, WritingProgressChangedEventArgs args)
    {
        _runningPrintJobs.Add((PrintJob)sender);
    }

    private void OnPrintingCompleted(object sender, WritingCompletedEventArgs args)
    {
        PrintJob printJob = (PrintJob)sender;
        _runningPrintJobs.Remove(printJob);
        printJob.JobStarted -= OnPrintingStarted;
        printJob.JobCompleted -= OnPrintingCompleted;
        DisposePrintableDocument(printJob.PrintableDocument);
    }

    private void OnSaveToXpsStarted(object sender, WritingProgressChangedEventArgs args)
    {
        DebugTools.LogData("EngineeringDocument.OnSaveToXpsStarted: Enter");
        _runningPrintJobs.Add((PrintJob)sender);
        _waitForSaveXps.Reset();
    }

    private void OnSaveToXpsCompleted(object sender, WritingCompletedEventArgs args)
    {
        DebugTools.LogData("EngineeringDocument.OnSaveToXpsCompleted: Enter");
        _waitForSaveXps.Set();
        PrintJob printJob = (PrintJob)sender;
        _runningPrintJobs.Remove(printJob);
        printJob.JobStarted -= OnSaveToXpsStarted;
        printJob.JobCompleted -= OnSaveToXpsCompleted;
        DisposePrintableDocument(printJob.PrintableDocument);
    }

    private static void DisposePrintableDocument(IDocumentPaginatorSource printableDoc)
    {
        if (printableDoc is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public void Print(Func<bool> askCancelPrintInProgress)
    {
        if (IsPrinting && !askCancelPrintInProgress())
        {
            return;
        }
        try
        {
            Quire.Wpf.Printing.PrintDialog printDialog = new Quire.Wpf.Printing.PrintDialog(FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.SelectPrintRange) ? (SelectedRegionCount > 0 ? Quire.Wpf.Printing.PrintDialog.RangeSelectionOptions.AllEnabled : Quire.Wpf.Printing.PrintDialog.RangeSelectionOptions.AllButSelection) : Quire.Wpf.Printing.PrintDialog.RangeSelectionOptions.NoneEnabled)
            {
                MaxPage = (uint)PageCount,
                MinPage = 1u
            };
            SetPrintDialogPageLayoutOptionsFromDocument(Worksheet, printDialog);
            if (printDialog.ShowDialog() == true)
            {
                PrintDocument(printDialog, true);
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }

    private bool FireWaitForCalculationForSaveAsApi(out Exception exception)
    {
        if (!DocCalculationOptions.IsAutoCalcOn)
        {
            exception = null;
            return true;
        }
        int defaultCalculationTimeout = InputOutputRegionManager.DefaultCalculationTimeout;
        try
        {
            if (defaultCalculationTimeout != 0)
            {
                FireWaitForCalculationIdle(defaultCalculationTimeout, CalcResultsProcessingPriorities.DefaultResultDispatchPriority_Background);
            }
            exception = null;
        }
        catch (CalculationDisabledException)
        {
            exception = null;
        }
        catch (Exception ex2)
        {
            exception = ex2;
            DebugTools.DevAssert(ex2 is TimeoutException, null);
            return false;
        }
        return true;
    }

    public bool SaveAsPdf_ForApiOnly(string filename)
    {
        if (IsPrinting)
        {
            return false;
        }
        Exception exception;
        if (!FireWaitForCalculationForSaveAsApi(out exception))
        {
            return false;
        }
        try
        {
            SavePdfDocument(filename);
        }
        catch
        {
            return false;
        }
        return true;
    }

    private void PrintDocument(IPrintDialogOptions printOptions, bool async)
    {
        IDocumentPaginatorSource documentPaginatorSource = CreatePrintableDocument(printOptions.PrintRange, printOptions.PageRange);
        if (documentPaginatorSource == null)
        {
            return;
        }
        PrintJob printJob = new PrintJob
        {
            PrintableDocument = documentPaginatorSource
        };
        printJob.JobStarted += OnPrintingStarted;
        printJob.JobCompleted += OnPrintingCompleted;
        if (printOptions.PrintToFile)
        {
            printJob.PrintToFile(printOptions.FileName, async);
            return;
        }
        printOptions.PrintQueue.CurrentJobSettings.Description = FilePathToNameConverter.Convert(DisplayFilePath, DocumentName);
        printJob.Print(printOptions.PrintQueue, printOptions.PrintTicket, async);
    }

    private void SaveXpsDocument(string filePath, bool shouldEnrichPrintingTree = false)
    {
        DebugTools.LogData("EngineeringDocument.SaveXpsDocument: Enter");
        IDocumentPaginatorSource documentPaginatorSource = CreatePrintableDocument(Win32PrintRange.AllPages, default, shouldEnrichPrintingTree);
        if (documentPaginatorSource != null)
        {
            PrintJob printJob = new PrintJob();
            printJob.JobStarted += OnSaveToXpsStarted;
            printJob.JobCompleted += OnSaveToXpsCompleted;
            printJob.PrintableDocument = documentPaginatorSource;
            printJob.PrintToFile(filePath, false);
        }
    }

    private void SaveRtfDocument(string filePath)
    {
        RtfItemWrapperHelper helper = new RtfItemWrapperHelper(this, Worksheet, true);
        new SaveAsRtfWorksheetItemsVisitor(filePath, helper).VisitAll(Worksheet.WorksheetItems);
    }

    private static void SetPrintDialogPageLayoutOptionsFromDocument(IWorksheetControl worksheet, Quire.Wpf.Printing.PrintDialog printDlg)
    {
        printDlg.PrintTicket.PageOrientation = GetSystemPrintingPageOrientationEquvalent(worksheet.PageLayout.PageOrientation);
        printDlg.PrintTicket.PageMediaSize = new PageMediaSize(GetSystemPrintingPageMediaSizeNameEquvalent(worksheet.PageLayout.PageSize));
    }

    private static System.Printing.PageOrientation GetSystemPrintingPageOrientationEquvalent(Quire.Controls.Core.PageOrientation pageOrientation)
    {
        if (!_pageOrientationTranslation.ContainsKey(pageOrientation))
        {
            return System.Printing.PageOrientation.Portrait;
        }
        return _pageOrientationTranslation[pageOrientation];
    }

    private static PageMediaSizeName GetSystemPrintingPageMediaSizeNameEquvalent(PageSize pageSize)
    {
        if (!_pageSizeTranslation.ContainsKey(pageSize))
        {
            return PageMediaSizeName.NorthAmericaLetter;
        }
        return _pageSizeTranslation[pageSize];
    }

    partial class RootDecorator
    {
        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public RootDecorator() { }

        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public Visual Visual => this;
    }
}
