# `Quire.Engine.Utl` mixed-mode implementation proof

## Result

The Windows proof builds `Quire.Engine.Utl.dll` as an x64 C++/CLI mixed-mode assembly and references it from a managed .NET 10 consumer. The consumer loads managed types originating in both the C++/CLI source and a Roslyn C# netmodule, calls an exported unmanaged logging function, and exercises `SafeMutex`. The proof demonstrates that this build shape works.

It is not ready to replace the production C# project. The strict Release IL gate fails, API/reference compatibility is incomplete, and the DLL adds x64 and native deployment requirements. The production `src/Quire.Engine.Utl/Quire.Engine.Utl.csproj` and other Quire projects were not changed.

The source `UtlNativeLog.cpp` is a real unmanaged logging adapter. It accepts UTF-8 byte spans, calls the existing `utl::Log(Substring<char const>)` API through an import library, writes the message to the returned native stream, and flushes on request. It is compiled as unmanaged code and exported as `UtlNativeLogUtf8`. The consumer calls this export. No dummy native entry point or copied CRT, exception, startup, or security-cookie pseudocode is used. `UTL.DLL` was not analyzed. The original `utl-mgd.dll` was only read as reference data and was never modified or executed.

## Build design

The proof project is `proofs/utl-mixed-mode/UtlNetmoduleSpike/UtlNetmoduleSpike.vcxproj`. It is a Visual C++ Dynamic Library project configured for x64, MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`), and `net10.0-windows`. It emits assembly `Quire.Engine.Utl`, version `0.1.0.0`.

Most proof types are C++/CLI ports in `proofs/utl-mixed-mode/UtlPortSpike.cpp`. To retain selected Roslyn-shaped method bodies, `UtlManagedModule.csproj` emits a C# `.netmodule` with `HashSetUtils`, `Pair`, `StringUtils`, and `Triple`; the VCX project links this module into the final mixed-mode assembly. `McmiscInlineSupport.cs` contributes private static character-map data on `StringUtils` so the linker carries the compiler-generated data required by the transplanted utility body. C++ duplicate definitions of those four managed types are excluded for this build.

`IlMaxStack` is a proof-only post-link tool. It normalizes hidden helper names, relocates metadata when required, and transplants selected original Release method bodies and local signatures. It does not change the shared validator or its result. The current approach is fragile: the tool reports unmapped operands and unsupported local signatures, and it skips several candidate methods. It is evidence for a possible migration technique, not a production build step approved by parity.

`UtlNativeLog.cpp` is compiled with `CompileAsManaged=false`. `UTLLogging.def` and the project’s `BuildUtlLoggingImportLibrary` target generate the import library using MSVC `lib.exe`. The decorated logging import was observed in the reference metadata. The adapter's exported name and managed consumer declaration are proof-owned.

## Build and load verification

Build on Windows with Visual Studio's C++ workload, C++/CLI support for the selected MSVC toolset, the Windows SDK, and the .NET 10 SDK:

```powershell
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe'
& $msbuild proofs\utl-mixed-mode\UtlNetmoduleSpike\UtlNetmoduleSpike.vcxproj /m /p:Configuration=Release /p:Platform=x64 /v:minimal
dotnet build proofs\utl-mixed-mode\UtlNetmoduleSpike\Consumer\Consumer.csproj --configuration Release
$env:PATH = "C:\Program Files\PTC\Mathcad Prime 12.0.1.0;$env:PATH"
dotnet run --project proofs\utl-mixed-mode\UtlNetmoduleSpike\Consumer\Consumer.csproj --configuration Release --no-build
```

The Release build completed with zero compiler warnings and errors. The managed consumer printed successful results for `Str.RemoveWhite`, the C# netmodule `Pair` and `StringUtils` methods, the `UtlNativeLogUtf8` native export, and `SafeMutex`. Without the Prime install directory on `PATH`, Windows could not load the mixed-mode DLL because the static `UTL.DLL` dependency was unresolved. This is a controlled load test on the installed Prime machine, not evidence that the assembly loads on a machine without the Mathcad native provider.

## Release IL result

The maintainer's strict C++/CLI-aware validator was run against the current mixed-mode VCX candidate, using the integrated `main` commit as the base:

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlNetmoduleSpike/UtlNetmoduleSpike.vcxproj --results .mirror-cache/release-il-netmodule-hybrid-postmerge
```

It failed: **542 checked, 311 failures**. The ignored report is `.mirror-cache/release-il-netmodule-hybrid-postmerge/release-il.json` (base and tip `dda988fa80bce6a77d65606741e56c8fd08f25c1`). The reported failure groups are:

| Failure | Count | Detail |
|---|---:|---|
| Global `<Module>` methods without reviewed Mathcad bindings | 264 | MSVC/C++/CLI generated support methods have no reviewed Mathcad method binding. They remain failures under the strict gate. |
| Methods without a Mathcad counterpart | 36 | Candidate methods without a matching reference member. |
| Methods reported removed | 5 | Candidate metadata omits methods present in the reference. |
| Mapped body differences | 6 | `SafeMutex::.ctor(String,Int32)`, `MessageQueue.Start`, `XmlUtils.TestCheck`, `LispUtils.badSyntax`, `FileUtils.ReadExpanded`, and `StringUtils::.cctor`. |

