# Mcfront12 lane checkpoint

## Parked `NodeMap.GetLastCENode` codegen check (2026-11-25)

- Rechecked current `main`: `release_il.py --base main --project Quire.Mcfront12` passes with 251 checked and 0 failures; 77 members have no Quire pair.
- A C++/CLI body can produce the boxed-`Int32` return signature, but still differs in jump-threaded control flow (49 vs. 47 IL entries) and `GC.KeepAlive` placement. Reverted the experiment; keep this member parked.

- Owner: AGY, Orca task `task_5ef210cf3d6a`.
- Branch: `feature/mcfront12-agy`.
- Members: Quire.Mcfront12 project split established as C++/CLI (`/clr:NetCore` targeting net10.0-windows); independent engine support types implemented matching `mcfront12.dll`:
  - `Quire.Engine.Direction` (native C++ enum)
  - `Quire.Engine.PathComponent` (sequential value struct)
  - `Quire.Engine.DCount` (`.ctor`)
  - `Quire.Engine.Empty` (`List`, `Set`, `.ctor`, `.cctor`)
  - `Quire.Engine.IMode` (`BEFORE`, `AFTER`, `CHILD`)
  - `Quire.Engine.Task` (`Do`)
  - `Quire.Engine.TaskDelegate0` (`.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke`)
  - `Quire.Engine.Task0` (`.ctor`, `Do`)
  - `Quire.Engine.TaskDelegate1`1`..`TaskDelegate6`6` (`.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke`)
  - `Quire.Engine.Task1`1`..`Task6`6` (`.ctor`, `Do`)
  - `Quire.Engine.DefinitionAction` (`.ctor`, `~DefinitionAction`, `Invoke`, `GetDefs`, `IsUndefine`, `IsChildOnly`, `ToString`, `Dispose(bool)`, `Dispose()`)
  - `Quire.Engine.NodeData` (`.ctor`, `SetZone`, `SetError`, `Update`, `Handle`, `IsGlobal`, `GetDefinitions`)
  - `Quire.Engine.NormalDefinition` (`.ctor`, `GetDefinitions`)
  - `Quire.Engine.GlobalDefinition` (`.ctor`, `IsGlobal`)
  - `Quire.Engine.UnaryNodeDataFunc` (delegate)
  - `Quire.Engine.NodeTracker` (interface)
  - `Quire.Engine.IFactoryCallbacks` (`OnCreate`, `OnDestroy`)
  - `Quire.Engine.HandleFactory` (`.ctor`, `NewHandle`, `IsValid`)
  - `Quire.Engine.HasErrors` (`.ctor`, `Invoke`)
  - `Quire.Engine.TagEncoder` (`.ctor`, `encode`)
  - `Quire.Engine.RekeyDict` (`.ctor`, `Invoke`)
  - `Quire.Engine.MergeState` (`.ctor`, `Invoke`)
  - `Quire.Engine.GetRuntimeOptionsNumber` (`.ctor`, `Invoke`)
  - `Quire.Engine.NodeCollection` (`.ctor`, `get_Item`, `get_Count`, `Add`)
  - `Quire.Engine.FScope` (`.ctor(owner)`, `.ctor()`, `get_Parent`, `get_Id`, `First`, `FindPreviousInScope`, `FindNextInScope`, `Last`, `Owner`, `OnFirst`, `OnNew`, `OnBefore`, `OnAfter`, `OnRemove`, `.cctor`)
  - `Quire.Engine.NodeNavigation` (`NavigateTo`, `GetParent`, `GetChildren`, `GetPrevious`, `GetNext`, `Exists`, `mapNodes_`)
  - `Quire.Engine.NodeMap` (`last_ce_node`, `_DeleteCENode`, `ll_move_before`, `ll_move_after`, `scope_detach`, `ce_sync`, `ce_sync_child`, `get_children`, `RemoveFrontNode`, `DeleteCENodes`, `First`, `.ctor`, `GetFrontNode`, `NewCENodeGen`, `UpdateCENode`, `DeleteCENode`, `GetCENodes`, `LockCENodes`, `LockCEInvalidNodes`, `UnLockCENodes`, `LockCENodesAfterNode`, `InsertFrontNode`, `MoveFrontNode`, `GetChildren`, `GetChild`, `GetParent`, `GetPrevious`, `GetNext`, `Exists`, `GetTopNodes`, `mapNodes_`, `NewScope`, `NewChildCollection`, `NodePath`)
  - `Quire.Engine.GlobalMap` (`.ctor`, `OnUpdate`, `BeforeRemove`, `AfterRemove`, `BeforeMove`, `AfterMove`, `OnOutOfDate`, `OnRemove`, `StartTracking`, `StopTracking`, `AddDefinition`, `RemoveDefinition`, `GetGlobals`, `CheckGlobals`, `GetParent`, `GetTopAncestor`, `IsTopLevel`, `FilterTopLevel`, `OperateRecursively`, `SetZoneRecursive`, `OnRemoveRecursive`, `Update`, `.cctor`)
  - `Quire.Engine.SetZone` (`.ctor`, `Operate`)
- Candidate Release IL report: `.mirror-cache/release-il-a8dbb2ccd0c7/release-il.json`.
- Gate line: `Release IL: PASS; 210 checked; C:\Users\DuyDang\orca\workspaces\Quire\mslisp-agy\.mirror-cache\release-il-a8dbb2ccd0c7\release-il.json`.
- Classification: With `Quire.Engine` types carrying `[MirrorOf("Engine.<Type>, mcfront12")]`, `ModuleOriginal` resolves `mcfront12.dll` cleanly. CRT and Module support methods are classified as toolset support (D19/D23), and all 6 native bodies match exact instruction-for-instruction and target bytes (including `__security_init_cookie` with hash-bound data target landed in `eded0f976`).
- Rule result: Release IL gate passed with 210 checked, 0 failures, 6 native pairs matching.
- Exact MirrorGen changes: none required; `Rules.cs` already maps `mcfront12` to `Quire.Mcfront12` and recognizes C++/CLI via `IsCppProject`.
- Remaining mismatches: 0 gate failures; 116 unmapped/missing members in mcfront12 (113 in <Module> / AST scanner / MCFront12 singleton, plus 3 checkpointed NodeMap members: NewCENode, NewCENodeTopTop, GetLastCENode omitted due to compiler-inserted KeepAlive / jump-threading codegen differences).
