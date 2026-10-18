# Ptc.VCard.VCardContentLine

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcVCardVCardContentLineMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Name get` | 0x060006a6 | MemberGen: `public string Name { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Name set` | 0x060006a7 | MemberGen: `public string Name { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Params get` | 0x060006a8 | MemberGen: `public Dictionary<string, string> Params { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Params set` | 0x060006a9 | MemberGen: `public Dictionary<string, string> Params { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Values get` | 0x060006aa | MemberGen: `public string[] Values { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Values set` | 0x060006ab | MemberGen: `public string[] Values { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcVCardVCardContentLineMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
