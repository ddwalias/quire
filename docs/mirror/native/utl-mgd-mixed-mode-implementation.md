# C++/CLI mixed-mode feasibility proof for `Quire.Engine.Utl`

## Outcome

The Windows C++/CLI proof now includes a small port of `SafeWaitHandle`, `SafeEvent`, and `SafeMutex`. Its managed .NET 10 consumer loads `Quire.Engine.Utl.dll`, exercises an event, and returns successfully. REA classifies the DLL as `mixed-clr-native` / `cpp-cli-mixed`; Ghidra imported and auto-analyzed the x64 PE. This proves that real Utl Win32 handle behavior can be built into a mixed-mode Quire-named DLL.

This is a three-type feasibility spike, not a replacement for the 89-type C# assembly. Its shared methods retain matching metadata signatures, but their Release IL is different. It also drops the rest of the managed API and adds C++/CLI compiler support types. The production C# project remains intact; no same-DLL Release IL parity claim is made.

The documented validator command, `python tools/validation/release_il.py --base main --project Quire.Engine.Utl`, passes for the current C# project: 129 methods checked, 129 exact, zero failures. Its local report is `.mirror-cache/release-il-28d8e663e644/release-il.json`. The maintainer has added a C++/CLI candidate path; the spike was subsequently run through that gate and failed closed with 428 mismatches, detailed below.

The reference REA report was not present in this worktree's `.mirror-cache/rea`, so I repeated the static boundary inspection against the supplied file. It matched the required SHA-256 and MVID. REA reported 55 native P/Invoke methods, two IL P/Invokes, eight runtime-provided methods (65 native-boundary rows total), and two module references. The native procedures are interop/runtime support, not an application-authored algorithm to port. The earlier addition routine exists only in the separate call-path proof; it is not proposed for production.

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

The spike ports the constructors and operations for `SafeWaitHandle`, `SafeEvent`, and `SafeMutex`. Native helpers call the actual Win32 APIs those classes already use: `CreateEventW`, `CreateMutexW`, `SetEvent`, `ResetEvent`, `CloseHandle`, `ReleaseMutex`, `WaitForSingleObject`, and `WaitForMultipleObjects`. They provide the handle-management behavior required by the managed types. No dummy native method or reverse-engineered CRT/startup implementation was added. The C++ compiler generated its own exception, cookie, startup, and mixed-mode support.

Build and load the spike on Windows with the MSVC x64 C++/CLI workload and the Windows SDK desktop x64 libraries installed:

```powershell
$vs = (& "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -all -products '*' -format json | ConvertFrom-Json | Select-Object -First 1)
& "$($vs.installationPath)\MSBuild\Current\Bin\MSBuild.exe" proofs\utl-mixed-mode\UtlPortSpike.vcxproj /m /p:Configuration=Release /p:Platform=x64 /v:minimal
dotnet build proofs\utl-mixed-mode\UtlPortConsumer\UtlPortConsumer.csproj -c Release
dotnet proofs\utl-mixed-mode\UtlPortConsumer\bin\Release\net10.0-windows\UtlPortConsumer.dll
```

On this machine the mixed DLL compiled with zero warnings or errors. The managed consumer compiled with zero warnings or errors and ran successfully, printing `managed consumer loaded the C++/CLI Utl spike and exercised its Win32 event`. The output folder contains `Ijwhost.dll`; the consumer includes it beside the application. This confirms the tested direct-reference deployment path, not deployment on a clean machine.

### Metadata and IL assessment

REA inspected both the C# Release output and the spike without loading either assembly. The final spike had SHA-256 `858bdd5af5fa7b4eda48af34484b20c8f1f1d4be72c0cecfb6a8901f8e801e3a`, assembly name/version `Quire.Engine.Utl, 0.1.0.0`, target framework `.NETCoreApp,Version=v10.0`, no public key, PE32+, x64 machine type, and the `native-entry-point` CLI flag with `ILONLY` clear. Its MVID was `5e1a6c3b-7408-4dab-8a22-b8490181f63a`. The final output was rebuilt and loaded by the managed consumer.

