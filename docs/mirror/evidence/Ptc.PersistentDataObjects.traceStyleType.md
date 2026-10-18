# Ptc.PersistentDataObjects.traceStyleType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `color get` | 0x06000d16 | MemberGen: `colorField; set => colorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `color set` | 0x06000d17 | MemberGen: `colorField; set => colorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbol get` | 0x06000d18 | MemberGen: `symbolField; set => symbolField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbol set` | 0x06000d19 | MemberGen: `symbolField; set => symbolField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolweight get` | 0x06000d1a | MemberGen: `symbolweightField; set => symbolweightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolweight set` | 0x06000d1b | MemberGen: `symbolweightField; set => symbolweightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolfrequency get` | 0x06000d1c | MemberGen: `symbolfrequencyField; set => symbolfrequencyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `symbolfrequency set` | 0x06000d1d | MemberGen: `symbolfrequencyField; set => symbolfrequencyField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `linestyle get` | 0x06000d1e | MemberGen: `linestyleField; set => linestyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `linestyle set` | 0x06000d1f | MemberGen: `linestyleField; set => linestyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `lineweight get` | 0x06000d20 | MemberGen: `lineweightField; set => lineweightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `lineweight set` | 0x06000d21 | MemberGen: `lineweightField; set => lineweightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `automaticStyle get` | 0x06000d22 | MemberGen: `automaticStyleField; set => automaticStyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `automaticStyle set` | 0x06000d23 | MemberGen: `automaticStyleField; set => automaticStyleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value get` | 0x06000d24 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Value set` | 0x06000d25 | MemberGen: `valueField; set => valueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectstraceStyleTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
