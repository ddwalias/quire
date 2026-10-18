# Ptc.PersistentDataObjects.Trace2dResultType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d7e | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RangeInfo get` | 0x06000d76 | MemberGen: `rangeInfoField; set => rangeInfoField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RangeInfo set` | 0x06000d77 | MemberGen: `rangeInfoField; set => rangeInfoField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResultInfo get` | 0x06000d78 | MemberGen: `resultInfoField; set => resultInfoField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResultInfo set` | 0x06000d79 | MemberGen: `resultInfoField; set => resultInfoField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Data get` | 0x06000d7a | MemberGen: `dataField; set => dataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Data set` | 0x06000d7b | MemberGen: `dataField; set => dataField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TraceType get` | 0x06000d7c | MemberGen: `traceTypeField; set => traceTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `TraceType set` | 0x06000d7d | MemberGen: `traceTypeField; set => traceTypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsTrace2dResultTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
