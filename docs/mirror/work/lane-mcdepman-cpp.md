# mcdepman C++/CLI spike

- Owner: Codex, Orca task `task_c5fad519dd24`.
- Branch: `feature/mcdepman-cpp-spike` (new Orca worktree from `main` at `9527b6b9c151c7838e8f7a800c6c9faecf98f569`).
- Rebased base for the repeated candidate gate: `1e8d06128e7b5cfd179bacab647ec50440299943`.
- Members: `depman.IDepman.SetAutoCalc(bool)` and `depman.IDepman.Remove(long)` from `mcdepman`; declarations only, as in Mathcad.
- Release IL report: `.mirror-cache/release-il-dbee358fcf96/release-il.json`.
- Exact gate stdout (exit 0):

```text
Building candidate Release assemblies
Release IL: PASS; 83 checked; C:\Users\DuyDang\orca\workspaces\Quire\mcdepman-cpp-spike\.mirror-cache\release-il-dbee358fcf96\release-il.json
```

Both selected rows have `declaration_only: true`, `shape_matches: true`, and no structure differences. Original tokens: Remove `0x060002cd`, SetAutoCalc `0x060002da`. `long h` emits `int32 modopt([System.Runtime.CompilerServices.VisualC]System.Runtime.CompilerServices.IsLong)`; `bool b` emits `marshal(unsigned int8)`. Candidate IL from the initial production Release build stays in `.mirror-cache/release-il-66345c2f60a4/candidate.il`. The production Release project also builds successfully with zero warnings/errors.

Remaining mismatches: none in this two-member spike. Remaining migration: the rest of mcdepman and existing C# depman mirrors in Quire.Mcd; this spike does not remove that host's declarations.

## Migration resume (2026-11-10)

- Owner: Codex, Orca task `task_3bebf9de662c`; branch `feature/mcdepman-migration-resume`.
- Step 1 (previous worker): `Mapper.Build` uses `Rules.HostFor`; `Rules.IsSplit` detects the spike vcxproj on main, so `HostFor` already selects `Quire.Mcdepman`. The depman C# files in `Quire.Mcd` are stale pre-spike hosts. No mapper change is needed.
- Unchanged main baseline `b7d24322d709e9f661a6b36c27ccfeab88781ae8`: Quire.Mcd Release build succeeds (143 warnings, 0 errors); foreground gate PASS, 0 checked, 658 declarations and 0 inspection errors. Report: `.mirror-cache/release-il-e1564258258b/release-il.json`.
- Initial batch 1 attempt (superseded below): IDepman scalar/collection declarations (35 including the two spike declarations), removing the corresponding C# members in the same change. Original Release IL: artifact SHA-256 `7c510fadb0d735ecae5a564efc0a572de21119825188d124dfd040519248a87a`.
- Foreground C++ candidate gate PASS, 114 checked; all 35 IDepman rows have matching declaration shapes. Report: `.mirror-cache/release-il-a8a18502915f/release-il.json`. No rejected C++ source shapes.
- Quire.Mcd foreground gate FAIL, 0 checked: 20 migrated hand-written declarations report `method removed`; Release compilation still succeeds (143 warnings, 0 errors). Report: `.mirror-cache/release-il-46e15b8ffef1/release-il.json`.
- Diagnostic joint production gate (`--project Quire.Mcd --project Quire.Mcdepman`) FAIL, 95 checked, only C# `IDepman.SetAutoCalc(bool)` reports `method removed`. Report: `.mirror-cache/release-il-a36b2a6d75f5/release-il.json`. Its C++ replacement already existed in the spike base, so the validator's move matching does not consume the removed duplicate C# row. No gate exemptions or tool changes made.
- Initial blocker (superseded by revised plan): standalone Quire.Mcd validation cannot see replacements in Quire.Mcdepman; joint validation handles 19 moved declarations but fails removal of the already-replaced spike declaration. That removal attempt was held uncommitted/unlanded for maintainer direction.

## Revised batch 1 — retain the C# host

