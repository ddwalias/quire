# Ptc.PersistentDataObjects.MC14MathConversionValidation

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ContainsSymbolicEvaluationAsEquals(XContainer)` | 0x06000091 | MemberGen: `ContainsAttributeWithValue("symEval", "style", "equals", document)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ContainsBooleanEqualityAsEquals(XContainer)` | 0x06000092 | MemberGen: `ContainsAttributeWithValue("equal", "style", "equal", document)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ContainsLocalDefinitionAsEquals(XContainer)` | 0x06000093 | MemberGen: `ContainsAttributeWithValue("localDefine", "style", "equal", document)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetMc14Math30XName(string)` | 0x060000a7 | MemberGen: `XName.Get(localName, "http://schemas.mathsoft.com/math30")` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsMC14MathConversionValidationMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
