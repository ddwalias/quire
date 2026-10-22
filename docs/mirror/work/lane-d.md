# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI spike adds FileUtils Unicode support, exact-transplants the TaskQueue constructor, MessageQueue.Init/MsgLoop/finalizer/destructor and LispUtils.IndentTo, with managed-consumer coverage.
- Release IL: production C# PASS, 129/129 (`.mirror-cache/release-il-a8226ea73911/release-il.json`); C++/CLI candidate FAIL, 390 checked / 248 exact / 200 failures (`.mirror-cache/release-il-8ebd9c1cfae6/release-il.json`).
- Remaining: candidate public API is incomplete; 117 generated global methods lack reviewed bindings, 34 methods have no original counterpart, 11 are removed, and mapped bodies still differ. Ghidra imported the candidate, but P1 export is blocked by missing `AutoAnalyze.java`. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
