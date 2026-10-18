# Ptc.PersistentDataObjects.valueType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000a8f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef get` | 0x06000a87 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRef set` | 0x06000a88 | MemberGen: `resultRefField; set => resultRefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified get` | 0x06000a89 | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `resultRefSpecified set` | 0x06000a8a | MemberGen: `resultRefFieldSpecified; set => resultRefFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml get` | 0x06000a8b | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml set` | 0x06000a8c | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EquationSExpression get` | 0x06000a8d | MemberGen: `public string[] EquationSExpression { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `EquationSExpression set` | 0x06000a8e | MemberGen: `public string[] EquationSExpression { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsvalueTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
