# Ptc.IO.UrlUtils

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcIOUrlUtilsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `UriProtocols: Contains(string)` | 0x060013a9 | MemberGen: `Protocols.Contains(element)` | Mechanical; Release IL equals Mathcad's | Verified |
| `UriProtocolCreationData: .ctor(string, Regex)` | 0x060013ab | MemberGen: `SchemeWithDelimiter = schemeWithDelimiter; MatchPattern = matchPattern;` | Mechanical; Release IL equals Mathcad's | Verified |
| `UriProtocolCreationData: MatchPattern get` | 0x060013ac | MemberGen: `public Regex MatchPattern { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `UriProtocolCreationData: SchemeWithDelimiter get` | 0x060013ad | MemberGen: `public string SchemeWithDelimiter { get; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcIOUrlUtilsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
