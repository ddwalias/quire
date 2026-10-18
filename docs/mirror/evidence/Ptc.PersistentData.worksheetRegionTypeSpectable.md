# Ptc.PersistentData.worksheetRegionTypeSpectable

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor()` | 0x0600091a | MemberGen: `disablecalcField = false;` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotations get` | 0x06000916 | MemberGen: `MC14ConversionLossAnnotationsLocal` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x06000917 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x06000918 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCalculationDisabled get` | 0x06000919 | MemberGen: `disablecalc` | Mechanical; Release IL equals Mathcad's | Verified |
| `math get` | 0x0600091b | MemberGen: `mathField; set => mathField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `math set` | 0x0600091c | MemberGen: `mathField; set => mathField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc get` | 0x0600091d | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc set` | 0x0600091e | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataworksheetRegionTypeSpectableMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
