# Ptc.PersistentDataObjects.domainBaseType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000c6f | MemberGen: `allownegativevaluesField = true;` | Mechanical; Release IL equals Mathcad's | Verified |
| `startValue get` | 0x06000c70 | MemberGen: `startValueField; set => startValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `startValue set` | 0x06000c71 | MemberGen: `startValueField; set => startValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `secondTickValue get` | 0x06000c72 | MemberGen: `secondTickValueField; set => secondTickValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `secondTickValue set` | 0x06000c73 | MemberGen: `secondTickValueField; set => secondTickValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `endValue get` | 0x06000c74 | MemberGen: `endValueField; set => endValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `endValue set` | 0x06000c75 | MemberGen: `endValueField; set => endValueField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `scaletype get` | 0x06000c76 | MemberGen: `scaletypeField; set => scaletypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `scaletype set` | 0x06000c77 | MemberGen: `scaletypeField; set => scaletypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `allownegativevalues get` | 0x06000c78 | MemberGen: `allownegativevaluesField; set => allownegativevaluesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `allownegativevalues set` | 0x06000c79 | MemberGen: `allownegativevaluesField; set => allownegativevaluesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `allownegativevaluesSpecified get` | 0x06000c7a | MemberGen: `allownegativevaluesFieldSpecified; set => allownegativevaluesFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `allownegativevaluesSpecified set` | 0x06000c7b | MemberGen: `allownegativevaluesFieldSpecified; set => allownegativevaluesFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `autoscale get` | 0x06000c7c | MemberGen: `autoscaleField; set => autoscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `autoscale set` | 0x06000c7d | MemberGen: `autoscaleField; set => autoscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsdomainBaseTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
