#nullable disable
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.Controls.Worksheet;
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

    partial class RootDecorator
    {
        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public RootDecorator() { }

        [MirrorVerified("PtcControlsEngineeringDocumentMechanicalDifferentialTests")]
        public Visual Visual => this;
    }
}
