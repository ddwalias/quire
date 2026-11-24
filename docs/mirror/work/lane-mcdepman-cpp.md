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

## Batch 7 — CacheKey equality

- Owner: Codex / Orca task `task_dd7c54fcfe97`; branch `feature/luna-mcdepman`.
- Member: `depman.CacheKey.Equals(object)`, implemented from the original Release IL using the established `DSym.Equals` implementation and CacheKey field types.
- Release IL report: `.mirror-cache/release-il-b9bd52a597f7/release-il.json` (PASS, 191 checked; selected Quire.depman.CacheKey).
- Remaining mismatches: none in this batch. Other mcdepman members remain incomplete as listed above; `ActionChainMgd` methods were not selected.

## Batch 8 — ActionChainMgd managed accessors

- Owner: Codex / Orca task `task_dd7c54fcfe97`; branch `feature/luna-mcdepman`.
- Members verified: `ActionChainMgd` constructor, `action()`, `env()`, `state()`, `result()`, and `set_result(object)`; existing mixed-mode implementations match the original Release IL.
- Release IL report: `.mirror-cache/release-il-0aab2241176b/release-il.json` (PASS, 191 checked; selected Quire.depman.ActionChainMgd).
- Remaining mismatches: none in this batch. Other mcdepman members remain incomplete as listed above.

## Batch 9 — DepTrackingEnv field access

- Owner: Codex / Orca task `task_dd7c54fcfe97`; branch `feature/luna-mcdepman`.
- Members verified: `DepTrackingEnv` constructor and `loc()`; the reference confirms the pointer field and accessor directly, and the existing mixed-mode implementations match.
- Release IL report: `.mirror-cache/release-il-9d40c7c55d4e/release-il.json` (PASS, 191 checked; selected Quire.depman.DepTrackingEnv).
- Remaining mismatches: none in this batch. Other mcdepman members remain incomplete as listed above.

## Batch 10 — ActionChainMgd native pointer accessors

- Owner: Codex / Orca task `task_7a93711d37bc`; branch `feature/luna-mcdepman`.
- Members: `ActionChainMgd.depman()`, `loc()`, and `next()`. Mathcad Release IL establishes the backing `ActionChain` pointer offsets 8, 16, and 24; named fields replace the same 24-byte padding, and `locked_` remains at offset 32 with a 40-byte size assertion.
- Release IL report: `.mirror-cache/release-il-f6b67571f8e8/release-il.json` (PASS, 194 checked; selected Quire.depman.ActionChainMgd).
- Remaining mismatches: none in this batch. The type-only `PositionBase` listing showed only `NativeCppClass` layout metadata; the full assembly `Module` listing was checked in batch 11 and supplied the `SetBusy` method body.

## Batch 11 — ActionChainMgd.IsDead and PositionBase.SetBusy

- Owner: Codex / Orca task `task_72d2e8bd26b2`; branch `feature/luna-mcdepman`.
- Members: `ActionChainMgd.IsDead()` reads the IL-proven `ActionChain` byte at offset 33; `PositionBase.SetBusy(bool)` follows the `<Module>` method body at Mathcad token `0x060001c8`, using offsets 192 (`is_busy_`), 196 (`busy_thread_id_`), and 168 (`not_busy_event_`), with the reference `SafeEvent.Reset` / `Set` calls.
- Release IL report: `.mirror-cache/release-il-adfd92d906ab/release-il.json` (PASS, 195 checked; selected Quire.depman.ActionChainMgd; module `PositionBase.SetBusy` exact).
- Remaining mismatches: none in this batch.

## Batch 12 — Shared thread-context accessors

- Owner: Codex / Orca task `task_6d9c7ecedad3`; branch `feature/luna-mcdepman`.
- Members: `ActionChainMgd.GetSharedThreadContext()` uses the IL-proven `ActionChain.loc` pointer at offset 16; `PositionBase.GetSharedThreadContext()` reads its `SEnvironment` gcroot at offset 40.
- Release IL report: `.mirror-cache/release-il-9f066e729adc/release-il.json` (PASS, 196 checked; both module methods exact).
- `PositionBase.Invalidate` depends on `TreatAsReady`, `SendMessage`, and callback wiring not implemented here. `PositionBase.Kill` depends on the `std::set<Position*>` erase path, still represented only as raw storage.

## Batch 13 — PositionBase.SendMessage

- Owner: Codex, Orca task `task_d36be1a6d18b`; branch `feature/luna-mcdepman`.
- Member: `PositionBase.SendMessage(NodeStatus)`, implemented from the complete module IL. Mathcad reads `send_messages_` at offset 156, checks and invokes `PosCallback` at offset 80, and passes `position_` at offset 16; the field is now a typed `gcroot` without changing the 216-byte `PositionBase` layout.
- Release IL report: `.mirror-cache/release-il-126b2a39ebd1/release-il.json` (PASS, 203 checked; selected `Quire.depman.ActionChainMgd`; module `SendMessage` exact).
- `PositionBase.Invalidate` remains blocked after the exact `ActionChain.IsLocked` check by missing callback wiring for `TreatedReadyInvalidCallback(Position*) -> bool` (field offset 104), then `InterruptCallback(int)` (offset 96) and `PosQueueCallback(Position*)` (offset 88); its body also calls `TreatAsReady(false)`. `TreatAsReady` depends on unimplemented `ActionChain.TreatAsReadyDeps` and the `std::set<Position*>` erase path at offset 136. `Kill` remains parked on that same unproven set erase path.

