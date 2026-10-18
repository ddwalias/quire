# Ptc.PersistentDataObjects.plotBaseType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d4a | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `title get` | 0x06000d3f | MemberGen: `titleField; set => titleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `title set` | 0x06000d40 | MemberGen: `titleField; set => titleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `legend get` | 0x06000d41 | MemberGen: `legendField; set => legendField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `legend set` | 0x06000d42 | MemberGen: `legendField; set => legendField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `traces get` | 0x06000d43 | MemberGen: `tracesField; set => tracesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `traces set` | 0x06000d44 | MemberGen: `tracesField; set => tracesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize get` | 0x06000d45 | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `graphsize set` | 0x06000d46 | MemberGen: `graphsizeField; set => graphsizeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotations get` | 0x06000d47 | MemberGen: `MC14ConversionLossAnnotationsLocal` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x06000d48 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x06000d49 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsplotBaseTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
