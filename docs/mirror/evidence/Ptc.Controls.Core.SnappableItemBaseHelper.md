# Ptc.Controls.Core.SnappableItemBaseHelper

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsClearImageEnabled get` | 0x060006ad | MemberGen: `public static bool IsClearImageEnabled { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsClearImageEnabled set` | 0x060006ae | MemberGen: `public static bool IsClearImageEnabled { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsPageDescendant get` | 0x060006b0 | MemberGen: `public bool IsPageDescendant { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsPageDescendant set` | 0x060006b1 | MemberGen: `public bool IsPageDescendant { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemElement get` | 0x060006b7 | MemberGen: `public UIElement ItemElement { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ItemElement set` | 0x060006b8 | MemberGen: `public UIElement ItemElement { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreSnappableItemBaseHelperMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
