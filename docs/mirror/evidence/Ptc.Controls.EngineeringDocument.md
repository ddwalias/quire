# Ptc.Controls.EngineeringDocument

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsEngineeringDocumentMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CloseOpenedComboBoxEditingMode()` | 0x060000f8 | MemberGen: `WpfUtils.ExecuteCommandIfFocusedElementIsDescendantOf<EngineeringDocument>(this, EditableComboBoxCommands.CloseEditingMode)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WaitForLoad(int)` | 0x060000fc | MemberGen: `FileManager.WaitFor(seconds, _waitForLoad)` | Mechanical; Release IL equals Mathcad's | Verified |
| `FireCalculationOptionUpdated(string, object)` | 0x06000101 | MemberGen: `IsDirty = true` | Mechanical; Release IL equals Mathcad's | Verified |
| `TraceWarning(string, object[])` | 0x0600010e | MemberGen: `Trace.TraceWarning(format, args)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Worksheet get` | 0x06000094 | MemberGen: `WorksheetControl` | Mechanical; Release IL equals Mathcad's | Verified |
| `NeedsDictionaryRefresh get` | 0x06000095 | MemberGen: `public bool NeedsDictionaryRefresh { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NeedsDictionaryRefresh set` | 0x06000096 | MemberGen: `public bool NeedsDictionaryRefresh { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Zoom get` | 0x0600009c | MemberGen: `DocumentAttachedProperties.GetZoom(this); set => DocumentAttachedProperties.SetZoom<EngineeringDocument>(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Zoom set` | 0x0600009d | MemberGen: `DocumentAttachedProperties.GetZoom(this); set => DocumentAttachedProperties.SetZoom<EngineeringDocument>(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FilePath get` | 0x060000ad | MemberGen: `DocumentAttachedProperties.GetFilePath(this); set => DocumentAttachedProperties.SetFilePath(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FilePath set` | 0x060000ae | MemberGen: `DocumentAttachedProperties.GetFilePath(this); set => DocumentAttachedProperties.SetFilePath(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MaxUpdateLayoutTimeMsec get` | 0x060000b5 | MemberGen: `public long MaxUpdateLayoutTimeMsec { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MaxUpdateLayoutTimeMsec set` | 0x060000b6 | MemberGen: `public long MaxUpdateLayoutTimeMsec { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetData get` | 0x060000c2 | MemberGen: `public IWorksheetPersistentData WorksheetData { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetData set` | 0x060000c3 | MemberGen: `public IWorksheetPersistentData WorksheetData { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsNetworkDriveDisconnected get` | 0x060000d7 | MemberGen: `_isNetworkDriveDisconnected` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentSerializationHelper get` | 0x060000d9 | MemberGen: `public ISerializationHelper DocumentSerializationHelper { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDocumentFromPTCPlaces get` | 0x060000da | MemberGen: `_isDocumentFromPtcPlaces; set => _isDocumentFromPtcPlaces = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDocumentFromPTCPlaces set` | 0x060000db | MemberGen: `_isDocumentFromPtcPlaces; set => _isDocumentFromPtcPlaces = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TextStylesManager get` | 0x060000e1 | MemberGen: `_textStylesManager` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsNew get` | 0x06000137 | MemberGen: `_isNew` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnPrepareSnapshotStart add` | 0x0600008b | MemberGen: `public event Action<EngineeringDocument> OnPrepareSnapshotStart;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnPrepareSnapshotStart remove` | 0x0600008c | MemberGen: `public event Action<EngineeringDocument> OnPrepareSnapshotStart;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnPrepareSnapshotEnd add` | 0x0600008d | MemberGen: `public event Action<EngineeringDocument> OnPrepareSnapshotEnd;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnPrepareSnapshotEnd remove` | 0x0600008e | MemberGen: `public event Action<EngineeringDocument> OnPrepareSnapshotEnd;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDirtyChanged add` | 0x06000090 | MemberGen: `public event Action<IEngineeringDocument> IsDirtyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsDirtyChanged remove` | 0x06000091 | MemberGen: `public event Action<IEngineeringDocument> IsDirtyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnDispose add` | 0x060000e2 | MemberGen: `public event Action<EngineeringDocument> OnDispose;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnDispose remove` | 0x060000e3 | MemberGen: `public event Action<EngineeringDocument> OnDispose;` | Mechanical; Release IL equals Mathcad's | Verified |
| `CwdUpdated add` | 0x06000146 | MemberGen: `public event Action<string> CwdUpdated;` | Mechanical; Release IL equals Mathcad's | Verified |
| `CwdUpdated remove` | 0x06000147 | MemberGen: `public event Action<string> CwdUpdated;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x06000150 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x06000151 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `FilePathChanged add` | 0x06000152 | MemberGen: `public event Action<IEngineeringDocument> FilePathChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `FilePathChanged remove` | 0x06000153 | MemberGen: `public event Action<IEngineeringDocument> FilePathChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RootDecorator: .ctor()` | 0x060002ea | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RootDecorator: Visual get` | 0x060002e9 | MemberGen: `this` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsEngineeringDocumentMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
