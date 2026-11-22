# Msgen lane checkpoint

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
