# Ptc.PersistentDataObjects.unitMonomialUnitReference

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `unit get` | 0x06000b76 | MemberGen: `unitField; set => unitField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `unit set` | 0x06000b77 | MemberGen: `unitField; set => unitField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `powernumerator get` | 0x06000b78 | MemberGen: `powernumeratorField; set => powernumeratorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `powernumerator set` | 0x06000b79 | MemberGen: `powernumeratorField; set => powernumeratorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `powerdenominator get` | 0x06000b7a | MemberGen: `powerdenominatorField; set => powerdenominatorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `powerdenominator set` | 0x06000b7b | MemberGen: `powerdenominatorField; set => powerdenominatorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsunitMonomialUnitReferenceMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