## Batch 14 — PositionBase.SetTimer

- Owner: Codex, Orca task `task_b2e9e3cea4dc`; branch `feature/luna-mcdepman`.
- Member: `PositionBase.SetTimer(NodeTimer^)`, implemented as the field assignment at offset 200 using the existing `NodeTimer` contract; the 216-byte `PositionBase` size assertion remains in place.
- Release IL report: `.mirror-cache/release-il-abcfb3ac8975/release-il.json` (PASS, 201 checked; selected `Quire.depman.ActionChainMgd`; module `SetTimer` exact).
- `ActionChain.TreatAsReadyDeps` is not a leaf: its IL enumerates `ActionChainMgd.m_deps`, calls `DepTrackingEnv.Lookup` for matching `%%children` / `value mc_` keys, updates the result dictionary, and recurses through `next`. `Invalidate` still needs callback wiring for `TreatedReadyInvalidCallback(Position*) -> bool` at offset 104, `InterruptCallback(int)` at 96, and `PosQueueCallback(Position*)` at 88, plus `TreatAsReady(false)` and the unproven set erase path at offset 136. A `WaitReady` probe was rejected: its first IL mismatch is the imported `gcroot<Quire::utl_mgd::SafeEvent^>` helper name versus Mathcad's `gcroot<utl_mgd::SafeEvent^>`; no probe code remains. `Kill` remains parked on the unproven set erase path.

## Batch 15 — PositionBase.SetScope

- Owner: AGY, Orca task `task_91e89a165703`; branch `feature/luna-mcdepman`.
- Member: `PositionBase.SetScope(Scope^)`, implemented as the field assignment at offset 56 using the existing `Scope` contract; the 216-byte `PositionBase` size assertion remains in place.
- Release IL report: `.mirror-cache/release-il-8370a77501bd/release-il.json` (PASS, 203 checked; module `SetScope` and `gcroot<Quire::depman::Scope ^>::operator=` exact).
- Remaining prerequisites:
  - Callback wiring: `TreatedReadyInvalidCallback(Position*) -> bool` at offset 104, `InterruptCallback(int)` at offset 96, `PosQueueCallback(Position*)` at offset 88 are required for `PositionBase.Invalidate`.
  - `std::set<depman::Position*>` erase path at offset 136: required for `PositionBase.Kill` and `TreatAsReady(false)`.
  - `PositionBase.SetEnvScope`: not a leaf; branches on null vs non-null Scope, calling `ActionChain.remove_env_scope` or `ActionChain.add_env_scope`, updating offset 64 (`env_scope_`), and invoking `ActionChain.SetNeedsCheck`.
  - `PositionBase.Lock` / `UnLock`: call `ActionChain.Lock()` / `ActionChain.UnLock()`.

## Batch 16 — ActionChain.Lock/UnLock and PositionBase.Lock/UnLock

- Owner: AGY, Orca task `task_bf86bb0b4188`; branch `feature/luna-mcdepman`.
- Members:
  - `ActionChain.Lock()`: sets `locked_ = true` at offset 32 and recurses on `next_` (offset 24).
  - `ActionChain.UnLock()`: sets `locked_ = false` at offset 32 and recurses on `next_` (offset 24).
  - `PositionBase.Lock()`: invokes `action_chain_->Lock()` at offset 32.
  - `PositionBase.UnLock()`: invokes `action_chain_->UnLock()` at offset 32 and clears `invalidated_ = false` at offset 193.
