# Ptc.Controls.Core.MathFormatForPaste

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600045f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Instance get` | 0x06000460 | MemberGen: `_instance` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentMathFormat get` | 0x06000464 | MemberGen: `public IMathFormat DocumentMathFormat { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentMathFormat set` | 0x06000465 | MemberGen: `public IMathFormat DocumentMathFormat { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentLabeledIdFormat get` | 0x06000466 | MemberGen: `public IDictionary<IdLabels, ExtendedFont> DocumentLabeledIdFormat { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DocumentLabeledIdFormat set` | 0x06000467 | MemberGen: `public IDictionary<IdLabels, ExtendedFont> DocumentLabeledIdFormat { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreMathFormatForPasteMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
