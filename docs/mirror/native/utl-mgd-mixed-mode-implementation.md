# `Quire.Engine.Utl` C++/CLI mixed-mode spike

## Result

The reviewable proof builds an x64 C++/CLI DLL named `Quire.Engine.Utl.dll`. A managed .NET 10 consumer references and loads it successfully. REA confirms the PE has managed metadata and a native entry point; Ghidra imported the current image and completed automatic analysis of its x64 code. The proof does not replace the production C# project and does not meet the complete Release IL or API parity requirement.

The strict candidate comparison was rerun against `origin/main` at `69a6b0df5a3f137b32f12a88460160056c426a22`:

```powershell
python tools/validation/release_il.py --base origin/main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

It failed closed with 394 candidate members checked, 282 exact and 165 failures. The breakdown is 118 generated global `<Module>` methods without reviewed Mathcad bindings, 34 methods with no Mathcad counterpart, five methods removed from the candidate, and eight mapped Release IL differences. The report is `.mirror-cache/release-il-81e62a36a16b/release-il.json`. No suppression or validator bypass was added. The VCX project now relies on its `Quire.Mirror` project reference and `$(OutDir)` assembly search path; removing the duplicate hard-coded HintPath and `#using` fixed the isolated validator build, which had been importing two copies of `Quire.Mirror.dll` under `/WX`.

The eight remaining mapped differences are:

- `SafeMutex::.ctor(String,Int32)`
- `StringUtils::.cctor`
- `XmlUtils.TestCheck(String,Boolean,Boolean,Int32)`
- `LispUtils.badSyntax(String,String,Int32)`
- `FileUtils::.cctor`
- `FileUtils.ReadExpanded(String,TextWriter)`
- `Logging.Log(String,String)`
- `Logging.LogLine(String,String)`

The candidate now has the `Logging` type and all four original members. `Logging::.ctor` and `Logging.IsLoggingEnabled` match their original Release IL. The `Log` and `LogLine` implementations call the existing UTF-8/native export boundary from the C# mirror, but their CIL still differs from the vendor bodies. `IsLoggingEnabled` calls a real native export through a C++/CLI module thunk; that thunk is one of the unbound global helpers in the strict report. The consumer intentionally does not call Logging, because that path dynamically loads the separate PTC `UTL.DLL`.

`FileUtils.LongUnicodeToShortcutKey(String,Int32&,Char&)`, `XmlUtils.SelfTest(String[])`, and `LispUtils.SelfTest(String[])` transplant exactly. The Lisp self-test exposed a proof transplant defect: catch-clause type tokens were copied without mapping. `BodyTransplant.cs` now remaps catch tokens in small and fat exception sections. The exact Unicode shortcut body also exposed a `List<T>.Enumerator` value-type encoding error; both `+Enumerator` and `/Enumerator` spellings are now classified as value types. The rebuilt managed consumer loaded the candidate successfully after these fixes.

The production `Quire.Engine.Utl` C# project remains intact. The current candidate has only a subset of its managed API, so no full migration or same-DLL parity claim is made.

## Build design and changes

