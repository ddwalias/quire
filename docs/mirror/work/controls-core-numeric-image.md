# Controls Core, Numeric and Image

- Owner: sub-lane "Controls Core, Numeric and Image" (task_31558eb4d965); branch `mirror/controls-core-numeric-image-20261114`.
- Types: `WorksheetPageLayout`, `Serialization.NumberFormatSerializer`, `NumberFormat`, `ImageControl`, `NumericUpDown`, `IORegions.IOItemsAliasManager`, `Serialization.McdxDeserializer`, `CommonDragger`.

## Batch 1 — IOItemsAliasManager

- Landed members: ctor, `GetItem`, `Add`, `Remove`, `IsExist`, `SetInputAliases`, `SetOutputAliases`, `SetIOAliasMap`, `GetIOAliasMap`, `ClearIOAliasMap`, `GetAliases`, `SetAliases`, `FindSingleCaseInsensitiveAliasMatch`, `HandleInAliassesPropertyChanged`, `HandleOutAliassesPropertyChanged`, fields/events.
- Parked (stubs): `DistinctIds` (closure ordinal `b__10_*` needs one fewer member ahead of it than the generated stub leaves; the generated partial compiles first), `GetSExpressionsList` (Release IL returns the new empty list through a temporary `IList<string>` local; direct return and declared local both compile to `newobj; ret`).
- Report: full `--project Quire.Controls.Core` gate PASS, 2477 checked, 0 failures.
- Stub count left in this type: 2.
