# Ptc.PersistentDataObjects.includeWorksheetType

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `error get` | 0x06000b88 | MemberGen: `errorField; set => errorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `error set` | 0x06000b89 | MemberGen: `errorField; set => errorField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `warning get` | 0x06000b8a | MemberGen: `warningField; set => warningField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `warning set` | 0x06000b8b | MemberGen: `warningField; set => warningField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `href get` | 0x06000b8c | MemberGen: `hrefField; set => hrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `href set` | 0x06000b8d | MemberGen: `hrefField; set => hrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `lastsaveddate get` | 0x06000b8e | MemberGen: `lastsaveddateField; set => lastsaveddateField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `lastsaveddate set` | 0x06000b8f | MemberGen: `lastsaveddateField; set => lastsaveddateField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `usedcache get` | 0x06000b90 | MemberGen: `usedcacheField; set => usedcacheField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `usedcache set` | 0x06000b91 | MemberGen: `usedcacheField; set => usedcacheField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc get` | 0x06000b92 | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `disablecalc set` | 0x06000b93 | MemberGen: `disablecalcField; set => disablecalcField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref get` | 0x06000b94 | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `itemidref set` | 0x06000b95 | MemberGen: `itemidrefField; set => itemidrefField = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotations get` | 0x06000b96 | MemberGen: `MC14ConversionLossAnnotationsLocal` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal get` | 0x06000b97 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MC14ConversionLossAnnotationsLocal set` | 0x06000b98 | MemberGen: `public List<string> MC14ConversionLossAnnotationsLocal { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCalculationDisabled get` | 0x06000b99 | MemberGen: `disablecalc` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcPersistentDataObjectsincludeWorksheetTypeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
