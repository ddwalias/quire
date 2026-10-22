# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI spike ports Win32-backed Utl methods and transplants exact queue, XML, FileUtils, formatter, Lisp parser, and line-partition bodies; consumer covers these paths.
- Release IL: production C# base 129/129 (prior report `.mirror-cache/release-il-a8226ea73911/release-il.json`); C++/CLI candidate FAIL, 390 checked / 268 exact / 178 failures (`.mirror-cache/release-il-98e53790a4f9/release-il.json`).
- Remaining: candidate public API is incomplete; 117 generated global methods lack reviewed bindings, 34 methods have no original counterpart, nine are removed, and 18 mapped bodies differ. Ghidra imported/analyzed the candidate; P1 export helper is unavailable. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
