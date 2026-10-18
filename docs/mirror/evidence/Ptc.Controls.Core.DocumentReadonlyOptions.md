# Ptc.Controls.Core.DocumentReadonlyOptions

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `RequestToUpdateInputsEnabled get` | 0x060002bf | MemberGen: `public bool RequestToUpdateInputsEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RequestToUpdateInputsEnabled set` | 0x060002c0 | MemberGen: `public bool RequestToUpdateInputsEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FileLocationHistoryEnabled get` | 0x060002c1 | MemberGen: `public bool FileLocationHistoryEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FileLocationHistoryEnabled set` | 0x060002c2 | MemberGen: `public bool FileLocationHistoryEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `GenerateInputsOutputsStatesOnSave get` | 0x060002c3 | MemberGen: `public bool GenerateInputsOutputsStatesOnSave { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `GenerateInputsOutputsStatesOnSave set` | 0x060002c4 | MemberGen: `public bool GenerateInputsOutputsStatesOnSave { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CaseInsensitiveAliasComparisonEnabled get` | 0x060002c5 | MemberGen: `public bool CaseInsensitiveAliasComparisonEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `CaseInsensitiveAliasComparisonEnabled set` | 0x060002c6 | MemberGen: `public bool CaseInsensitiveAliasComparisonEnabled { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreDocumentReadonlyOptionsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
