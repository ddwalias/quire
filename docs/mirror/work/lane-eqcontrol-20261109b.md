# EquationControl lane checkpoint

- Owner: dispatched EquationControl worker
- Branch: `mirror/eqcontrol-20261109b`
- Batch: `Controls.InputControls.StringCompressor::{CompressString, DecompressString}`
- Shapes tried: direct `if` equality and inverted `if` branches differed in branch opcodes; enum `switch` matched both methods.
- Release IL: PASS, 614 checked (`.mirror-cache/release-il-2eedc1c08c83/release-il.json`). Earlier candidate reports: FAIL at `ae31463cd9e7` and `01f85df6906e` while trying the two `if` shapes.
- Batch: `Controls.InputControls.DoubleRule::{Validate, .ctor}`, `UDoubleRule::{Validate, .ctor}`.
- Shapes tried: `string.Empty` differed from the required empty string literal; `value == null ? null : value.ToString()` differed in branch direction; reversed ternary predicate matched.
- Release IL: PASS, 616 checked (`.mirror-cache/release-il-e9752a832884/release-il.json`). Earlier candidate reports: FAIL at `d7dcf40a63da` (empty-string field), `d252be5d7941` (UDoubleRule null branch).
- Batch: `Controls.InputControls.LocBamlHelper::GetValues`.
- Shapes tried: direct null comparison and enum check matched.
- Release IL: PASS, 617 checked (`.mirror-cache/release-il-faa1d717a424/release-il.json`).
- Remaining: all other stub-marked members in EquationControl; 5,374 markers across 313 types after this batch.
