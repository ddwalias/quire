# Ptc.Controls.Core.DocumentAttachedProperties

- **Original:** `Ptc.Controls.Core.dll` 12.0.1.0, SHA-256 `6b4b032634fa4c3f3718f42de0d769248c787dff9c37bf39abe21b09a16b2f61`, MVID `12616361-9bfc-40b5-82d2-bb6f4644e027`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Controls.Core`; tests: `Quire.Differential.Tests.PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `SetDocCalculationOptions(DependencyObject, CalculationOptions)` | 0x060001d0 | MemberGen: `o.SetValue(DocCalculationOptionsProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetDocCalculationVariables(DependencyObject, CalculationVariables)` | 0x060001d2 | MemberGen: `o.SetValue(DocCalculationVariablesProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetInputOutputRegionManager(DependencyObject, IIORegionManager)` | 0x060001de | MemberGen: `region.SetValue(InputOutputRegionManagerProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetBookmarksManager(DependencyObject, IBookmarksManager)` | 0x060001e0 | MemberGen: `region.SetValue(BookmarksManagerProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetLabel(DependencyObject, string)` | 0x060001e2 | MemberGen: `dependencyObject.SetValue(LabelProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `SetFilePath(DependencyObject, string)` | 0x060001e5 | MemberGen: `o.SetValue(FilePathProperty, value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClearIsInPrinting(DependencyObject)` | 0x060001e8 | MemberGen: `o.ClearValue(IsInPrintingProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClearIsInCopy(DependencyObject)` | 0x060001eb | MemberGen: `o.ClearValue(IsInCopyProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClearIsInPasting(DependencyObject)` | 0x060001ee | MemberGen: `o.ClearValue(IsInPastingProperty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `ClearCopyAsImageSettings(DependencyObject)` | 0x060001f1 | MemberGen: `target.ClearValue(CopyAsImageSettingsProperty)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.
