# Lane A — Quire.Core

- Scope: all managed members in `src/Quire.Core`, including former C/D tier members (D18).
- Status: active; passing batches are being landed.
- Owner: Lane A.
- Branch: `mirror/a-core-20261021-quirecore`; worktree: `C:/Projects/Quire-wt/lane-a-core`.
- Selected members: 346 accepted mechanical methods (+63 fields), 466 manually reconstructed methods (+79 fields), and 659 abstract interface declarations across Core, WPF, serialization, undo, quality agent, service contracts, WPF COM sinks, and XPS print interfaces; 12 mechanical candidates rejected by MemberGen. Latest members: `ApplicationLogger.SubProduct`, `FullProductName`, and `LogFolder` getters.
- Release IL report: `.mirror-cache/release-il-4a8c21c789a4/release-il.json` — PASS, 2,799 exact, 0 mismatches; Quire.Core has 723 unfinished methods.
- Remaining mismatches: 723 unfinished methods in the full Quire.Core inventory.

Use only the [five steps](../PLAN.md) and the [launch prompt](agent-prompts.md).
