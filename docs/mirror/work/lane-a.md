# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Exact members: `Wpf.RichTextBoxExtensions.Satisfy(TextElement|Block|Inline)`; `Xml.AppVersionProvider.LoadVersionData`; `TextElementAttachedProperties.SetTextStyleOnElement`; `Wpf.WpfUtils.DumpLogicalTreeLines`.
- Release IL: `.mirror-cache/release-il-7a491645b22c/release-il.json` — PASS, 3,668 exact, 135 unfinished.
- Remaining: 135 Core members. Licensing members require reviewed `Ptc.License` bindings. Other exact-IL blockers recorded from attempts: `TextRangeExtension.EnumerateRunsInRange` local/branch lowering; `ImageUtils.DumpBounds` value evaluation order; `EncodeImage` `String.get_Length` callvirt vs call; footer lambda null-check lowering; converter local initialization and branch direction. `TextFormat.Initialize` helpers land at ordinal 77; Mathcad uses 32.