No validation rules were weakened or bypassed. A separate run of the production C# project passed its scoped Release comparison, but that only validates the unchanged C# project; it does not establish parity for this VCX candidate. The mixed-mode candidate is not an exact Release IL match and cannot be claimed as a parity-preserving replacement.

## Identity, API, and references

REA inspected the project-local final DLL at SHA-256 `67fae7335180a538a6f1e36e7e6a6cb4f5f81dc7295cfc3bdd8167c38ec69016`, MVID `6f254fc6-f230-4706-ac68-d2a4705aac94`. It reports assembly `Quire.Engine.Utl`, version `0.1.0.0`, targeting `.NETCoreApp,Version=v10.0`, with complete managed metadata. The CLI header has `ILONLY` clear and a native entry point. REA counted 812 TypeDefs, 610 MethodDefs, 1,135 Fields, and 168 AssemblyRefs. Its managed-boundary report counted 51 P/Invoke imports and 64 native implementation boundaries.

The current production C# Release assembly has the same simple assembly name, version, and target framework. Its inspected output is AnyCPU and contains 38 TypeDefs, 310 MethodDefs, 168 Fields, and 8 AssemblyRefs. The proof candidate is x64 and introduces 160 additional AssemblyRefs plus native implementation and import metadata. Therefore, matching the assembly name/version is not enough to establish consumer compatibility. The proof consumer covers only a small set of types and calls; the strict IL gate also reports removed methods and body differences. The assembly references and public API need a complete migration review before replacement.

The Mathcad reference itself has a different identity: assembly `utl-mgd`, SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`. The proof preserves Quire's current simple name/version, not the original Mathcad assembly identity.

## Native code and dependencies

`dumpbin /headers` reports an x64 PE32+ DLL, a COM descriptor, and `_CorDllMain` at `0x18000D6B8`. The sections observed in the final build are:

| Section | Virtual address range | Role |
|---|---|---|
| `.text` | `0x180001000–0x18000D7B7` | Executable native code, including C++/CLI transitions and the unmanaged logging adapter. |
| `.nep` | `0x18000E000–0x18000ED3F` | Compiler-generated native entry support. |
| `.ilnew` | `0x18003A000–0x180139FFF` | Reserved IL bodies and relocated metadata; this is not native code. |

`dumpbin /exports` reports `UtlNativeLogUtf8` at RVA `0xA250`. `dumpbin /dependents` reports `UTL.DLL`, `MSVCP140.dll`, `VCRUNTIME140.dll`, UCRT API sets, `KERNEL32.dll`, and `mscoree.dll`. The consuming .NET application also needs a compatible `Ijwhost.dll`, the .NET 10 runtime, and the matching x64 VC++ runtime. A deployment must supply the Mathcad native provider in the Windows DLL search path. These requirements are different from the current AnyCPU C# assembly.

## REA and Ghidra inspection

REA's final reports are local ignored evidence in `.mirror-cache/netmodule-hybrid-g6-final-rea.json`, `.mirror-cache/netmodule-hybrid-g6-final-members.json`, and `.mirror-cache/netmodule-hybrid-g6-final-native-boundaries.json`. The final artifact hash in the reports is the hash listed above. REA admitted the x64 PE/CLI image, classified it as `mixed-clr-native`, parsed its metadata completely, and identified the native boundary declarations without loading target code.

Ghidra 12.1.4 imported the same final hash through `ghidra-agent-cli` into target `quire-utl-netmodule-final-g6`; import and automatic analysis succeeded. The loader identified the x64 PE, native entry, sections, dependencies, and PDB. Ghidra emitted CLI metadata markup errors for several relocated custom attribute, standalone-signature, and method-spec blobs, plus C++/CLI constructor p-code warnings. Its library search did not find `UTL.DLL`; it was not inspected. The dedicated CLI `ghidra auto-analyze` command is separately unusable in this installation because its configured `AutoAnalyze.java` script is absent, but the supported import command completed its automatic analysis. No replacement Ghidra scripts were created.

## Remaining migration work and risks

- The strict Release IL report fails; no parity or API-completeness claim is supported.
- C++/CLI-generated global methods need a reviewed comparison strategy that still checks every candidate method. The proof does not suppress them.
- Six mapped method bodies differ, five methods are reported removed, and 36 candidate methods have no reference counterpart.
- The netmodule/transplant path depends on post-link metadata rewriting and has unresolved operand/local-signature cases.
- Consumer loading was demonstrated only on x64 Windows with Prime's native provider and the required VC++/.NET runtime components available.
- All Quire consumers still reference the production C# project. Their production project references and runtime deployment have not been migrated or verified against this VCX candidate.

Keep the production C# assembly in place until the mixed-mode project can pass the unmodified Release IL gate and a complete consumer/reference migration review succeeds.
