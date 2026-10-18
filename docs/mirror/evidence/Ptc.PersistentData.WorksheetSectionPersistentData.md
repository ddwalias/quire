# Ptc.PersistentData.WorksheetSectionPersistentData

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SerializedRegions get` | 0x06000082 | MemberGen: `public IList<IRegionPersistentData> SerializedRegions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SerializedRegions set` | 0x06000083 | MemberGen: `public IList<IRegionPersistentData> SerializedRegions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RegionsToSerialize get` | 0x06000084 | MemberGen: `public IDictionary<UIElement, Point> RegionsToSerialize { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RegionsToSerialize set` | 0x06000085 | MemberGen: `public IDictionary<UIElement, Point> RegionsToSerialize { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ConvertedFromMC14 get` | 0x06000086 | MemberGen: `public bool ConvertedFromMC14 { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ConvertedFromMC14 set` | 0x06000087 | MemberGen: `public bool ConvertedFromMC14 { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MsgId get` | 0x06000088 | MemberGen: `public worksheetMessageIdType MsgId { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MsgId set` | 0x06000089 | MemberGen: `public worksheetMessageIdType MsgId { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataWorksheetSectionPersistentDataMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
