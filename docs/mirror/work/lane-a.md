# Lane A — Quire.Core

- Owner: Lane A; branch `mirror/a-core-20261021-quirecore`; worktree `C:/Projects/Quire-wt/lane-a-core`.
- Members: `Wpf.RichTextBoxExtensions.Satisfy(TextElement|Block|Inline)` implemented exactly; recursive-clear overloads remain unmarked placeholders in Mathcad order.
- Release IL: `.mirror-cache/release-il-e13df53bfb98/release-il.json` — PASS, 3,663 exact, 138 unfinished.
- Remaining: 138 unfinished Core members; recursive-clear overloads are among them. Their direct ternary form differed only at `Object.GetType` (`callvirt` vs Mathcad `call`), so they remain placeholders pending an exact lowering.
