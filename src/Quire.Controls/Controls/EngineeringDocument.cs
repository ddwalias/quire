#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.Controls.Worksheet;
using Quire.Core;
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

    partial class RootDecorator
    {
        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public RootDecorator() { }

        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public Visual Visual => this;
    }
}
