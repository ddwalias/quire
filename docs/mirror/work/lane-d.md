# Lane D — Quire.Engine.Utl

- Owner: Lane D; branch `mirror/d-engine-utl-parity-20261022-h2`.
- Members: Windows C++/CLI mixed-mode proof and unmanaged logging export; production C# project unchanged.
- Release IL: VCX gate **FAIL**, 631 checked / 292 exact / 396 failures; `.mirror-cache/release-il-netmodule-methodfocus-v14/release-il.json` (base `314e37fa`, tip `0b65b4dc`). No mapped body differences; the report fails on C++/CLI support methods and metadata forms.
- Remaining mismatches: 355 generated global methods and 32 C++/CLI/CRT members without Mathcad bindings; see [implementation note](../native/utl-mgd-mixed-mode-implementation.md).