REA compared the spike with the current C# Release output by metadata signatures and normalized CIL:

| Comparison | Result |
|---|---:|
| Shared methods with exact signatures | 18 |
| Shared methods with exact Release IL | 0 |
| Changed methods among shared signatures | 18 |
| Removed / unimplemented methods | 359 |
| Added compiler/runtime methods | 224 |
| Unknown matches | 5 |
| MethodDef rows, C# / C++/CLI | 310 / 136 |
| AssemblyRef rows, C# / C++/CLI | 8 / 167 |

The 18 signature matches are the methods from the three ported types. None retained identical C# Release IL. Most of the removed members belong to types not included in this deliberately limited spike; the removed list also includes implementation helpers in those three types. Added C++/CLI metadata includes compiler/runtime support. REA marked the overall comparison partial because some C++-generated metadata was not fully decoded. This is a failed full-assembly compatibility assessment, not an IL-parity pass.

### Maintainer validator run

The new candidate option builds the `.vcxproj` with MSBuild and sends its mixed-mode output through the unchanged Release IL comparison:

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj
```

The command built both the C# base and the C++/CLI candidate, then returned `Release IL: FAIL; 136 checked`. The local report is `.mirror-cache/release-il-b8f81404f448/release-il.json`.

| Validator result | Count |
|---|---:|
| Candidate methods checked | 136 |
| Exact methods | 0 |
| Unfinished candidate methods | 136 |
| Total failures | 428 |
| Existing methods absent from this three-type spike | 292 |
| Global `<Module>` methods without a reviewed Mathcad binding | 87 |
| Other methods without a Mathcad counterpart | 49 |

The gate also reports failures among the three spike types: 3 `SafeEvent`, 6 `SafeMutex`, and 14 `SafeWaitHandle` methods lack Mathcad counterparts; three existing helper methods are absent. REA's separate C#-to-C++/CLI metadata comparison matched 18 signatures but found zero exact IL matches. The validator correctly keeps generated global methods and unbound candidate methods visible; no suppression or gate relaxation was added. This confirms that the current spike cannot replace the production assembly under the exact Release IL rule. The maintainer's `.vcxproj` build support is working; the candidate itself remains incomplete and non-parity.

The assembly simple name and version can be set to match. The output still differs in architecture, CLI flags, references, MVID, and managed surface. Current consumers reference `Quire.Engine.Utl.csproj`; replacing it with this `.vcxproj` would require a repository-wide project-reference migration, which is outside this spike and Lane D's allowed edits.

### Native inspection and dependencies

Ghidra 12.1.4 imported the final mixed DLL as an x64 PE through `ghidra-agent-cli` and reported successful analysis. It parsed the PDB and native sections; C++/CLI attributes and generated functions produced unresolved-constructor and PDB-type warnings. Ghidra's import operation performs its own automatic analysis. Separate CLI `auto-analyze`, baseline export, and decompile operations could not run because the installed `ghidra-agent-cli` distribution referenced missing scripts (`AutoAnalyze.java`, `ExportBaseline.java`, and `DecompileFunction.java`). `dumpbin` independently confirmed the executable code regions and decoded the native Win32-call wrappers; the native helpers are in `.text` at approximately `0x180001100–0x1800011c6`. The PE has executable `.text` (`0x180001000–0x1800044b7`) and `.nep` (`0x180005000–0x18000546f`) sections.

The generated DLL's direct imports are `KERNEL32.dll`, `VCRUNTIME140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, and `ijwhost.dll`. Kernel32 imports include the eight Win32 calls above plus compiler/runtime calls. The consumer output must carry `Ijwhost.dll`; deployment also needs the x64 .NET 10 runtime and compatible MSVC/UCRT runtime components. The machine already had these runtimes, so clean-machine deployment was not tested. A usable x64 Windows SDK desktop library component is required to build the project.

The original `utl-mgd.dll` was statically inspected by REA for its identity and native boundaries; it was never modified or executed. The spike implements existing Win32 handle operations from Quire's managed source and does not reproduce native bodies from the original assembly or inspect `UTL.DLL`.

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
