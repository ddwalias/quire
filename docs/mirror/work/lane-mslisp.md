# Quire.Mslisp checkpoint

- Owner: AGY, Orca task `task_f40761abc054`.
- Branch: `feature/mslisp-agy`.
- Target: `src/Quire.Mslisp` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Landed in Batch 1

- Established the original-language C++/CLI project split for `Quire.Mslisp` mapped to `mslisp.dll` (`Quire.Mslisp.csproj` and `MixedMode/Quire.Mslisp.vcxproj`).
- `mslisp.Stream`: `Write(string)`, `WriteLine(string)`.
- `mslisp.ConsoleStream`: constructor, `Write(string)`, `WriteLine(string)`.
- `mslisp.DebugStream`: constructor, `Write(string)`, `WriteLine(string)`.
- `mslisp.Condition`: constructors `Condition(string, object)`, `Condition(string)`, `Condition(object[])`.
- `mslisp.Symbol`: `get_name()`, `get_fullname()`, `get_package()`, `get_value()`, `set_value(object)`, `not_set()`, `get_Base()`, `get_Tag()`, `get_Labels()`, `get_Wild()`, `WithWild(bool)`.

## Batch 2

- `mslisp.Func0` .. `mslisp.Func6`: delegates (`.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke`).
- `mslisp.BuiltinDelegate`: delegate (`.ctor`, `Invoke(object[])`, `BeginInvoke`, `EndInvoke`).
- `mslisp.GenCall`: `Emit(ILGenerator, short)`.
- `mslisp.FileStream`: `GetMutex(string)`, `TryDelete(string)`, constructor, `Write(string)`, `WriteLine(string)`, class constructor.
- `mslisp.TaggedSymbol`: constructor, `ToString()`, `get_name()`, `get_fullname()`, `get_package()`, `get_value()`, `set_value(object)`, `not_set()`, `get_Base()`, `get_Tag()`, `get_Labels()`, `get_Wild()`, `WithWild(bool)`.
- `mslisp.SymbolImp`: constructors, `GetObjectData`, `ToString()`, `get_name()`, `get_fullname()`, `get_package()`, `get_value()`, `set_value(object)`, `not_set()`, `get_Base()`, `get_Tag()`, `get_Labels()`, `get_Wild()`, `WithWild(bool)`.
- `mslisp.Package`: constructor, `intern(string)`, `_export(Symbol)`, `_export(string)`, `find_export(string)`, `get_name()`.
- `mslisp._fcomparer`: constructor, `Compare(object, object)`.

## Release IL report

- `release_il.py --base main --project Quire.Mslisp`: PASS, 132 checked, 130 exact, 0 failures, 72 Mathcad `<Module>` members not paired (`.mirror-cache/release-il-fbee254d2310/release-il.json`).
