# Quire.Mslisp checkpoint

- Owner: AGY, Orca task `task_23c65ba2f170`.
- Branch: `feature/mslisp-agy`.
- Target: `src/Quire.Mslisp` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Landed Commits

- `4698f51dc` (Batch 1): Established original-language C++/CLI project split for `Quire.Mslisp` (`Quire.Mslisp.csproj` and `MixedMode/Quire.Mslisp.vcxproj`); mirrored `mslisp.Stream`, `mslisp.ConsoleStream`, `mslisp.DebugStream`, `mslisp.Condition`, `mslisp.Symbol`.
- `66f5478bc` (Batch 2): Mirrored `mslisp.Func0` .. `mslisp.Func6`, `mslisp.BuiltinDelegate`, `mslisp.GenCall`, `mslisp.FileStream`, `mslisp.TaggedSymbol`, `mslisp.SymbolImp`, `mslisp.Package`, `mslisp._fcomparer`.
- `0aa549913` (Batch 3): Mirrored `Error::report(String^)`, `Error::report(String^, Object^)`, `is_true(Object^)`, `FindMethods(Type^, String^, short)`, `InterpreterState`, `Expression`, `Settable`, `StrcatFunction`, `SignalFunction`, `SingleMethod`, `DelegateFunction`, `CodeGen`, `Constructor`, `QuoteExpr`, `Env` (.ctor).
- `44b761b95` (Batch 4): Mirrored `Analyzer`, `Env` (lookup & full evaluation), `LocalVar`, `QuoteExpr`, `PrognExpr`, `IfExpr`, `IgnoreErrorsExpr`, `UnwindProtectExpr`, `CarExpr`, `CdrExpr`, `ArgSpec`, `LambdaExpr`, `MacroExpr`, `DefmacroExpr`, `HandlerCaseExpr`, `SetfExpr`.
- `5c986acf9` (Batch 5): Mirrored `FastApp`, `FastApp0`..`6`, `GlobalVar`, `InstanceMethod`, `StaticOrInstanceMethod`, `ApplyFunction`, `PlusFunction`, `StandardBuiltins`.
- Pending Batch 6 (committed on `feature/mslisp-agy`, blocked on maintainer shared-tool fixes): Mirrored ALL remaining types and `<Module>` helpers in `mslisp.dll`: `mslisp.LoggedFastApp`, `mslisp.LoggedFastApp0`..`6`, `mslisp.ApplyExpr`, `mslisp.Interpreter`, `mslisp.InterpreterImp`, `mslisp.Factory`, `mslisp.LogIndent`, `mslisp.LogIndented`, `mslisp.dbg_msg`, `utl.ToStrng`, `sVerboseTrace`, and `UTLLogging.def` (`utl::Log`, `utl::mcGetSetting`).

## Release IL Report & Gates

- Batch 1: PASS, 103 checked, 88 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-1aba42d8ae27/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-851996b0cdd6.json`).
- Batch 2: PASS, 132 checked, 130 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-fbee254d2310/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-28dd53bdfe00.json`).
- Batch 3: PASS, 187 checked, 187 exact, 0 failures, 75 Mathcad members not paired (`.mirror-cache/release-il-203781de17ec/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-6fc2507c6612.json`).
- Batch 4: PASS, 206 checked, 206 exact, 0 failures, 69 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-8bbac256b1e6/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-69182d4745a7.json`).
- Batch 5: PASS, 292 checked, 289 exact, 0 failures, 67 Mathcad members not paired (`.mirror-cache/release-il-547c6bdd32d7/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-1a7dfa703cda.json`).
- Batch 6 (branch `feature/mslisp-agy`): 457 checked, 452 exact, 5 failures (`.mirror-cache/release-il-9f748e9c870d/release-il.json`) — **0 code/IL defects in `Quire.Mslisp`**, all 5 failures are caused by 3 shared-tool / maintainer-owned blockers (see below).

## Concrete Blockers (Shared Tools / Manifest — Maintainer Action Needed)

All types and members of `mslisp.dll` are implemented with exact Release IL parity in `src/Quire.Mslisp/MixedMode/MslispPort.cpp` (plus `UTLLogging.def` and `Quire.Mslisp.vcxproj`), but landing Batch 6 is blocked by 3 shared-tool/manifest gaps outside our assembly boundary:

1. **VC++ CRT `new` and `delete[]` native bodies (`docs/mirror/native/internal-targets.json`, D23)**:
   - `Quire.Mslisp::<Module>::new` (`native body differs: +0x38: +0x9: +0x0: data +0x10: a pointer on one side only`)
   - `Quire.Mslisp::<Module>::delete[]` (`native body differs: length 5 != original 21`)
   - `mslisp.dll` (`sha256: 6c97e5e7e4437613c0ce0cae4cb113bb45a542032dc13bb3c1ee5b54c12d5ebb`) needs `"managed": { "new": { "size": 68, "toolchain": true }, "delete[]": { "size": 21, "toolchain": true } }` in `docs/mirror/native/internal-targets.json` (same pattern as `mceng.dll` and `engutils-mgd.dll`).
2. **MSVC translation-unit anonymous hash (`?A0x<8-hex>`) normalization in `tools/Quire.Tools.MethodAudit/`**:
   - File-static `sVerboseTrace` has source-path-dependent hash `?A0x0144f16f` in Quire vs `?A0xd2f58043` in Mathcad.
   - Causes 2 failures + 1 skipped:
     - `<Module>::?A0x0144f16f.??__E?A0x0144f16f@sVerboseTrace@@YMXXZ` unpaired with Mathcad's `?A0xd2f58043.??__E?A0xd2f58043@sVerboseTrace@@YMXXZ`
     - `ApplyExpr::execute` (exact 267/267 IL instructions) fails only on `ldsfld <Module>::?A0x0144f16f.sVerboseTrace` vs `?A0xd2f58043.sVerboseTrace`.
3. **MSVC mangling backreference shift in `tools/Quire.Tools.MethodAudit/Structure.cs::LocalStaticName`**:
   - `InterpreterImp::_gensym` (exact 25/25 IL instructions) has function-local `static unsigned int s_count = 0;`.
   - `LocalStaticName` maps `InterpreterImp@mslisp@Quire@@` to `InterpreterImp@mslisp@@`, but does not decrement MSVC mangling backreferences in the suffix (`PE$AAV56@@` in Quire vs `PE$AAV45@@` in Mathcad, shifted by +1 because `Quire::` adds one identifier before `Object@System@@`).

