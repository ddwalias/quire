#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.InputControls;
using Quire.Controls.Worksheet;
using Quire.Core;
using Quire.IO;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.Serialization;
using Quire.Wpf;

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

    partial class RootDecorator
    {
        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public RootDecorator() { }

        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public Visual Visual => this;
    }
}
