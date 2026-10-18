# Ptc.PersistentDataObjects.presentationMathRendering

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000dd1 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `operators get` | 0x06000dc7 | MemberGen: `operatorsField; set => operatorsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `operators set` | 0x06000dc8 | MemberGen: `operatorsField; set => operatorsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mathStyles get` | 0x06000dc9 | MemberGen: `mathStylesField; set => mathStylesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `mathStyles set` | 0x06000dca | MemberGen: `mathStylesField; set => mathStylesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolics get` | 0x06000dcb | MemberGen: `symbolicsField; set => symbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolics set` | 0x06000dcc | MemberGen: `symbolicsField; set => symbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `formatting get` | 0x06000dcd | MemberGen: `formattingField; set => formattingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `formatting set` | 0x06000dce | MemberGen: `formattingField; set => formattingField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `results get` | 0x06000dcf | MemberGen: `resultsField; set => resultsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `results set` | 0x06000dd0 | MemberGen: `resultsField; set => resultsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspresentationMathRenderingMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