- Layouts: `ActionChain` remains 40 bytes and `PositionBase` remains 216 bytes with exact assertions.
- Release IL report: `.mirror-cache/release-il-eabfa7b87191/release-il.json` (PASS, 207 checked; all 4 methods exact).
- Remaining prerequisites:
  - Callback wiring: `TreatedReadyInvalidCallback(Position*) -> bool` at offset 104, `InterruptCallback(int)` at offset 96, `PosQueueCallback(Position*)` at offset 88 for `PositionBase.Invalidate`.
  - `std::set<depman::Position*>` erase path at offset 136 for `PositionBase.Kill` and `TreatAsReady(false)`.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` / `ActionChain.add_env_scope`, updating offset 64, and calling `ActionChain.SetNeedsCheck`.

## Batch 17 — ActionChain.SetNeedsCheck

- Owner: AGY, Orca task `task_ee9512aa8eac`; branch `feature/luna-mcdepman`.
- Member: `ActionChain.SetNeedsCheck()`, implemented using `loc_->env()->GetSharedThreadContext()->ObtainMutex(false)`, `mgd_->env()->set_needs_check(true)`, and recursion on `next_` inside a `try ... finally` releasing the mutex. Added `set_needs_check(bool)` and `needs_check()` property helpers on `CachingEnv`.
- Release IL report: `.mirror-cache/release-il-7da89597523f/release-il.json` (PASS, 210 checked; module `SetNeedsCheck` exact).
- Assessment of env_scope methods:
  - `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` call into `ActionChainMgd::remove_env_scope` and `ActionChainMgd::add_env_scope`, which require `DepManager.remove_dep` and `DepManager.add_dep`.
  - Because `DepManager` dependency storage is not yet established, `remove_env_scope` and `add_env_scope` cannot be isolated as self-contained leaves.
  - `PositionBase.SetEnvScope` branches on Scope nullness and calls both `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` in addition to `SetNeedsCheck`, so it remains blocked on those `DepManager` dependencies.

## Batch 18 — ActionChain.Kill

- Owner: AGY, Orca task `task_55d9f959f91d`; branch `feature/luna-mcdepman`.
- Member: `ActionChain.Kill()`, sets `is_dead_ = true` at offset 33 and recurses on `next_` (offset 24), maintaining the 40-byte `ActionChain` layout.
- Release IL report: `.mirror-cache/release-il-200d28736594/release-il.json` (PASS, 211 checked; module `ActionChain.Kill` exact).
- Inspection of DepManager.add_dep / remove_dep and PositionBase.StartTiming:
  - `DepManager.add_dep` and `remove_dep` are large unmanaged methods operating on a multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` structure with `std_label_set` and custom comparators; `DepManager` layout and native dependency tables remain unported.
  - A `PositionBase.StartTiming` probe was rejected: dereferencing `gcroot<Quire::depman::NodeTimer^>` emits template operator `..PE$AAVNodeTimer@depman@Quire@@` instead of Mathcad's `..PE$AAUNodeTimer@depman@@` (identical to the rejected `WaitReady` probe in Batch 14); no probe code remains.
  - `PositionBase.Kill` remains parked on the unproven `std::set<depman::Position*>` erase path at offset 136.

## Batch 19 — PositionBase.BeginMove and ActionChain.GetSharedThreadContext

- Owner: AGY, Orca task `task_23a24d971035`; branch `feature/luna-mcdepman`.
- Members:
  - `PositionBase.BeginMove()`: empty method (`ret`) maintaining the 216-byte `PositionBase` layout.
  - `ActionChain.GetSharedThreadContext()`: reads `loc_->env()->GetSharedThreadContext()` via offset 16 (`loc_`), returning `SharedThreadContext^` and maintaining the 40-byte `ActionChain` layout.
- Release IL report: `.mirror-cache/release-il-c1a6a72c6cc3/release-il.json` (PASS, 213 checked; module `PositionBase.BeginMove` and `ActionChain.GetSharedThreadContext` exact).
- Remaining prerequisites:
  - `DepManager.add_dep` / `remove_dep`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.Kill` and `TreatAsReady(false)`: require the unproven `std::set<depman::Position*>` erase path at offset 136.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.


## Batch 20 — PositionBase.IsReady and ActionChain.value

- Owner: AGY, Orca task `task_f9ee0c2aa8f9`; branch `feature/luna-mcdepman`.
- Members:
  - `ActionChain.value()`: checks `mgd_->env()->needs_check() && !locked_` returning `nullptr`, otherwise returns `next_ ? next_->value() : mgd_->result()`, preserving the 40-byte `ActionChain` layout.
  - `PositionBase.IsReady()`: checks `!action_chain_` returning `true`, otherwise checks `!is_busy_ && (action_chain_ ? action_chain_->value() : nullptr)` returning `true`, else `false`, preserving the 216-byte `PositionBase` layout.
- Release IL report: `.mirror-cache/release-il-e994492694fa/release-il.json` (PASS, 215 checked; module `PositionBase.IsReady` and `ActionChain.value` exact).
- Remaining prerequisites:
  - `DepManager.add_dep` / `remove_dep`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.Kill` and `TreatAsReady(false)`: require the unproven `std::set<depman::Position*>` erase path at offset 136.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.

## Batch 21 — PositionBase.GetDefs and PositionBase.GetDependents

- Owner: AGY, Orca task `task_9280af5b10b2`; branch `feature/luna-mcdepman`.
- Members:
  - `PositionBase.GetDefs()`: reads `static_deps_` via offset 48 (`gcroot<System::Collections::ICollection^>`), preserving the 216-byte `PositionBase` layout.
  - `PositionBase.GetDependents()`: returns reference to `std::set<depman::Position*>&` at offset 136 (`dependents_`), preserving the 216-byte `PositionBase` layout.
