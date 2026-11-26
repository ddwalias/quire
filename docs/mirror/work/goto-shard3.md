# Goto cleanup shard 3

- Owner: Orca task `task_d9e651f38c11`
- Branch: `mirror/goto-shard3`
- Members: `CustomScriptRunBase.CoerceCustomScriptFontSizeCallback`; `CustomScriptServiceFunctions.ConvertScriptLevelInternal`; `OleContainerModel.SetVisibleSize`; `OleContainerViewModel.ExecutedOleResetSize` lambda; `SymbolDescriptorFilter.FilterExcludingCategoryKey`; `LispUtils.SelfTest`
- Release IL: `Quire.Controls.Core` PASS (3,379 checked, `release-il-492b7c384fec/release-il.json`); `Quire.Controls.EquationControl` PASS (4,068 checked, `release-il-51ad87163a4b/release-il.json`); `Quire.Engine.Misc` PASS (131 checked, `release-il-da9fe98dac16/release-il.json`)
- Remaining mismatches: none. Seven goto sites remain with one-line Release IL comments after idiomatic rewrites failed comparison; two `CustomScriptRunBase` goto sites were removed.
