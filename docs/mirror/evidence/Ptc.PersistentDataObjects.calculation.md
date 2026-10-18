# Ptc.PersistentDataObjects.calculation

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectscalculationMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000bc4 | MemberGen: `cacheworksheetField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `builtInVariables get` | 0x06000bc5 | MemberGen: `builtInVariablesField; set => builtInVariablesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `builtInVariables set` | 0x06000bc6 | MemberGen: `builtInVariablesField; set => builtInVariablesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `calculationBehavior get` | 0x06000bc7 | MemberGen: `calculationBehaviorField; set => calculationBehaviorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `calculationBehavior set` | 0x06000bc8 | MemberGen: `calculationBehaviorField; set => calculationBehaviorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `units get` | 0x06000bc9 | MemberGen: `unitsField; set => unitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `units set` | 0x06000bca | MemberGen: `unitsField; set => unitsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `cacheworksheet get` | 0x06000bcb | MemberGen: `cacheworksheetField; set => cacheworksheetField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `cacheworksheet set` | 0x06000bcc | MemberGen: `cacheworksheetField; set => cacheworksheetField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectscalculationMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
