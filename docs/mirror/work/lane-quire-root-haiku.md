# Lane Quire root (MathcadPrime.dll) — Quire

- Owner: Orca task `task_0462b3d3c4e2` (dispatched worker; no WORK.md lane row).
- Branch: `mirror/quire-root-haiku-20261119`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/quire-root-haiku`.
- Baseline (main `96bfb65f2`): 273 generated files with MirrorStub, 4942 stub lines.

## Batch 1 — BooleanToMultiThreadingValueConverter

- Members: `.ctor` (moved from the generated stub to the hand-written partial).
- Release IL: PASS (`--type Quire.BooleanToMultiThreadingValueConverter`), 742 checked, 0 failures; `Convert`, `Convert(bool)`, `ConvertBack` already exact.
- Remaining mismatches in this batch: none.

## Batch 2 — converter and rule constructors (38 types)

- Members: the default `.ctor` of CalculationStatusConverter, CaretInsertionTypeToBooleanConverter, ComplexNumberStyleToStringConverter, DocNameToTabHeaderConverter, EnumMatchToBooleanConverterWithNull, FlowDocumentFamilyConverter, FontSizeToStringConverter, FontStyleToBooleanConverter, FontVariantsToBooleanConverter, FontWeightToBooleanConverter, FormatPlotValueVisiblityConverter, FormatPlotValuesColumnConverter, FormatPlotValuesRowConverter, IncrementDecimalPlaces, IndentRule, IntToStringConverter, IsCalcNowEnabledConverter, MultiThreadingValueToBooleanConverter, NumberStyleEnablesExponentialThresholdConverter, NumberStyleEnablesRadixStyleMultiConverter, NumberStyleToStringConverter, ObjectToBoolConverter, RadixStyleToStringConverter, RadixStyleToStringMultiConverter, SizeToVisibilityMultiConverter, SplitCamelCase, StringToTextIndent, StyleFontSizeToStringConverter, StyleFontSizeValidationRule, StylePropertiesLocalized, TextDecorationCollectionToBooleanConverter, TextStyleNameToHeight, TextStyleNameToVisibility, UnitSystemsToStringConverter, WorksheetPageLayoutToRowCountConverter, FontSizeValidationRule, DipsToDisplayUnitsConverter (private), PageBackgroundColorMatchToBooleanConverter (private).
- Moved: each stub removed from `Generated/Mirror/<Type>.g.cs`; the constructor added to `Mirrored/<Type>.cs`.
- Release IL: PASS (project-wide `--project Quire`), 0 failures; each constructor is in the report's `checked` list.
- Remaining mismatches in this batch: none.

## Batch 3 — TextStyleInheritedFontWeight, PageNumberIsCheckedConverter

- Members: `TextStyleInheritedFontWeight` `Convert`, `ConvertBack`, `.ctor`; `PageNumberIsCheckedConverter` `Convert`, `ConvertBack` (returns `parameter`), `.ctor`.
- Shape note: `PageNumberIsCheckedConverter.Convert` tests `value as IPageNumberControl != null` and reads the property from `value as IPageNumberControl`; the ternary and `is`/cast forms gave `brtrue` / `castclass`, which Mathcad does not emit.
- Release IL: PASS (`--project Quire`), 746 checked, 0 failures.
- Remaining mismatches in this batch: none.

## Batch 4 — six small types

- MathFontSizeValidationRule: `get_MinFontSize` override and `.ctor` (`FontConstants.MinimumFontSizeInPoints`).
- XmlLanguageToCultureInfoConverter: `Instance` (static initializer), private `.ctor`, `Convert`, `ConvertBack`.
- OpenEngineeringDocumentParameter: `ReadonlyOptions` field, `.ctor`; TwoParametersObject: field declarations (`_parameterOne`, `_parameterTwo`).
- SavedDateIsCheckedConverter: `Convert` (`value as ISavedDateControl != null` shape), `ConvertBack`, field `DateFormatsDictionary`. Parked: `.ctor` stays a stub; Mathcad stores the dictionary before the base `Object` constructor call, which C# cannot emit.
- StyleFontFamilyValidationRule: `.ctor` only. Parked: `Validate` stays a stub; Mathcad shares one failure return and branches to the valid return with `brtrue` after a `FirstOrDefault` closure. The `if (found != null) return Valid` form and the ternary both produced `brfalse`.
- Release IL: PASS (`--project Quire`), 752 checked, 0 failures.

## Batch 5 — AliasTextBox, CultureHelper (first half of the split)

- Split: this worker keeps generated files from AboutWindow through DropDownSelector.g.cs; the second worker takes DropDownSelectorItem through Zoom.
- AliasTextBox: `.ctor`. CultureHelper: `Invariant` getter (`CultureInfo.InvariantCulture`).
- Parked, same file group: AliasTextBox `PreMouseDown` event (Interlocked add/remove) stays generated.
- Release IL: PASS (`--project Quire`), 753 checked, 0 failures.

## Batch 6 — AutomationSpiritResult (three variants)

- Members: fields `_errorCode`, `_realResult`, `_units`; getters `ErrorCodeInternal`, `ErrorCode`, `RealResult`, `Units`; `.ctor(IUnitedScalarState)`, for `AutomationSpiritResult`, `AutomationServiceEngNote.AutomationSpiritResult` and `AutomationServiceEngNotePlus.AutomationSpiritResult` (IL identical across the three).
- Release IL: PASS (`--project Quire`), 768 checked, 0 failures; all 15 new members appear in the checked list.
- Remaining mismatches in this batch: none.

## Batch 7 — AutomationSpiritOutputResultAs (three variants)

- Members: fields `_errorCode`, `_realResult`; `ErrorCodeInternal`; `.ctor(IUnitedScalarState)`.
- Parked (stubs kept): `ErrorCode` and `RealResult` getters. Mathcad's IL is `try { stloc; leave } catch { pop; leave } ldc; ret` with the catch leaving to the default return. Two forms tried: `try { return x; } catch { return -1; }` and `try { return x; } catch { } return -1;`; both emit the fall-through `ldc.i4.m1` with no `leave` after the catch, so they differ at the catch exit.
- Release IL: PASS (`--project Quire`), 779 checked, 0 failures.
