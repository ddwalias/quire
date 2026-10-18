# Ptc.VCard.Telephone

- **Original:** `Ptc.Core.dll` 12.0.1.0, SHA-256 `d60141dbcf757ae64e7b636416c3421412d6f54e2584b318de58d5d6a6f96d56`, MVID `fc0806aa-db73-489f-8ae4-4729770faca6`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Core`; tests: `Quire.Differential.Tests.PtcVCardTelephoneMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Number get` | 0x06000681 | MemberGen: `public string Number { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Number set` | 0x06000682 | MemberGen: `public string Number { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Type get` | 0x06000683 | MemberGen: `public Telephone.Types Type { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Type set` | 0x06000684 | MemberGen: `public Telephone.Types Type { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFax get` | 0x06000685 | MemberGen: `public bool IsFax { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFax set` | 0x06000686 | MemberGen: `public bool IsFax { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsVoice get` | 0x06000687 | MemberGen: `public bool IsVoice { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsVoice set` | 0x06000688 | MemberGen: `public bool IsVoice { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcVCardTelephoneMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
