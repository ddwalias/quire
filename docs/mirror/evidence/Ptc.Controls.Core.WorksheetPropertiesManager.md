# Ptc.Controls.Core.WorksheetPropertiesManager

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SetWorksheetPropertiesSource(DependencyObject, IWorksheetPropertiesSource)` | 0x060008fb | MemberGen: `dObj.SetValue(WorksheetPropertiesSourcePropertyKey, propertiesSource)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetWorksheetPropertiesChangeInfo(DependencyObject, WorksheetPropertiesChangeInfo)` | 0x060008fd | MemberGen: `dObj.SetValue(WorksheetPropertiesChangeInfoKey, newInfo)` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetPropertiesChangeInfo: WorksheetFrameworkElement get` | 0x06001074 | MemberGen: `public FrameworkElement WorksheetFrameworkElement { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WorksheetPropertiesChangeInfo: WorksheetFrameworkElement set` | 0x06001075 | MemberGen: `public FrameworkElement WorksheetFrameworkElement { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
