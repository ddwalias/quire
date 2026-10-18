# Ptc.Controls.Core.Serialization.McdxDeserializer

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `TextStyleChangedHandler get` | 0x06000b10 | MemberGen: `_textStyleChangedHandler` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializedRegions get` | 0x06000b16 | MemberGen: `public IList<IRegionPersistentData> DeserializedRegions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DeserializedRegions set` | 0x06000b17 | MemberGen: `public IList<IRegionPersistentData> DeserializedRegions { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SkipRegionsNotAffectingCalculation get` | 0x06000b18 | MemberGen: `public bool SkipRegionsNotAffectingCalculation { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `SkipRegionsNotAffectingCalculation set` | 0x06000b19 | MemberGen: `public bool SkipRegionsNotAffectingCalculation { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `PackagePart get` | 0x06000b1c | MemberGen: `_packagePart` | Mechanical; Release IL equals Mathcad's | Verified |
| `Helper get` | 0x06000b1d | MemberGen: `_serializationHelper` | Mechanical; Release IL equals Mathcad's | Verified |
| `NameSpaceManager get` | 0x06000b1e | MemberGen: `XmlWorksheetData.McdxNameSpaceManager` | Mechanical; Release IL equals Mathcad's | Verified |
| `XmlContentDocument get` | 0x06000b1f | MemberGen: `_xmlContentDocument` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseOverrides get` | 0x06000b20 | MemberGen: `public bool UseOverrides { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UseOverrides set` | 0x06000b21 | MemberGen: `public bool UseOverrides { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FilePath get` | 0x06000b22 | MemberGen: `_filePath` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSerializationMcdxDeserializerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