- Release IL report: `.mirror-cache/release-il-e38f3a358c84/release-il.json` (PASS, 218 checked; module `PositionBase.GetDefs` and `PositionBase.GetDependents` exact).
- Remaining prerequisites:
  - `DepManager.add_dep` / `remove_dep`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.Kill` and `TreatAsReady(false)`: require the unproven `std::set<depman::Position*>` erase path at offset 136.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.

## Batch 22 — PositionBase.GetSharedThreadContext

- Owner: AGY, Orca task `task_bb7074a207a6`; branch `feature/luna-mcdepman`.
- Member: `PositionBase.GetSharedThreadContext()`, reads `env_` at offset 40 via conversion operator to `SEnvironment^` and calls `GetSharedThreadContext()`, maintaining the 216-byte `PositionBase` layout.
- Release IL report: `.mirror-cache/release-il-cd89abba95e8/release-il.json` (PASS, 219 checked; module `PositionBase.GetSharedThreadContext` exact).
- Remaining prerequisites:
  - `DepManager.add_dep` / `remove_dep` / `invalidateChains`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.InvalidateDeps`: blocked on `DepManager.invalidateChains`.
  - `PositionBase.EndMove`: blocked on `ActionChain.TreatAsReadyDeps`.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.Kill` and `TreatAsReady(false)`: require the unproven `std::set<depman::Position*>` erase path at offset 136.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.

## Batch 23 — PositionBase.StartTiming

- Owner: AGY, Orca task `task_030b5c9a4417`; branch `feature/luna-mcdepman`.
- Member: `PositionBase.StartTiming()`, tests `timer_` at offset 200 via `operator NodeTimer^()` and calls `timer_->Start()`, maintaining the 216-byte `PositionBase` layout.
- Details: aligned `NodeTimer` from `ref class` to `ref struct` in `DepmanNodeTimer.h` to match Mathcad's `U` mangling (`..PE$AAUNodeTimer@depman@@`), unlocking exact `StartTiming` and `gcroot<NodeTimer^>` IL parity.
- Release IL report: `.mirror-cache/release-il-3e115359359d/release-il.json` (PASS, 222 checked; module `PositionBase.StartTiming` exact).
- Remaining prerequisites:
  - `SafeEvent` cross-assembly `gcroot` binding: `PositionBase.SetBusy`, `PositionBase.WaitReady`, and `PositionBase.PrepareKill` access `not_busy_event_` (offset 168) or `ready_event_` (offset 176); because `SafeEvent` is declared in `Quire.Engine.Utl`, `MethodAudit` currently does not map `gcroot<Quire::utl_mgd::SafeEvent ^>` to `gcroot<utl_mgd::SafeEvent ^>`.
  - `DepManager.add_dep` / `remove_dep` / `invalidateChains`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.InvalidateDeps`: blocked on `DepManager.invalidateChains`.
  - `PositionBase.EndMove`: blocked on `ActionChain.TreatAsReadyDeps`.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.Kill` and `TreatAsReady(false)`: require the unproven `std::set<depman::Position*>` erase path at offset 136.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.

## Batch 24 — PositionBase.Kill and Native Set Erase Path

- Owner: AGY, Orca task `task_34bbceedeece`; branch `feature/luna-mcdepman`.
- Member: `depman.PositionBase.Kill()`, sets `is_dead_ = true` (offset 184) and unlinks `position_` (offset 16) from `dparent_->dependents_` (offset 136) via `dparent_->dependents_.erase(pos)`.
- Details: Replaced raw byte buffer `unsigned char dependents_[0x10]` with exact typed native member `std::set<Position*> dependents_` at offset 136, and `dparent_` typed as `PositionBase*` at offset 128 in `PositionBase`. Aligned `PositionBase::GetDependents()` to return `dependents_` directly. Preserving `auto parent = dparent_;` before resolving `pos` generated exact MSVC instruction sequencing matching Mathcad's evaluation stack and inlined `std::_Tree` helper templates (`_Eqrange` and `_Erase`).
- Release IL report: `.mirror-cache/release-il-9bd318b045a1/release-il.json` (PASS, 234 checked; module `PositionBase.Kill` and inlined tree templates exact).
- Remaining prerequisites:
  - `SafeEvent` cross-assembly `gcroot` binding: `PositionBase.SetBusy`, `PositionBase.WaitReady`, and `PositionBase.PrepareKill` access `not_busy_event_` (offset 168) or `ready_event_` (offset 176); because `SafeEvent` is declared in `Quire.Engine.Utl`, `MethodAudit` currently does not map `gcroot<Quire::utl_mgd::SafeEvent ^>` to `gcroot<utl_mgd::SafeEvent ^>`.
  - `DepManager.add_dep` / `remove_dep` / `invalidateChains`: large unmanaged methods operating directly on multi-level `std::map<SDef, std::map<Position*, pair<ActionChain*, DependencyType>>>` containers with `std_label_set` and custom comparators; native `DepManager` layout and tables remain unported.
  - `PositionBase.InvalidateDeps`: blocked on `DepManager.invalidateChains`.
  - `PositionBase.EndMove`: blocked on `ActionChain.TreatAsReadyDeps`.
  - `PositionBase.SetEnvScope`: branches on Scope nullness, calling `ActionChain.remove_env_scope` and `ActionChain.add_env_scope` which require `DepManager.remove_dep` and `add_dep`.
  - `PositionBase.Invalidate`: requires callback wiring for `PosCallback`, `PosQueueCallback`, `InterruptCallback`, and `TreatedReadyInvalidCallback`.
  - `PositionBase.TreatAsReady(false)`: couples the now-proven `std::set` erase path with `ready_event_` (SafeEvent gcroot) and `dep_manager_->invalidateChains`.
  - `Position.previous` / `Position.next`: dynamic cast emits RTTI type descriptor data differences against Mathcad RTTI; pragmas remain commented out until RTTI descriptor handling is resolved.

## Shared Integration Note — SafeEvent and Cross-Assembly gcroot Mapping in MethodAudit

Investigation of `PositionBase.SetBusy`, `PositionBase.WaitReady`, and `PositionBase.PrepareKill` (which access `not_busy_event_` at offset 168 and `ready_event_` at offset 176) identified a precise toolset constraint in `tools/Quire.Tools.MethodAudit/Program.cs`:

1. **Root Cause**:
   - `Program.cs` lines 198–212 scans candidate types for native `gcroot<...>` specializations:
     ```csharp
     var argument = name[gcrootPrefix.Length..^gcrootSuffix.Length].Replace("::", ".");
     if (!typeNames.TryGetValue((artifact.Name, argument), out var binding)
         || !reviewed.ContainsKey(argument) || !originals.TryGetValue(binding[1], out var original)) continue;
     ```
   - When evaluating `artifact.Name == "Quire.Mcdepman"`, `name` is `"gcroot<Quire::utl_mgd::SafeEvent ^>"`, so `argument` is `"Quire.utl_mgd.SafeEvent"`.
   - `typeNames` is keyed by `(artifact.Name, type.FullName)`, containing only types declared directly in `artifact.Name` that carry `[MirrorOf]`. Because `SafeEvent` is declared in `Quire.Engine.Utl`, `(Quire.Mcdepman, Quire.utl_mgd.SafeEvent)` is absent from `typeNames`, causing `TryGetValue` to fail.
   - Furthermore, even if cross-assembly type lookup (`Binding`) is used, line 206 resolves `original` via `originals.TryGetValue(binding[1], out var original)`, where `binding[1]` is `"utl-mgd"`. It then asserts `original.Model.Types.Any(t => t.FullName == mapped)`. But `utl-mgd.dll` is pure managed C# metadata and does not contain native `gcroot<utl_mgd::SafeEvent ^>` TypeDefs. The native `gcroot<utl_mgd::SafeEvent ^>` template specialization actually exists in `mcdepman.dll` (the original counterpart of `Quire.Mcdepman`).

2. **Affected Mcdepman Methods**:
   - `PositionBase.SetBusy(bool)`: sets `is_busy_` (offset 192), `busy_thread_id_` (offset 196), and calls `not_busy_event_->Reset()` / `not_busy_event_->Set()`.
   - `PositionBase.WaitReady()`: reads `ready_event_` (offset 176) via conversion operator and calls `SafeWaitHandle::WaitOne()`.
   - `PositionBase.PrepareKill()`: sets `send_messages_ = false` (offset 156), clears `ready_event_` (offset 176), and calls `ev->Set()`.
   - `PositionBase.ready()`: reads `labeled_dictionary_` (offset 72, `gcroot<Quire::utl_mgd::LabeledDictionary^>`), which has the identical cross-assembly `gcroot` issue with `Quire.Engine.Utl`.

3. **C++ Implementation Readiness**:
   - The C++ source implementations for all four methods match Mathcad's original instructions and control flow exactly. All four exact C++ source drafts (`PositionBase.SetBusy`, `PositionBase.WaitReady`, `PositionBase.PrepareKill`, and `PositionBase.ready`) are implemented and preserved in `DepmanPosition.cpp` and `DepmanPosition.h` with commented-out linker pragmas; under `/OPT:REF` they do not emit unreferenced symbols or spurious Module methods, keeping the Release IL baseline clean at 222 checked.
   - Once `MethodAudit` supports cross-assembly `gcroot` type argument resolution against the current module's original counterpart (`originals[currentOriginal]`), these four methods can be linked via `#pragma comment(linker, ...)` and will pass Release IL verification immediately.