- Owner: Codex, Orca task `task_1d51167b984f`; branch `feature/mcdepman-migration-resume`.
- Maintainer option 2: add C++ members without removing C# members; gate the C++ candidate and unchanged Quire.Mcd separately. Remove a C# host only after its entire type exists in C++, in a separate batch that keeps Quire.Mcd building. No tools or gate changes.
- Clean current-main joint production gate at `b7d24322d709e9f661a6b36c27ccfeab88781ae8`: PASS, 81 checked, 741 declarations, 0 inspection errors; `.mirror-cache/release-il-fd0d665fb9af/release-il.json`. SetAutoCalc does **not** fail on main; the earlier failure required removal of its duplicate C# declaration.
- Scope: retain the 35 C++ IDepman scalar/collection declarations (33 new and 2 spike declarations); restore both C# IDepman files byte-for-byte from main. Remaining IDepman declarations: 19 requiring depman/engine/utl/mpl types; Action (4), BooleanMediator (4), Environment (4), SEnvironment (3) and other mcdepman members remain unfinished.
- Revised foreground C++ candidate gate: PASS, 114 checked; `.mirror-cache/release-il-0a8a4aa17647/release-il.json`. No rejected C++ source shapes or remaining mismatches in this batch.
- Revised foreground Quire.Mcd gate: PASS, 0 checked; `.mirror-cache/release-il-5d5d08a68d0d/release-il.json`. Quire.Mcd still builds; all its C# declarations remain in place.

## Batch 2 — IDepman engine callbacks and runtime options

- Owner: Codex, Orca task `task_2be85ce3d345`; branch `feature/mcdepman-migration-resume`.
- Members: seven callback overloads using utl-mgd/iengine types and `GetRuntimeOptions`, bringing IDepman to 43/54 declarations. The C++ and production projects now reference Quire.Engine, Quire.Engine.Utl and Quire.Mpl; C# hosts are unchanged.
- Foreground candidate gate PASS, 89 checked; `.mirror-cache/release-il-372bbb4bc411/release-il.json`. No rejected shapes. Eleven IDepman declarations still need depman types; subsequent interface groups remain unfinished.
- Foreground Quire.Mcd gate PASS, 0 checked; `.mirror-cache/release-il-068c328c06b7/release-il.json`; Quire.Mcd still builds with unchanged declarations.

## Batch 3 — depman contracts and their IDepman consumers

- Owner: Codex, task `task_2be85ce3d345`; branch `feature/mcdepman-migration-resume`. Batch 2 landed as `325df3b0a7210525e75ff22663412e0ba9c38970`, confirmed ancestor of main; lane report `C:/Projects/Quire/.mirror-cache/gate-il-5eb97a6c26fc/gate.json` (PASS, 89 checked).
- Members: complete Action, ActionResult, BooleanMediator, Environment, Scope, DepNodeCallback, ScopingMode and ResolveMode; nine IDepman declarations consuming those types. IDepman is now 52/54 declarations. The source declaration SEnvironment.GetRegionId was subsequently confirmed un-emitted (see batch 4); SEnvironment remains 0/3. C# hosts remain unchanged.
- Foreground candidate gate PASS, 116 checked; `.mirror-cache/release-il-e77c56457560/release-il.json`. Scope and ActionResult bodies match without rejected source shapes.
- Foreground Quire.Mcd gate PASS, 0 checked; `.mirror-cache/release-il-a0cf0829695b/release-il.json`; Quire.Mcd still builds. Remaining selected declarations need Position, DSym, SharedThreadContext and NodeTimer.

## Batch 4 — symbols and the shared thread-context contract

