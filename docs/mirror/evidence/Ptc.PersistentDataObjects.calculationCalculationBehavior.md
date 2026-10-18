# Ptc.PersistentDataObjects.calculationCalculationBehavior

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscalculationCalculationBehaviorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `stringsuseorigin get` | 0x06000bdd | MemberGen: `stringsuseoriginField; set => stringsuseoriginField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `stringsuseorigin set` | 0x06000bde | MemberGen: `stringsuseoriginField; set => stringsuseoriginField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `automaticrecalculation get` | 0x06000bdf | MemberGen: `automaticrecalculationField; set => automaticrecalculationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `automaticrecalculation set` | 0x06000be0 | MemberGen: `automaticrecalculationField; set => automaticrecalculationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `matrixstrictsingularitycheck get` | 0x06000be1 | MemberGen: `matrixstrictsingularitycheckField; set => matrixstrictsingularitycheckField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `matrixstrictsingularitycheck set` | 0x06000be2 | MemberGen: `matrixstrictsingularitycheckField; set => matrixstrictsingularitycheckField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `optimizeexpressions get` | 0x06000be3 | MemberGen: `optimizeexpressionsField; set => optimizeexpressionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `optimizeexpressions set` | 0x06000be4 | MemberGen: `optimizeexpressionsField; set => optimizeexpressionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exactboolean get` | 0x06000be5 | MemberGen: `exactbooleanField; set => exactbooleanField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `exactboolean set` | 0x06000be6 | MemberGen: `exactbooleanField; set => exactbooleanField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitsinsymbolics get` | 0x06000be7 | MemberGen: `unitsinsymbolicsField; set => unitsinsymbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unitsinsymbolics set` | 0x06000be8 | MemberGen: `unitsinsymbolicsField; set => unitsinsymbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `constantsinsymbolics get` | 0x06000be9 | MemberGen: `constantsinsymbolicsField; set => constantsinsymbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `constantsinsymbolics set` | 0x06000bea | MemberGen: `constantsinsymbolicsField; set => constantsinsymbolicsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `binocthexmode get` | 0x06000beb | MemberGen: `binocthexmodeField; set => binocthexmodeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `binocthexmode set` | 0x06000bec | MemberGen: `binocthexmodeField; set => binocthexmodeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerooverzero get` | 0x06000bed | MemberGen: `zerooverzeroField; set => zerooverzeroField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `zerooverzero set` | 0x06000bee | MemberGen: `zerooverzeroField; set => zerooverzeroField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolicengine get` | 0x06000bef | MemberGen: `symbolicengineField; set => symbolicengineField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolicengine set` | 0x06000bf0 | MemberGen: `symbolicengineField; set => symbolicengineField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscalculationCalculationBehaviorMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
