# Quire.Mcd lane checkpoint

Owner: dispatched worker, task `task_035c784699b8` (resuming `task_485dc1a8580c`); branch `mirror/quire-mcd-20261116`.

## Landed batches
- Batch 1: 14 bodyless interface method declarations on `depman.NodeQueue` (`StartWorkers`, `StopWorkers`, `GetRuntimeOptions`, `SetNumThreads`, `GetNumThreads`, `Interrupt`, `SetAutoCalc`, `GetAutoCalc`, `SetCalcNow`, `GetCalcNow`, `BeginTransaction`, `EndTransaction`, `GetUpToDateMediator`, `GetBusyMediator`) and 2 abstract property declarations on `depman.Scope` (`Id`, `Parent`).
- Hand-written partials: `src/Quire.Mcd/Mirrored/depman.NodeQueue.cs` and `src/Quire.Mcd/Mirrored/depman.Scope.cs`.
- Release IL: PASS; 16 checked (`.mirror-cache/release-il-a6291be4621b/release-il.json`, `.mirror-cache/release-il-6d1a2ebc6e16/release-il.json`).
- Landed on `main`: `2d0670d50` (landing receipt: `C:\Projects\Quire\.mirror-cache\receipts\gate-il-d52f77f32596.json`) and checkpoint `e8bcf985c`.

## Exact opcode-level Release IL diffs for remaining parked members

All remaining 570 stubs in `Quire.Mcd` are blocked by toolchain/compiler differences between Roslyn C# (`csc`) and the original compilers (MSVC C++/CLI and MPL compiler `mplc`):

### 1. MSVC C++/CLI minimal .maxstack difference (`depman.DepException::.ctor`)
Opcode-level diff via `python tools/validation/il_diff.py Quire.depman.DepException --project Quire.Mcd`:
```text
Quire.depman.DepException::.ctor`0()->[System.Runtime]System.Void|False (Mathcad mcdepman 0x060002ae): structure matches; instructions differ
  first difference at instruction #3 (Quire | Mathcad  [Quire source line])
    #0    ldarg.0                                                | ldarg.0  [src\Quire.Mcd\Generated\Mirror\depman.DepException.g.cs:15]
    #1    call System.Exception::instance .ctor`0()->System.Void | call System.Exception::instance .ctor`0()->System.Void  [src\Quire.Mcd\Generated\Mirror\depman.DepException.g.cs:15]
    #2    ret                                                    | ret  [src\Quire.Mcd\Generated\Mirror\depman.DepException.g.cs:15]
  > #3    .maxstack 8                                            | .maxstack 1
    #4    .initlocals False                                      | .initlocals False
    #5    .locals                                                | .locals 
    #6    (end)                                                  | (end)
