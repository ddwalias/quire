# `Quire.Engine.Utl` C++/CLI mixed-mode spike

## Result

The reviewable proof builds an x64 C++/CLI DLL named `Quire.Engine.Utl.dll`. A managed .NET 10 consumer references and loads it successfully. REA confirms the PE has managed metadata and a native entry point; Ghidra imported the current image and analyzed its x64 code. The proof does not replace the production C# project and does not meet the complete Release IL or API parity requirement.

The latest strict candidate comparison was run against `origin/main`:

```powershell
python tools/validation/release_il.py --base origin/main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

It failed closed with 390 candidate members checked, 278 exact and 168 failures. The failure breakdown is 117 generated global `<Module>` methods without reviewed Mathcad bindings, 34 methods with no Mathcad counterpart, nine methods removed from the candidate, and eight mapped Release IL differences. The report is `.mirror-cache/release-il-1400f5b52bda/release-il.json`. No suppression or validator bypass was added.

The eight remaining mapped differences are:

- `SafeMutex::.ctor(String,Int32)`
- `StringUtils::.cctor`
- `XmlUtils.TestCheck(String,Boolean,Boolean,Int32)`
- `XmlUtils.SelfTest(String[])`
- `LispUtils.badSyntax(String,String,Int32)`
- `LispUtils.SelfTest(String[])`
- `FileUtils::.cctor`
- `FileUtils.ReadExpanded(String,TextWriter)`

`FileUtils.LongUnicodeToShortcutKey(String,Int32&,Char&)` now transplants exactly. Its first runtime check exposed an error in the proof transplant utility: `List<T>.Enumerator` had been encoded as a class. The proof utility now classifies both `+Enumerator` and `/Enumerator` spellings as value types. A rebuilt consumer then loaded and exercised the method successfully.

The production `Quire.Engine.Utl` C# project remains intact. The current candidate has only a subset of its managed API, so no full migration or same-DLL parity claim is made.

## Build design and changes

`proofs/utl-mixed-mode/UtlPortSpike.vcxproj` is an x64 Visual C++ Dynamic Library project using MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`) and target framework `net10.0-windows`. It emits the assembly simple name `Quire.Engine.Utl` and version `0.1.0.0`, matching the current production C# assembly identity. The proof source ports existing managed utility behavior to C++/CLI and uses Win32 synchronization APIs for events, mutexes, waits, and critical sections. It does not add a dummy native function or transcribe CRT, exception, cookie, or startup routines from Ghidra. MSVC supplies its own mixed-mode startup and transition code.

The spike ports Utl synchronization and collection types, queue and mailbox types, `LabeledDictionary`, and selected `Engine.Misc` utility types including `XmlUtils`, `LispUtils` with its nested line partition, `FileUtils`, `StringUtils`, `Pair`, `Triple`, and `HashSetUtils`. `IlMaxStack/BodyTransplant.cs` is a proof-local post-link helper: it raises candidate max-stack values when safe, maps metadata tokens for selected original method bodies, preserves exception sections, and stores relocated bodies in `.ilnew`. The helper rejects bodies whose metadata cannot be represented safely. The recent generic MethodSpec type mapping enables the exact `MessageQueue.Start` transplant; the value-type correction enables the exact FileUtils shortcut enumerator body.

The earlier small proof in `MixedModeProof.vcxproj` demonstrates a standalone unmanaged function called by a public C++/CLI wrapper. The C# consumer references that DLL and successfully calls the wrapper. The Utl port consumer separately references the candidate by assembly path. These proofs establish Windows C++/CLI reference and load behavior before any production project replacement.

The `ReadExpanded(String,TextWriter)` body remains different. Its original local signature uses the .NET compiler's `InlineArray4<object>` representation while lowering a `ReadOnlySpan<object>`. MSVC C++/CLI rejects declaring `ReadOnlySpan<Object>` as a local (`C3225`/`C4950`: embedded reference types are unsupported in this compiler mode), and the proof transplant utility cannot encode the original internal `InlineArray4<object>` local signature. The candidate keeps a source implementation and the strict report records the mismatch. The gate is not altered to hide it.

## Identity, API, and references

REA compared the current candidate with the production C# Release output. Both have the same assembly simple name (`Quire.Engine.Utl`), version (`0.1.0.0`), neutral culture, and unsigned public-key token. Their deployment properties differ:

| Property | Production C# Release | C++/CLI proof |
|---|---|---|
| Architecture | AnyCPU / x86 PE | x64 |
| CLI mode | IL-only | Mixed CLR/native; native entry point |
| Assembly references | 8 | 168 |
| MethodDef rows | 310 | 457, including compiler support methods |
| Public surface | Existing production API | Partial port plus C++/CLI support types |

