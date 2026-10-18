# Ptc.PersistentDataObjects.plotType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `OverridePropertySerializationToRawXmlNode(XmlAttributeOverrides, Type, string, string, string, int)` | 0x06000c4f | MemberGen: `OverridePropertySerializationToRawXmlNode(overiddes, propertyOwnerType, oldPropertyName, newPropertyName, elementName, "http://schemas.ma...` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item get` | 0x06000c3a | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Item set` | 0x06000c3b | MemberGen: `itemField; set => itemField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `title get` | 0x06000c3c | MemberGen: `titleField; set => titleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `title set` | 0x06000c3d | MemberGen: `titleField; set => titleField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc get` | 0x06000c3e | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc set` | 0x06000c3f | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `backgroundtype get` | 0x06000c40 | MemberGen: `backgroundtypeField; set => backgroundtypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `backgroundtype set` | 0x06000c41 | MemberGen: `backgroundtypeField; set => backgroundtypeField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `originpositioning get` | 0x06000c42 | MemberGen: `originpositioningField; set => originpositioningField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `originpositioning set` | 0x06000c43 | MemberGen: `originpositioningField; set => originpositioningField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showaxisexpressions get` | 0x06000c44 | MemberGen: `showaxisexpressionsField; set => showaxisexpressionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `showaxisexpressions set` | 0x06000c45 | MemberGen: `showaxisexpressionsField; set => showaxisexpressionsField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `width get` | 0x06000c46 | MemberGen: `widthField; set => widthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `width set` | 0x06000c47 | MemberGen: `widthField; set => widthField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `widthSpecified get` | 0x06000c48 | MemberGen: `widthFieldSpecified; set => widthFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `widthSpecified set` | 0x06000c49 | MemberGen: `widthFieldSpecified; set => widthFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `height get` | 0x06000c4a | MemberGen: `heightField; set => heightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `height set` | 0x06000c4b | MemberGen: `heightField; set => heightField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `heightSpecified get` | 0x06000c4c | MemberGen: `heightFieldSpecified; set => heightFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `heightSpecified set` | 0x06000c4d | MemberGen: `heightFieldSpecified; set => heightFieldSpecified = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x06000c52 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x06000c53 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCalculationDisabled get` | 0x06000c54 | MemberGen: `disablecalc` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsplotTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
