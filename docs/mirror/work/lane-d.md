# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI spike covers 15 Utl types plus `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`; managed consumer runs those plus the Win32 handle and label helpers.
- Release IL: C# PASS, 129/129 (`.mirror-cache/release-il-d267b9469c99/release-il.json`); C++/CLI candidate FAIL, 216 checked / 120 exact / 279 failures (`.mirror-cache/release-il-e88cb8b74b76/release-il.json`).
- Remaining: 177 methods removed, 87 generated global methods and 26 other methods unbound, six mapped methods differ. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
