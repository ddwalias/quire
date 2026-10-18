# Ptc.PersistentDataObjects.xyPlotType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x06000d61 | MemberGen: `equalscaleField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotTitle get` | 0x06000d62 | MemberGen: `plotTitleField; set => plotTitleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `plotTitle set` | 0x06000d63 | MemberGen: `plotTitleField; set => plotTitleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes get` | 0x06000d64 | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `axes set` | 0x06000d65 | MemberGen: `axesField; set => axesField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale get` | 0x06000d66 | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `equalscale set` | 0x06000d67 | MemberGen: `equalscaleField; set => equalscaleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml get` | 0x06000d68 | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `InnerXml set` | 0x06000d69 | MemberGen: `public XmlNode[] InnerXml { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsxyPlotTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
