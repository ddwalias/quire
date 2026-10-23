# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TableCoordinate.ToString()`, `TableCoordinate.GetHashCode()`, `TableCoordinate.Equals(TableCoordinate)`, `TableSize.ToString()`, and `TableSize.GetHashCode()` passed Release IL.
- Release IL report: `.mirror-cache/release-il-1468de821cb2/release-il.json` — PASS, 3,055 exact, 502 unfinished.
- Remaining mismatches: `headersContainerRegionCollectionSerializer.Serialize` callback IL; `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`; `DebugTools.NotCalledFromApplicationLogger` callback ordinal; `WeakMultiCastDelegate.Remove` closure allocation IL; `WpfUtils.GroupTransformsAsFrozen` and `WpfUtils.IsLoaded` compiler-generated IL shape. `IO.FileUtils.GetProductNamePerVariant` depends on missing `LicenseManager` mapping.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
