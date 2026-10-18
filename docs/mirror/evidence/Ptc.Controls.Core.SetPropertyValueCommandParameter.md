# Ptc.Controls.Core.SetPropertyValueCommandParameter

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000571 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CreateInstanceCore()` | 0x0600057b | MemberGen: `new SetPropertyValueCommandParameter()` | Mechanical; Release IL equals Mathcad's | Verified |
| `DependencyPropVal get` | 0x06000577 | MemberGen: `base.GetValue(DependencyPropValProperty); set => base.SetValue(DependencyPropValProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DependencyPropVal set` | 0x06000578 | MemberGen: `base.GetValue(DependencyPropValProperty); set => base.SetValue(DependencyPropValProperty, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UserData get` | 0x06000579 | MemberGen: `public string UserData { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UserData set` | 0x0600057a | MemberGen: `public string UserData { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
