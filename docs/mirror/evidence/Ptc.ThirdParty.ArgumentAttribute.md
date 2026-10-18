# Ptc.ThirdParty.ArgumentAttribute

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcThirdPartyArgumentAttributeMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Type get` | 0x06000708 | MemberGen: `type` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShortName get` | 0x0600070a | MemberGen: `shortName; set => shortName = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ShortName set` | 0x0600070b | MemberGen: `shortName; set => shortName = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `LongName get` | 0x0600070d | MemberGen: `longName; set => longName = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `LongName set` | 0x0600070e | MemberGen: `longName; set => longName = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DefaultValue get` | 0x0600070f | MemberGen: `defaultValue; set => defaultValue = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DefaultValue set` | 0x06000710 | MemberGen: `defaultValue; set => defaultValue = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HelpText get` | 0x06000713 | MemberGen: `helpText; set => helpText = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `HelpText set` | 0x06000714 | MemberGen: `helpText; set => helpText = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValidValues get` | 0x06000715 | MemberGen: `validValues; set => validValues = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValidValues set` | 0x06000716 | MemberGen: `validValues; set => validValues = value; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcThirdPartyArgumentAttributeMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
