# Lane Quire.Mpl.Modules Checkpoint

- Owner: Lane Quire.Mpl.Modules (task `task_5609601cd4c4`). Branch `mirror/mpl-modules-20261116` from `main` `b1ec103b7`. Worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-quire-mpl-modules`.
- Target: `src/Quire.Mpl.Modules` (D2 host project hosting 15 Mpl assemblies: `Hashtable`, `McdRun`, `McdStaticUnitSystem`, `McdStaticUnitSystemCGS`, `McdStaticUnitSystemMKS`, `McdStaticUnitSystemNone`, `McdStaticUnitSystemSI`, `McdStaticUnitSystemUS`, `Mpl`, `MplConstants`, `MplMacro`, `MplPrelude`, `MplPrimitives`, `MplSexpr`, `MplUnitSystemSI`).

## Release IL Report

- `release_il.py --base main --project Quire.Mpl.Modules`: PASS; 0 checked; 0 failures; 530 stubs skipped (not implemented), 119 Mathcad members without Quire counterpart.
- `release_il.py --base main --project Quire.Mpl.Modules --all`: 530 checked; FAIL (all checked members differ from Mathcad Release IL due to Mpl compiler conventions: `.maxstack 48` fat method headers, `tail.` calls, and Mpl module attributes).
- `[MirrorStub]` count in `src/Quire.Mpl.Modules/Generated`: 1,312 stubs across 82 generated files.

## Status and Blockers

All members of `src/Quire.Mpl.Modules` are parked with an exact blocker:
- **Blocker:** Missing Mpl compiler toolchain (`msmpl`/`mshex` native ports, `msgen`/`msclr` C++/CLI, `mplasb`/`mplrun` C#).
- **Reason (Decision D20 / D84):** The 15 assemblies hosted by `Quire.Mpl.Modules` are compiled by Mathcad's proprietary Mpl compiler, not Roslyn C#. The Mpl compiler emits `.maxstack 48` on every method header, uses `tail.` call instructions, and emits `ModuleInterfaceAttribute` metadata. Roslyn C# compiles methods with minimal maxstack (typically 1–8) and never emits `tail.` opcodes. C# stand-ins for Mpl modules cannot achieve Release IL parity.
- Per `docs/mirror/assembly-map.md` and `docs/mirror/decisions.md` D20, these 15 assemblies remain deferred / out of scope until the Quire Mpl toolchain exists.

## Remaining Mismatches

- All 1,312 stubs are blocked on the Mpl toolchain (see above).
