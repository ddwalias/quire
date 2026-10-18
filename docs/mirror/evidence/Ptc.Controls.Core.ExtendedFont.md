# Ptc.Controls.Core.ExtendedFont

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreExtendedFontMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000280 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateInstanceCore()` | 0x0600028d | MemberGen: `new ExtendedFont()` | Mechanical; Release IL equals Mathcad's | Verified |
| `RevisionNumber get` | 0x06000268 | MemberGen: `public int RevisionNumber { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RevisionNumber set` | 0x06000269 | MemberGen: `public int RevisionNumber { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatChanged add` | 0x06000287 | MemberGen: `public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `FormatChanged remove` | 0x06000288 | MemberGen: `public event Delegate1<DependencyPropertyChangedEventArgs> FormatChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreExtendedFontMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
