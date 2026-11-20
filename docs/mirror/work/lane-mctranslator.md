# Quire.Native.Mctranslator (mctranslator.dll) checkpoint

- Owner: native-mctranslator worker (Orca task task_e7c920f9aad1).
- Branch: `feature/native-mctranslator-agy`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/mslisp-agy`, from `main`.
- Gate: `python tools/validation/native_gate.py --base main --project Quire.Native.Mctranslator`.

## Members landed this batch

- Established `src/Quire.Native.Mctranslator/` native C++ dynamic library project (`Quire.Native.Mctranslator.vcxproj`, MSVC `v145`, x64 Release, `/O2 /Oi`, `/EHa`, `/GS-`, `stdcpp17`).
- Implemented and verified 3 exported `ModuleEnv` accessor methods matching Mathcad Prime 12.0.1.0's `mctranslator.dll` with 100% byte parity:
  - `?GetHexEnv@ModuleEnv@Engine@@QEBAPEAVEnvironment@Hex@@XZ`
  - `?SetBuilderFactory@ModuleEnv@Engine@@QEAAXPEAX@Z`
  - `?SetModuleInit@ModuleEnv@Engine@@QEAAXPEAX@Z`
- Gate: PASS, 3 checked, 0 failures, 0 regressions, 0 unfinished among checked.

## Status against the original

- Exports: 3 of 22 exports matched (13.6% export coverage).
- Byte parity: 3/3 candidate functions matched 100% byte-for-byte.

## Remaining mismatches and blockers

The remaining 19 exports of `mctranslator.dll` are concretely blocked on unrecovered internal compiler/translation infrastructure:

1. **Vtable with unexported internal slots**:
   - `??_7ModuleEnv@Engine@@6B@`: `ModuleEnv` interface vtable contains 5 virtual slots (`0x14630`, `0x5f3c0`, `0x14810`, `0x431d0`, `0x436f5`) that are internal, unexported functions in `mctranslator.dll`.

2. **Internal `ModuleEnv` methods and data structures**:
   - `??0ModuleEnv@Engine@@QEAA@PEAUlabel_root@Unmanaged@1@@Z` (constructor)
   - `??1ModuleEnv@Engine@@UEAA@XZ` (destructor)
   - `?RegisterExports@ModuleEnv@Engine@@AEAAPEBVModule@Hex@@PEBV34@@Z`
   - `?Load@ModuleEnv@Engine@@QEAAPEBVModule@Hex@@PEB_W0PEAX0@Z`
   - `?LoadMplString@ModuleEnv@Engine@@QEAAXPEB_W0PEAX0@Z`
   - `?LoadMplModuleRec@ModuleEnv@Engine@@QEAAXPEB_W@Z`
   - `?GetModuleDLL@ModuleEnv@Engine@@AEAA?AV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@PEB_W@Z`
   - `?LoadMplAssembly@ModuleEnv@Engine@@AEAAPEBVModule@Hex@@PEB_W@Z`
   - `?Find@ModuleEnv@Engine@@QEBAPEBVModule@Hex@@PEBD@Z`
   - `?LookupSig@ModuleEnv@Engine@@QEBAPEBDPEBD@Z`
   - `?FindImportSpec@ModuleEnv@Engine@@QEBAPEAVImportSpec@2@PEBD@Z`
   - `?OnErrors@ModuleEnv@Engine@@AEAAXV?$Range@$$CBVError@Hex@@@ms@@PEB_W@Z`
   All of these require the internal `ModuleEnv` layout, internal module dictionary/hash map, `Engine::ImportSpec`, `ms::Range<Hex::Error const>`, and internal loader/linker functions.

3. **Translator initialization and helper functions**:
   - `?NewTranslator@Engine@@YAPEAVInterpreter@Hex@@PEAVNames@3@PEAUlabel_root@Unmanaged@1@PEAVModuleEnv@1@@Z`: calls internal allocator and concrete interpreter constructor (`0x43070`, `0x43190`).
   - `?NewLabelRoot@Engine@@YAPEAUlabel_root@Unmanaged@1@XZ`: allocates and invokes internal `label_root` constructor (`0x6350`).
   - `?FindStaticDeps@Engine@@YAXPEAVInterpreter@Hex@@PEAV?$basic_streambuf@DU?$char_traits@D@std@@@std@@PEAXP6AX2PEBD@Z@Z`: invokes internal static dependency analysis helper (`0x447bc`).
   - `?FindEfiLabels@Engine@@YAPEBVinterned_label_set@Unmanaged@1@PEAVInterpreter@Hex@@PEBD11@Z`: invokes internal label search engine (`0x447bc`).
   - `?BindingErrorValue@@YAPEAVValue@Hex@@XZ`: reads thread-local storage (`_tls_index`) and calls internal error value constructor (`0x43d40`).
   - `?NewInternTable@Engine@@YAPEAVNames@Hex@@XZ`: interns >70 string literals into `Hex::Names`, requiring unexported string constant targets and internal `Hex::Names` methods.