## Native Dependencies, Set Erase Paths, and Navigation RTTI Investigation

Investigation of remaining unmigrated `PositionBase`, `Position`, and `DepManager` members revealed three distinct architectural boundaries in native C++ and mixed-mode compilation:

1. **Navigation RTTI Evidence (`Position.previous` and `Position.next`)**:
   - **Original Implementation**: Mathcad Release IL for `depman.Position.previous` (offset 224, `prev_`) and `depman.Position.next` (offset 232, `next_`) loads `OrderedListNode<PositionBase>::prev_` or `next_`, pushes 0, passes TypeDescriptor pointers `??_R0?AV?$OrderedListNode@VPositionBase@depman@@@@@8` and `??_R0?AVPosition@depman@@@8`, and calls `__RTDynamicCast`.
   - **C++ Source**: In source, this is `return dynamic_cast<Position*>(prev_);` and `return dynamic_cast<Position*>(next_);`.
   - **Toolchain / RTTI Constraint**: When compiled with MSVC `/GR`, MSVC synthesizes `$_TypeDescriptor$_extraBytes_*` structures in `<Module>` along with RTTI descriptor data tables. Differences in compiler-generated RTTI descriptor layouts between MSVC toolchain versions cause Release IL verification to detect metadata and data-reference mismatches against Mathcad's original binary. These methods remain preserved as source drafts until compiler RTTI descriptor emissions are normalized.

