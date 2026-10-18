# Ptc.PersistentDataObjects.propertiesType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x060009b0 | MemberGen: `applicationField = "MathcadPrime";` | Mechanical; Release IL equals Mathcad's | Verified |
| `application get` | 0x060009b1 | MemberGen: `applicationField; set => applicationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `application set` | 0x060009b2 | MemberGen: `applicationField; set => applicationField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `appVersion get` | 0x060009b3 | MemberGen: `appVersionField; set => appVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `appVersion set` | 0x060009b4 | MemberGen: `appVersionField; set => appVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `serializationVersion get` | 0x060009b5 | MemberGen: `serializationVersionField; set => serializationVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `serializationVersion set` | 0x060009b6 | MemberGen: `serializationVersionField; set => serializationVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `engineVersion get` | 0x060009b7 | MemberGen: `engineVersionField; set => engineVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `engineVersion set` | 0x060009b8 | MemberGen: `engineVersionField; set => engineVersionField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `build get` | 0x060009b9 | MemberGen: `buildField; set => buildField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `build set` | 0x060009ba | MemberGen: `buildField; set => buildField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `schemaPropertiesList get` | 0x060009bb | MemberGen: `schemaPropertiesListField; set => schemaPropertiesListField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `schemaPropertiesList set` | 0x060009bc | MemberGen: `schemaPropertiesListField; set => schemaPropertiesListField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NewData get` | 0x060009bd | MemberGen: `newDataField; set => newDataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `NewData set` | 0x060009be | MemberGen: `newDataField; set => newDataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspropertiesTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
