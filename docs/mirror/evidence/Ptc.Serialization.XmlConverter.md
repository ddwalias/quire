# Ptc.Serialization.XmlConverter

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcSerializationXmlConverterMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `CreateInstance(ITypeDescriptorContext, IDictionary)` | 0x06000817 | MemberGen: `base.CreateInstance(context, propertyValues)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Equals(object)` | 0x06000818 | MemberGen: `base.Equals(obj)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetCreateInstanceSupported(ITypeDescriptorContext)` | 0x06000819 | MemberGen: `base.GetCreateInstanceSupported(context)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetHashCode()` | 0x0600081a | MemberGen: `base.GetHashCode()` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetProperties(ITypeDescriptorContext, object, Attribute[])` | 0x0600081b | MemberGen: `base.GetProperties(context, value, attributes)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetPropertiesSupported(ITypeDescriptorContext)` | 0x0600081c | MemberGen: `base.GetPropertiesSupported(context)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetStandardValues(ITypeDescriptorContext)` | 0x0600081d | MemberGen: `base.GetStandardValues(context)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetStandardValuesExclusive(ITypeDescriptorContext)` | 0x0600081e | MemberGen: `base.GetStandardValuesExclusive(context)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetStandardValuesSupported(ITypeDescriptorContext)` | 0x0600081f | MemberGen: `base.GetStandardValuesSupported(context)` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsValid(ITypeDescriptorContext, object)` | 0x06000820 | MemberGen: `base.IsValid(context, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ToString()` | 0x06000821 | MemberGen: `base.ToString()` | Mechanical; Release IL equals Mathcad's | Verified |
| `.ctor()` | 0x06000822 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcSerializationXmlConverterMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
