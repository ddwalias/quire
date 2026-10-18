# Ptc.Controls.Core.VisualDragger

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreVisualDraggerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `OnContextMenuOpening(ContextMenuEventArgs)` | 0x060007b6 | MemberGen: `e.Handled = true` | Mechanical; Release IL equals Mathcad's | Verified |
| `ResetDrag(Point)` | 0x060007c6 | MemberGen: `DragStartPoint = point` | Mechanical; Release IL equals Mathcad's | Verified |
| `DragStartPoint get` | 0x060007c3 | MemberGen: `public Point DragStartPoint { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `DragStartPoint set` | 0x060007c4 | MemberGen: `public Point DragStartPoint { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreVisualDraggerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
