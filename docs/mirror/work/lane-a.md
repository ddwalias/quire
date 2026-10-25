# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Exact members: `Wpf.RichTextBoxExtensions.Satisfy(TextElement|Block|Inline)`; `Xml.AppVersionProvider.LoadVersionData`; `TextElementAttachedProperties.SetTextStyleOnElement`.
- Release IL: `.mirror-cache/release-il-595daf7fd972/release-il.json` — PASS, 3,665 exact, 136 unfinished.
- Remaining: 136 Core members, including the three recursive-clear overloads; their `Object.GetType` call differs (`callvirt` vs Mathcad `call`).
