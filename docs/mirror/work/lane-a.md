# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Exact members: `Wpf.RichTextBoxExtensions.Satisfy(TextElement|Block|Inline)`; `Xml.AppVersionProvider.LoadVersionData`.
- Release IL: `.mirror-cache/release-il-8b8a52bb6902/release-il.json` — PASS, 3,664 exact, 137 unfinished.
- Remaining: 137 Core members, including the three recursive-clear overloads; their `Object.GetType` call differs (`callvirt` vs Mathcad `call`).
