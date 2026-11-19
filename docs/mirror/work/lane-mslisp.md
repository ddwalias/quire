# Quire.Mslisp checkpoint

- Owner: AGY, Orca task `task_f40761abc054`.
- Branch: `feature/mslisp-agy`.
- Target: `src/Quire.Mslisp` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Landed Commits

- `4698f51dc` (Batch 1): Established original-language C++/CLI project split for `Quire.Mslisp` (`Quire.Mslisp.csproj` and `MixedMode/Quire.Mslisp.vcxproj`); mirrored `mslisp.Stream`, `mslisp.ConsoleStream`, `mslisp.DebugStream`, `mslisp.Condition`, `mslisp.Symbol`.
- `66f5478bc` (Batch 2): Mirrored `mslisp.Func0` .. `mslisp.Func6`, `mslisp.BuiltinDelegate`, `mslisp.GenCall`, `mslisp.FileStream`, `mslisp.TaggedSymbol`, `mslisp.SymbolImp`, `mslisp.Package`, `mslisp._fcomparer`.

## Release IL Report & Gates

- Batch 1: PASS, 103 checked, 88 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-1aba42d8ae27/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-851996b0cdd6.json`).
- Batch 2: PASS, 132 checked, 130 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-fbee254d2310/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-28dd53bdfe00.json`).

## Remaining Members and Actionable Blockers

All remaining members in `mslisp.dll` are concretely blocked on upstream unmirrored assemblies:
1. `mspcons-mgd` (`Engine.PCons`): 15 expression types (`mslisp.ApplyExpr`, `mslisp.ArgSpec`, `mslisp.CarExpr`, `mslisp.CdrExpr`, `mslisp.DefmacroExpr`, `mslisp.Env`, `mslisp.HandlerCaseExpr`, `mslisp.IfExpr`, `mslisp.IgnoreErrorsExpr`, `mslisp.LambdaExpr`, `mslisp.MacroExpr`, `mslisp.PrognExpr`, `mslisp.SetfExpr`, `mslisp.StandardBuiltins`, `mslisp.UnwindProtectExpr`), expression executors (`mslisp.FastApp` / `FastApp0`..`6`, `mslisp.LoggedFastApp` / `LoggedFastApp0`..`6`), `mslisp.CodeGen`, `mslisp.LocalVar`, and `mslisp.GlobalVar` require `Engine.PCons` from `mspcons-mgd.dll` (which is deferred in `Quire.Engine.Native` stubs and pending the `Quire.MspconsMgd` project split).
2. `engutils-mgd` (`PushbackReader`, `PathCache`): `mslisp.InterpreterImp`, `mslisp.Interpreter`, and `mslisp.Factory` depend on `PushbackReader` and `PathCache` from `engutils-mgd.dll` (pending the `Quire.EngutilsMgd` project split).
3. `<Module>`: 72 compiler/CRT support methods (e.g., `std.basic_ostream`, `std.basic_string`, `utl.ToStrng`, `mslisp.LogIndent`) will resolve automatically with the remaining module dependencies.
