# Lane Engine — Quire.Engine

- Owner: Lane Engine; branch `mirror/engine-20261109`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/engine-20261109`.
- Scope: `Quire.Engine` (C#), hosting Mathcad C++/CLI assemblies `iengine` and `engutils-mgd`. 315 stub markers across 22 generated mirror files.
- Evaluated groups and candidate shapes:
  - `Engine.AlgoInfo` (`Name`, `Tag`, `Region` getters/setters, `.ctor`): idiomatic and explicit backing-field property shapes. Gate: FAIL (`.mirror-cache/release-il-d5bf2ad17d55/release-il.json`), instruction sequence matches but `.maxstack 8` (Roslyn tiny header format for bodies < 64 bytes with no locals) vs Mathcad `.maxstack 1` (MSVC C++/CLI 12-byte fat header).
  - `Engine.CollectionUtils` (`Copy`, `Concat`, `CopyDict`, `.ctor`): idiomatic `foreach`, `while`, and `goto` top-tested loop with C++/CLI slot ordering. Gate: FAIL (`.mirror-cache/release-il-2172a2f8b6a7/release-il.json`), instruction sequence matches on `Copy` but `.maxstack 2` vs Mathcad `.maxstack 4` and `.initlocals True` vs Mathcad `.initlocals False`.
  - `Engine.StringUtils` (`MangleForXAML`): idiomatic `[SkipLocalsInit]` do-while shape with slot alignment. Gate: FAIL (`release-il`), `callvirt` on `String.Length` (Roslyn requirement) vs Mathcad non-virtual `call` (MSVC C++/CLI).
  - `Engine.Reflection` (`FindType`): idiomatic loop over assemblies. Gate: FAIL on `callvirt` vs MSVC C++/CLI non-virtual `call` on `AppDomain.GetAssemblies`.
  - `Engine.ComponentHelpers` (`Lock`, `Unlock`, `SetValue`, `GetValue`): fails on `.maxstack` differences (1 vs 8, 2 vs 4) and non-virtual calls.
  - `Native.EngutilsMgd.PushbackReader` & `Native.EngutilsMgd.PathCache`: blocked on C++/CLI-injected `GC.KeepAlive(this)` calls and constructor base-call order (`Object..ctor()` emitted last by MSVC C++/CLI vs first by Roslyn).
  - `Engine.IEngine` (50 interface stubs): declaring as abstract interface methods causes `Quire.Engine.Internal.WorksheetEngine` compilation failure; adding stubs to `WorksheetEngine` introduces 50 new methods without counterpart, failing D21.
  - Native/C++ groups (`Engine.Timing`, `Engine.Timing2`, `Engine.Logging`, `Native.EngutilsMgd.MplUtils`, `Engine.UnmanagedMemoryStream`): depend on C++ STL/CRT types (`std.basic_string`, `utl.TimeIt`, `utl.PerformanceTimer`, unmanaged memory stream `fault` blocks) not expressible in C# Release IL.
- Blocker: `Quire.Engine` is a C# project (`Quire.Engine.csproj`), but both hosted Mathcad assemblies (`iengine` and `engutils-mgd`) were compiled by MSVC C++/CLI 19.51.36246 (D19, D20). Roslyn cannot emit MSVC C++/CLI method header shapes (`.maxstack` on tiny methods), non-virtual calls on sealed framework classes, post-field constructor chaining, or `fault` blocks. Under D20 and mapping.md, exact Release IL parity requires splitting `engutils-mgd` into `Quire.EngutilsMgd` and providing a C++/CLI candidate (`MixedMode/*.vcxproj`) for `Quire.Engine`.
- Release IL report: `.mirror-cache/release-il-deaa4909ccf7/release-il.json` — PASS, 0 checked, 863 methods in assembly, 0 exact.
- Remaining: 315 stub markers across 22 generated mirror files in `src/Quire.Engine/Generated/Mirror/`.
