# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-nativeinterop-20261022-f5`.
- Members: C++/CLI mixed-mode proof and managed consumers; production C# project remains unchanged.
- Release IL: FAIL, 390 checked / 280 exact / 166 failures (`.mirror-cache/release-il-d0daa6350eca/release-il.json`).
- Remaining: 117 generated globals lack reviewed bindings, 34 methods have no original counterpart, nine methods are removed, and six mapped methods differ. Mixed-mode proof loads and runs on Windows; see `docs/mirror/native/utl-mgd-mixed-mode-implementation.md`.
