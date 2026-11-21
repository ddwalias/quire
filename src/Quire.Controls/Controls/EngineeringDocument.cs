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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Bookmarks;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.IO;
using Quire.Controls.InputControls;
using Quire.Controls.Plots;
using Quire.Controls.Properties;
using Quire.Controls.Worksheet;
using Quire.Controls.Worksheet.Printing;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.FunctionalitiesLimitation;
using Quire.IO;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.Undo;
using Quire.Wpf;
using Quire.Wpf.Printing;
using Quire.Xml;

namespace Quire.Controls;

// Members are declared in Mathcad order to preserve closure ordinals (36, 187, 188, 196, 199, 203, 224, 247, 252, 255..257, 269, 277) and <>O slot order.
public partial class EngineeringDocument
{
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

    private static readonly Type _typeOfThis = typeof(EngineeringDocument);

    private const string UnitSystemHeaderCustom = "UnitSystemHeader.Custom";

    private readonly ResourceDictionary _errorWarningMessageResourceDictionary;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private readonly ITextStylesManager _textStylesManager;

    public static readonly DependencyProperty LastSavedDateProperty = DependencyProperty.Register("LastSavedDate", typeof(DateTime?), _typeOfThis, new PropertyMetadata(null, OnLastSavedDateChanged));

    private static void OnLastSavedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EngineeringDocument)
        {
            (d as EngineeringDocument).FirePropertyChanged("DisplaySavedDate");
        }
    }

    public static readonly DependencyProperty FileIsSavedProperty = DependencyProperty.Register("FileIsSaved", typeof(bool), _typeOfThis, new PropertyMetadata(false));

    static EngineeringDocument()
    {
        DocumentAttachedProperties.ZoomProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnZoomChanged));
        DocumentAttachedProperties.FilePathProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(DocumentAttachedProperties.FilePathProperty.DefaultMetadata.DefaultValue, OnFilePathChanged));
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

    public XElementSerializable GetMcdxPageModel(XName name) => new McdxPageModel(WorksheetData, name);

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnPrepareSnapshotStart;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnPrepareSnapshotEnd;

    public static string GetFirstNonEmptyString(params string[] values)
    {
        return values.FirstOrDefault((string value) => value.IsNotEmpty());
    }

    private IMetadataModel _metadataModel;

    private IFileOperationsProvider _fileOpsProvider;

    private IPackageOperationsProvider _packageOpsProvider;

    private readonly ManualResetEvent _waitForSaveXps = new ManualResetEvent(true);

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private readonly ManualResetEvent _waitForLoad = new ManualResetEvent(true);

    private readonly List<PrintJob> _runningPrintJobs = new List<PrintJob>();

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private static bool _isDocumentFromPtcPlaces = false;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument> IsDirtyChanged = delegate { };

    private WorksheetControl _worksheet;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public IWorksheetControl Worksheet => WorksheetControl;

    private readonly FrameworkElementFacade _frameworkElementFacade;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool NeedsDictionaryRefresh { get; set; }

    IWorksheetProperties IEngineeringDocument.Worksheet => Worksheet;

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

    public bool IsPrinting => _runningPrintJobs.Count > 0;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public double Zoom { get => DocumentAttachedProperties.GetZoom(this); set => DocumentAttachedProperties.SetZoom<EngineeringDocument>(this, value); }

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

    internal void HandleCtrlMouseWheel(int ticks)
    {
        EngineeringDocCommands.ZoomIn.Execute(ticks, this);
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

    private DocumentReadonlyOptions _readonlyOptions;

    private CustomizedValues _customizedValues;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public string FilePath { get => DocumentAttachedProperties.GetFilePath(this); set => DocumentAttachedProperties.SetFilePath(this, value); }

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public long MaxUpdateLayoutTimeMsec { get; set; } = 10000L;

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

    private string _unsavedDocumentName;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public IWorksheetPersistentData WorksheetData { get; private set; }

    public TextFormat GetContentTextFormat() => Worksheet.ContentTextFormat;

    public TextFormat GetHeaderTextFormat() => Worksheet.HeaderTextFormat;

    public TextFormat GetFooterTextFormat() => Worksheet.FooterTextFormat;

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

    public static readonly DependencyProperty IsDirtyProperty = DependencyProperty.Register("IsDirty", typeof(bool), typeof(EngineeringDocument), new PropertyMetadata(OnIsDirtyChanged));

    public bool IsDirty
    {
        get => (bool)GetValue(IsDirtyProperty);
        set => SetValue(IsDirtyProperty, value);
    }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsNetworkDriveDisconnected => _isNetworkDriveDisconnected;

    private static void OnIsDirtyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EngineeringDocument engineeringDocument)
        {
            engineeringDocument.IsDirtyChanged(engineeringDocument);
        }
    }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public ISerializationHelper DocumentSerializationHelper { get; }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsDocumentFromPTCPlaces { get => _isDocumentFromPtcPlaces; set => _isDocumentFromPtcPlaces = value; }

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

    private bool _isDocumentATemplate;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public ITextStylesManager TextStylesManager => _textStylesManager;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<EngineeringDocument> OnDispose;

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

    public EngineeringDocument(DocumentReadonlyOptions readonlyOptions = null)
    {
        CommandBuffer.CreateBufferForKey(this);
        IsDirty = false;
        _isDoneOpeningFile = true;
        VerticalAlignment = VerticalAlignment.Center;
        HorizontalAlignment = HorizontalAlignment.Center;
        Padding = new Thickness(0.0, 24.0, 0.0, 24.0);
        DocumentSerializationHelper = new SerializationHelper();
        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
        _textStylesManager = new TextStylesCollectionManager();
        ResourceDictionary resources = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Quire.Controls;component/EngineeringDocumentResources.xaml")
        };
        Resources = resources;
        _errorWarningMessageResourceDictionary = (ResourceDictionary)Resources["WorksheetMessages"];
        WorksheetControl worksheetControl = new WorksheetControl(_textStylesManager)
        {
            InputOutputRegionManager = new IOManager(this),
            BookmarksManager = new BookmarksManager()
        };
        worksheetControl.SetBinding(WorksheetControl.WorksheetNameProperty, new MultiBinding
        {
            Converter = FileNameConverter.Instance,
            Bindings =
            {
                new Binding
                {
                    Path = new PropertyPath("DisplayFileName"),
                    Mode = BindingMode.OneWay,
                    Source = this
                },
                new Binding
                {
                    Path = new PropertyPath("DisplayDocumentName"),
                    Mode = BindingMode.OneWay,
                    Source = this
                }
            }
        });
        WorksheetControl = worksheetControl;
        Content = Worksheet;
        ReadonlyOptions = readonlyOptions;
        HookupCommandBindings();
        AddHandler(CommonRoutedEvents.CopyWhiteboardItemsEvent, new CustomRoutedEventHandler<IEnumerable<IWhiteboardItem>>(HandleCopyWhiteboardItems), true);
        AddHandler(CommonRoutedEvents.CopySingleItemNoImageEvent, new CustomRoutedEventHandler<IWhiteboardItem>(HandleCopySingleWhiteboardItemNoImage), true);
        AddHandler(VisualAdornerBase.AdornerAddedEvent, new RoutedEventHandler(HandleAdornerAdded));
        _frameworkElementFacade = new FrameworkElementFacade();
        _isNew = true;
        WorksheetControl.Changed += HandleWorksheetControlChanged;
    }

    internal List<NodeMessageAdorner> GetVisibleNodeMessageAdornersForElement(FrameworkElement element)
    {
        return (from adorner in GetTopLevelAdornersForElement(element).OfType<NodeMessageAdorner>()
                where adorner.IsVisible
                select adorner).OrderBy((NodeMessageAdorner adorner) => adorner.RelativeLocation.Y).ToList();
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

    private bool HandleCopyWhiteboardItems(ICollection<IWhiteboardItem> whiteboardItems, bool copyAsImage)
    {
        bool flag = false;
        CompoundCommand compoundCommand = new CompoundCommand();
        using (new CommandScope(compoundCommand))
        {
            try
            {
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    IContainerItemManager itemManager = WhiteboardItemContainerExtensions.ItemManager;
                    DataObject dataObject = new DataObject();
                    try
                    {
                        whiteboardItems.Each((IWhiteboardItem item) =>
                        {
                            itemManager.SetItemIsInCopy(item, true);
                        });
                        flag = WpfClipboard.CopyPrimeXamlToDataObject(dataObject, new WhiteboardItemsCollection(whiteboardItems), whiteboardItems.Count);
                    }
                    finally
                    {
                        whiteboardItems.Each(itemManager.ClearItemIsInCopy);
                    }
                    if (flag)
                    {
                        RtfItemWrapperHelper helper = new RtfItemWrapperHelper(this, Worksheet, Worksheet.IsWorksheetContentScoped);
                        IWorksheetItemCopyVisitor worksheetItemCopyVisitor;
                        if (whiteboardItems.Count == 1)
                        {
                            worksheetItemCopyVisitor = new SingleWorksheetItemCopyVisitor(dataObject, helper, copyAsImage);
                        }
                        else
                        {
                            worksheetItemCopyVisitor = new MultiWorksheetItemCopyVisitor(dataObject, helper, copyAsImage);
                        }
                        worksheetItemCopyVisitor.VisitAll(whiteboardItems);
                    }
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            }
            catch (DevAssertionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertionDoNotThrow(ex);
            }
            DebugTools.DevAssert(compoundCommand.Count == 0, null);
        }
        return flag;
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

    partial class RootDecorator
    {
        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public RootDecorator() { }

        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public Visual Visual => this;
    }

    public FrameworkElement PrepareRegionCloneForImage(IWhiteboardItem item, RegionImageSettings imageSettings, CopyWidgetBase popup, ResourceDictionary resourceDictionary = null, Rect? boundsConstraint = null)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            FrameworkElement frameworkElement = item as FrameworkElement;
            FrameworkElement frameworkElement2;
            Dictionary<IdLabels, ExtendedFont> dictionary = LabeledIdFormat.ToDictionary((KeyValuePair<IdLabels, ExtendedFont> extendedFont) => extendedFont.Key, (KeyValuePair<IdLabels, ExtendedFont> extendedFont) => extendedFont.Value.Clone());
            try
            {
                DocumentAttachedProperties.SetCopyAsImageSettings(frameworkElement, imageSettings);
                using (MathFormatForPaste.Generate(new StaticMathFormat(MathFormat), dictionary))
                {
                    ISupportPrintViewport supportPrintViewport = frameworkElement as ISupportPrintViewport;
                    if (supportPrintViewport != null)
                    {
                        supportPrintViewport.PrintViewport = new Rect(new Point(0.0, 0.0), _frameworkElementFacade.GetSize(frameworkElement));
                    }
                    frameworkElement2 = AttachedProperties.GetPrintView(frameworkElement) as FrameworkElement;
                    if (supportPrintViewport != null)
                    {
                        supportPrintViewport.PrintViewport = null;
                    }
                }
            }
            finally
            {
                DocumentAttachedProperties.ClearCopyAsImageSettings(frameworkElement);
            }
            if (DebugTools.AssertNotNullDoNotThrow(frameworkElement2, null).Failed)
            {
                return null;
            }
            AttachedProperties.SetIsOnVisiblePage(frameworkElement2, true);
            MC14ConversionAnnotation.ClearConversionAnnotations(frameworkElement2);
            WorksheetPropertiesManager.SetOrRemoveWorksheetPropertiesSource(frameworkElement2, new CachedWorksheetPropertiesSource(Worksheet), true);
            if (imageSettings.ApplyTransparency && AttachedProperties.GetPlotBackgroundType(frameworkElement2) != BackgroundTypeE.Gray)
            {
                AttachedProperties.SetPlotBackgroundType(frameworkElement2, BackgroundTypeE.Transparent);
            }
            RegionCloneForImageHelper regionCloneForImageHelper = new RegionCloneForImageHelper(this, frameworkElement, imageSettings, boundsConstraint);
            Border border = new Border
            {
                Background = imageSettings.ApplyTransparency ? Brushes.Transparent : Brushes.White,
                Child = frameworkElement2
            };
            border.SetValue(EquationControl.ResultMatrixFormatProperty, MathResultMatrixFormat.Clone());
            border.SetValue(EquationControl.DisplayNumberFormatProperty, MathDisplayNumberFormat.Clone());
            border.SetValue(AttachedProperties.LabeledIdFormatProperty, dictionary);
            border.SetValue(AttachedProperties.WarningDisplayFilterProperty, AttachedProperties.GetWarningDisplayFilter(this));
            Worksheet.MathFormat.CopyTo(new MathFormat(border));
            if (resourceDictionary != null)
            {
                border.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            DocumentAttachedProperties.SetShowInputOutputDesignation(border, DocumentAttachedProperties.GetShowInputOutputDesignation(WorksheetControl));
            Rect descendantBounds = VisualTreeHelper.GetDescendantBounds(frameworkElement);
            RootDecorator rootDecorator = new RootDecorator
            {
                Child = border
            };
            popup.ChildBorder.Child = rootDecorator;
            regionCloneForImageHelper.ApplyLayoutSettings(descendantBounds, frameworkElement2, border, popup);
            WpfUtils.CreateVisualTreeForPrinting(popup, MaxUpdateLayoutTimeMsec);
            Rect descendantBounds2 = VisualTreeHelper.GetDescendantBounds(frameworkElement2);
            if (!descendantBounds.IsEmpty && !descendantBounds2.IsEmpty && (descendantBounds2.TopLeft.X.IsLessThan(descendantBounds.TopLeft.X, 1E-05) || descendantBounds2.TopLeft.Y.IsLessThan(descendantBounds.TopLeft.Y, 1E-05) || descendantBounds2.BottomRight.X.IsGreaterThan(descendantBounds.BottomRight.X, 1E-05) || descendantBounds2.BottomRight.Y.IsGreaterThan(descendantBounds.BottomRight.Y, 1E-05)))
            {
                regionCloneForImageHelper.ApplyLayoutSettings(descendantBounds2, frameworkElement2, border, popup);
                popup.UpdateLayout();
                if (frameworkElement is IPlotControl)
                {
                    DebugTools.FailAssertionDoNotThrow("origBounds: {0}, cloneBounds: {1}", descendantBounds, descendantBounds2);
                }
                else if (!(frameworkElement is EquationControl equationControl) || !(equationControl.Content is IVisualMathRegionNode content) || (content.LhsOfDefinitionVisibility != Visibility.Collapsed && content.LhsOfDefinitionVisibility != Visibility.Visible))
                {
                    DebugTools.FailAssertion("origBounds: {0}, cloneBounds: {1}", descendantBounds, descendantBounds2);
                }
            }
            DebugTools.DevAssert(popup.ChildBorder.Width.IsEqualTo(rootDecorator.ActualWidth, 1E-05) && popup.ChildBorder.Height.IsEqualTo(rootDecorator.ActualHeight, 1E-05), null);
            return frameworkElement2;
        }
    }

    private void HandleWorksheetControlChanged(object sender, RoutedEventArgs e)
    {
        if (IsDoneOpeningFile())
        {
            IsDirty = true;
        }
    }

    private bool _isDoneOpeningFile;

    private bool IsDoneOpeningFile() => IsLoaded && _isDoneOpeningFile;

    private void HookupCommandBindings()
    {
        WpfUtils.AddPreemptiveCommandBinding(this, ApplicationCommands.Undo, UndoRedoHelper.HandleUndoCommand, UndoRedoHelper.CanExecuteUndoCommandPreview, true);
        WpfUtils.AddPreemptiveCommandBinding(this, ApplicationCommands.Redo, UndoRedoHelper.HandleRedoCommand, UndoRedoHelper.CanExecuteRedoCommandPreview, true);
        WpfUtils.AddPreemptiveCommandBinding(this, RegionCommands.Replace, (object sender, ExecutedRoutedEventArgs args) =>
        {
            HandleReplace(args.Parameter);
        }, (object sender, CanExecuteRoutedEventArgs args) =>
        {
            HandleReplace(args.Parameter);
        }, false);
        CommandBindings.Add(new CommandBinding(EngineeringDocCommands.Traverse, HandleTraverse));
    }

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public void CloseOpenedComboBoxEditingMode() => WpfUtils.ExecuteCommandIfFocusedElementIsDescendantOf<EngineeringDocument>(this, EditableComboBoxCommands.CloseEditingMode);

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public void WaitForLoad(int seconds) => FileManager.WaitFor(seconds, _waitForLoad);

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private void FireCalculationOptionUpdated(string name, object value) => IsDirty = true;

    private void FireSystemVariableUpdated(string name, object value)
    {
        if (!name.Equals("mc_CWD"))
        {
            IsDirty = true;
        }
    }

    public void SetAutoCalculationEnabledWithoutTriggeringIsDirty(bool value)
    {
        bool isDirty = IsDirty;
        IsAutoCalcOn = value;
        IsDirty = isDirty;
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

    public void Dispose()
    {
        EnsureStreamRelease();
        _runningPrintJobs.Each((PrintJob job) =>
        {
            job.CancelAsyncJob();
        });
        Worksheet.DocCalculationOptions.CalculationOptionChanged -= FireCalculationOptionUpdated;
        Worksheet.DocCalculationVariables.CalculationVariableChanged -= FireSystemVariableUpdated;
        Worksheet.DocCalculationOptions.BinOctHexModeOptionChanged -= FireCalculationOptionUpdated;
        WorksheetControl.Changed -= HandleWorksheetControlChanged;
        Worksheet.InputOutputRegionManager.Dispose();
        try
        {
            CommandBuffer.RemoveBufferForKey(this);
        }
        finally
        {
            InvokeOnDispose();
        }
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

    private static void ThrowNotSupported(string message, params object[] args)
    {
        throw new NotSupportedException(string.Format(message, args));
    }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private static void TraceWarning(string format, params object[] args) => Trace.TraceWarning(format, args);

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

    public string FileExtension
    {
        get => Path.GetExtension(FilePath);
        set => FilePath = Path.ChangeExtension(FilePath, value);
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

    private void Read(Package package, ref FileLoadResult result, string fullFilePath, bool newFromDocument)
    {
        try
        {
            WorksheetData = new WorksheetPersistentData();
            PackageSerializable.GetApplicationPropertiesSerializer(PackageOpsProvider, DocumentSerializationHelper).Read(package);
            PackageSerializable.GetPresentationSettingsSerializable(this).Read(package);
            PackageSerializable.GetCalculationSettingsSerializable(this).Read(package);
            DeserializeWorksheetAndHeaderFooter(package, fullFilePath);
            Worksheet.ApplyWorksheetData(WorksheetData);
            TextStylesManager.LoadWorkSheetStyles(WorksheetData.TextStyles);
            if (!newFromDocument)
            {
                using (Stream stream = PackageOpsProvider.GetCoreMetadataPartFromPackage(package).GetStream(FileMode.Open))
                using (StreamReader reader = new StreamReader(stream))
                {
                    new SpiritMetadataSerializer().Deserialize(reader, MetadataModel);
                }
            }
            new ResultDataSerializer(PackageOpsProvider).Read(package);
            new IntegrationDataSerializer(this, Worksheet).Read(package);
            DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.WorksheetContent.SerializedRegions.Collect((IRegionPersistentData regionPersistentData) => regionPersistentData.Control).ToList());
            DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.Header.SerializedRegions.Collect((IRegionPersistentData regionPersistentData) => regionPersistentData.Control).ToList());
            DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.Footer.SerializedRegions.Collect((IRegionPersistentData regionPersistentData) => regionPersistentData.Control).ToList());
            DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.FirstPageHeader.SerializedRegions.Collect((IRegionPersistentData regionPersistentData) => regionPersistentData.Control).ToList());
            DocumentSerializationHelper.CleanSerializerTempData(WorksheetData.FirstPageFooter.SerializedRegions.Collect((IRegionPersistentData regionPersistentData) => regionPersistentData.Control).ToList());
            result.ErrorsOccurredWhenLoadingRegions = DocumentSerializationHelper.ErrorsOccurredWhenLoadingRegions;
            result.DeserializationExceptions = DocumentSerializationHelper.HeaderRegions.AllDeserializationExceptions().Union(DocumentSerializationHelper.MainRegions.AllDeserializationExceptions()).Union(DocumentSerializationHelper.FooterRegions.AllDeserializationExceptions()).Union(DocumentSerializationHelper.FirstPageHeaderRegions.AllDeserializationExceptions()).Union(DocumentSerializationHelper.FirstPageFooterRegions.AllDeserializationExceptions());
        }
        finally
        {
            WorksheetData = null;
            DocumentSerializationHelper.Reset();
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

    private IList<IRegionPersistentData> DeserializeRegionsFromCollection(IRegionCollectionType regionCollection, IWorksheetSectionPersistentData sectionData, string fullFilePath, PackagePart packagePart)
    {
        List<IRegionPersistentData> list = new List<IRegionPersistentData>();
        if (regionCollection?.Regions == null)
        {
            return list;
        }
        IRegionCollectionSerializer regionCollectionSerializer;
        if (sectionData == WorksheetData.Header || sectionData == WorksheetData.FirstPageHeader)
        {
            regionCollectionSerializer = new headerRegionCollectionSerializer();
        }
        else
        {
            regionCollectionSerializer = new footerRegionCollectionSerializer();
        }
        foreach (IRegionInWhiteboardType item in regionCollection.Regions.OfType<IRegionInWhiteboardType>())
        {
            try
            {
                using (McdxDeserializer deS11NProvider = new McdxDeserializer(fullFilePath, packagePart, new WorksheetSectionDeserializationStrategy(sectionData, MathFormat, LabeledIdFormat), DocumentSerializationHelper, regionCollectionSerializer, true, _textStylesManager.TextStyleChangedHandler))
                {
                    IRegionPersistentData regionPersistentData = RegionFactory.ConvertFromIRegionType(deS11NProvider, (UIElement control) => { }, item, null);
                    if (regionPersistentData != null)
                    {
                        regionPersistentData.Location = item.Location;
                        list.Add(regionPersistentData);
                        sectionData.SerializedRegions.Add(regionPersistentData);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertionDoNotThrow("Error deserializing region from container: {0}", ex);
            }
        }
        return list;
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

    private void SerializeWorksheet(Package package)
    {
        WorksheetData = Worksheet.GetWorksheetData();
        SerializeWorksheetSection(PackageOpsProvider.AddMainPartToPackage(package), WorksheetData.WorksheetContent, () => new worksheetRegionType(), new worksheetRegionCollectionSerializer
        {
            RegionCollection =
            {
                msgId = WorksheetData.WorksheetContent.MsgId
            }
        });
        if (!Worksheet.IsFirstPageHeaderFooterDifferent)
        {
            SerializeWorksheetSection(PackageOpsProvider.AddHeaderPartToPackage(package), WorksheetData.Header, () => new headerFooterRegionType(), new headerRegionCollectionSerializer());
            SerializeWorksheetSection(PackageOpsProvider.AddFooterPartToPackage(package), WorksheetData.Footer, () => new headerFooterRegionType(), new footerRegionCollectionSerializer());
        }
        else
        {
            SerializeHeadersContainer(package);
            SerializeFootersContainer(package);
        }
    }

    private void SerializeHeadersContainer(Package package)
    {
        headersContainerRegionCollectionSerializer headersContainerRegionCollectionSerializer = new headersContainerRegionCollectionSerializer();
        WorksheetSectionSerializationStrategy headerStrategy = new WorksheetSectionSerializationStrategy(WorksheetData.Header, () => new headerFooterRegionType());
        WorksheetSectionSerializationStrategy firstPageHeaderStrategy = new WorksheetSectionSerializationStrategy(WorksheetData.FirstPageHeader, () => new headerFooterRegionType());
        HeadersContainerSerializationStrategy serializationStrategy = new HeadersContainerSerializationStrategy(headerStrategy, firstPageHeaderStrategy, headersContainerRegionCollectionSerializer);
        PackagePart part = PackageOpsProvider.AddHeadersContainerPartToPackage(package);
        new McdxSerializer(part, serializationStrategy, DocumentSerializationHelper, headersContainerRegionCollectionSerializer, SerializableXmlPackagePart.OnWriteValidationFailedDevAssert, true).Serialize(part);
    }

    private void SerializeFootersContainer(Package package)
    {
        footersContainerRegionCollectionSerializer footersContainerRegionCollectionSerializer = new footersContainerRegionCollectionSerializer();
        WorksheetSectionSerializationStrategy footerStrategy = new WorksheetSectionSerializationStrategy(WorksheetData.Footer, () => new headerFooterRegionType());
        WorksheetSectionSerializationStrategy firstPageFooterStrategy = new WorksheetSectionSerializationStrategy(WorksheetData.FirstPageFooter, () => new headerFooterRegionType());
        FootersContainerSerializationStrategy serializationStrategy = new FootersContainerSerializationStrategy(footerStrategy, firstPageFooterStrategy, footersContainerRegionCollectionSerializer);
        PackagePart part = PackageOpsProvider.AddFootersContainerPartToPackage(package);
        new McdxSerializer(part, serializationStrategy, DocumentSerializationHelper, footersContainerRegionCollectionSerializer, SerializableXmlPackagePart.OnWriteValidationFailedDevAssert, true).Serialize(part);
    }

    private void SerializeWorksheetSection(PackagePart part, IWorksheetSectionPersistentData sectionData, DelegateFunction0<IRegionType> worksheetSectionRegionDataCreator, IRegionCollectionSerializer regionCollectionSerializer)
    {
        new McdxSerializer(part, new WorksheetSectionSerializationStrategy(sectionData, worksheetSectionRegionDataCreator), DocumentSerializationHelper, regionCollectionSerializer, SerializableXmlPackagePart.OnWriteValidationFailedDevAssert, true).Serialize(part);
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

    private void SavePdfDocument(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                File.Delete(filePath);
            }
            catch (IOException innerException)
            {
                throw new FileIsUsedByAnotherProcessException(innerException);
            }
        }
        string microsoftPdfDriverName = GetFirstNonEmptyString(Settings.Default.MicrosoftPdfDriverName, "Microsoft Print To PDF");
        Exception ex = null;
        PrintQueue printQueue = new LocalPrintServer().GetLocalNonRedirectedPrintQueues().FirstOrDefault((PrintQueue queue) => object.Equals(microsoftPdfDriverName, queue.QueueDriver.Name));
        int jobId = -1;
        string text = printQueue?.FullName;
        if (text != null)
        {
            string text2 = null;
            try
            {
                try
                {
                    text2 = Path.GetTempFileName();
                    SaveXpsDocument(text2);
                }
                catch (IOException innerException2)
                {
                    ex = new PrintToFileException(null, innerException2, PrintFileErrorCode.TakeInnerExceptionByOS);
                }
                if (ex == null)
                {
                    XpsPrintInterop.XpsFileToPrinterFile(text, text2, filePath, out jobId);
                    PrintSystemJobInfo printSystemJobInfo = printQueue.GetPrintJobInfoCollection().FirstOrDefault((PrintSystemJobInfo jobItem) => jobItem.JobIdentifier == jobId);
                    if (printSystemJobInfo != null && printSystemJobInfo.IsInError)
                    {
                        ex = new PrintToFileException("Error printing document: " + filePath + ". Please inspect printer queue: " + text, null, PrintFileErrorCode.PrintQueueJobError, filePath, text);
                    }
                }
            }
            catch (Exception innerException3)
            {
                ex = new PrintToFileException("Unexpected error when saving document: " + filePath, innerException3, PrintFileErrorCode.Unknown, filePath);
            }
            finally
            {
                try
                {
                    if (text2 != null)
                    {
                        File.Delete(text2);
                    }
                }
                catch (Exception ex2)
                {
                    DebugTools.FailAssertion(ex2);
                }
            }
        }
        else
        {
            ex = new PrintToFileException("No " + microsoftPdfDriverName + " found", null, PrintFileErrorCode.MissingMicrosoftPDFDriver, microsoftPdfDriverName);
        }
        if (ex != null)
        {
            throw ex;
        }
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

    private static readonly IDictionary<Quire.Controls.Core.PageOrientation, System.Printing.PageOrientation> _pageOrientationTranslation = new Dictionary<Quire.Controls.Core.PageOrientation, System.Printing.PageOrientation>
    {
        { Quire.Controls.Core.PageOrientation.Landscape, System.Printing.PageOrientation.Landscape },
        { Quire.Controls.Core.PageOrientation.Portrait, System.Printing.PageOrientation.Portrait }
    };

    private static System.Printing.PageOrientation GetSystemPrintingPageOrientationEquvalent(Quire.Controls.Core.PageOrientation pageOrientation)
    {
        if (!_pageOrientationTranslation.ContainsKey(pageOrientation))
        {
            return System.Printing.PageOrientation.Portrait;
        }
        return _pageOrientationTranslation[pageOrientation];
    }

    private static readonly IDictionary<PageSize, PageMediaSizeName> _pageSizeTranslation = new Dictionary<PageSize, PageMediaSizeName>
    {
        { PageSize.A1, PageMediaSizeName.ISOA1 },
        { PageSize.A2, PageMediaSizeName.ISOA2 },
        { PageSize.A3, PageMediaSizeName.ISOA3 },
        { PageSize.A4, PageMediaSizeName.ISOA4 },
        { PageSize.A5, PageMediaSizeName.ISOA5 },
        { PageSize.Letter, PageMediaSizeName.NorthAmericaLetter },
        { PageSize.Legal, PageMediaSizeName.NorthAmericaLegal },
        { PageSize.Ledger, PageMediaSizeName.NorthAmerica11x17 }
    };

    private static PageMediaSizeName GetSystemPrintingPageMediaSizeNameEquvalent(PageSize pageSize)
    {
        if (!_pageSizeTranslation.ContainsKey(pageSize))
        {
            return PageMediaSizeName.NorthAmericaLetter;
        }
        return _pageSizeTranslation[pageSize];
    }

    private IDocumentPaginatorSource CreatePrintableDocument(Win32PrintRange printRange, PageIndexRange pageRange, bool shouldEnrichPrintingTree = false)
    {
        DebugTools.LogData("EngineeringDocument.CreatePrintableDocument: Enter");
        Worksheet.PrePrintUpdateWorksheetLayout(printRange, pageRange);
        WpfUtils.DoEvents();
        EngineeringPages engPages = null;
        try
        {
            engPages = new EngineeringPages
            {
                PageLayout = Worksheet.PageLayout.Clone(),
                DisplayGrid = Worksheet.DisplayGrid,
                IsSolveBlockLabelVisible = Worksheet.IsSolveBlockLabelVisible,
                DisplayHFGrid = Worksheet.DisplayHFGrid,
                UseMajorGridLines = Worksheet.UseMajorGridLines,
                UseHFMajorGridLines = Worksheet.UseHFMajorGridLines,
                DisplayFrame = Worksheet.DisplayFrame,
                DisplayHeaderFrame = Worksheet.DisplayHeaderFrame,
                DisplayFooterFrame = Worksheet.DisplayFooterFrame,
                TotalPages = PageCount,
                StartingPageNumber = Worksheet.StartingPageNumber,
                TotalPagesOffset = Worksheet.TotalPagesOffset,
                IsFirstPageHeaderFooterDifferent = Worksheet.IsFirstPageHeaderFooterDifferent,
                DisplaySavedDate = DisplaySavedDate,
                DisplayFilePath = DisplayFilePath,
                DisplayFileName = DisplayFileName,
                DisplayDocumentName = DisplayDocumentName,
                MaxUpdateLayoutTimeMsec = MaxUpdateLayoutTimeMsec
            };
            Dictionary<IdLabels, ExtendedFont> dictionary = Worksheet.LabeledIdFormat.ToDictionary((KeyValuePair<IdLabels, ExtendedFont> extendedFont) => extendedFont.Key, (KeyValuePair<IdLabels, ExtendedFont> extendedFont) => extendedFont.Value.Clone());
            engPages.SetValue(EquationControl.ResultMatrixFormatProperty, Worksheet.ResultMatrixFormat.Clone());
            engPages.SetValue(EquationControl.DisplayNumberFormatProperty, Worksheet.DisplayNumberFormat.Clone());
            engPages.SetValue(AttachedProperties.LabeledIdFormatProperty, dictionary);
            AttachedProperties.SetWarningDisplayFilter(engPages, AttachedProperties.GetWarningDisplayFilter(this));
            MathFormat mathFormat = new MathFormat(engPages);
            Worksheet.MathFormat.CopyTo(mathFormat);
            DocumentAttachedProperties.SetShowInputOutputDesignation(engPages, DocumentAttachedProperties.GetShowInputOutputDesignation(Worksheet as DependencyObject));
            engPages.Resources["PageBodyWidth"] = Worksheet.FindResource("PageBodyWidth");
            engPages.Resources["PageBodyContentWidth"] = Worksheet.FindResource("PageBodyContentWidth");
            WorksheetPropertiesManager.SetOrRemoveWorksheetPropertiesSource(engPages, new CachedWorksheetPropertiesSource(Worksheet), true);
            OnPrepareSnapshotStart?.Invoke(this);
            using (MathFormatForPaste.Generate(mathFormat, dictionary))
            {
                using (ChartComponentUtils.CreateDisposablePrintHelper(Worksheet.WorksheetItems, true))
                {
                    switch (printRange)
                    {
                    case Win32PrintRange.CurrentPage:
                        engPages.AddPage(Worksheet.GetPageSnapshot(Worksheet.CurrentPageNumber - 1));
                        break;
                    case Win32PrintRange.SomePages:
                        pageRange.FirstIndex.Range(pageRange.LastIndex).Each((int pgIdx) =>
                        {
                            engPages.AddPage(Worksheet.GetPageSnapshot(pgIdx));
                        });
                        break;
                    case Win32PrintRange.Selection:
                        (from pgIdx in Worksheet.SelectedItems.Select((IWhiteboardItem item) => Worksheet.GetPagesContaining(item)).SelectMany((PageIndexRange range) => range.FirstIndex.Range(range.LastIndex))
                         orderby pgIdx
                         select pgIdx).Distinct().Each((int pgIdx) =>
                        {
                            engPages.AddPage(Worksheet.GetPageSnapshot(pgIdx));
                        });
                        break;
                    case Win32PrintRange.AllPages:
                        PageCount.Times((int pgIdx) =>
                        {
                            engPages.AddPage(Worksheet.GetPageSnapshot(pgIdx));
                        });
                        break;
                    }
                }
            }
            engPages.CreateVisualTreeForPrinting(shouldEnrichPrintingTree);
        }
        catch (Exception ex)
        {
            if (engPages != null)
            {
                engPages.Dispose();
                engPages = null;
            }
            string arg = string.Format((string)_errorWarningMessageResourceDictionary["WorksheetMessages.CantPrint"], ex.Message);
            AppServiceRegistry.GetService<IMessageBoxManager>().ShowCriticalErrorMessage(Application.Current.MainWindow, arg, WpfUtils.ApplicationFullName);
        }
        finally
        {
            Worksheet.PostPrintUpdateWorksheetLayout();
            OnPrepareSnapshotEnd?.Invoke(this);
        }
        return engPages;
    }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private bool _isNew;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    private bool _isNetworkDriveDisconnected;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public bool IsNew => _isNew;

    public int PageCount => Worksheet.PageCount;

    public int SelectedRegionCount => Worksheet.SelectedItems.Count();

    public bool IsHeaderFocused => Worksheet.IsHeaderActive;

    public bool IsFooterFocused => Worksheet.IsFooterActive;

    public bool IsMainActive => Worksheet.IsContentActive;

    public Point InsertionPoint => Worksheet.InsertionPoint;

    public IIORegionManager InputOutputRegionManager => Worksheet.InputOutputRegionManager;

    public void ActivateHeader() => Worksheet.ActivateHeader();

    public void ActivateFooter() => Worksheet.ActivateFooter();

    public void ActivateMain() => Worksheet.ActivateContent();

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

    private void UpdateCWDState(string fileName)
    {
        string prevCwd = CWD;
        string newCwd = CWD = Path.GetDirectoryName(Path.GetFullPath(fileName));
        if (ReadonlyOptions.FileLocationHistoryEnabled && prevCwd != newCwd && CwdUpdated != null)
        {
            CwdUpdated(newCwd);
        }
    }

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<string> CwdUpdated;

    private FileStream _fileStream;

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

    private bool _showEngineChangesBanner;

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

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
    public event Action<IEngineeringDocument> FilePathChanged = delegate { };

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

    public void ActivateWorksheet() => WorksheetControl.Activate();
}