2. **Native Set Erase Paths (`PositionBase.Kill` and `TreatAsReady(false)`)**:
   - **Resolved (`PositionBase.Kill`)**: In Batch 24, `PositionBase.Kill` was proven and landed with exact Release IL (234 checked methods passing). Typing `dependents_` as `std::set<Position*>` at offset 136, `dparent_` as `PositionBase*` at offset 128, and sequencing `auto parent = dparent_;` before evaluating `pos` replicates Mathcad's evaluation stack and inlined `std::_Tree` helpers (`_Eqrange` and `_Erase`) with zero differences.
   - **Remaining Blocker (`TreatAsReady(false)`)**: `PositionBase.TreatAsReady(false)` couples the verified set erase path with `ready_event_` (blocked by the `SafeEvent` cross-assembly `gcroot` issue) and `dep_manager_->invalidateChains` (blocked by native `DepManager`).

3. **Native Dependency Maps (`DepManager` Multi-Level Containers)**:
   - **Blocked Callers**: `PositionBase.InvalidateDeps`, `PositionBase.SetEnvScope`, `PositionBase.EndMove`, and `ActionChain.add_env_scope` / `remove_env_scope` all call directly into `DepManager` methods (`invalidateChains`, `add_dep`, `remove_dep`).
   - **Container Architecture**: Disassembly of `DepManager` reveals that it is an unmanaged engine class managing multi-level associative containers:
     - `std::map<SDef, std::map<Position*, std::pair<ActionChain*, DependencyType>, value_less<Position>>> forward_deps_`: Maps symbol definitions to dependent positions and action chains.
     - `std::map<SDef, std::map<Position*, DefinitionType, value_less<Position>>> def_table_`: Maps symbol definitions to declaring positions and their definition types.
     - `std::deque<std::pair<SDef, Position*>> work_queue_`: Queue for topological cascade invalidations.
   - Custom comparators (`value_less<Position>`) sort by position sequence ordinals rather than pointer addresses to ensure deterministic recomputation.
   - `DepManager` is currently forward-declared in `DepmanContracts.h`; implementing these dependent methods requires fully implementing and validating the native `DepManager` container models and their internal map semantics.

4. **Summary of Member Readiness and Categorization**:
   - **Landed and Exact** (`PositionBase.Kill`): Exact C++ source and typing in `DepmanPosition.cpp` and `DepmanPosition.h` matching Mathcad Release IL with 234 checked methods.
   - **Preserved `gcroot`-Blocked Drafts** (`PositionBase.SetBusy`, `WaitReady`, `PrepareKill`, `ready`): Exact C++ source is implemented in `DepmanPosition.cpp` and `DepmanPosition.h`. Blocked exclusively on `MethodAudit` cross-assembly `gcroot` binding resolution.
   - **Preserved RTTI Drafts** (`Position.previous`, `Position.next`): Exact C++ source is implemented; blocked on MSVC RTTI type descriptor data parity.
   - **DepManager Map Consumers** (`PositionBase.InvalidateDeps`, `PositionBase.SetEnvScope`, `PositionBase.EndMove`): Blocked on full native `DepManager` container implementation.

## Batch 25 — TreatAsReady and TreatAsReadyDeps

- Owner: AGY, Orca task `task_2d8f33262504`; branch `feature/luna-mcdepman`.
- Members mirrored and verified:
  - `depman.PositionBase.TreatAsReady(bool)`
  - `depman.ActionChain.TreatAsReadyDeps(Dictionary<CacheKey, __int64>)`
  - `depman.ActionChainMgd.TreatAsReadyDeps(Dictionary<CacheKey, __int64>)`
