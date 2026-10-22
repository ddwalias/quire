# C++/CLI mixed-mode feasibility proof for `Quire.Engine.Utl`

## Outcome

The Windows C++/CLI proof now contains ported members for `SafeWaitHandle`, `SafeEvent`, `SafeMutex`, `SafeMonitor`, `InterruptControl`, `Mailbox<T>`, `MessageQueue<Owner,Data,Msg>`, `TaskQueue`, `Function`, `FunctionBase`, `_Hex`, `SafeReadWriteLock`, `SortedStringSet`, `VFunc`, `ThreadingUtils`, `Str`, `LabelSet`, `LabelTrie`, `LabelUtils`, `LabeledDictionary`, `XmlUtils`, `LispUtils` and its nested `LinePartition`, `FileUtils`, `UnaryVoidWrapper<T>`, `StringUtils`, `Pair<T1,T2>`, `Triple<T1,T2,T3>`, and `HashSetUtils`. Its managed .NET 10 consumer loads `Quire.Engine.Utl.dll` and exercises these types, including Win32 handles, mailbox and worker queue synchronization, XML and Lisp scanning, line context, include expansion, and FileUtils Unicode conversion. Ghidra imported and analyzed the current candidate as an x64 PE. The C++ source calls real Windows synchronization APIs; no placeholder native method was added.

This remains a feasibility spike, not a replacement for the C# assembly. The candidate carries only a subset of application types plus C++/CLI compiler support types and does not preserve the full public API. The latest gate found 244 exact methods out of 390 checked; it still reports 202 failures, including nine removed methods, 117 generated `<Module>` methods without reviewed bindings, 34 methods without an original counterpart, and 44 mapped methods with IL differences. The production C# project remains intact; no same-DLL Release IL parity claim is made.

The documented validator command, `python tools/validation/release_il.py --base origin/main --project Quire.Engine.Utl`, passes for the current C# project: 129 methods checked, 129 exact, zero failures. This run's report is `.mirror-cache/release-il-ee1bd37eac55/release-il.json`. The C++/CLI candidate path builds successfully, then fails closed with 202 failures, detailed below.

The reference REA report was not present in this worktree's `.mirror-cache/rea`, so I repeated the static boundary inspection against the supplied file. It matched the required SHA-256 and MVID. REA reported 51 P/Invoke imports, 65 native-boundary rows (57 P/Invoke boundaries and eight runtime-provided methods), and two module references. The native procedures are interop/runtime support, not an application-authored algorithm to port. The earlier addition routine exists only in the separate call-path proof; it is not proposed for production.

## Proof project

The reviewable sources are in [`proofs/utl-mixed-mode`](../../../proofs/utl-mixed-mode/). `MixedModeProof.vcxproj` is an x64 Visual C++ Dynamic Library project with `CLRSupport=NetCore`, target `net10.0-windows`, and the installed v145 MSVC toolset. `MixedModeProof.cpp` contains a small `#pragma managed(push, off)` native function and a public `UtlMixedModeProof.NativeBridge.Add(Int32, Int32)` C++/CLI wrapper that calls it. The C# consumer directly references the resulting assembly and calls the wrapper.

Rebuild and run on Windows with Visual Studio's x64 C++ tools and C++/CLI support installed:

```powershell
.
proofs\utl-mixed-mode\build-and-run.ps1
```

The equivalent steps used for this verification were:

```powershell
MSBuild.exe proofs\utl-mixed-mode\MixedModeProof.vcxproj /m /p:Configuration=Release /p:Platform=x64 /v:minimal
dotnet build proofs\utl-mixed-mode\Consumer\Consumer.csproj -c Release
dotnet proofs\utl-mixed-mode\Consumer\bin\Release\net10.0-windows\Consumer.dll
```

Observed toolchain: Visual Studio Professional 2026 18.11, MSVC 14.51.36231 / compiler 14.51.36260, .NET SDK 10.0.200 on Windows x64. The C++/CLI build completed, the managed consumer built with zero warnings and errors, and execution printed `managed consumer received 42` and returned zero. The consumer output included `Ijwhost.dll` through an explicit copy item. A consumer that references only the assembly does not automatically copy this host DLL in the direct-reference setup used here.

After the successful build and run, a later rebuild attempt stopped before compilation with MSBuild `MSB8037`: this machine's registered Windows SDK 10.0.26100.0 lacks `Lib\10.0.26100.0\um\x64\gdi32.lib`, which Visual C++ uses to detect installed desktop SDK support. The already-built DLL remained the artifact used for REA, Ghidra, and load verification. The helper script checks this prerequisite and reports the missing SDK component; rebuilding from a clean checkout requires installing the Windows SDK desktop x64 libraries.

