# Ptc.Controls.DimensionSelectorButton

- **Original:** `Ptc.Controls.dll` 12.0.1.0, SHA-256 `d0a6d16f9be02820ac6db8b18c8f9999ba6b98588bad309d7ee25c85752e3e9a`, MVID `3078875f-74af-4583-9090-ac2777a6a95e`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls`; tests: `Quire.Differential.Tests.PtcControlsDimensionSelectorButtonMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `IsSelected get` | 0x06000038 | MemberGen: `GetIsSelected(this); set => SetIsSelected(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `IsSelected set` | 0x06000039 | MemberGen: `GetIsSelected(this); set => SetIsSelected(this, value); }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Content get` | 0x06000048 | MemberGen: `_child` | Mechanical; Release IL equals Mathcad's | Verified |
| `MouseOverCell add` | 0x06000033 | MemberGen: `public event MouseOverCellHandler MouseOverCell;` | Mechanical; Release IL equals Mathcad's | Verified |
| `MouseOverCell remove` | 0x06000034 | MemberGen: `public event MouseOverCellHandler MouseOverCell;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Commit add` | 0x06000035 | MemberGen: `public event Delegate0 Commit;` | Mechanical; Release IL equals Mathcad's | Verified |
| `Commit remove` | 0x06000036 | MemberGen: `public event Delegate0 Commit;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsDimensionSelectorButtonMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
