# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI proof and consumer; `LispUtils.SelfTest` and `XmlUtils.SelfTest` transplant exactly, with exception-token and enumerator-signature fixes.
- Release IL: FAIL, 390 checked / 280 exact / 166 failures (`.mirror-cache/release-il-3114cf632f65/release-il.json`); production C# project remains unchanged.
- Remaining: 117 generated globals lack reviewed bindings, 34 methods have no counterpart, nine are removed, and six mapped methods differ. REA confirmed mixed-mode metadata; Ghidra imported and analyzed native code. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
