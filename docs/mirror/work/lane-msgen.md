# Msgen lane checkpoint

## Batch 26 (review only: gcroot CurrAsm/Callable; nothing landed)

- Owner: AGY, Orca task `task_32a5b7651a22`. Branch `feature/msgen-agy-2`, from current `main`.
- The shared fix on `main` (0e1edc8f9, `tools/Quire.Tools.MethodAudit/Program.cs`, `GlobalName`) maps the `gcroot<Quire::gen::IlAsm ^>` type prefix to the original `gcroot<gen::IlAsm ^>`. It does not rewrite the accessor suffix. The original `gen.CurrAsm` calls `'<Module>'::'gcroot<gen::IlAsm ^>..PE$AAVIlAsm@gen@@'`; the candidate spelling `..PE$AAVIlAsm@gen@Quire@@` keeps `Quire@@` in its mangled tail, so the name still does not match.
- The "pinned global `<gcroot.h>` compatibility header" is not in this repository. `Quire.Mcdepman` includes the standard MSVC `<gcroot.h>` only.
- `gen.CurrAsm` also reads the native global `?m_curr_asm@AsmProxy@gen@@2PEAV12@EA` at offset 16 of an `AsmProxy` struct. Msgen has no `AsmProxy` type or global, and the shared fix does not cover native global names.
- `Callable::IlAssembler` and `Clo` therefore stay parked as MirrorStub bodies until the accessor suffix normalisation covers `@Quire@@` and an `AsmProxy` global is declared.
- Parked (unchanged): `DataRep` consumer chain, `MethodClo` (constructor base-argument order), `CtxtMod::EndCtxt` (`conv.u1`), and the abstract `Ctxt::BegCtxt`/`EndCtxt`.
- Release IL: unchanged at 453 checked and 0 failures on `main`.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 25 (review only: DataRep consumers of NullConstructor; nothing landed)

- Owner: AGY, Orca task `task_b62966fede88`. Branch `feature/msgen-agy-2`, from current `main`.
- Real consumers of `NullConstructor::.ctor(IlAsm, void**)` in the original: `DataRep::MaybeTypeConstructors`, `DataRep::BoolTypeConstructors` (its non-record branch), and `DataRep::InitializeDataRep` (the arity-0 branch). The minimal one is `MaybeTypeConstructors`, which also calls `RecordTypeConstructor(void**, int)` on both branches.
- `RecordTypeConstructor(void**, int)` needs, absent on `main`: the `DataRep` class itself; `DataRep::SpecialConstructor` (with `LiftConstructorParameter`); `IlAsm::ImportedConstructor` (a try/fault region with `Hex::Error` and `std::string` unwinding); `IlAsm::NewConstructorType` and `NewRecordType` (which need `RecordClosure` and `DynamicType.DefineRecord`); `RecordConstructor` and `NullaryConstructor`.
- Without `DataRep` as a type with a real member consumer, a `NullConstructor` candidate is omitted from the inventory, as with `VoidConstructor` before Batch 13. A `DataRep` subset would be omitted the same way, so there is no independent verifiable DataRep group yet.
- Parked: `MethodClo` (constructor evaluation order), `CtxtMod::EndCtxt` (`conv.u1`), and the abstract `Ctxt::BegCtxt`/`EndCtxt`.
- Release IL: unchanged at 453 checked and 0 failures on `main`.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 24 (review only: MethodClo, NullConstructor; nothing landed)

- Owner: AGY, Orca task `task_1572aed179f2`. Branch `feature/msgen-agy-2`, from current `main`.
- `MethodClo`: a subset of the class (fields, the simple `Closure` overrides `ClosureArgs`, `DynClosureType`, `StaClosureType`, `ModuleScope`, `SetInstance`, `LdVal`, `HasInstance`, `GetInstanceType`) passed exact, but the constructor did not. The original evaluates `module.StaClosureType()` before the `Closure` base's `module.IlAssembler()` argument, and C++ evaluates base-initializer arguments first, so the call order differs. Reordering needs a helper member the original does not have. The subset is not landed.
- `NullConstructor` (`: Constructor`): its members pass when compiled, but the compiler omits the type from the candidate, so the gate never compares them. Its only Mathcad consumers are in `DataRep`, which is absent. Like `VoidConstructor` before Batch 13, it cannot be verified until a consumer exists. Not landed.
- The other `Constructor` subclasses (`BoxConstructor`, `PropConstructor`, `RecordConstructor`, `VectorConstructor`, `NullaryConstructor`) share this blocker through `DataRep`.
- Parked: `CtxtMod::EndCtxt` (`conv.u1`), and the abstract `Ctxt::BegCtxt`/`EndCtxt` declarations.
- Release IL: unchanged; `main` at this batch has 453 checked and 0 failures.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 23 (`Quire.gen.CtxtTry` subset: fields, `.ctor`, `lookup`, `EmitBranch`)

