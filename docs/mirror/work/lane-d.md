# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-vcxproj-gate-20261022-c2`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-vcxproj-gate-20261022-c2`.
- Members: `SafeWaitHandle`, `SafeEvent`, `SafeMutex` C++/CLI port spike; managed consumer proof.
- Release IL: C# project PASS, 129/129 exact; `.vcxproj` candidate FAIL, 136 checked / 428 mismatches. Report: `.mirror-cache/release-il-b8f81404f448/release-il.json`.
- Remaining: three-type spike omits 292 methods; 18 shared signatures have 0 exact IL; 87 global and 49 unbound methods lack Mathcad bindings. Full details in `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
