# Ptc.Controls.Core.ImageControl

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreImageControlMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetImageExtension()` | 0x06000355 | MemberGen: `GetImageExtension(_originalImageUri)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SimulateMouseClick(Point, int)` | 0x06000369 | MemberGen: `Activate(true)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetHelpContext()` | 0x06000373 | MemberGen: `"ImageControl"` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalStreamHash get` | 0x06000347 | MemberGen: `public byte[] OriginalStreamHash { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OriginalStreamHash set` | 0x06000348 | MemberGen: `public byte[] OriginalStreamHash { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `VerticalAlignmentOffset get` | 0x06000365 | MemberGen: `0.0` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsEmpty get` | 0x06000366 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `PreserveAspectRatio get` | 0x0600036e | MemberGen: `true` | Mechanical; Release IL equals Mathcad's | Verified |
| `SubElementToResizeData get` | 0x0600036f | MemberGen: `null` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600035f | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x06000360 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnGotFocusEvent add` | 0x06000361 | MemberGen: `public event Action<IWhiteboardItem> OnGotFocusEvent;` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnGotFocusEvent remove` | 0x06000362 | MemberGen: `public event Action<IWhiteboardItem> OnGotFocusEvent;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestRemove add` | 0x06000363 | MemberGen: `public event Action<IWhiteboardItem, Direction> RequestRemove;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestRemove remove` | 0x06000364 | MemberGen: `public event Action<IWhiteboardItem, Direction> RequestRemove;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestResizeToFitOrClearRestriction add` | 0x06000371 | MemberGen: `public event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestResizeToFitOrClearRestriction remove` | 0x06000372 | MemberGen: `public event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreImageControlMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
