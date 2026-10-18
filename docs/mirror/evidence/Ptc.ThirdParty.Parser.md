# Ptc.ThirdParty.Parser

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcThirdPartyParserMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `NullErrorReporter(string)` | 0x06000720 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValidValues(ArgumentAttribute, FieldInfo)` | 0x06000738 | MemberGen: `attribute.ValidValues` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsCollectionType(Type)` | 0x0600073f | MemberGen: `type.IsArray` | Mechanical; Release IL equals Mathcad's | Verified |
| `HelpArgument: .ctor()` | 0x06001328 | MemberGen: `{ }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ArgumentHelpStrings: .ctor(string, string)` | 0x06001329 | MemberGen: `this.syntax = syntax; this.help = help;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: LongName get` | 0x06001335 | MemberGen: `longName` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: ExplicitShortName get` | 0x06001336 | MemberGen: `explicitShortName` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: ShortName get` | 0x06001337 | MemberGen: `shortName` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: HasHelpText get` | 0x0600133a | MemberGen: `hasHelpText` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: HelpText get` | 0x0600133b | MemberGen: `helpText` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: DefaultValue get` | 0x0600133c | MemberGen: `defaultValue` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: SeenValue get` | 0x06001341 | MemberGen: `seenValue` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: Type get` | 0x06001344 | MemberGen: `fieldInfo.FieldType` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: IsDefault get` | 0x06001346 | MemberGen: `isDefault` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: ValidValues get` | 0x06001347 | MemberGen: `validValues; set => validValues = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Argument: ValidValues set` | 0x06001348 | MemberGen: `validValues; set => validValues = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcThirdPartyParserMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