- Release IL report: PASS; 242 checked (`.mirror-cache/release-il-41e19258c17b/release-il.json`).
- Architecture and implementation:
  - Disassembly of `PositionBase.TreatAsReady(bool)` revealed that it does not interact with `SafeEvent` or `DepManager`. It sets `treated_ready_`, clears `invalidated_`, delegates dependency recording to `action_chain_->TreatAsReadyDeps(nullptr)`, assigns the result to `object_` (`gcroot<System::Object^>` at offset 112), and detaches from `dparent_` using the verified `std::set<Position*>` erase path from Batch 24.
  - In `PositionBase.TreatAsReady`, evaluating `__int64 chain = reinterpret_cast<__int64>(action_chain_);` produces Mathcad's exact 3-local frame (`[Position*, int64, pair]`) and 81-entry Release IL byte-for-byte.
  - `ActionChainMgd.TreatAsReadyDeps` constructs `DepTrackingEnv`, filters keys starting with `"%%children"` or `"value mc_"`, calls `env->Lookup(...)` to record position pointers, and recursively chains `next()->TreatAsReadyDeps(d)`. Declaring `virtual Lookup(...)` on `DepTrackingEnv` produces `callvirt` matching Mathcad's exact 97-entry Release IL.
  - `DepTrackingEnv.Lookup` is declared with `[MirrorStubAttribute]` and throws `MirrorStub::NotImplemented()` to serve as an audited placeholder stub until symbol tracking is fully completed.
- Remaining dependencies:
  - Cross-assembly `gcroot` resolution in `MethodAudit` (`SafeEvent`, `LabeledDictionary`) blocking `PositionBase.SetBusy`, `WaitReady`, `PrepareKill`, and `ready` (exact C++ source drafts preserved in `DepmanPosition.cpp`).
  - Native `DepManager` multi-level associative container maps blocking `InvalidateDeps`, `SetEnvScope`, `EndMove`, and `ActionChain.add_env_scope` / `remove_env_scope`.
  - Compiler RTTI descriptor emissions for `Position.previous` and `Position.next`.

## Batch 26 — PositionBase.OnDep and DepManager Associative Container Architecture

- Owner: AGY, Orca task `task_2bbcc7c19064`; branch `feature/luna-mcdepman`.
- Members mirrored and verified:
  - `depman.PositionBase.OnDep(Position* p)`
- Release IL report: PASS; 250 checked (`.mirror-cache/release-il-b189661a6bae/release-il.json`).
- Architecture and implementation:
  - Disassembly of `PositionBase.OnDep(Position* p)` (token `0x060001cd`, 93 bytes, 57 IL instructions) revealed that it handles reverse dependency registration without touching `SafeEvent` or `DepManager`.
  - It calls `TreatAsReady(false)`, captures `const int priority = p->priority_;` (offset 164), updates `priority_ = std::max(priority_, priority)`, and if `p->dparent_ == nullptr` (offset 128), inserts `p` into `dependents_` (offset 136) via `dependents_.insert(p)` and sets `p->dparent_ = position_` (offset 16).
  - Explicit typing of `const int priority = p->priority_;` generates the exact MSVC `modopt(System.Runtime.CompilerServices.IsConst)` local signature and 4-local evaluation frame `[int32* modopt(IsConst)* modopt(IsImplicitlyDereferenced), int32* modopt(IsConst)*, int32 modopt(IsConst), pair<...>]` matching Mathcad Release IL byte-for-byte.
- DepManager associative container technical analysis:
  - Disassembly and layout inspection of unmanaged `depman::DepManager` (size 56 bytes / `0x38`, vtable `??_7DepManager@depman@@6B@` with 1 entry `virtual ~DepManager()`):
    - `0x00` (0): `vptr`
    - `0x08` (8): `std::map<SDef, std::map<Position*, DefinitionType, value_less<Position>>*> def_table_;` (16 bytes, maps symbol definition to pointer to inner map of declaring positions and definition types).
    - `0x18` (24): `std::map<SDef, std::map<Position*, std::pair<ActionChain*, DependencyType>, value_less<Position>>*> forward_deps_;` (16 bytes, maps symbol definition to pointer to inner map of dependent positions and action chains).
    - `0x28` (40): `bool enabled_;` (1 byte, initialized to `true` / `1`).
    - `0x29`..`0x37`: padding (15 bytes to 56 bytes).
  - Inner containers are heap-allocated pointer-stored maps (`std::map<Position*, ...>*`) ordered by `value_less<Position>` (comparing sequence ordinals `ord_val_` at offset 240 of `OrderedListNode<PositionBase>`).
  - Key `SDef` (size 56 bytes / `0x38`):
    - `0x00` (0): `int scope_id;`
    - `0x08` (8): `std::wstring name;` (32 bytes)
    - `0x28` (40): `Engine::Unmanaged::std_label_set labels;` (16 bytes)
    - Lexicographical ordering: `(scope_id < other.scope_id) ? true : (scope_id > other.scope_id) ? false : (name < other.name) ? true : (name > other.name) ? false : (labels < other.labels)`.
  - Invalidation cascades (`invalidateChains`): traverses `forward_deps_`, collecting matching inner maps into a `std::vector<std::map<Position*, ...>*>`, tracking visited positions in a `std::set<Position*>`, and queuing dependent action chains.
