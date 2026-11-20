# Quire.Mceng checkpoint

- Owner: AGY, Orca task `task_878df5122bb4`, dispatch `ctx_8bb7f06c566c`.
- Branch: `feature/mceng-agy`.
- Target: `src/Quire.Mceng` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20).

## Implemented Members and Parity Status

Established original-language C++/CLI project split for `Quire.Mceng` (`Quire.Mceng.csproj` and `MixedMode/Quire.Mceng.vcxproj`) referencing pinned MSVC 19.51.36246 (D19). Registered `mceng.dll` internal toolchain targets in `docs/mirror/native/internal-targets.json` (D23).

- Checked: 159 members in `Quire.Mceng`
- Exact matches (158 of 159):
  - `Quire.Engine.FactoryImp::.ctor`: **EXACT**
  - `Quire.Engine.FactoryImp::get_WorkerStackSize`: **EXACT**
  - `Quire.Engine.FactoryImp::set_WorkerStackSize`: **EXACT**
  - `Quire.Engine.FactoryImp::Initialize`: **EXACT**
  - `Quire.Engine.FactoryImp::NewEngine`: **EXACT**
  - `Quire.Engine.FactoryImp::GetEngines`: **EXACT**
  - `Quire.Engine.FactoryImp::OnCreate`: **EXACT**
  - `Quire.Engine.FactoryImp::OnDestroy`: **EXACT**
  - `Quire.Engine.FactoryImp::~FactoryImp`: **EXACT**
  - `Quire.Mceng::<Module>::new` (68 bytes): **EXACT** (toolchain target)
  - `Quire.Mceng::<Module>::delete[]` (21 bytes): **EXACT** (toolchain target)
  - All remaining managed methods and CRT helper functions: **EXACT**

## Remaining Blocker: `VerifyIntegrity` Static Field Decoration

- Method: `Quire.Engine.FactoryImp::VerifyIntegrity() -> bool`
- Code structure: **291 of 291 instructions match exactly** (identical branch counts [9], identical exception handler clauses [5], identical calls and string constants).
- Only difference:
  - Quire: `ldsfld [<self>]<Module>::?bNeedInit@?1??VerifyIntegrity@FactoryImp@Engine@Quire@@UE$AAM_NXZ@4_NA : System.Boolean data 4BF5122F344554C53BDE2EBB8CD2B7E3D1600AD631C385A5D7CCE23C7785459A`
  - Mathcad: `ldsfld [<self>]<Module>::?A0x0d0e4802.?bNeedInit@?1??VerifyIntegrity@FactoryImp@Engine@@UE$AAM_NXZ@4_NA : System.Boolean data 4BF5122F344554C53BDE2EBB8CD2B7E3D1600AD631C385A5D7CCE23C7785459A`
- Root Cause:
  1. `?A0x0d0e4802.` is MSVC's anonymous namespace tag encoding the hash of PTC's build machine source path. It is machine- and path-dependent (analogous to the `ANONYMOUS` regex in `tools/validation/native_bytes.py`).
  2. `@Engine@Quire@@` vs `@Engine@@` reflects the `Quire` namespace prefix in `Quire.Engine.FactoryImp`.
  3. Shared judge `MethodAudit` (`tools/Quire.Tools.MethodAudit/Structure.cs` line 411) currently formats fields via `md.GetString(field.Name)` without stripping `?A0x[0-9a-fA-F]+\.` or normalizing `@Quire@@` in mangled symbols. Per `AGENTS.md`, shared tools changes belong to the maintainer.
