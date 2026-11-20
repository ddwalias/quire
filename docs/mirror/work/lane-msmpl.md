# Quire.Native.Msmpl (msmpl.dll) checkpoint

- Owner: native-msmpl worker (Orca task task_0751cbf2f446).
- Branch: `feature/native-msmpl-agy`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/mslisp-agy`, from `main`.
- Gate: `python tools/validation/native_gate.py --base main --project Quire.Native.Msmpl`.

## Members landed this batch

- Established `src/Quire.Native.Msmpl/` native C++ dynamic library project (`Quire.Native.Msmpl.vcxproj`, MSVC `v145`, x64 Release, `/O2 /Oi`, `/EHa`, `/GS-`, `stdcpp17`).
- Implemented and verified 8 exported module inspection functions matching Mathcad Prime 12.0.1.0's `msmpl.dll` with 100% byte parity and relocation verification:
  - `?GetModuleName@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CBD@@@Z`
  - `?GetModuleText@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CBD@@@Z`
  - `?GetModuleInterface@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CBD@@@Z`
  - `?IsModule@Mpl@@YA_NV?$Substring@$$CBD@@@Z`
  - `?GetModuleName@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CB_W@@@Z`
  - `?GetModuleText@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CB_W@@@Z`
  - `?GetModuleInterface@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CB_W@@@Z`
  - `?IsModule@Mpl@@YA_NV?$Substring@$$CB_W@@@Z`
- Manifest additions in `docs/mirror/native/internal-targets.json`:
  - `msmpl.dll`: SHA-256 `30f5af92d002e4b1e03bd161fa0a176f33e70ab60cfa5a3ef52c861eb42ac976`
  - Internal target `??1?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@QEAA@XZ` at RVA `0xffc0`
  - Toolchain target `??3@YAXPEAX_K@Z` at RVA `0x5e070`
- Gate: PASS, 10 checked, 0 failures, 0 regressions, 0 unfinished among checked.

## Status against the original

- Exports: 8 of 14 exports matched (57.1% export coverage).
- Byte parity: 10/10 functions matched (including candidate-defined COMDATs and manifest targets).

## Remaining mismatches and blockers

- `?GetModuleDependencies@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CBD@@@Z`
- `?GetModuleDependencies@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@V?$Substring@$$CB_W@@@Z`
- `?IsModule@Mpl@@YA_NAEAV?$basic_streambuf@DU?$char_traits@D@std@@@std@@@Z`
- `?GetModuleName@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAV?$basic_streambuf@DU?$char_traits@D@std@@@3@@Z`
- `?GetModuleDependencies@Mpl@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEAV?$basic_streambuf@DU?$char_traits@D@std@@@3@@Z`
- `?NewInterpreter@Mpl@@YAPEAVInterpreter@Hex@@XZ`

### Named Blocker: Internal MPL Compiler/Interpreter Pipeline
All 6 remaining exports depend directly on internal unexported MPL compiler components:
1. `NewInterpreter()` instantiates a concrete internal subclass of `Hex::Interpreter` with an 886-byte constructor referencing internal parser subsystems and vtables not yet recovered or mirrored.
2. `GetModuleDependencies` and the stream-based `basic_streambuf` variants require the full internal MPL parser, lexer, and AST dependency graph analyzer in `msmpl.dll`.
These components require deep reverse engineering and recovery of hundreds of internal compiler classes across `msmpl.dll` and cannot be implemented without the full internal MPL parser architecture.
