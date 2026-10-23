# Lane D — Quire.Engine.Utl

- Owner: Lane D; branch `mirror/d-engine-utl-cppcli-honest-20261023-h1`; worktree `C:/Projects/Quire-wt/lane-d-utl-cppcli-honest`.
- Members: utl-mgd only, honest C++/CLI port (D19). The production build does not use the netmodule, `Engine.Misc` or IlMaxStack/BodyTransplant proofs. The Build Tools MSBuild builds `MixedMode/Quire.Engine.Utl.vcxproj` under `$(QuireMsvcInstallDir)` with `/std:c++17` and `/OPT:REF` (both needed to match utl-mgd).
- Release IL: `release_il.py --base main --project Quire.Engine.Utl`: **PASS**, 0 failures, 0 unfinished, 0 toolset_support, 55 native_pairs. Report `.mirror-cache/release-il-886fd0230e64/release-il.json`. Uncommitted.
- Remaining mismatches: none.
- Dependents (Engine, Mcd, Efi, Engine.Native) get the pinned MSVC through `release_il.py`'s `builds_cppcli`; `--project Quire.Engine` PASS.