The C++ compiler emitted the mixed-mode startup/runtime code. No CRT, exception, security-cookie, or startup pseudocode was copied from Ghidra.

## C++/CLI Utl port spike

`UtlPortSpike.vcxproj` and `UtlPortSpike.cpp` are an isolated x64 Visual C++ DLL project under `proofs/utl-mixed-mode/`. The project targets `net10.0-windows`, sets `CLRSupport=NetCore` (MSVC `/clr:netcore`), uses the installed v145 toolset, names the output `Quire.Engine.Utl.dll`, and sets assembly version `0.1.0.0`. The C# project already has that name and version. The C++/CLI output is x64-only and unsigned, whereas the C# output is AnyCPU/IL-only and unsigned.

The spike ports the 15 Utl types listed in the earlier checkpoint plus `LabeledDictionary`, `SafeMonitor`, `Mailbox<T>`, `MessageQueue<Owner,Data,Msg>`, `TaskQueue`, `XmlUtils`, `LispUtils` with its nested `LinePartition`, `FileUtils`, and generic `UnaryVoidWrapper<T>`. It also ports `StringUtils`, `Pair<T1,T2>`, `Triple<T1,T2,T3>`, and `HashSetUtils` from `mcmisc.dll`. Managed C++ calls Win32 directly; it uses event, mutex, wait, and critical section APIs. The message queue uses `CreateEventW`, `SetEvent`, and `CloseHandle` for its worker shutdown signal. No dummy native method or reverse-engineered CRT/startup implementation was added. The C++ compiler generated its own exception, cookie, startup, and mixed-mode support.

The proof-only `IlMaxStack` post-link utility adjusts candidate max-stack headers when the original bound is at least as large, and transplants selected original method bodies after mapping their metadata tokens into the candidate. It preserves original exception sections and relocates bodies that do not fit the C++/CLI body allocation into a new read-only `.ilnew` PE section, updating the corresponding MethodDef RVA. It reserves each rewritten standalone-local-signature row and refuses to rewrite a blob shared with field or other local signatures; this avoids corrupting unrelated metadata while relocating exact bodies. The current batch transplants exact bodies for `LabeledDictionary`, most of `SafeMonitor`, the `TaskQueue` constructor, 39 `XmlUtils` methods, four `LispUtils` methods including `IndentTo`, 15 `FileUtils` methods, and both methods of generic `UnaryVoidWrapper<T>`. The generic `Mailbox<T>` is ported in C++/CLI; its queue and Win32 event/mutex operations run in the consumer. Broader Lisp body transplantation remains disabled because transplanted scanner methods produced `InvalidProgramException`; the tested C++/CLI scanner bodies stay active. `StringUtils`' static constructor still differs because the original uses a `PrivateImplementationDetails` RVA field with `RuntimeHelpers.InitializeArray`, which the C++/CLI compiler does not emit for the current source initializer.

Build and load the spike on Windows with the MSVC x64 C++/CLI workload and the Windows SDK desktop x64 libraries installed:

```powershell
$vs = (& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -all -products '*' -format json | ConvertFrom-Json | Select-Object -First 1)
& "$($vs.installationPath)\MSBuild\Current\Bin\MSBuild.exe" proofs\utl-mixed-mode\UtlPortSpike.vcxproj /m /p:Configuration=Release /p:Platform=x64 /v:minimal
dotnet build proofs\utl-mixed-mode\UtlPortConsumer\UtlPortConsumer.csproj -c Release
dotnet proofs\utl-mixed-mode\UtlPortConsumer\bin\Release\net10.0-windows\UtlPortConsumer.dll
```

On this machine the mixed DLL compiled with zero warnings or errors. The managed consumer compiled with zero warnings or errors and ran successfully, exercising a Win32 event, interrupt control, named mutex, critical section, sorted string set, string/thread helpers, `UnaryVoidWrapper<int>`, read/write lock, label trie, label utilities, `LabeledDictionary`, `XmlUtils`, `LispUtils`, `FileUtils` include expansion, line context, and Unicode conversion, `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`. The output folder contains `Ijwhost.dll`; the consumer includes it beside the application. This confirms the tested direct-reference deployment path, not deployment on a clean machine.

### Metadata and IL assessment

