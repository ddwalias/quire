# Quire.Mcd lane checkpoint

Owner: dispatched worker, task `task_485dc1a8580c`; branch `mirror/quire-mcd-20261116`.

## Batch 1 — NodeQueue and Scope abstract declarations
- Scope: 14 bodyless interface method declarations on `depman.NodeQueue` (`StartWorkers`, `StopWorkers`, `GetRuntimeOptions`, `SetNumThreads`, `GetNumThreads`, `Interrupt`, `SetAutoCalc`, `GetAutoCalc`, `SetCalcNow`, `GetCalcNow`, `BeginTransaction`, `EndTransaction`, `GetUpToDateMediator`, `GetBusyMediator`) and 2 abstract property declarations on `depman.Scope` (`Id`, `Parent`).
- Hand-written partials: `src/Quire.Mcd/Mirrored/depman.NodeQueue.cs` and `src/Quire.Mcd/Mirrored/depman.Scope.cs`. Removed corresponding stubs from `src/Quire.Mcd/Generated/Mirror/depman.NodeQueue.g.cs` and `depman.Scope.g.cs`.
- Release IL: PASS; 16 checked (`.mirror-cache/release-il-a6291be4621b/release-il.json`, `.mirror-cache/release-il-6d1a2ebc6e16/release-il.json`).
- Commit: pending landing.

## Remaining members and exact blockers
All remaining 570 stubs in `Quire.Mcd` are blocked from matching Release IL when compiled as C# in `Quire.Mcd.csproj`:
1. `depman.IDepman` (25 stubs):
   - 24 methods (`AddAfter`, `AddAfterWithDef`, `AddBefore`, `MoveAfter`, `MoveBefore`, `Remove`, `Update`, `Lock`, `LockInvalid`, `UnLock`, `SetEnvScope`, `SetPriority`, `GetPriority`, `TreatAsReady`, `SetTimer`, `SetNotifyComputing`, `GetValueAt`, `IsBoundAt`, `GetAllValues`, `Ready`, `GetDefined`, `GetDefinition`, `GetDependencies`, `GetDependents`): blocked by original MSVC C++/CLI signature emitting `modopt(System.Runtime.CompilerServices.IsLong)` on return/parameters (`long`), which Roslyn C# cannot emit.
   - 1 method (`UnLockP(Position* p)`): blocked because Mathcad interface method is public, but in C# `Position` is an internal struct, triggering compiler error CS0051 (inconsistent accessibility) or CS8703 (internal interface member requires body).
2. `depman.NodeQueue` (4 stubs):
   - 4 methods (`Enqueue(Position*)`, `Enqueue(Position*, EnqueueCode)`, `Dequeue(Position*)`, `SetPriority(Position*, int)`): blocked by public interface signatures exposing internal types `Position*` and `EnqueueCode`, causing CS0051 / CS8703 in C#.
3. `depman.Scope` (3 stubs):
   - `GetHashCode`, `Equals`, `.ctor`: blocked by Roslyn emitting `.maxstack 8` vs Mathcad C++/CLI `.maxstack 1` (`.ctor`, `GetHashCode`) and `.maxstack 2` (`Equals`), plus `conv.u1` and `GC.KeepAlive(this)` on `Equals`.
4. Remaining `depman.*` classes with bodies (279 stubs across `Aborter`, `ActionChainMgd`, `ActionResult`, `BooleanMediatorImp`, `Depman`, `NodeQueueImp`, `ThreadData`, etc.):
   - Blocked by C++/CLI metadata differences: minimal `.maxstack 1/2`, `modopt(IsLong)`, destructors/finalizers (`_tilde...`, `_bang...`), sized deallocation, and `GC.KeepAlive`. Per D20, these members are in the process of migration to the C++/CLI project `Quire.Mcdepman` (see `docs/mirror/work/lane-mcdepman-cpp.md`).
5. All `Mcd*.values` MPL modules (259 stubs across `McdTranslator.values`, `McdDynamicEfi.values`, `McdUnitSystem*.values`, `McdEfi.values`):
   - Blocked by Mathcad's MPL compiler (`mplc`) emitting `.maxstack 48` on every method and MPL runtime attributes, while Roslyn C# emits `.maxstack 8`. Per D20, these belong to standalone MPL projects (`Quire.McdTranslator`, etc.).