- Owner: AGY, Orca task `task_f7ee5e725387`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `CtxtTry` class with its seven fields, `.ctor(Ctxt, Type)` (in-class field initialisers match the original's stores), `lookup(void*)` (the ternary is written as `m_ExceptionVar == key ? m_ExceptionObj : Parent()->lookup(key)`, because the original branches with `bne.un`), and `EmitBranch` (exact Release IL).
- Left out: `BegCtxt`, `EndCtxt`, `EmitReturn`, `BegHandler`, `EnterCatch`, `RetLabel`, `FallThroughLabel`. These need `Ctxt::BegCtxt`/`EndCtxt` declared on `Ctxt` (abstract in the original; declaring them would make `CtxtMod` abstract, and its own overrides are parked), plus `DefineLetVar`, `Protected`/`SetProtected`, and `Reachable`.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 453 checked; 0 failures; 7 unfinished.
- Parked: `CtxtMod::EndCtxt` (the original's `conv.u1` before the `!= 0` test is folded away by the candidate compiler).
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 22 (`ForeignConstructor::CallFunc`, `ForeignConstructor::LdVal`, `Constructor::FunctionValue`)

- Owner: AGY, Orca task `task_5b449a6680d2`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `ForeignConstructor::CallFunc` (`newobj` through `emit_instr`), `Constructor::FunctionValue` (caches `ImplementAsMethodFunction().FunctionValue()`, exact), and `ForeignConstructor::LdVal`.
- Token detail: the original `LdVal` calls `Callable::FunctionValue`, the base declaration. A direct call resolves to the `Constructor` override, so the call goes through `static_cast<Callable^>(this)`.
- `EmitFunctionValue` already exists on `main` (inherited from `Callable`), so no new prerequisite was needed.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 450 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 21 (`Quire.gen.ForeignConstructor::Deconstruct`, `MetaObj::NewMetaObj(IlAsm, ConstructorInfo, Type[])`, `ForeignCtor::CreateMetaObj`)

- Owner: AGY, Orca task `task_da1e51931b59`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `ForeignConstructor::Deconstruct` (do-while over field names, exact Release IL), `MetaObj::NewMetaObj(IlAsm, ConstructorInfo, Type[])` (static overload that builds a `ForeignConstructor` with the `"ctor"` name, exact), and `ForeignCtor::CreateMetaObj`.
- `ForeignConstructor` is now concrete: the `abstract` keyword is removed because `Deconstruct` implements the inherited pure member. `CallFunc` and `LdVal` remain unimplemented in this type; the gate does not require them to be declared.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 447 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 20 (`Quire.gen.ForeignConstructor` subset, `Constructor(IlAsm, string)`)

- Owner: AGY, Orca task `task_da1e51931b59`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `Constructor(IlAsm, String)` and `ForeignConstructor` (class, fields `m_ctor`, `m_arity`, `m_types`) with `.ctor(IlAsm, String, String, ConstructorInfo, Type[])`, `Arity`, `ReturnType`, `RepresentationType`, `MplParameterTypes` (exact Release IL).
- Still missing: `ForeignConstructor::Deconstruct` (pure in `Constructor`, so the class stays abstract), `CallFunc`, and `LdVal`. `LdVal` needs `Constructor::FunctionValue`, which needs `ImplementAsMethodFunction().FunctionValue()`.
- `MetaObj::NewMetaObj(IlAsm, ConstructorInfo, Type[])` and `ForeignCtor::CreateMetaObj` need `ForeignConstructor` to be concrete, so they remain blocked until `Deconstruct` lands.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 444 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 19 (`Quire.gen.ForeignCtor` constructor)

- Owner: AGY, Orca task `task_faa779f237e3`. Branch `feature/msgen-agy-2`, from current `main`.
- Member: `ForeignCtor(String, Type[], ConstructorInfo)` (exact Release IL). It uses `ResolvedForeignId(String)` and `CacheId()` from `main`.
- Not landed: `ForeignCtor::CreateMetaObj`. Its original calls `MetaObj::NewMetaObj(IlAsm, ConstructorInfo, Type[])`, which builds the absent `ForeignConstructor` class (a `Constructor` subclass with its own members).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 438 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 18 (`Quire.gen.IlAsm::Emitter`, `IlAsm::SetEmitter`)

- Owner: AGY, Orca task `task_82261cd9eb87`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `IlAsm::Emitter()` (getter for `m_emit`) and `IlAsm::SetEmitter(IlEmit)` (stores the new emitter and returns the previous one), exact Release IL.
- `CtxtMod::BegCtxt` and `CtxtMod::EndCtxt` are NOT landed. `BegCtxt` matches once its `(byte)` test is explicit. `EndCtxt` calls `MetaObj::FinishClosure` (the original IL's token is `gen.MetaObj::FinishClosure`, so the earlier receiver-token note was wrong). Its only mismatch is the original's `conv.u1` before the `!= 0` test: the candidate folds that conversion away whenever the value reaches the call, through the `num` local or directly. Reaching the original shape would need an IL-level change, so this stays blocked.
- `Rts.StackDepth` and `Rts.ResetStackDepth` already exist in `Quire.Mpl`.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 437 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 17 (`Quire.gen.Ctxt`: `Parent`, `Root`, `lookup`)

- Owner: AGY, Orca task `task_6ef4c62e31da`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `Ctxt::Parent`, `Ctxt::Root` (the original's null test is `brfalse` on the parent handle, so the ternary is written as `m_parent ? m_parent->Root() : this`), and the virtual `Ctxt::lookup(void*)` forwarding to the parent (exact Release IL).
- Blocked: `PushCtxt`, `PopCtxt` and `PopCtxtFirst` need `Ctxt::BegCtxt` and `Ctxt::EndCtxt`, which the original declares abstract. Declaring them pure would make `CtxtMod` abstract, and the original `CtxtMod` implements them with bodies that need `Closure::Update`, `Closure::FinishClosure`, `IlAsm::SetEmitter` and `Rts.StackDepth`.
- Still blocked: `CtxtTry` and therefore `EmitProlog` and `EmitEpilog`.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 435 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 16 (`Quire.gen.MethodFunction::EmitStackProbe`)

- Owner: AGY, Orca task `task_863729880aea`. Branch `feature/msgen-agy-2`, from current `main`.
- Member: `EmitStackProbe(IlAsm, bool)` override: `typeof(AppException)` (`Quire.Mpl.Run.AppException`, referenced by Msgen) `.GetMethod("ProbeStack"|"RestoreStack")`, `emit_instr(OpCodes::Call, ...)` (exact Release IL on the first run).
- Still blocked: `EmitProlog` and `EmitEpilog` (tracing). Their original IL calls `IlAsm::beg_exception_block(Type)`, `beg_catch_block()` and `end_exception_block()`, which forward to `PushCtxt(new CtxtTry(...))`, `CtxtTry::BegHandler` and `PopCtxtFirst`. On `main` there is no `CtxtTry`, `PushCtxt`, `PopCtxtFirst` or `BegHandler`, and the `IlAsm` forwarders are absent.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 432 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 15 (`Quire.gen.MethodFunction` constructors `(string, Closure, Type, Type[])` and `(sbyte*, Closure)`)

- Owner: AGY, Orca task `task_863729880aea`. Branch `feature/msgen-agy-2`, from current `main`.
- `.ctor(string, Closure, Type, Type[])`: the unset `ms::Range` (-1 begin/end) passed to `DeclareFunction(null, ...)`, exact Release IL on the first run.
- `.ctor(const char*, Closure)` (Mathcad `sbyte*`): `IlEmit(::gen::ToString(Substring<const char>(f)), clo, null)`. The `strlen` inside `Substring(char*)` matched the original's loop. It needed `gen::ToString` marked `__declspec(noinline)` (declared in `MsgenCommon.h`, defined in `MsgenPort.cpp`), because the constructor sits in the same translation unit and otherwise inlines it; the body is unchanged.
- Still blocked in `MethodFunction`: `EmitProlog`, `EmitEpilog`, `EmitStackProbe` (tracing; `typeof(AppException)` reflection, `m_einfo_local`, `beg_exception_block`).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 431 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 14 (`Quire.gen.MethodFunction::CreateParameterEnv`)

- Owner: AGY, Orca task `task_efc73f6f7302`. Branch `feature/msgen-agy-2`, from current `main`.
- Member: `MethodFunction::CreateParameterEnv(Range)` override (exact Release IL on the first gate run). It uses `Ctxt::NewCtxtEnv` (out of line, Batch 13), `Closure::ClosureArgs`, `MplParameterTypes`, `IlAsm::VoidType`, the Batch 13 `VoidConstructor(IlAsm, void*)`, and `MetaObj::NewMetaObj(void*, int)`, all on `main`.
- Still blocked in `MethodFunction`: the `string` and `sbyte*` constructors (they need the `Range`/`Unsafe` substring path), and `EmitProlog`/`EmitEpilog`/`EmitStackProbe` (tracing and `AppException` reflection).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 429 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 13 (`Quire.gen.VoidConstructor`, `ContinuationFunction::DeclareFunction`)

- Owner: AGY, Orca task `task_1c9bf6789f1d`. Branch `feature/msgen-agy-2`, from current `main`.
- `VoidConstructor` (`MirrorOf("gen.VoidConstructor, msgen")`, same pattern as `Constructor`): `.ctor(IlAsm, void*)`, `LdVal`, `Arity`, `ReturnType`, `MplParameterTypes`, `Deconstruct`. Exact Release IL. Diagnosis: in Batch 12 the type had no consumer, so the C++/CLI compiler did not emit it and the gate never saw its rows. `ContinuationFunction::DeclareFunction` is the consumer that forces emission; with it the six rows appear in the candidate inventory.
- `ContinuationFunction::DeclareFunction` (exact Release IL): loop over `ms::Range` calling `VoidConstructor` or `NewMetaObj(void*, LocalBuilder)`.
- `Ctxt::NewCtxtEnv` is now defined out of line in `MsgenPort.cpp` and marked `__declspec(noinline)`, so the `DeclareFunction` call site matches Mathcad's call to `Ctxt::NewCtxtEnv`. Its body is unchanged and stays exact.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 428 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 12 (`Quire.gen.Constructor` base subset)

- Owner: AGY, Orca task `task_8d00981af8a9`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `Constructor` abstract class (field `m_method_function`), `.ctor(IlAsm)`, `GetMethod`, `ImplementAsMethodFunction`, and pure `Deconstruct(MetaObj, Range)` (exact Release IL). `ImplementAsMethodFunction` uses the existing `LdFuncArg`, `BegArg`, `EndArg`, `CallFunc`, `NewMethodFunction` and `ModuleClosure` on main.
- `VoidConstructor` is NOT landed: `release_il.py` never compared its members (no inventory rows and no skipped rows for `gen.VoidConstructor`, although `type-map.csv` binds it to the `Quire.Engine.Native` project). A member the gate does not compare is unverified, so it stays out until that binding is fixed.
- `ContinuationFunction::DeclareFunction` remains blocked on `VoidConstructor(IlAsm, void*)`.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 422 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 11 (`IlAsm::Branch`, `ContinuationFunction::CallFunc`)

- Owner: AGY, Orca task `task_b88bf996af63`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `IlAsm::Branch(Closure, Label)` (forwards to `m_ctxt->EmitBranch` with `GC::KeepAlive(this)`, as in the original) and `ContinuationFunction::CallFunc` (idiomatic top-tested loop over `m_params`, then `Branch`). Both exact Release IL with no changes to the loop form.
- Still blocked: `ContinuationFunction::DeclareFunction`, which needs `VoidConstructor` for its void-argument branch.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 418 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 10 (`Quire.gen.ContinuationFunction` subset, `ContinuationClo::DefineFunction`)

- Owner: AGY, Orca task `task_5ae4301f31f6`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `ContinuationFunction` class (fields `m_label`, `m_arity`, `m_return_type`, `m_param_types`, `m_params`, and the derived `m_method`/`m_clo`), `.ctor`, `Arity`, `ReturnType`, `MplParameterTypes`, `CreateParameterEnv`, `GetMethodFunction`, `GetMethod`, `EmitProlog`; and `ContinuationClo::DefineFunction` (exact Release IL).
- `DeclareFunction` is still blocked: its Mathcad IL includes the `VoidConstructor` branch. Batch 9's `MethodFunction::DeclareFunction` does not cover this, since it has no `VoidConstructor` branch.
- `CallFunc` is still blocked on `IlAsm::Branch`, which is absent on `main`.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 416 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 9 (`Quire.gen.MethodFunction::DeclareFunction`)

- Owner: AGY, Orca task `task_54df3dcaa425`. Branch `feature/msgen-agy-2`, from current `main`.
- Member: `MethodFunction::DeclareFunction` override (exact Release IL). It needs only `MethodParameterTypes`, `Name`, `Closure::CreateMethod` and `IlGenMethod`, all on `main`; the `Hex::Tag` and `Range` arguments are unused in the original.
- The `Closure^ clo = m_clo;` local is needed to load `m_clo` before the `MethodParameterTypes()` call, as the Release IL does.
- Still blocked: the `string` and `sbyte*` constructors (they need `ms::Range`/`Unsafe` substring handling), `CreateParameterEnv` (needs `VoidConstructor`), and `EmitProlog`/`EmitEpilog`/`EmitStackProbe` (tracing/`AppException`).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 407 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 8 (`Quire.gen.MethodFunction`, subset)

- Owner: AGY, Orca task `task_4fa0e5aab024`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: class `MethodFunction : IlEmit` with fields `m_arity`, `m_return_type`, `m_param_types`; `.ctor(void*, Closure)`, `Arity`, `ReturnType`, `MplParameterTypes`, `GetMethod`, `GetMethodFunction`, `LdClosureRecord`, `ClosureType`, `CallFunc` (exact Release IL).
- Left out: the `string` and `sbyte*` constructors (need `ms::Range`/`Unsafe` and `ToString` of a substring), `DeclareFunction` and `CreateParameterEnv` (need the `Range` path and `VoidConstructor`), and `EmitProlog`/`EmitEpilog`/`EmitStackProbe` (need `AppException` reflection and the `m_einfo_local` tracing path). These are Mathcad members with no Quire counterpart yet.
- `ContinuationClo::DefineFunction` remains blocked on `ContinuationFunction`, which needs the `DeclareFunction` path.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 406 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 7 (`Quire.gen.ContinuationClo`, partial)

- Owner: AGY, Orca task `task_5692e63e3586`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `ContinuationClo` class with field `m_parent`, `.ctor`, `LdVal`, `HasInstance` (exact Release IL).
- Left out: `DefineFunction`, which needs the absent `ContinuationFunction` class (an `IlEmit` subclass with its own prerequisites).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 397 checked; 0 failures; 7 unfinished.
- Backup: `msgen-batch3-wip-backup` at 9205680fd remains intact.

---

## Batch 6 (`Quire.gen.CctorFunction`, `IlBuilderType::Cctor`)

- Owner: AGY, Orca task `task_fd17b1d0d474`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `CctorFunction` class (`.ctor`, `Arity`, `ReturnType`, `MplParameterTypes`, `GetMethod`) and `IlBuilderType::Cctor`.
- `GetMethod` returns through a named local initialised to `nullptr` before `GC::KeepAlive(this)`. The comma-expression form emits the `ldnull` after the call and fails the gate.
- Constructor: the base call takes `nullptr` for the ILGenerator, then `m_il` is set from `m_ctor`. This matches Mathcad's order for the checked members.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 394 checked; 0 failures; 7 unfinished.
- Preserved candidate on branch `msgen-cctor-candidate-wip`; WIP backup remains on `msgen-batch3-wip-backup` at 9205680fd.

---

## Batch 5 (`IlAsm::GetTypeInitializer`, `IlBuilderType::TypeInitializer`)

- Owner: AGY, Orca task `task_0e0c0bd093fb`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `IlAsm::GetTypeInitializer` (static forwarder to `Rts::GetTypeInitializer`, no KeepAlive, as in the original) and `IlBuilderType::TypeInitializer` (with `GC::KeepAlive(this)` after the call, as in the original).
- `Cctor` remains out: it needs the absent `CctorFunction` class.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 388 checked; 0 failures; 7 unfinished.
- Preserved WIP remains on `msgen-batch3-wip-backup` at 9205680fd.

---

## Batch 4 (`Quire.gen.IlBuilderType`, candidate for landing)

- Owner: AGY, Orca task `task_236af414ffdf`. Branch `feature/msgen-agy-2`, from current `main`.
- Members: `IlBuilderType` class (fields `m_tb`, `m_cctor`), `.ctor`, `DefineConstructor`, `DefineField`, `CreateMethod`, `GetTypeBuilder`, `GetInstanceType`, `CreateType` (7 members, exact Release IL).
- Left out, blocked on absent types: `Cctor` (needs `CctorFunction`) and `TypeInitializer` (needs `IlAsm::GetTypeInitializer` and its `ConstructorBuilder` path); both are Mathcad members with no Quire counterpart yet, so they are skipped rather than stubbed.
- `DefineField` and `CreateMethod` keep `GC::KeepAlive(this)` after the builder call, as Mathcad's IL requires.
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 386 checked; 0 failures; 7 unfinished.
- Preserved WIP remains on `msgen-batch3-wip-backup` at 9205680fd.

---

## Batch 3 (landed: `Quire.gen.Ctxt::EmitBranch` only)

- Owner: AGY, Orca task `task_b44f336613f9` (previously `task_3e02aeb04edd`). Branch `feature/msgen-agy-2`, from current `main`.
- Members: `Quire.gen.Ctxt::EmitBranch` (new virtual on `Ctxt`, body in `MsgenPort.cpp`, `GC::KeepAlive(this)` after each arm as in the source).
- Release IL: `release_il.py --base main --project Quire.Msgen` PASS; 379 checked; 0 failures; 7 unfinished.
- The full earlier Batch 3 WIP (`Closures.cpp`, `Constructors.cpp`, `MsgenStubs.cpp` and tracked edits) is preserved on branch `msgen-batch3-wip-backup` (commit 9205680fd), not landed. Its gate state was 42 failures before the `KeepAlive` removal and 32 after; placeholder stubs do not pass.
- Remaining mismatch families (WIP): closure receiver (`MethodClo`/`ModuleRecordClo`/`ModuleTypeClo` `DefineFunction` call `Closure::NewMethodFunction`), `ldarg.0`/`ldloc.0` in `DataRep`, and `Constructor` `DelegateValue`/`FunctionValue`. These need classes (`MethodClo`, `DataRep`, the `Constructor` hierarchy) that are not on `main`.

---

- Owner: AGY, Orca task `task_3e02aeb04edd` (continuing `task_cd3115e8168a`).
- Branch: `feature/msgen-agy-2`.
- Members (Batch 2, cumulative):
  - `Quire.gen.IlEmit` (**74/74 exact**, including `emit_call`, `emit_value_assertion`, `CreateParameterEnv` via `Hex::Error`/`Hex::OnError`, `Clo`, `ModuleScope`, `HasInstance`, `GetMethodFunction`, `DefineLocal(void*)`, all 3 ctors).
  - `Quire.gen.MetaObj` (**43/45 exact**; remaining 2 static factories wait on `ForeignConstructor` / `ResolvedForeignId::ResolveForeignId` in Batch 3).
  - `Quire.gen.Callable` (**28/30 exact**; `IlAssembler` and `Clo` are blocked stubs on `gen::CurrAsm()` / `gcroot<Quire::gen::IlAsm ^>` mangled name normalization).
  - `Quire.gen.Closure` (**31/31 exact**).
  - All 8 `Var` subclasses (**33/33 exact**): `ArgVar` (4/4), `LocalVar` (4/4), `FieldVar` (4/4), `StaticField` (5/5), `TopLevelVar` (3/3), `PropertyVar` (3/3), `ElementVar` (6/6), `TopLevelElementVar` (4/4).
  - `Quire.gen.IlOpCode` (**9/9 exact**), `Quire.gen.IlMethod` (**8/8 exact**), `Quire.gen.LateBoundMethod` (**8/8 exact**).
  - `Quire.gen.ForeignField` (**3/3 exact**), `Quire.gen.ForeignMethod` (**3/3 exact**).
  - `Quire.gen.IlGenMethod` (**4/4 exact**), `Quire.gen.ResolvedForeignId` (9/16 exact).
  - `Quire.gen.Ctxt` (2 exact: `.ctor`, `NewCtxtEnv`), `Quire.gen.CtxtMod` (3 exact: `.cctor`, `ModuleClosure`, `GetApplyMethod`; `.ctor` & `ForeignValue` placeholder stubs).
  - `Quire.gen.IlAsm` (32 exact helpers including `AstInfo` 13-slot vtable dispatch and `Raise(Substring<const char>)`; `Raise(String^)` blocked stub on `std::string` CRT `new`/`delete[]`; `GetReturnType(Substring, uint64)` and `.ctor` temporary stubs).
  - `<Module>` exact: `gen.ToString`, `IsDyn`, `IsTuple`, `IsStrVal`.
- Candidate Release IL report: `.mirror-cache/release-il-8f4a16c07a54/release-il.json`.
- Gate line: `Release IL: PASS; 378 checked; C:\Users\DuyDang\orca\workspaces\Quire\engutils-mgd-agy\.mirror-cache\release-il-8f4a16c07a54\release-il.json`.
- Batch 1 landing: landed on `main` at `027638a3e` via `lane_merge.py`; receipt `C:\Projects\Quire\.mirror-cache\receipts\gate-il-f5bc976e1c3d.json`.
- Concrete blockers identified:
  1. `std::string` (`gen.ToStrng`, `IlAsm::Raise(String^)`, etc.) pulls in CRT `operator new` / `operator delete[]` native bodies whose machine code differs from Mathcad.
  2. `gen.CurrAsm()` uses `gcroot<Quire::gen::IlAsm ^>` whose `<Module>` mangled name embeds `Quire::` (`gcroot<Quire::gen::IlAsm ^>..PE$AAVIlAsm@gen@Quire@@`), which MethodAudit does not normalize to `gcroot<gen::IlAsm ^>..PE$AAVIlAsm@gen@@`.
- Remaining: `Closure` subclasses, `MethodFunction` / `IlEmit` subclasses, `Constructor` hierarchy, `DataRep`, `Ctxt` hierarchy, `ModuleEnv`, `IlAsm` remaining methods.

---

## Previous checkpoint (`task_cd3115e8168a`)

- Owner: AGY, Orca task `task_cd3115e8168a`.
- Branch: `feature/msgen-agy`.
- Members: Quire.Msgen project split established as C++/CLI (`/clr:NetCore` targeting net10.0-windows); `Quire.gen.ResolvedForeignId` (9 members exact: `InitForeignIdCache`, `LoadField`, `ClearCache`, `CreateMetaObj`, `.ctor`, `CacheId`, `GobbleWhite`, `ArrayTypeFromElementType`, `.cctor`).
- Candidate Release IL report: `.mirror-cache/release-il-2e198c05eef6/release-il.json`.
- Gate line: `Release IL: PASS; 90 checked; C:\Users\DuyDang\orca\workspaces\Quire\msgen-agy\.mirror-cache\release-il-2e198c05eef6\release-il.json`.
- Classification: With `gen.ResolvedForeignId` carrying `[MirrorOf("gen.ResolvedForeignId, msgen")]`, `ModuleOriginal` resolves `msgen.dll` cleanly. CRT and Module support methods are classified as toolset support (D19), and all 6 native bodies match byte-for-byte.
- Global module methods: 61 passed / toolset support.
- Unbound CRT/ThisPointer members: 14 passed / toolset support.
- Native bodies without IL: 6 byte-matched, 0 failed. All six found original native counterparts in `msgen` and matched exactly through `native_bodies`.
- Rule result: Release IL gate passed with 90 checked, 87 exact compiler/runtime support matches, 0 failures, 6 native pairs matching.
- Exact MirrorGen changes: none required; `Rules.cs` already maps `msgen` to `Quire.Msgen` and recognizes C++/CLI via `IsCppProject`.
- Landing: landed in commit `fd1bb1398fe444d833ae4ccf9e1f62e399a2028e` via `lane_merge.py`; receipt `C:\Projects\Quire\.mirror-cache\receipts\gate-il-d147751f6be5.json`.
- Remaining mismatches: 0 gate failures; 131 skipped members in msgen compiler pipeline.

Blocked global module methods (61; first differing offset: n/a, no original native counterpart):

```text
<CrtImplementationDetails>.ThrowNestedModuleLoadException`0([System.Runtime]System.Exception,[System.Runtime]System.Exception)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.ThrowModuleLoadException`0([System.Runtime]System.String)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.ThrowModuleLoadException`0([System.Runtime]System.String,[System.Runtime]System.Exception)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.RegisterModuleUninitializer`0([System.Runtime]System.EventHandler)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.DoCallBackInDefaultDomain`0(fnptr,[System.Runtime]System.Void*)->[System.Runtime]System.Void|True
__scrt_is_safe_for_managed_code`0()->[System.Runtime]System.Boolean|True
<CrtImplementationDetails>.DefaultDomain.DoNothing`0([System.Runtime]System.Void*)->[System.Runtime]System.Int32|True
<CrtImplementationDetails>.DefaultDomain.HasPerProcess`0()->[System.Runtime]System.Boolean|True
<CrtImplementationDetails>.DefaultDomain.HasNative`0()->[System.Runtime]System.Boolean|True
<CrtImplementationDetails>.DefaultDomain.NeedsInitialization`0()->[System.Runtime]System.Boolean|True
<CrtImplementationDetails>.DefaultDomain.NeedsUninitialization`0()->[System.Runtime]System.Boolean|True
<CrtImplementationDetails>.DefaultDomain.Initialize`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?Initialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?Uninitialized@CurrentDomain@<CrtImplementationDetails>@@$$Q2HA@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?IsDefaultDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2_NA@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?InitializedVtables@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4Progress@2@A@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?InitializedNative@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4Progress@2@A@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?InitializedPerProcess@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4Progress@2@A@@YMXXZ`0()->[System.Runtime]System.Void|True
?A0xeb4f7d22.??__E?InitializedPerAppDomain@CurrentDomain@<CrtImplementationDetails>@@$$Q2W4Progress@2@A@@YMXXZ`0()->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializeVtables`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializeDefaultAppDomain`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializeNative`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializePerProcess`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializePerAppDomain`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.InitializeUninitializer`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport._Initialize`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.UninitializeAppDomain`0()->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport._UninitializeDefaultDomain`0([System.Runtime]System.Void*)->[System.Runtime]System.Int32|True
<CrtImplementationDetails>.LanguageSupport.UninitializeDefaultDomain`0()->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.DomainUnload`0([System.Runtime]System.Object,[System.Runtime]System.EventArgs)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.Cleanup`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*,[System.Runtime]System.Exception)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.{ctor}`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*|True
<CrtImplementationDetails>.LanguageSupport.{dtor}`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.LanguageSupport.Initialize`0([Quire.Msgen]<CrtImplementationDetails>.LanguageSupport*)->[System.Runtime]System.Void|True
.cctor`0()->[System.Runtime]System.Void|True
gcroot<System::String ^>..PE$AAVString@System@@`0([Quire.Msgen]gcroot<System::String ^>*)->[System.Runtime]System.String|True
gcroot<System::String ^>.=`0([Quire.Msgen]gcroot<System::String ^>*,[System.Runtime]System.String)->[Quire.Msgen]gcroot<System::String ^>*|True
gcroot<System::String ^>.{dtor}`0([Quire.Msgen]gcroot<System::String ^>*)->[System.Runtime]System.Void|True
gcroot<System::String ^>.{ctor}`0([Quire.Msgen]gcroot<System::String ^>*)->[Quire.Msgen]gcroot<System::String ^>*|True
_CPPCLI_IMPL_DETAILS.AtExitLock._handle`0()->[System.Runtime]System.ValueType|True
_CPPCLI_IMPL_DETAILS.AtExitLock._lock_Construct`0([System.Runtime]System.Object)->[System.Runtime]System.Void|True
_CPPCLI_IMPL_DETAILS.AtExitLock._lock_Set`0([System.Runtime]System.Object)->[System.Runtime]System.Void|True
_CPPCLI_IMPL_DETAILS.AtExitLock._lock_Get`0()->[System.Runtime]System.Object|True
_CPPCLI_IMPL_DETAILS.AtExitLock._lock_Destruct`0()->[System.Runtime]System.Void|True
_CPPCLI_IMPL_DETAILS.AtExitLock.IsInitialized`0()->[System.Runtime]System.Boolean|True
_CPPCLI_IMPL_DETAILS.AtExitLock.AddRef`0()->[System.Runtime]System.Void|True
_CPPCLI_IMPL_DETAILS.AtExitLock.RemoveRef`0()->[System.Runtime]System.Void|True
?A0xdf32f105.__alloc_global_lock`0()->[System.Runtime]System.Boolean|True
?A0xdf32f105.__dealloc_global_lock`0()->[System.Runtime]System.Void|True
_exit_callback`0()->[System.Runtime]System.Void|True
_initatexit_m`0()->[System.Runtime]System.Int32|True
_initatexit_app_domain`0()->[System.Runtime]System.Int32|True
_app_exit_callback`0()->[System.Runtime]System.Void|True
DecodePointer`0([System.Runtime]System.Void*)->[System.Runtime]System.Void*|True
EncodePointer`0([System.Runtime]System.Void*)->[System.Runtime]System.Void*|True
_initterm_e`0(fnptr*,fnptr*)->[System.Runtime]System.Int32|True
_initterm`0(fnptr*,fnptr*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.ThisModule.Handle`0()->[System.Runtime]System.ModuleHandle|True
_initterm_m`0(fnptr*,fnptr*)->[System.Runtime]System.Void|True
<CrtImplementationDetails>.ThisModule.ResolveMethod<void const * __clrcall(void)>`0(fnptr)->fnptr|True
___CxxCallUnwindDtor`0(fnptr,[System.Runtime]System.Void*)->[System.Runtime]System.Void|True
```

Blocked unbound CRT/ThisPointer members (14; first differing offset: n/a, no bound original native counterpart):

```text
vc.cppcli.modopts.ThisPointer::.ctor`0()->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadException::.ctor`0([System.Runtime]System.Runtime.Serialization.SerializationInfo,[System.Runtime]System.Runtime.Serialization.StreamingContext)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadException::.ctor`0([System.Runtime]System.String,[System.Runtime]System.Exception)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadException::.ctor`0([System.Runtime]System.String)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::.ctor`0([System.Runtime]System.Runtime.Serialization.SerializationInfo,[System.Runtime]System.Runtime.Serialization.StreamingContext)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::.ctor`0([System.Runtime]System.String,[System.Runtime]System.Exception,[System.Runtime]System.Exception)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::get_NestedException`0()->[System.Runtime]System.Exception|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::set_NestedException`0([System.Runtime]System.Exception)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::ToString`0()->[System.Runtime]System.String|False
<CrtImplementationDetails>.ModuleLoadExceptionHandlerException::GetObjectData`0([System.Runtime]System.Runtime.Serialization.SerializationInfo,[System.Runtime]System.Runtime.Serialization.StreamingContext)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleUninitializer::AddHandler`0([System.Runtime]System.EventHandler)->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleUninitializer::.cctor`0()->[System.Runtime]System.Void|True
<CrtImplementationDetails>.ModuleUninitializer::.ctor`0()->[System.Runtime]System.Void|False
<CrtImplementationDetails>.ModuleUninitializer::SingletonDomainUnload`0([System.Runtime]System.Object,[System.Runtime]System.EventArgs)->[System.Runtime]System.Void|False

Matched native bodies (6):

```text
_getFiberPtrId`0()->[System.Runtime]System.Void*|True — no Mathcad counterpart paired; the body equals msgen 0x060000b8 (instructions and targets match)
_cexit`0()->[System.Runtime]System.Void|True — no Mathcad counterpart paired; the body equals msgen 0x060000b9 (import _cexit)
Sleep`0([System.Runtime]System.UInt32)->[System.Runtime]System.Void|True — no Mathcad counterpart paired; the body equals msgen 0x060000ba (import Sleep)
abort`0()->[System.Runtime]System.Void|True — no Mathcad counterpart paired; the body equals msgen 0x060000bb (import abort)
__security_init_cookie`0()->[System.Runtime]System.Void|True — no Mathcad counterpart paired; the body equals msgen 0x060000bc (instructions and targets match)
__FrameUnwindFilter`0([Quire.Msgen]_EXCEPTION_POINTERS*)->[System.Runtime]System.Int32|True — no Mathcad counterpart paired; the body equals msgen 0x060000b7 (import __FrameUnwindFilter)
```
