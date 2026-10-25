# Lane N — Quire.Controls.Core

- Owner: Lane N; branch `mirror/n-controls-core-20261025-01`; worktree `C:/Projects/Quire-wt/lane-n-controls-core`.

## Batch 1 — Converters

- Scope:
  - `XmlBooleanConverter.ConvertTo`
  - `FindOptionToBooleanConverter` (`.cctor`, `Convert`, `ConvertBack`, `_allSet`, `_targetValue`)
  - `FindAndReplaceControlRemoveFormatConverter` (`_variableRegex`, `.ctor`, `ConvertBack`)
  - `WarningTypesToBooleanConverter` (`_targetValue`, `Convert`, `ConvertBack`)
  - `IdLabelToStringConverter` (`Convert(object, bool?)`, `Convert(object, Type, object, CultureInfo)`, `ConvertBack(string, bool?)`, `ConvertBack(object, Type, object, CultureInfo)`)
  - `DictionaryKeyToValueConverter` (`Instance`, `.cctor`, `Convert`)
  - `SystemIconConverter` (`Instance`, `.cctor`, `Convert`)
  - `XmlLanguageConverterWithDefaultValue` (`.ctor`, `ConvertTo`)
  - `MessageListToMessagesDecoratorConverter` (`Instance`, `.cctor`, `Convert`)
  - `GetCellValueAtColumnConverter` (`Convert`)
  - `UnitSystemsConverter` (`.ctor`, `ConvertTo`, `ConvertFrom`)
- Report: `.mirror-cache/release-il-ef561dffb403/release-il.json` — PASS, 852 checked, 852 exact, 0 failures, 3,000 unfinished.
- Remaining mismatches: none in claimed batch.
