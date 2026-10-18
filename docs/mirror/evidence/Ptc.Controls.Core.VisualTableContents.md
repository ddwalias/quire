# Ptc.Controls.Core.VisualTableContents

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreVisualTableContentsMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetRowHeight(int)` | 0x060007e5 | MemberGen: `_layoutManager.GetRowHeight(row)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetColumnWidth(int)` | 0x060007e6 | MemberGen: `_layoutManager.GetColumnWidth(column)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns get` | 0x060007d8 | MemberGen: `_layoutManager.Columns; set => _layoutManager.Columns = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Columns set` | 0x060007d9 | MemberGen: `_layoutManager.Columns; set => _layoutManager.Columns = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ColumnAlignment get` | 0x060007da | MemberGen: `public ColumnAlignments ColumnAlignment { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ColumnAlignment set` | 0x060007db | MemberGen: `public ColumnAlignments ColumnAlignment { get; set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ColumnSpacing get` | 0x060007dc | MemberGen: `_layoutManager.PaddingX; set => _layoutManager.PaddingX = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `ColumnSpacing set` | 0x060007dd | MemberGen: `_layoutManager.PaddingX; set => _layoutManager.PaddingX = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RowSpacing get` | 0x060007de | MemberGen: `_layoutManager.PaddingY; set => _layoutManager.PaddingY = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `RowSpacing set` | 0x060007df | MemberGen: `_layoutManager.PaddingY; set => _layoutManager.PaddingY = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Padding get` | 0x060007e0 | MemberGen: `_layoutManager.Padding; set => _layoutManager.Padding = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Padding set` | 0x060007e1 | MemberGen: `_layoutManager.Padding; set => _layoutManager.Padding = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaselineToMidPointAdjustment get` | 0x060007e2 | MemberGen: `_layoutManager.BaselineToMidPointAdjustment; set => _layoutManager.BaselineToMidPointAdjustment = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `BaselineToMidPointAdjustment set` | 0x060007e3 | MemberGen: `_layoutManager.BaselineToMidPointAdjustment; set => _layoutManager.BaselineToMidPointAdjustment = value; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `MatrixBaseline get` | 0x060007e4 | MemberGen: `_layoutManager.MatrixBaseline` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreVisualTableContentsMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
