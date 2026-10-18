# Ptc.Undo.CompoundCommand

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcUndoCompoundCommandMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(string, IEnumerable)` | 0x060006e0 | MemberGen: `_name = name;` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x060006e1 | MemberGen: `_name = string.Empty;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name get` | 0x060006de | MemberGen: `_name; set => _name = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x060006df | MemberGen: `_name; set => _name = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcUndoCompoundCommandMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
