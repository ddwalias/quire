# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-native-20261021`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl-native`.
- Original assemblies: utl-mgd and mcmisc.
- Selected members: unmanaged boundary audit for the complete `utl-mgd.dll` image, in addition to the previously landed managed surface.
- Release IL report: prior managed batch PASS against `main`; 286 checked; 129 exact bodies. The report's 163 `engine_native` flags are heuristic audit classifications, not verified native routines. New native inspection: 55 P/Invoke declarations, 2 IL P/Invokes, 8 runtime-provided delegate methods, and 57 analyzed non-thunk internal native procedures; no app-authored native algorithm identified. Evidence and report: `.mirror-cache/rea/utl-mgd-native-evidence.json` and `.mirror-cache/rea/utl-mgd-native-report.md`.
- Remaining mismatches: none in the checked Release IL. Native uncertainties are indirect `.nep` targets, authoritative provider names for 49 P/Invokes whose metadata scope is empty, and unexecuted Windows runtime behavior; no separate native implementation is warranted by this binary's application logic.
