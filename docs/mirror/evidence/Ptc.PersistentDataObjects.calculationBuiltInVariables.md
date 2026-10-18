# Ptc.PersistentDataObjects.calculationBuiltInVariables

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `arrayorigin get` | 0x06000bd0 | MemberGen: `arrayoriginField; set => arrayoriginField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `arrayorigin set` | 0x06000bd1 | MemberGen: `arrayoriginField; set => arrayoriginField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convergencetolerance get` | 0x06000bd2 | MemberGen: `convergencetoleranceField; set => convergencetoleranceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `convergencetolerance set` | 0x06000bd3 | MemberGen: `convergencetoleranceField; set => convergencetoleranceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `constrainttolerance get` | 0x06000bd4 | MemberGen: `constrainttoleranceField; set => constrainttoleranceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `constrainttolerance set` | 0x06000bd5 | MemberGen: `constrainttoleranceField; set => constrainttoleranceField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `randomseed get` | 0x06000bd6 | MemberGen: `randomseedField; set => randomseedField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `randomseed set` | 0x06000bd7 | MemberGen: `randomseedField; set => randomseedField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `prnprecision get` | 0x06000bd8 | MemberGen: `prnprecisionField; set => prnprecisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `prnprecision set` | 0x06000bd9 | MemberGen: `prnprecisionField; set => prnprecisionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `prncolwidth get` | 0x06000bda | MemberGen: `prncolwidthField; set => prncolwidthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `prncolwidth set` | 0x06000bdb | MemberGen: `prncolwidthField; set => prncolwidthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscalculationBuiltInVariablesMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