`proofs/utl-mixed-mode/UtlPortSpike.vcxproj` is an x64 Visual C++ Dynamic Library project using MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`) and target framework `net10.0-windows`. It emits the assembly simple name `Quire.Engine.Utl` and version `0.1.0.0`, matching the current production C# assembly identity. The proof source ports existing managed utility behavior to C++/CLI and uses Win32 synchronization APIs for events, mutexes, waits, and critical sections. It does not add a dummy native function or transcribe CRT, exception, cookie, or startup routines from Ghidra. MSVC supplies its own mixed-mode startup and transition code.

The spike ports Utl synchronization and collection types, queue and mailbox types, `LabeledDictionary`, and selected `Engine.Misc` utility types including `XmlUtils`, `LispUtils` with its nested line partition, `FileUtils`, `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`. `IlMaxStack/BodyTransplant.cs` is a proof-local post-link helper: it raises candidate max-stack values when safe, maps metadata tokens for selected original method bodies, remaps catch clauses, preserves exception sections, and stores relocated bodies in `.ilnew`. The helper rejects bodies whose metadata cannot be represented safely. The recent generic MethodSpec type mapping enables the exact `MessageQueue.Start` transplant; the value-type correction enables the exact FileUtils shortcut enumerator body.

The earlier small proof in `MixedModeProof.vcxproj` demonstrates a standalone unmanaged function called by a public C++/CLI wrapper. The C# consumer references that DLL and successfully calls the wrapper. The Utl port consumer separately references the candidate by assembly path. These proofs establish Windows C++/CLI reference and load behavior before any production project replacement.

Three mapped differences are sensitive to C# compiler lowering: `XmlUtils.TestCheck`, `LispUtils.badSyntax`, and `FileUtils.ReadExpanded`. The C# compiler emits calls to generic helpers on `<PrivateImplementationDetails>` and uses a `ReadOnlySpan<object>` local in `ReadExpanded`. A C++/CLI helper with an equivalent implementation changes the helper's declaring type, so the strict comparison still sees different calls. C++/CLI also rejects a `ReadOnlySpan<Object>` local (`C3225`/`C4950`: embedded reference types are unsupported in this compiler mode); the proof transplant helper cannot safely create a matching local-signature entry. The updated `badSyntax` body now calls the existing `FileUtils.ComputeLineNumber` helper like the C# port, but its `Console.WriteLine` inline-array lowering still differs. The source implementation remains functional and the strict report records these differences. No validator rule was weakened.

A C# netmodule experiment shows one possible route for keeping Roslyn-emitted IL in a mixed DLL, but it is not yet usable for a whole existing type. A small C# netmodule linked as a normal linker input produces a single-file mixed-mode DLL that a .NET 10 C# consumer can load. Two netmodules that each declare a partial fragment of the same type fail with duplicate-type and duplicate-constructor diagnostics (`C3846`/`LNK2005`). Linking the complete generated `LispUtils` C# type instead fails in MSVC 14.51 link-time code generation with `C1001` at `imeta.cpp:579`, even in a reduced C++/CLI link project. A one-method `LispUtils.badSyntax` module links only after removing the C++ `LispUtils` type; it cannot be combined with the remaining C++ fragments of that type. No source from these experiments is tracked; the logs and modules remain in `.mirror-cache`.

## Identity, API, and references

REA compared the current candidate with the production C# Release output. Both have the same assembly simple name (`Quire.Engine.Utl`), version (`0.1.0.0`), neutral culture, and unsigned public-key token. Their deployment properties differ:

| Property | Production C# Release | C++/CLI proof |
|---|---|---|
| Architecture | AnyCPU / x86 PE | x64 |
| CLI mode | IL-only | Mixed CLR/native; native entry point |
| Assembly references | 8 | 168 |
| TypeDef rows | 38 | 543, including compiler support types |
| MethodDef rows | 310 | 462, including compiler support methods |
| Public surface | Existing production API | Partial port plus C++/CLI support types |

The strict comparison still reports five removed members: the two source-qualified `LabeledDictionary` enumerators, `SafeMonitor.GetCriticalSection(IntPtr)`, and two generated `PrivateImplementationDetails` methods. The C++ candidate contains `_e1`/`_e2` enumerators and pointer-returning `SafeMonitor.GetCriticalSection`, matching the original mixed-mode metadata names; the C# baseline uses different enumerator names and `IntPtr`, so the comparison reports those as removals. The generated inline-array helpers are absent. `Engine`, `Engine.Native`, `Efi`, and `Mcd` still reference the C# project. The proof does not change those projects or prove a project-reference migration. Matching the simple name and version alone is insufficient to establish API, reference, architecture, or deployment compatibility.

REA's name-agnostic inventory comparison between the current C# Release assembly and the mixed-mode candidate reports 8 unchanged, 86 changed, 514 added, 0 removed, and 237 unknown matches; coverage is partial, with 282 candidate fields and 178 ambiguous candidates omitted. This cannot establish public API equivalence because it does not match by names and leaves many members unknown. The strict Release IL report identifies the five removals listed above. The original managed artifact identity was independently checked with REA: SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`, assembly name `utl-mgd`, version `0.0.0.0`, 89 TypeDefs and 370 MethodDefs. It was statically inspected only; it was never modified or executed. `UTL.DLL` was not analyzed.

## Verification

The Windows Release build used Visual Studio Professional 2026 18.11, MSVC 14.51.36231, .NET SDK 10.0.200, and x64. The candidate compiled successfully. The existing `UtlPortConsumer` built with zero warnings and errors and ran successfully after the enumerator signature correction. It exercised the C++/CLI synchronization types, queues, mailbox, XML and Lisp routines, FileUtils Unicode maps, and the other ported utilities. This confirms loadability on the build machine; a clean-machine install was not tested.

