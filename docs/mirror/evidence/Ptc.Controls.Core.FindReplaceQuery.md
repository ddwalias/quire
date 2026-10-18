# Ptc.Controls.Core.FindReplaceQuery

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `FindReplaceOption get` | 0x06000690 | MemberGen: `public FindOptions FindReplaceOption { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueToFind get` | 0x06000691 | MemberGen: `public string ValueToFind { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ValueToReplaceWith get` | 0x06000692 | MemberGen: `public string ValueToReplaceWith { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FindSubscriptPositions get` | 0x06000693 | MemberGen: `public List<Pair<int, int>> FindSubscriptPositions { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReplaceSubscriptPositions get` | 0x06000694 | MemberGen: `public List<Pair<int, int>> ReplaceSubscriptPositions { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Status get` | 0x06000695 | MemberGen: `public Nullable<FindReplaceStatus> Status { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Status set` | 0x06000696 | MemberGen: `public Nullable<FindReplaceStatus> Status { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsFindContainSubscript get` | 0x06000697 | MemberGen: `public bool IsFindContainSubscript { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `FindIsFullySubscript get` | 0x06000698 | MemberGen: `public bool FindIsFullySubscript { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsReplaceContainSubscript get` | 0x06000699 | MemberGen: `public bool IsReplaceContainSubscript { get; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ReplaceIsFullySubscript get` | 0x0600069a | MemberGen: `public bool ReplaceIsFullySubscript { get; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreFindReplaceQueryMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
