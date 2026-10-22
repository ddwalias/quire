# `Quire.Engine.Utl` C++/CLI mixed-mode spike

## Result

The reviewable proof builds an x64 C++/CLI DLL named `Quire.Engine.Utl.dll`. A managed .NET 10 consumer references and loads it successfully. REA confirms the PE has managed metadata and a native entry point; Ghidra imported the current image and completed automatic analysis of its x64 code. The proof does not replace the production C# project and does not meet the complete Release IL or API parity requirement.

The strict candidate comparison was rerun against the current `origin/main` tip:

```powershell
python tools/validation/release_il.py --base origin/main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

It failed closed with 394 candidate members checked, 282 exact and 165 failures. The failure breakdown is 118 generated global `<Module>` methods without reviewed Mathcad bindings, 34 methods with no Mathcad counterpart, five methods removed from the candidate, and eight mapped Release IL differences. The current report is `.mirror-cache/release-il-65bd10a0f829/release-il.json`, based on `origin/main` at `3a4a6e11`. No suppression or validator bypass was added. The VCX project adds `$(OutDir)` to the C++/CLI assembly search path so the strict candidate build works when the validator redirects outputs to its isolated directory.

The six remaining mapped differences are:

- `SafeMutex::.ctor(String,Int32)`
- `StringUtils::.cctor`
- `XmlUtils.TestCheck(String,Boolean,Boolean,Int32)`
- `LispUtils.badSyntax(String,String,Int32)`
- `FileUtils::.cctor`
- `FileUtils.ReadExpanded(String,TextWriter)`

The candidate now has the `Logging` type and all four original members. `Logging::.ctor` and `Logging.IsLoggingEnabled` match their original Release IL. The `Log` and `LogLine` implementations call the existing UTF-8/native export boundary from the C# mirror, but their CIL still differs from the vendor bodies. `IsLoggingEnabled` calls a real native export through a C++/CLI module thunk; that thunk is one of the unbound global helpers in the strict report.

`FileUtils.LongUnicodeToShortcutKey(String,Int32&,Char&)`, `XmlUtils.SelfTest(String[])`, and `LispUtils.SelfTest(String[])` transplant exactly. The Lisp self-test exposed a proof transplant defect: catch-clause type tokens were copied without mapping. `BodyTransplant.cs` now remaps catch tokens in small and fat exception sections. The exact Unicode shortcut body also exposed a `List<T>.Enumerator` value-type encoding error; both `+Enumerator` and `/Enumerator` spellings are now classified as value types. The rebuilt managed consumer loaded the candidate successfully after these fixes.

The production `Quire.Engine.Utl` C# project remains intact. The current candidate has only a subset of its managed API, so no full migration or same-DLL parity claim is made.

## Build design and changes

`proofs/utl-mixed-mode/UtlPortSpike.vcxproj` is an x64 Visual C++ Dynamic Library project using MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`) and target framework `net10.0-windows`. It emits the assembly simple name `Quire.Engine.Utl` and version `0.1.0.0`, matching the current production C# assembly identity. The proof source ports existing managed utility behavior to C++/CLI and uses Win32 synchronization APIs for events, mutexes, waits, and critical sections. It does not add a dummy native function or transcribe CRT, exception, cookie, or startup routines from Ghidra. MSVC supplies its own mixed-mode startup and transition code.

The spike ports Utl synchronization and collection types, queue and mailbox types, `LabeledDictionary`, and selected `Engine.Misc` utility types including `XmlUtils`, `LispUtils` with its nested line partition, `FileUtils`, `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`. `IlMaxStack/BodyTransplant.cs` is a proof-local post-link helper: it raises candidate max-stack values when safe, maps metadata tokens for selected original method bodies, remaps catch clauses, preserves exception sections, and stores relocated bodies in `.ilnew`. The helper rejects bodies whose metadata cannot be represented safely. The recent generic MethodSpec type mapping enables the exact `MessageQueue.Start` transplant; the value-type correction enables the exact FileUtils shortcut enumerator body.

The earlier small proof in `MixedModeProof.vcxproj` demonstrates a standalone unmanaged function called by a public C++/CLI wrapper. The C# consumer references that DLL and successfully calls the wrapper. The Utl port consumer separately references the candidate by assembly path. These proofs establish Windows C++/CLI reference and load behavior before any production project replacement.

