# Lane D — Quire.Engine.Utl

- Owner: Lane D. Branch `mirror/d-engine-utl-coverage-20261023-h1` (from `main` f909e58e); worktree `C:/Projects/Quire-wt/lane-d-utl-coverage`.
- Members: utl-mgd, honest C++/CLI port (D19): `MixedMode/Quire.Engine.Utl.vcxproj`, `/std:c++17`, `/OPT:REF`.
- Release IL: `release_il.py --base main --project Quire.Engine.Utl` and `--project Quire.Engine` (see Status for the report ids).

## Coverage audit (utl-mgd reference IL `589523c705e87663`)

| Surface | Mathcad | Quire candidate | Missing | Class |
|---|---:|---:|---|---|
| Class headers (managed, native, toolset) | 89 | same names | 0 | — |
| Method declarations (incl. `<Module>` globals, nested) | 370 | 370, identical signatures and visibility | 0 | — |
| Managed fields and properties | 44 fields, 11 properties | same | 0 | — |
| Events | 0 | 0 | 0 | — |
| `beforefieldinit` on `LabelTrie`, `LabelUtils` | set | absent (explicit static constructor) | 2 | (a) ported |
| Custom attributes | 261 | 286 | 25 extra `MirrorOf` (Quire metadata) | Quire-only |

- (b) and (c): none. Under D18 every managed member of Utl is in scope, and no member is missing.
- `MessageQueue::m_stack_size` is present in the candidate. The audit compares declarations; bodies are the gate's job.

## Done

- `LabelTrie`, `LabelUtils`: explicit static constructors replaced by static field initializers (restores `beforefieldinit`).
- `#using <System.Linq.dll>` (unused) and `#pragma managed(push, on)/(pop)` removed; the gate passes without them.
- vcxproj `AssemblyVersion` removed (inert). The source `[assembly:AssemblyVersionAttribute]` stays.
- csproj nested MSBuild uses `/t:Rebuild`, so an up-to-date vcxproj cannot leave the C# DLL in `bin`. Verified: the `bin` DLL carries the pinned linker's Rich header.
- csproj passes the `Quire.Mirror.dll` it references (`QuireMirrorPath`); the vcxproj builds Quire.Mirror with dotnet only when that property is empty (hand builds).
- `reads_install` scans the whole ProjectReference closure (`build_closure`), with a test.
- `release_il.py` `compare()`: a native pair that was exact in a mixed-mode base keeps its obligation once it stops being exact (import record differs). Approved by the maintainer. Tests added.

## Status

- Unit tests: `python -m unittest test_release_il` in `tools/validation`.
- Release IL gate: see the report ids recorded at commit time.
- Debug and tests run the C# sources; Release ships the C++/CLI DLL (decision recorded here).
- Release test pass: `dotnet test tests/Quire.Engine.Tests -c Release`: 97 passed, 0 failed. The DLL in the test output is the mixed-mode one (pinned linker's Rich header).

## Remaining

- `QuireMsvcInstallDir` defaults to `C:\BuildTools\VS18.6-msvc\` in the csproj when unset (the maintainer's choice, machine-specific; the gate's vswhere value still overrides it). Plain Release builds work on this machine. Other machines must set the variable or change the default.
- `/t:Rebuild` recompiles UtlPort.cpp on every Release build. Accepted cost for a correct DLL.
