# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-d3`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-nativeinterop-20261022-d3`.
- Members: C++/CLI proof and consumer; this batch makes `FileUtils.LongUnicodeToShortcutKey` an exact, loadable transplant and corrects the proof-local enumerator value-type signature mapping.
- Release IL: candidate FAIL, 390 checked / 278 exact / 168 failures (`.mirror-cache/release-il-1400f5b52bda/release-il.json`); production C# project remains unchanged.
- Remaining: 117 generated global methods lack reviewed bindings, 34 methods have no counterpart, nine are removed, and eight mapped bodies differ. REA confirmed mixed-mode metadata; Ghidra imported/analyzed the candidate, but the configured AutoAnalyze helper is missing. See `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