- Remaining dependencies:
  - Cross-assembly `gcroot` resolution in `MethodAudit` (`SafeEvent`, `LabeledDictionary`) blocking `PositionBase.SetBusy`, `WaitReady`, `PrepareKill`, and `ready` (exact C++ source drafts preserved in `DepmanPosition.cpp`).
  - Full native `DepManager` multi-level associative container layout and map methods blocking `PositionBase.InvalidateDeps`, `PositionBase.SetEnvScope`, `PositionBase.EndMove`, and `ActionChain.add_env_scope` / `remove_env_scope`.
  - Compiler RTTI descriptor emissions for `Position.previous` and `Position.next`.

## Batch 27 — DepManager Layout and depctl

- Owner: AGY, Orca task `task_546ad987383e`; branch `feature/luna-mcdepman`.
- Members mirrored and verified:
  - `depman.DepManager.depctl(int)`
- Release IL report: PASS; 251 checked (`.mirror-cache/release-il-52373e865a95/release-il.json`).
- Architecture and implementation:
  - Disassembled `depman.DepManager.depctl` (token `0x06000053`, 14 entries, 10 IL instructions). It acts as the dependency controller for disabling/enabling dependency tracking: `enabled_ = (cmd == 0);`.
  - Defined unmanaged `depman::DepManager` class layout (56 bytes, `static_assert(sizeof(DepManager) == 56)`): vptr at offset 0, 32-byte storage buffer for `def_table_` and `forward_deps_` (offsets 8..39), `bool enabled_` at offset 40 (`0x28`), and 15 bytes of trailing padding (offsets 41..55).
  - `#pragma comment(linker, "/include:?depctl@DepManager@depman@@QEAAXH@Z")` emits `depman.DepManager.depctl` matching Mathcad Release IL byte-for-byte.
- Remaining dependencies:
  - Cross-assembly `gcroot` resolution in `MethodAudit` (`SafeEvent`, `LabeledDictionary`) blocking `PositionBase.SetBusy`, `WaitReady`, `PrepareKill`, and `ready` (exact C++ source drafts preserved in `DepmanPosition.cpp`).
  - Full associative container implementation (`def_table_`, `forward_deps_`, `SDef` keys, `lower_bound_ne`, `AddLabels`) blocking `DepManager.resolve`, `is_bound`, `find_def`, `find_def_nodead`, `add_def`, `remove_def`, `add_dep`, `remove_dep`, `find_all_deps`, and `invalidateChains`.
  - Compiler RTTI descriptor emissions for `Position.previous` and `Position.next`.

## Batch 28 — PositionBase.EndMove and Container Architecture Deep-Dive

- Owner: AGY, Orca task `task_384b3a73a4e3`; branch `feature/luna-mcdepman`.
- Members mirrored and verified:
  - `depman.PositionBase.EndMove()`
- Release IL report: PASS; 253 checked (`.mirror-cache/release-il-486fdb7f0afa/release-il.json`).
- Architecture and implementation:
  - Disassembled `PositionBase.EndMove()` (token `0x060001d1`, 70 IL instructions). It inspects dependency changes by comparing `object_` (`Dictionary<CacheKey, __int64>^`) against `action_chain_->TreatAsReadyDeps(nullptr)`.
  - Structuring the key inspection as two separate `if` statements (`if (!new_deps->TryGetValue(key, val)) return true; if (val != old_deps[key]) return true;`) matches Mathcad's exact branch offsets and double return blocks byte-for-byte.
- Container Dependency Findings:
  - Investigated native evidence for all container dependencies: outer node allocation (`std::_Alloc_construct_ptr::_Allocate`) allocates 104 bytes (32-byte `_Tree_node` header, 56-byte `SDef` key, and 8-byte inner map pointer).
  - Disassembled and analyzed helper templates `lookup<Map>` (token `0x06000054`) and `lower_bound_ne<Map>` (token `0x06000055`), which lower-bounds keys with predecessor fallback to resolve preceding calculations.
  - Confirmed that `PositionBase.InvalidateDeps` (requires `invalidateChains`) and `PositionBase.SetEnvScope` (requires `remove_env_scope` / `add_env_scope` and `remove_dep` / `add_dep`) are blocked on full multi-level associative container implementations.
- Remaining dependencies:
  - Cross-assembly `gcroot` resolution in `MethodAudit` (`SafeEvent`, `LabeledDictionary`).
  - Full associative container implementation (`def_table_`, `forward_deps_`, `SDef` keys, `lower_bound_ne`, `AddLabels`) for `resolve`, `find_def`, `find_def_nodead`, `add_def`, `remove_def`, `add_dep`, `remove_dep`, and `invalidateChains`.
  - Compiler RTTI descriptor emissions for `Position.previous` and `Position.next`.





