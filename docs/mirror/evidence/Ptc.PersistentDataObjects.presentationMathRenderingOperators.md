# Ptc.PersistentDataObjects.presentationMathRenderingOperators

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspresentationMathRenderingOperatorsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `multiplication get` | 0x06000dd3 | MemberGen: `multiplicationField; set => multiplicationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `multiplication set` | 0x06000dd4 | MemberGen: `multiplicationField; set => multiplicationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `derivative get` | 0x06000dd5 | MemberGen: `derivativeField; set => derivativeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `derivative set` | 0x06000dd6 | MemberGen: `derivativeField; set => derivativeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `literalsubscript get` | 0x06000dd7 | MemberGen: `literalsubscriptField; set => literalsubscriptField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `literalsubscript set` | 0x06000dd8 | MemberGen: `literalsubscriptField; set => literalsubscriptField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `definition get` | 0x06000dd9 | MemberGen: `definitionField; set => definitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `definition set` | 0x06000dda | MemberGen: `definitionField; set => definitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `globaldefinition get` | 0x06000ddb | MemberGen: `globaldefinitionField; set => globaldefinitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `globaldefinition set` | 0x06000ddc | MemberGen: `globaldefinitionField; set => globaldefinitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `localdefinition get` | 0x06000ddd | MemberGen: `localdefinitionField; set => localdefinitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `localdefinition set` | 0x06000dde | MemberGen: `localdefinitionField; set => localdefinitionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equality get` | 0x06000ddf | MemberGen: `equalityField; set => equalityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equality set` | 0x06000de0 | MemberGen: `equalityField; set => equalityField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolicevaluation get` | 0x06000de1 | MemberGen: `symbolicevaluationField; set => symbolicevaluationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolicevaluation set` | 0x06000de2 | MemberGen: `symbolicevaluationField; set => symbolicevaluationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspresentationMathRenderingOperatorsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
