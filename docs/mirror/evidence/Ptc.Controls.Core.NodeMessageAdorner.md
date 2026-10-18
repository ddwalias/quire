# Ptc.Controls.Core.NodeMessageAdorner

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `AddToAdornerLayer()` | 0x060004b3 | MemberGen: `FireAdornerAdded()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Message get` | 0x060004a4 | MemberGen: `_message` | Mechanical; Release IL equals Mathcad's | Verified |
| `Type get` | 0x060004a5 | MemberGen: `_type` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
