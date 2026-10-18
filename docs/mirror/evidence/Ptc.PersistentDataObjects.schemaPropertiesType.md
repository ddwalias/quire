# Ptc.PersistentDataObjects.schemaPropertiesType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060009ca | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `name get` | 0x060009c6 | MemberGen: `nameField; set => nameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `name set` | 0x060009c7 | MemberGen: `nameField; set => nameField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `version get` | 0x060009c8 | MemberGen: `versionField; set => versionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `version set` | 0x060009c9 | MemberGen: `versionField; set => versionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsschemaPropertiesTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
