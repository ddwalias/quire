# Lane D — Quire.Engine.Utl

- Owner: Lane D; branch `mirror/d-engine-utl-nativeinterop-20261022-f5`.
- Members: Windows C++/CLI mixed-mode proof; `Logging.Log` and `Logging.LogLine` now transplant exact Release IL.
- Release IL: strict VCX gate FAIL, 548 checked / 291 exact / 308 failures; `.mirror-cache/release-il-lane-d-final/release-il.json` (base `987703a6`, tip `ce913339`).
- Remaining mismatches: 267 global `<Module>` methods without reviewed bindings, 37 methods without Mathcad counterparts, three reported removals, and one `SafeMutex` IL difference. Production C# project remains unchanged.
- Implementation and REA/Ghidra/build details: [mixed-mode implementation note](../native/utl-mgd-mixed-mode-implementation.md).
