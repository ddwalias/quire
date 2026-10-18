# Ptc.Controls.Core.Serialization.DeserializationStrategy

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsRegionNotAffectingCalculation(UIElement)` | 0x06000a90 | MemberGen: `false` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeRegionProlog(UIElement)` | 0x06000a96 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeRegionsEpilog(XmlDocument)` | 0x06000a97 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000a98 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeMathFormat get` | 0x06000a89 | MemberGen: `public IMathFormat DeserializeMathFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeMathFormat set` | 0x06000a8a | MemberGen: `public IMathFormat DeserializeMathFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeLabeledIdFormat get` | 0x06000a8b | MemberGen: `public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializeLabeledIdFormat set` | 0x06000a8c | MemberGen: `public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationDeserializationStrategyMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
