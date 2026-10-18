# Ptc.Controls.Core.ViewModelBase

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreViewModelBaseMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ModelPropertyChanged(string)` | 0x06000782 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `OnDispose()` | 0x06000785 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateCommandBinding(ICommand, Action)` | 0x06000787 | MemberGen: `CreateCommandBinding(command, execute, null)` | Mechanical; Release IL equals Mathcad's | Verified |
| `DisplayName get` | 0x0600077d | MemberGen: `public virtual string DisplayName { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DisplayName set` | 0x0600077e | MemberGen: `public virtual string DisplayName { get; protected set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CommandBindings get` | 0x06000786 | MemberGen: `_commandBindings` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600077f | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x06000780 | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreViewModelBaseMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