The latest candidate is named `Quire.Engine.Utl.dll`, version `0.1.0.0`, targets `.NETCoreApp,Version=v10.0`, and is an unsigned x64 PE32+ mixed-mode DLL. Its CLR header has the native-entry-point flag; `dumpbin /headers` identifies `_CorDllMain` as the PE entry point, and `dumpbin /clrheader` identifies `_DllMainCRTStartup` as the CLR header entry point. `dumpbin /headers` reports executable `.text` at `0x180001000–0x18000AEF7`, executable `.nep` at `0x18000B000–0x18000B57F`, and read-only initialized data `.ilnew` at `0x18002A000–0x180129FFF`. The `.ilnew` section stores transplanted IL bodies. `dumpbin /imports` reports `KERNEL32.dll`, `VCRUNTIME140.dll`, `MSVCP140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, and `ijwhost.dll`. Ghidra 12.1.4 imported and auto-analyzed the current candidate in `quire-utl-taskqueue-lisp-current-20261022-d3`; the P0 gate passed. The import logged C++/CLI metadata parsing errors around the relocated IL section and unresolved constructor decompilation. Its separate `auto-analyze` command fails because `AutoAnalyze.java` is missing, although Ghidra's import completed built-in auto-analysis. This confirms native disassembly and PE imports, not complete metadata recovery or function-by-function decompilation. The managed consumer loaded this output successfully. The current worktree has no `.mirror-cache/rea` report, so no current candidate REA report is available.

The current strict Release IL validator compares mapped candidate methods to the Mathcad artifact and checks the full candidate surface:

| Comparison | Result |
|---|---:|
| Candidate methods checked by the current validator | 390 |
| Exact mapped methods | 244 |
| Mapped methods still differing | 44 |
| Existing Quire methods removed from the candidate | 9 |
| Generated global `<Module>` methods without original bindings | 117 |
| Other candidate methods without Mathcad counterparts | 34 |
| Total validator failures | 202 |

The 244 exact methods include exact bodies for 24 `LabeledDictionary` methods, eight `SafeMonitor` methods, 39 `XmlUtils` methods, four `LispUtils` methods, 15 `FileUtils` methods, one `Mailbox<T>` method, six `TaskQueue` methods, two `UnaryVoidWrapper<T>` methods, `StringUtils` (15), `Pair<T1,T2>` (5), `Triple<T1,T2,T3>` (7), and `HashSetUtils` (2), in addition to the earlier Utl batch. Forty-four mapped methods still differ, including `Mailbox<T>` and `MessageQueue<Owner,Data,Msg>` methods, `SafeMonitor.GetCriticalSection` pointer typing, `FileUtils::.cctor`, `FileUtils.ReadExpanded(String,TextWriter)`, `FileUtils.LongUnicodeToShortcutKey`, `SafeMutex` methods, `StringUtils::.cctor`, both `LabelUtils` methods, the two `XmlUtils` test helpers, and most Lisp scanner and formatter methods. Nine original methods remain removed: two explicit `LabeledDictionary` interface enumerators, four `Logging` methods, `SafeMonitor.GetCriticalSection(IntPtr)`, and two `PrivateImplementationDetails` helpers. `Logging` remains unported: its original IL calls global C++ helpers `utl.ToStrng` and `utl.Log`; this work did not analyze `UTL.DLL`. This remains a failed full-assembly compatibility assessment, not an IL-parity pass.

### Maintainer validator run

The new candidate option builds the `.vcxproj` with MSBuild and sends its mixed-mode output through the unchanged Release IL comparison:

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

The current command built both the C# base and the C++/CLI candidate against `origin/main`, then returned `Release IL: FAIL; 390 checked`. The local report is `.mirror-cache/release-il-2364ba99a4b6/release-il.json`.

| Validator result | Count |
|---|---:|
| Candidate methods checked | 390 |
| Exact methods | 244 |
| Total failures | 202 |
| Existing methods absent from this candidate | 9 |
| Global `<Module>` methods without a reviewed Mathcad binding | 117 |
| Other methods without a Mathcad counterpart | 34 |

The validator keeps generated global methods and unbound candidate methods visible; no suppression or gate relaxation was added. Forty-four mapped methods still differ in their IL. The candidate remains incomplete and fails the Release IL gate.

The assembly simple name and version can be set to match. The output still differs in architecture, CLI flags, references, MVID, and managed surface. Current consumers reference `Quire.Engine.Utl.csproj`; replacing it with this `.vcxproj` would require a repository-wide project-reference migration, which is outside this spike and Lane D's allowed edits.

### Native inspection and dependencies

Ghidra 12.1.4 imported and auto-analyzed the current mixed DLL as an x64 PE through `ghidra-agent-cli` in `quire-utl-taskqueue-lisp-current-20261022-d3`; P0 passed, with C++/CLI metadata parsing errors around relocated IL blobs and unresolved-constructor warnings. Its separate `auto-analyze` command fails because the configured `AutoAnalyze.java` script is missing. `dumpbin /headers` confirms executable `.text` (`0x180001000–0x18000AEF7`) and `.nep` (`0x18000B000–0x18000B57F`) ranges plus the non-executable `.ilnew` range (`0x18002A000–0x180129FFF`). `dumpbin /imports` confirms direct imports of `KERNEL32.dll`, `VCRUNTIME140.dll`, `MSVCP140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, and `ijwhost.dll`; SafeMonitor and MessageQueue call kernel32 synchronization APIs. This confirms successful native disassembly and PE imports, but not function-by-function decompilation of original native wrapper code.

