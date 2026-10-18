# Ptc.PersistentDataObjects.polarPlotType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d2f | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotTitle get` | 0x06000d29 | MemberGen: `plotTitleField; set => plotTitleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotTitle set` | 0x06000d2a | MemberGen: `plotTitleField; set => plotTitleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes get` | 0x06000d2b | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes set` | 0x06000d2c | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml get` | 0x06000d2d | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml set` | 0x06000d2e | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectspolarPlotTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