Three of the six mapped differences are sensitive to C# compiler lowering: `XmlUtils.TestCheck`, `LispUtils.badSyntax`, and `FileUtils.ReadExpanded`. The C# compiler emits calls to generic helpers on `<PrivateImplementationDetails>` and uses a `ReadOnlySpan<object>` local in `ReadExpanded`. A C++/CLI helper with an equivalent implementation changes the helper's declaring type, so the strict comparison still sees different calls. C++/CLI also rejects a `ReadOnlySpan<Object>` local (`C3225`/`C4950`: embedded reference types are unsupported in this compiler mode); the proof transplant helper cannot safely create a matching local-signature entry. The source implementation remains functional and the strict report records these differences. No validator rule was weakened.

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

REA's static name-agnostic inventory comparison against the original `utl-mgd.dll` returned 8 unchanged, 132 changed, 421 added, and 293 unknown members; its coverage was partial because candidate fields and ambiguous matches were truncated. The Release IL validator report is the authoritative mapped-method result. The original managed artifact identity was independently checked with REA: SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`, assembly name `utl-mgd`, version `0.0.0.0`, 89 TypeDefs and 370 MethodDefs. It was statically inspected only; it was never modified or executed. `UTL.DLL` was not analyzed.

## Verification

The Windows Release build used Visual Studio Professional 2026 18.11, MSVC 14.51.36231, .NET SDK 10.0.200, and x64. The candidate compiled successfully. The existing `UtlPortConsumer` built with zero warnings and errors and ran successfully after the enumerator signature correction. It exercised the C++/CLI synchronization types, queues, mailbox, XML and Lisp routines, FileUtils Unicode maps, and the other ported utilities. This confirms loadability on the build machine; a clean-machine install was not tested.

REA statically inspected the current candidate (SHA-256 `2ccca0acbf96db5f5aeee8281707c908d0005e4c04dca361e658ba66188b3977`, MVID `21a1ad34-a25b-4288-91cc-acc5e45dc970`). It reported a complete CLI metadata parse, x86-64 PE32+, a native-entry-point CLI flag, CLI native RVA entry point `0xbbad`, assembly identity above, and `.NETCoreApp,Version=v10.0`. It found 543 TypeDefs, 462 MethodDefs and 168 AssemblyRefs. Its native-boundary summary reports 26 P/Invoke declarations, 39 native implementation/runtime entries, and two module references. The production C# Release build inspected in the same worktree has 38 TypeDefs, 310 MethodDefs and 8 AssemblyRefs.

`dumpbin /headers` reports `_CorDllMain` at `0x18000B84D` and these image ranges:

| Section | Virtual address range | Use |
|---|---|---|
| `.text` | `0x180001000–0x18000BCA7` | Executable code |
| `.nep` | `0x18000C000–0x18000C58F` | Native entry-point support |
| `.ilnew` | `0x18002B000–0x18012AFFF` | Relocated managed IL bodies |

`dumpbin /imports` reports `KERNEL32.dll`, `VCRUNTIME140.dll`, `MSVCP140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, and `ijwhost.dll`. The candidate does not statically import `UTL.DLL`; its logging methods use `NativeLibrary` when called.

Ghidra 12.1.4 imported the current candidate through `ghidra-agent-cli` as an x64 PE and completed automatic analysis during import. The executable code ranges are `.text` at `0x180001000–0x18000BCA7` and `.nep` at `0x18000C000–0x18000C58F`; `.ilnew` at `0x18002B000–0x18012AFFF` contains relocated IL bodies and is not executable. Ghidra loaded the MSVC/UCRT dependencies and `ijwhost.dll` for symbol resolution; it reported CLI custom-attribute blob conflicts, PDB type-alignment warnings, and unresolved C++/CLI constructor-decompilation warnings. Its separate auto-analysis and baseline-export wrappers still fail because their configured scripts are absent. The current candidate has no static UTL import, so this inspection did not load or analyze `UTL.DLL`. A gated Ghidra baseline export and per-function decompilation were not obtained.

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
