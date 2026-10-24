# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `TextStyleBase.StyleUniqueKeySign`; `TextStyleBase.FontSizeRoundingDecimal` (prior batches are in git history).
- Release IL: `.mirror-cache/release-il-0b7099cb2f4c/release-il.json` — PASS, 3,585 checked, 166 unfinished.
- Remaining: 166 unfinished. Known compiler-lowering blockers include callback ordinals/null-conditional lowering in the region serializers and Wpf helpers, plus IL-shape differences in `MessageBoxTraceListener.Fail`, `DataObjectExtensions.GetTextWithEmbeddedItemsInfo`, `AppVersionProvider.LoadVersionData`, `BooleansToVisibilityMultiConverter.Convert`, and `ImageUtils.DumpBounds`. Missing license type mappings block dependent members.