```
- **Analysis:** All IL instructions, calls, and operands match identically. Roslyn emits `.maxstack 8` by default for methods using <= 8 stack slots, whereas MSVC C++/CLI computes the exact minimal stack depth (`.maxstack 1`). There is no C# language or Roslyn compiler option to lower `.maxstack` below 8.
- **Affected members:** `depman.DepException::.ctor`, `depman.Scope::.ctor`, `depman.Scope::GetHashCode`, `depman.Msg::.ctor`, `depman.ActionResult::GetResult`, and all other 279 `depman` classes with bodies originating from `mcdepman.dll`. Per D20, these are being migrated to the C++/CLI project `Quire.Mcdepman` (see `docs/mirror/work/lane-mcdepman-cpp.md`).

### 2. MPL compiler .maxstack 48 and .initlocals difference (`Quire.McdEfi.values::.ctor`)
Opcode-level diff via `python tools/validation/il_diff.py Quire.McdEfi.values --project Quire.Mcd`:
```text
Quire.McdEfi.values::.ctor`0()->[System.Runtime]System.Void|False (Mathcad McdEfi 0x06000003): structure matches; instructions differ
  first difference at instruction #3 (Quire | Mathcad  [Quire source line])
    #0    ldarg.0                                             | ldarg.0  [src\Quire.Mcd\Generated\Mirror\McdEfi.values.g.cs:17]
    #1    call System.Object::instance .ctor`0()->System.Void | call System.Object::instance .ctor`0()->System.Void  [src\Quire.Mcd\Generated\Mirror\McdEfi.values.g.cs:17]
    #2    ret                                                 | ret  [src\Quire.Mcd\Generated\Mirror\McdEfi.values.g.cs:17]
  > #3    .maxstack 8                                         | .maxstack 48
    #4    .initlocals False                                   | .initlocals True
    #5    .locals                                             | .locals 
    #6    (end)                                               | (end)
```
- **Analysis:** Instruction stream matches identically. Mathcad's MPL compiler (`mplc`) sets `.maxstack 48` and `.initlocals True` on every emitted method header, whereas Roslyn C# sets `.maxstack 8` and `.initlocals False`.
- **Affected members:** All 259 stubs in `McdTranslator.values` (110), `McdDynamicEfi.values` (107), `McdUnitSystemCGS.values` (11), `McdUnitSystemNone.values` (11), `McdUnitSystemSI.values` (11), `McdUnitSystemUS.values` (4), `McdUnitSystemMKS.values` (3), and `McdEfi.values` (2). Per D20, these belong to standalone MPL projects (`Quire.McdTranslator`, `Quire.McdDynamicEfi`, etc.).

### 3. C++ modopt(IsLong) custom modifiers (`depman.IDepman::Remove`)
Declaration diff via `python tools/validation/release_il.py --base main --project Quire.Mcd --type Quire.depman.IDepman`:
```text
Quire.Mcd::Quire.depman.IDepman::Remove`0([System.Runtime]System.Int32)->[System.Runtime]System.Void|False:
  declaration differs:
    custom modifiers of parameter 1: Quire none, Mathcad modopt(System.Runtime.CompilerServices.IsLong)
```
- **Analysis:** Mathcad's `IDepman` interface methods written in C++ use `long` (32-bit), which the C++/CLI compiler emits as `int32 modopt(System.Runtime.CompilerServices.IsLong)`. The C# language has no syntax or attribute to emit `modopt(IsLong)`.
- **Affected members:** 24 `depman.IDepman` interface methods (`AddAfter`, `AddAfterWithDef`, `AddBefore`, `MoveAfter`, `MoveBefore`, `Remove`, `Update`, `Lock`, `LockInvalid`, `UnLock`, `SetEnvScope`, `SetPriority`, `GetPriority`, `TreatAsReady`, `SetTimer`, `SetNotifyComputing`, `GetValueAt`, `IsBoundAt`, `GetAllValues`, `Ready`, `GetDefined`, `GetDefinition`, `GetDependencies`, `GetDependents`).

### 4. Public interface exposing internal types (`depman.IDepman::UnLockP` & `depman.NodeQueue`)
Diff / compilation error via candidate build:
- Mathcad declared `UnLockP(Position* p)` on `IDepman` and `Enqueue(Position* p)`, `Enqueue(Position* p, EnqueueCode c)`, `Dequeue(Position* p)`, `SetPriority(Position* p, int)` on `NodeQueue` as public abstract interface methods.
- In `Quire.Mcd`, `Position` is an internal struct (`internal partial struct Position`) and `EnqueueCode` is an internal enum (`internal enum EnqueueCode`).
- C# prohibits public interface methods with internal parameter types (compiler error CS0051). Marking them internal requires a default body in C# interfaces (compiler error CS8703), which fails exact IL parity against Mathcad's bodyless public declarations.
- **Affected members:** 1 `depman.IDepman` method (`UnLockP`) and 4 `depman.NodeQueue` methods (`Enqueue`, `Enqueue`, `Dequeue`, `SetPriority`).
