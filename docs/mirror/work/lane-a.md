# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TableCoordinate.ToString()`, `GetHashCode()`, `Equals(TableCoordinate)`, `Equals(object)`; `TableSize.ToString()`, `GetHashCode()`, `Equals(TableSize)`, and `Equals(object)` passed Release IL.
- Release IL report: `.mirror-cache/release-il-34a4f16112a5/release-il.json` — PASS, 3,058 exact, 499 unfinished.
- Remaining mismatches: `headersContainerRegionCollectionSerializer.Serialize` callback IL; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`; `DebugTools.NotCalledFromApplicationLogger` callback ordinal; `WeakMultiCastDelegate.Remove` closure allocation IL; `WpfUtils.GroupTransformsAsFrozen` and `WpfUtils.IsLoaded` compiler-generated IL shape. `IO.FileUtils.GetProductNamePerVariant` depends on missing `LicenseManager` mapping.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
