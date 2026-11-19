# Quire.Mslisp checkpoint

- Owner: AGY, Orca task `task_f40761abc054`.
- Branch: `feature/mslisp-agy`.
- Target: `src/Quire.Mslisp` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Landed Commits

- `4698f51dc` (Batch 1): Established original-language C++/CLI project split for `Quire.Mslisp` (`Quire.Mslisp.csproj` and `MixedMode/Quire.Mslisp.vcxproj`); mirrored `mslisp.Stream`, `mslisp.ConsoleStream`, `mslisp.DebugStream`, `mslisp.Condition`, `mslisp.Symbol`.
- `66f5478bc` (Batch 2): Mirrored `mslisp.Func0` .. `mslisp.Func6`, `mslisp.BuiltinDelegate`, `mslisp.GenCall`, `mslisp.FileStream`, `mslisp.TaggedSymbol`, `mslisp.SymbolImp`, `mslisp.Package`, `mslisp._fcomparer`.
- `0aa549913` (Batch 3): Mirrored `Error::report(String^)`, `Error::report(String^, Object^)`, `is_true(Object^)`, `FindMethods(Type^, String^, short)`, `InterpreterState`, `Expression`, `Settable`, `StrcatFunction`, `SignalFunction`, `SingleMethod`, `DelegateFunction`, `CodeGen`, `Constructor`, `QuoteExpr`, `Env` (.ctor).

## Release IL Report & Gates

- Batch 1: PASS, 103 checked, 88 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-1aba42d8ae27/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-851996b0cdd6.json`).
- Batch 2: PASS, 132 checked, 130 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-fbee254d2310/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-28dd53bdfe00.json`).
- Batch 3: PASS, 187 checked, 187 exact, 0 failures, 75 Mathcad members not paired (`.mirror-cache/release-il-203781de17ec/release-il.json`). Merged via `lane_merge.py` (receipt: `gate-il-6fc2507c6612.json`).

## Remaining Members and Actionable Blockers

All remaining members in `mslisp.dll` are concretely blocked on `mspcons-mgd.dll` (`Quire.MspconsMgd`), which is being worked on concurrently by another worker:
1. `Quire.MspconsMgd` (`[mspcons-mgd]Engine.PCons`, `Engine.TagFunction`, `Engine.Reader`, `Engine.InternFunction`):
   - `mslisp.Interpreter`: Directly exposes public API accepting `[mspcons-mgd]Engine.TagFunction tf`.
   - `mslisp.Factory`: Returns `mslisp.Interpreter` and instantiates `mslisp.InterpreterImp`.
   - `mslisp.InterpreterImp`: Implements `mslisp.Interpreter`, holding and manipulating `PCons`, `Reader`, `TagFunction`, and `InternFunction`.
   - Expression types and executors: `mslisp.ApplyExpr`, `mslisp.ArgSpec`, `mslisp.CarExpr`, `mslisp.CdrExpr`, `mslisp.DefmacroExpr`, `mslisp.HandlerCaseExpr`, `mslisp.IfExpr`, `mslisp.IgnoreErrorsExpr`, `mslisp.LambdaExpr`, `mslisp.MacroExpr`, `mslisp.PrognExpr`, `mslisp.SetfExpr`, `mslisp.StandardBuiltins`, `mslisp.UnwindProtectExpr`, `mslisp.LocalVar`, `mslisp.FastApp` / `FastApp0`..`6`, `mslisp.LoggedFastApp` / `LoggedFastApp0`..`6`, and remaining `mslisp.Env` members require `Engine.PCons` from `mspcons-mgd.dll`.
   - Dependent reflection & function evaluators: `mslisp.GlobalVar`, `mslisp.PlusFunction`, `mslisp.InstanceMethod`, and `mslisp.StaticOrInstanceMethod` integrate with the evaluator and `PCons` argument passing.
2. `<Module>`: 75 compiler/CRT and internal support methods (e.g., `std.basic_ostream`, `std.basic_string`, `utl.ToStrng`, `mslisp.LogIndent`) will resolve automatically with the interpreter and parser integration once `Quire.MspconsMgd` is available.