- Owner: Codex, task `task_2be85ce3d345`; branch `feature/mcdepman-migration-resume`. Batch 3 landed as `22c838eced74b00108737e9f8c1838105a957e20`, confirmed ancestor of main; lane report `C:/Projects/Quire/.mirror-cache/gate-il-d2ca649098fc/gate.json` (PASS, 116 checked).
- Members: DSym (5/5), SharedThreadContext constructor, private self accessor and two property getters (4/19). Emitted inventories confirmed that MSVC omitted the unused internal SEnvironment contract entirely; its source-only declarations were removed and all 3 SEnvironment members remain incomplete until its PositionalEnv/native Position consumer is available. All C# hosts remain unchanged in the passing state.
- Rejected candidates: `.mirror-cache/release-il-89bff0eb9609/release-il.json` and `.mirror-cache/release-il-4046c200477d/release-il.json`; DSym.Equals compound comparison / final positive test differed. The separate final failed-name test matches. The first candidate's native Position forward declaration also emitted LNK4248 (unresolved TypeRef), so the pointer declarations were removed rather than accepted as complete.
- Foreground candidate gate PASS, 98 checked; `.mirror-cache/release-il-a92da0e2b195/release-il.json`. Remaining selected declarations: IDepman.UnLockP and SEnvironment.Lookup need a defined native Position (reference layout is 248 bytes); IDepman.SetTimer needs NodeTimer. Quire.Native.Utl has PerformanceTimer source, but its header/export list lack Stop and the destructor called by NodeTimer; that other project's changes belong to its maintainer. No opaque layout fabrication, missing-type acceptance or gate edits.
- Foreground unchanged Quire.Mcd gate PASS, 0 checked; `.mirror-cache/release-il-13bd6ee7a839/release-il.json`; Quire.Mcd still builds.
- Whole-type cleanup probe (BooleanMediator is complete in C++): removing both C# files and adding a project reference fails before comparison with CS0234 for BooleanMediator; `.mirror-cache/release-il-9d9def6f6a99/release-il.json`. An intermediate-assembly copy did not fix consumption (`.mirror-cache/release-il-a314c4c2241b/release-il.json`); suppressing parent-configuration unsetting instead failed with missing Quire.Engine.Utl metadata (`.mirror-cache/release-il-74646446b1e7/release-il.json`). No mixed-mode Quire.Mcdepman build ran in those dependency logs. All C# removals, project-reference/configuration changes and the ineffective copy were reverted. Mixed-mode project-reference integration needs the maintainer; no tools or gate changes were made.
- Final foreground candidate gate (explicitly selecting DSym and SharedThreadContext) PASS, 98 checked; `.mirror-cache/release-il-388b7847fb29/release-il.json`. Both helper types are present in the emitted inventory, with all nine implemented bodies exact; SEnvironment is absent and is not counted complete.
- Final restored-state foreground Quire.Mcd gate PASS, 0 checked, 658 declarations and 0 inspection errors; `.mirror-cache/release-il-cebe8f1f50d1/release-il.json`. Quire.Mcd still builds and all C# host declarations are retained.

## Batch 5 — SharedThreadContext thread map, exception and data contracts

- Owner: Codex / AGY, Orca task `task_641f4a973cd0`; branch `feature/mcdepman-threads`.
- Members: `depman.DepException` (1/1), `depman.SafeAbortException` (2/2), `depman.ThreadData` (6/6), and eleven `depman.SharedThreadContext` members (`check_interrupt`, `get_thread_data()`, `get_thread_data(int32)`, `clear_thread_data(int32)`, `ObtainMutex`, `ReleaseMutex`, `CheckInterrupt`, `RegisterThread`, `UnregisterThread`, `SyncToThread`, `RemoveUnitSystem`), bringing SharedThreadContext to 15/19 members. C# hosts remain unchanged.
- Configuration: enabled `/Zc:sizedDealloc` in `Quire.Mcdepman.vcxproj` and declared sized `operator delete` to match Mathcad's sized deallocation in `ThreadData` destructor without `<new>`'s array unwind filter.
- Foreground candidate gate PASS, 120 checked; `.mirror-cache/release-il-3a30c6ae00fa/release-il.json`. All 20 newly implemented methods match Mathcad Release IL exactly.
- Foreground unchanged Quire.Mcd gate PASS, 0 checked; `.mirror-cache/release-il-29db0311b092/release-il.json`.
- Remaining SharedThreadContext members (4): `Abort(int32)`, `ClearInterrupt(int32)`, `interrupt(int32)`, `ExecuteInterruptibly(ThreadStart, bool)` (require `Efi.Symbolics` / `Aborter` / `SafeRun` and native `InterruptControl.RequestInterrupt`).
- Remaining other members: IDepman.UnLockP (needs Position), IDepman.SetTimer (needs NodeTimer/PerformanceTimer Stop), SEnvironment (0/3; omitted until native Position consumer exists).

## Batch 6 — Aborter constructor and SafeRun

- Owner: AGY, Orca task `task_b162481f4f2e`; branch `feature/mcdepman-aborter-agy-20261116`.
- Members: `depman.SafeRun` (3/3: `.ctor`, `Run`, `CheckErrors`) and `depman.Aborter` constructor (1/3: `.ctor`).
- Release IL report: `.mirror-cache/release-il-5ad759097839/release-il.json` (PASS, 154 checked).
- Foreground unchanged Quire.Mcd gate PASS, 0 checked; `.mirror-cache/release-il-6dfa7796bc17/release-il.json`.
- Remaining Aborter members (2): `Do()` and `Abort()` require `Efi.Symbolics` (`RequestInterrupt` and `UnRequestInterrupt`), which is missing.
- Remaining SharedThreadContext members (4): `Abort(int32)`, `ClearInterrupt(int32)`, `interrupt(int32)` (require `Efi.Symbolics` / `Aborter`), `ExecuteInterruptibly(ThreadStart, bool)` (requires `SharedThreadContext` interrupt management wiring).


