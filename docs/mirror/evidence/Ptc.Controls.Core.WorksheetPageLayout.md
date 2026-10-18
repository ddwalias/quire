# Ptc.Controls.Core.WorksheetPageLayout

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `WideMargin(Thickness)` | 0x06000890 | MemberGen: `ScaledMargin(startingMargin, 2.0, 2.0)` | Mechanical; Release IL equals Mathcad's | Verified |
| `NarrowMargin(Thickness)` | 0x06000891 | MemberGen: `ScaledMargin(startingMargin, 0.5, 0.5)` | Mechanical; Release IL equals Mathcad's | Verified |
| `MetricMajorGridLines get` | 0x06000887 | MemberGen: `WpfUtils.CmToDips(2.0)` | Mechanical; Release IL equals Mathcad's | Verified |
| `USMajorGridLines get` | 0x06000888 | MemberGen: `WpfUtils.InchesToDips(1.0)` | Mechanical; Release IL equals Mathcad's | Verified |
| `USFooterMargin get` | 0x0600088d | MemberGen: `USHeaderMargin` | Mechanical; Release IL equals Mathcad's | Verified |
| `MetricFooterMargin get` | 0x0600088f | MemberGen: `MetricHeaderMargin` | Mechanical; Release IL equals Mathcad's | Verified |
| `GridCellSizeChanged add` | 0x06000830 | MemberGen: `public event Action<Size, Size> GridCellSizeChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GridCellSizeChanged remove` | 0x06000831 | MemberGen: `public event Action<Size, Size> GridCellSizeChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeaderFooterGridCellSizeChanged add` | 0x06000832 | MemberGen: `public event Action<Size, Size> HeaderFooterGridCellSizeChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `HeaderFooterGridCellSizeChanged remove` | 0x06000833 | MemberGen: `public event Action<Size, Size> HeaderFooterGridCellSizeChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetRecreateSuggested add` | 0x06000834 | MemberGen: `public event Action WorksheetRecreateSuggested;` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetRecreateSuggested remove` | 0x06000835 | MemberGen: `public event Action WorksheetRecreateSuggested;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged add` | 0x0600087b | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |
| `PropertyChanged remove` | 0x0600087c | MemberGen: `public event PropertyChangedEventHandler PropertyChanged;` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreWorksheetPageLayoutMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
