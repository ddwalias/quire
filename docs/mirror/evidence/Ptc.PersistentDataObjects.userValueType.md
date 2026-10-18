# Ptc.PersistentDataObjects.userValueType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000a3f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `name get` | 0x06000a39 | MemberGen: `nameField; set => nameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `name set` | 0x06000a3a | MemberGen: `nameField; set => nameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `type get` | 0x06000a3b | MemberGen: `typeField; set => typeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `type set` | 0x06000a3c | MemberGen: `typeField; set => typeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000a3d | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000a3e | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsuserValueTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
