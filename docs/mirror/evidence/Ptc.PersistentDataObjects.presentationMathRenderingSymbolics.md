# Ptc.PersistentDataObjects.presentationMathRenderingSymbolics

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `derivationstepsstyle get` | 0x06000dfd | MemberGen: `derivationstepsstyleField; set => derivationstepsstyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `derivationstepsstyle set` | 0x06000dfe | MemberGen: `derivationstepsstyleField; set => derivationstepsstyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showcomments get` | 0x06000dff | MemberGen: `showcommentsField; set => showcommentsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showcomments set` | 0x06000e00 | MemberGen: `showcommentsField; set => showcommentsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `evaluateinplace get` | 0x06000e01 | MemberGen: `evaluateinplaceField; set => evaluateinplaceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `evaluateinplace set` | 0x06000e02 | MemberGen: `evaluateinplaceField; set => evaluateinplaceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspresentationMathRenderingSymbolicsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