The strict mirror validator identifies nine missing original methods: the two explicit `LabeledDictionary` interface enumerators, four `Logging` methods, `SafeMonitor.GetCriticalSection(IntPtr)`, and two generated private implementation methods. It also reports the additional unbound methods described above. `Engine`, `Engine.Native`, `Efi`, and `Mcd` currently reference the C# project. The proof does not change those projects or prove a project-reference migration. Matching the simple name and version alone is insufficient to establish API, reference, architecture, or deployment compatibility.

REA's static name-agnostic inventory comparison against the original `utl-mgd.dll` returned 8 unchanged, 132 changed, 421 added, and 293 unknown members; its coverage was partial because candidate fields and ambiguous matches were truncated. The Release IL validator report is the authoritative mapped-method result. The original managed artifact identity was independently checked with REA: SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`, assembly name `utl-mgd`, version `0.0.0.0`, 89 TypeDefs and 370 MethodDefs. It was statically inspected only; it was never modified or executed. `UTL.DLL` was not analyzed.

## Verification

The Windows Release build used Visual Studio Professional 2026 18.11, MSVC 14.51.36231, .NET SDK 10.0.200, and x64. The candidate compiled successfully. The existing `UtlPortConsumer` built with zero warnings and errors and ran successfully after the enumerator signature correction. It exercised the C++/CLI synchronization types, queues, mailbox, XML and Lisp routines, FileUtils Unicode maps, and the other ported utilities. This confirms loadability on the build machine; a clean-machine install was not tested.

REA statically inspected the current candidate (SHA-256 `51c9d949fcce22ef813699321592667ce9ae850c778b442b112063a9e296123c`). It reported a complete CLI metadata parse, x86-64 PE32+, a native-entry-point CLI flag, native RVA entry point `0x92b0`, assembly identity above, and `.NETCoreApp,Version=v10.0`. It found 541 TypeDefs, 457 MethodDefs and 168 AssemblyRefs. Its native-boundary summary reports 26 P/Invoke declarations, 39 native implementation/runtime entries, and two module references.

`dumpbin /headers` reports `_CorDllMain` at `0x18000B82D` and these image ranges:

| Section | Virtual address range | Use |
|---|---|---|
| `.text` | `0x180001000–0x18000B927` | Executable code |
| `.nep` | `0x18000C000–0x18000C57F` | Native entry-point support |
| `.ilnew` | `0x18002B000–0x18012AFFF` | Relocated managed IL bodies |

`dumpbin /imports` reports `KERNEL32.dll`, `VCRUNTIME140.dll`, `MSVCP140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, and `ijwhost.dll`.

Ghidra 12.1.4 imported the same candidate through `ghidra-agent-cli` as an x64 PE and began analysis of all memory and code. Its output includes the executable sections and imported dependencies, with PDB alignment and unresolved C++/CLI constructor-decompilation warnings. The CLI's follow-on `auto-analyze` operation did not complete because its configured `AutoAnalyze.java` helper is absent; the P0 gate also reports the generated workspace state files as untracked. Therefore the import and native code analysis were observed, but a complete gated Ghidra baseline/export and per-function decompilation were not obtained. The PE ranges above are corroborated by `dumpbin`; the full Ghidra workflow remains a verification limitation.

## Deployment and remaining migration work

The tested consumer output carried `Ijwhost.dll` beside the application. Deployment requires a matching x64 .NET 10 runtime, `ijwhost.dll`, and compatible MSVC/UCRT runtime components (including the imported VCRUNTIME/MSVCP/CRT DLLs). The test machine already had those runtimes. Building requires the Visual Studio C++ workload, the MSVC C++/CLI component, and Windows SDK desktop x64 libraries.

Two single-file approaches were also tested before considering a production conversion. The C# module linked with Roslyn `/addmodule` builds but .NET 10 rejects loading the multi-module assembly (`0x8013101E`, single-file assemblies only). ILRepack 2.0.48 can read the mixed-mode and C# metadata but fails while writing because its Mono.Cecil writer does not support mixed-mode assemblies. These experiments do not prove no custom writer could be built; they show neither available path preserves the C# module in the tested single mixed-mode output.

Replacing the C# project with this `.vcxproj` would therefore still require complete managed API coverage, compatibility with all current project references, the eight mapped IL fixes, a treatment for 117 generated global methods and 34 unbound methods under the strict gate, removal of the nine missing methods, and a supported deployment/build setup. Until that is achieved, keep the production C# assembly. The mixed-mode DLL remains an isolated proof.

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