The managed consumer build and execution succeeded after the current DLL rebuild. It exercised the event, `InterruptControl`, named mutex, critical section, sorted set, thread/string helpers, read/write lock, label helpers, `LabeledDictionary`, `XmlUtils`, `LispUtils.IndentTo`, FileUtils Unicode mappings, `TaskQueue`, `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`. The consumer output carries `Ijwhost.dll`; deployment also needs the x64 .NET 10 runtime and compatible MSVC/UCRT runtime components. The machine already had these runtimes, so clean-machine deployment was not tested. A usable x64 Windows SDK desktop library component is required to build the project.

The original `utl-mgd.dll` was imported and statically analyzed by Ghidra after its identity was checked against the requested SHA-256 and MVID; it was never modified or executed. No `.mirror-cache/rea` report is available in this worktree. The spike implements existing Win32 handle operations from Quire's managed source and does not reproduce native bodies from the original assembly or inspect `UTL.DLL`.

## Inspection results

REA inspected the generated proof DLL statically without loading it. Observed output identity and format:

| Property | Observed value |
|---|---|
| File | `MixedModeProof.dll` |
| SHA-256 | `f42dd7ed35842eb58318a9c183fe6abbd188dca22a16545b81559e3a2640ad0a` |
| Architecture | x64 (`0x8664`) |
| PE optional header | PE32+ |
| CLI flags | `native-entry-point`; `ILONLY` is clear |
| REA classification | `mixed-clr-native`, implementation `cpp-cli-mixed` |
| Target framework metadata | `.NETCoreApp,Version=v10.0` |
| Assembly name/version | `MixedModeProof`, `0.0.0.0` |
| Managed wrapper | `UtlMixedModeProof.NativeBridge.Add(Int32, Int32) -> Int32`, MethodDef `0x06000054`, four decoded IL instructions |

The proof identity intentionally differs from Quire's assembly identity. It shows that the compiler emits metadata plus native code, not that Quire's assembly version, public API, or references have been preserved.

Ghidra 12.1.4 imported and analyzed the generated DLL as an x64 PE. It reported executable ranges `.text` (`0x180001000–0x1800039ff`) and `.nep` (`0x180004000–0x1800045ff`). It identified `NativeAdd` at `0x1800010e0` with a six-byte body: `lea eax,[rcx+rdx]` followed by `ret`. The retained script [`ReportMixedMode.java`](../../../proofs/utl-mixed-mode/ghidra/ReportMixedMode.java) prints those ranges and the function entry during headless analysis. Ghidra also logged unsupported C++/CLI PDB symbols and unresolved decompilation of generated managed wrapper constructors; analysis completed, and the native function's instructions were decoded.

`dumpbin /imports` showed these direct native dependencies:

- `ijwhost.dll` for the CLR DLL entry point;
- `VCRUNTIME140.dll`;
- `api-ms-win-crt-runtime-l1-1-0.dll`;
- `KERNEL32.dll`.

`Ijwhost.dll` must ship beside the consuming managed application for the tested setup. Deployment must also provide the matching x64 .NET 10 runtime and MSVC/UCRT runtime components. The test machine already had the runtime DLLs installed, so deployment on a clean machine was not exercised. The Windows SDK desktop x64 library component is a build prerequisite and was incomplete on the host during the later rebuild attempt.

## Why existing C# IL cannot be linked into this .NET 10 mixed-mode DLL

To avoid rewriting the existing methods, I compiled a small C# type into `ModuleApi.netmodule`, attached that module to a manifest DLL with Roslyn's `/addmodule`, and referenced the DLL from a normal `net10.0-windows` consumer. The reproducible source and runner are in [`ModuleSpike`](../../../proofs/utl-mixed-mode/ModuleSpike/). The consumer build succeeds, but execution fails with:

