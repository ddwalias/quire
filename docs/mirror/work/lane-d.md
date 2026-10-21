# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-mixedmode-20261022-a1`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-mixedmode-20261022-a1`.
- Members: C++/CLI consumer proof; .NET 10 C# netmodule retention spike.
- Release IL: PASS against `origin/main` (`782167ec`), 129 checked / 129 exact / 0 failures. Report: `.mirror-cache/release-il-7c28da18c9ea/release-il.json`.
- Remaining: module retention fails on .NET 10 (`FileLoadException 0x8013101E`); production remains C# to preserve exact IL. Full details in `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
