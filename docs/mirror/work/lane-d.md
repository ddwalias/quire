# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI spike covers 15 Utl types, `LabeledDictionary`, `SafeMonitor`, `UnaryVoidWrapper<T>`, and four `mcmisc` utility types; the managed consumer exercises the native synchronization and collection paths.
- Release IL: C# PASS, 129/129 (`.mirror-cache/release-il-3d280dfb40eb/release-il.json`); C++/CLI candidate FAIL, 268 checked / 158 exact / 295 failures (`.mirror-cache/release-il-516e1b099f97/release-il.json`).
- Remaining: 143 methods removed, 117 generated global methods and 30 other methods unbound, and five mapped method differences. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