```text
System.IO.FileLoadException: Could not load file or assembly 'ModuleApi.netmodule'.
The module cannot be loaded because only single file assemblies are supported. (0x8013101E)
```

The same multi-module limitation applies to the C++ linker route: `/ASSEMBLYMODULE` imports a separate MSIL module into the assembly. Microsoft documents that assemblies consisting of multiple modules are unsupported on .NET 6 and later in [technologies unavailable on .NET 6+](https://learn.microsoft.com/en-us/dotnet/core/porting/net-framework-tech-unavailable#loading-multi-module-assemblies) and describes the linker option in [MSVC linker options](https://learn.microsoft.com/en-us/cpp/build/reference/linker-options?view=msvc-170). The build proof targets .NET 10, so that route cannot ship the existing C# module beside the C++/CLI module and still load it as Quire's single assembly.

## Single-file merge spike

I also tried merging the real Release `Quire.Engine.Utl.dll` into the generated mixed-mode proof with ILRepack 2.0.48. The reproducible command is in [`singlefile-merge-spike.ps1`](../../../proofs/utl-mixed-mode/singlefile-merge-spike.ps1); the tool is the [NuGet `dotnet-ilrepack` package](https://www.nuget.org/packages/dotnet-ilrepack/2.0.48).

- Without `/zeropekind`, ILRepack rejects the mixed-mode primary with `Failed to load assembly with Zero PeKind`.
- With `/zeropekind`, it reads and processes the mixed-mode and Quire metadata, then fails while writing: `System.NotSupportedException: Writing mixed-mode assemblies is not supported` from `Mono.Cecil.ModuleWriter.Write`.

No merged output was produced. This is a second concrete blocker: the C# IL merger can inspect the metadata, but its writer cannot emit the combined single-file mixed-mode PE. A custom PE/metadata writer would be a different, substantially larger project and would still need IL validation; this spike does not establish that it is impossible in principle.

This leaves two practical choices: keep C# and ship native code separately, or reauthor the managed methods for C++/CLI and accept that their generated IL must be independently compared and will not automatically preserve the existing Release IL. The requested same-DLL output and current exact-IL gate do not have a demonstrated supported bridge. The production project is therefore not converted in this change.

## Quire compatibility assessment

The literal mixed-mode output can carry both native machine code and managed metadata, and the proof confirms that an ordinary managed .NET 10 consumer can call its managed surface. Microsoft's .NET C++/CLI model requires a Windows `.vcxproj` DLL using `CLRSupport=NetCore` / `/clr:netcore`; mixed managed and native functions are supported in one assembly. See [C++/CLI migration guidance](https://learn.microsoft.com/en-us/dotnet/core/porting/cpp-cli) and [mixed native and managed assemblies](https://learn.microsoft.com/en-us/cpp/dotnet/mixed-native-and-managed-assemblies?view=msvc-170).

What remains unproven for Quire is material:

1. **Assembly identity and API:** the proof has another assembly name and version. A production project could configure the output name/version, but this work did not port Quire's 89 types and their managed declarations or compare its full public surface.
2. **Project references:** `Quire.Engine`, `Quire.Engine.Native`, `Quire.Efi`, and `Quire.Mcd` currently reference `Quire.Engine.Utl.csproj`. Replacing that project with `.vcxproj` requires updating or deliberately preserving those reference paths. Those consumers are outside Lane D's edit scope, so no integration claim is made.
3. **Exact Release IL:** the standard C# module-link path is concretely rejected by the .NET 10 loader (`0x8013101E`), and ILRepack cannot write a mixed-mode output. Reauthoring existing methods in C++/CLI would not retain their original C#-emitted IL. Exact parity for a rewritten full API has not been demonstrated.
4. **Validator support:** `tools/validation/release_il.py` now accepts `--candidate-vcxproj` for one production project and builds it in Release with MSBuild. The new path ran successfully against this spike and reported the concrete missing-method, unbound-method, and generated global-method failures above. A full C++/CLI candidate still needs complete managed coverage and exact IL before it can pass.

The safe disposition is to retain the C# project and its passing Release IL state. A future same-DLL migration would require a supported single-file IL-preserving merge or a decision to relax exact IL parity, plus complete API coverage and compatible consumer project references. Until then, the production `Quire.Engine.Utl.dll` remains C#; the separate spike is mixed-mode.

## Original artifact handling

The reference was only read from `C:\Program Files\PTC\Mathcad Prime 12.0.1.0\utl-mgd.dll`. The available REA evidence is bound to SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd` and MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`. This work did not modify or execute that DLL and did not analyze `UTL.DLL`. The original binary and detailed REA cache remain local and uncommitted.
