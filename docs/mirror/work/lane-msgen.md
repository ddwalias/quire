# Msgen lane checkpoint

- Owner: AGY, Orca task `task_3e02aeb04edd` (continuing `task_cd3115e8168a`).
- Branch: `feature/msgen-agy-2`.
- Members (Batch 1):
  - `Quire.gen.ResolvedForeignId` (9 exact methods preserved: `InitForeignIdCache`, `LoadField`, `ClearCache`, `CreateMetaObj`, `.ctor`, `CacheId`, `GobbleWhite`, `ArrayTypeFromElementType`, `.cctor`).
  - `Quire.gen.IlGenMethod` (4/4 exact: `.ctor`, `StartMethod`, `EndMethod`, `GetEmit`).
  - `Quire.gen.MetaObj` (initial exact batch: `.ctor(void*)`, `.ctor()`, `maxargs`, `Arity`, `ModuleScope`, `SetInstance`).
  - `Quire.gen.Callable` (5 exact constructors + abstract virtuals).
  - `Quire.gen.IlAsm` (10 exact static helpers: `IntType`, `DblType`, `ObjType`, `FcnType`, `TagType`, `VoidType`, `NumValCtor`, `GetParameterTypes` x2, `TranslateType`; temporary `[MirrorStub]` `.ctor(void*, AstInfo*, unsigned int)` placeholder).
  - `Quire.gen.IlEmit` (57 exact methods: `.ctor`, `SetIlGenerator`, `GetIlGenerator`, `EmitProlog`, `EmitEpilog`, `EmitStackProbe`, `EmitReturn`, `Reachable`, `TracingEnabled`, `EnableTracing`, `Protected`, `SetProtected`, `DefineLocal`, `new_label`, `emit_label_ref` x2, `emit_label_def` x2, `Ldarg`, `Starg`, `Ldloc` x2, `Stloc` x2, `Ldloca`, `Ldsfld`, `Stsfld`, `Ldfld`, `Stfld`, `emit_double`, `emit_complex`, `emit_Double`, `emit_Int`, `emit_word` x2, `emit_handle`, `emit_string`, `emit_return`, `emit_leave`, `emit_branch`, `emit_raise` x2, `emit_rethrow`, `emit_pattern_match_failure`, `emit_begin_exception_block`, `emit_begin_catch_block`, `emit_begin_filter_block`, `emit_end_exception_block`, `emit_tailcall`, 14 `emit_instr` overloads).
  - `<Module>` `gen.ToString(Substring<char const>)` exact.
- Candidate Release IL report: `.mirror-cache/release-il-75d7185f86d9/release-il.json`.
- Gate line: `Release IL: PASS; 185 checked; C:\Users\DuyDang\orca\workspaces\Quire\engutils-mgd-agy\.mirror-cache\release-il-75d7185f86d9\release-il.json`.
- Remaining: 1 temporary `[MirrorStub]` (`IlAsm::.ctor`), plus remaining `gen.*` classes and methods across `msgen.dll`.

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
