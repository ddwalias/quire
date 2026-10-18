# Ptc.PersistentDataObjects.axisBaseTypePlotEquations

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000ccc | MemberGen: `displayField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquation get` | 0x06000ccd | MemberGen: `plotEquationField; set => plotEquationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotEquation set` | 0x06000cce | MemberGen: `plotEquationField; set => plotEquationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `display get` | 0x06000ccf | MemberGen: `displayField; set => displayField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `display set` | 0x06000cd0 | MemberGen: `displayField; set => displayField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsaxisBaseTypePlotEquationsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