REA statically inspected the current candidate (SHA-256 `5d319447eb88576b5e156356aa7baacdf9bba1e733d735ff401fcad90e3d9338`, MVID `0c59da97-dd40-448c-94b8-f27848424fc4`). It reported a complete CLI metadata parse, x86-64 PE32+, a native-entry-point CLI flag, native RVA entry point `0x9590`, assembly identity above, and `.NETCoreApp,Version=v10.0`. It found 543 TypeDefs, 462 MethodDefs and 168 AssemblyRefs. Its metadata reports 26 P/Invoke declarations and two module references. The production C# Release build inspected in the same worktree has 38 TypeDefs, 310 MethodDefs and 8 AssemblyRefs.

`dumpbin /headers` reports `_CorDllMain` at `0x18000BB0D` and these image ranges:

| Section | Virtual address range | Use |
|---|---|---|
| `.text` | `0x180001000–0x18000BC07` | Executable code |
| `.nep` | `0x18000C000–0x18000C58F` | Native entry-point support |
| `.ilnew` | `0x18002B000–0x18012AFFF` | Relocated managed IL bodies |

`dumpbin /imports` reports `KERNEL32.dll`, `VCRUNTIME140.dll`, `MSVCP140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, and `ijwhost.dll`. The candidate does not statically import `UTL.DLL`; its logging methods use `NativeLibrary` when called.

Ghidra 12.1.4 imported this exact candidate through `ghidra-agent-cli` as an x64 PE; the import log reports analysis succeeded in 28 seconds. The image maps executable code in `.text` at `0x180001000–0x18000BC07` and `.nep` at `0x18000C000–0x18000C58F`; `.ilnew` at `0x18002B000–0x18012AFFF` contains relocated IL bodies and is not executable. Ghidra logged CLI custom-attribute, standalone-signature, and MethodSpec metadata markup errors. The separate baseline-export wrapper is still unavailable because its configured scripts are absent, so no gated Ghidra baseline or per-function decompilation was obtained. The current candidate has no static UTL import, so this inspection did not load or analyze `UTL.DLL`.

## Deployment and remaining migration work

The tested consumer output carried `Ijwhost.dll` beside the application. The managed consumer loaded the candidate and exercised Win32 event/interrupt/mutex operations, queues, mailbox, task queue, XML/Lisp scanners, FileUtils, and collection helpers. It did not call the new `Logging` methods, so no `UTL.DLL` code was loaded or executed. This confirms loadability on the build machine; a clean-machine install was not tested. Deployment requires a matching x64 .NET 10 runtime, `ijwhost.dll`, compatible MSVC/UCRT runtime components (including VCRUNTIME/MSVCP/CRT DLLs), and `UTL.DLL` on the native search path when logging methods are called. Building requires the Visual Studio C++ workload, the MSVC C++/CLI component, and Windows SDK desktop x64 libraries.

Two single-file approaches were also tested before considering a production conversion. The C# module linked with Roslyn `/addmodule` builds but .NET 10 rejects loading the multi-module assembly (`0x8013101E`, single-file assemblies only). ILRepack 2.0.48 can read the mixed-mode and C# metadata but fails while writing because its Mono.Cecil writer does not support mixed-mode assemblies. These experiments do not prove no custom writer could be built; they show neither available path preserves the C# module in the tested single mixed-mode output.

Replacing the C# project with this `.vcxproj` would therefore still require complete managed API coverage, compatibility with all current project references, the eight mapped IL fixes, a treatment for 118 generated global methods and 34 unbound methods under the strict gate, removal or reconciliation of the five reported member removals, and a supported deployment/build setup. `Logging.Log` and `Logging.LogLine` remain functional ports with non-matching IL. Until those issues are resolved, keep the production C# assembly. The mixed-mode DLL remains an isolated proof.

## Reproduction

Build and run the spike on Windows with the prerequisites above:

```powershell
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
$vs = (& $vswhere -all -products '*' -format json | ConvertFrom-Json | Select-Object -First 1)
& (Join-Path $vs.installationPath 'MSBuild\Current\Bin\MSBuild.exe') proofs\utl-mixed-mode\UtlPortSpike.vcxproj -t:Rebuild /m /p:Configuration=Release /p:Platform=x64 /v:minimal
dotnet build proofs\utl-mixed-mode\UtlPortConsumer\UtlPortConsumer.csproj -c Release
dotnet proofs\utl-mixed-mode\UtlPortConsumer\bin\Release\net10.0-windows\UtlPortConsumer.dll
```

The small wrapper proof is reproduced with `proofs/utl-mixed-mode/build-and-run.ps1`. Original binaries, decompiled source, IL and generated REA/Ghidra evidence remain local and uncommitted.
