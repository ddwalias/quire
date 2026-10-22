# `Quire.Engine.Utl` mixed-mode implementation proof

## Result

The Windows Release proof builds `Quire.Engine.Utl.dll` as an x64 C++/CLI mixed-mode assembly and references it from a managed .NET 10 consumer. It exercises the transplanted two-argument `SafeMutex` constructor, both `LabeledDictionary` interface enumerators, all three `SafeMonitor` entry points, managed methods from the C# netmodule, and the exported unmanaged logging function. These calls run successfully.

The candidate now has exact Release IL for the 292 body-bearing methods that MethodAudit maps to Mathcad (`161` from `utl-mgd`, `131` from `mcmisc`) and preserves 18 declaration-only method signatures. The strict whole-candidate gate still reports compiler-generated C++/CLI support methods and metadata-name differences, described below; it reports no mapped method-body differences. This proof is not wired into the production C# project, and the project-wide API, reference, and deployment migration is not complete. The production `src/Quire.Engine.Utl/Quire.Engine.Utl.csproj` and other Quire projects were not changed.

The verified Release artifact is `proofs/utl-mixed-mode/UtlNetmoduleSpike/artifacts/bin/UtlNetmoduleSpike/Release/Quire.Engine.Utl.dll`.

The source `UtlNativeLog.cpp` is a real unmanaged logging adapter. It accepts UTF-8 byte spans, calls the existing `utl::Log(Substring<char const>)` API through an import library, writes the message to the returned native stream, and flushes on request. It is compiled as unmanaged code and exported as `UtlNativeLogUtf8`. The consumer calls this export. No dummy native entry point or copied CRT, exception, startup, or security-cookie pseudocode is used. `UTL.DLL` was not analyzed. The original `utl-mgd.dll` was only read as reference data and was never modified or executed.

## Build design

The proof project is `proofs/utl-mixed-mode/UtlNetmoduleSpike/UtlNetmoduleSpike.vcxproj`. It is a Visual C++ Dynamic Library project configured for x64, MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`), and `net10.0-windows`. It emits assembly `Quire.Engine.Utl`, version `0.1.0.0`.

Most proof types are C++/CLI ports in `proofs/utl-mixed-mode/UtlPortSpike.cpp`. To retain Roslyn-shaped bodies, `UtlManagedModule.csproj` emits a C# `.netmodule` with `HashSetUtils`, `Pair`, `StringUtils`, and `Triple`; the VCX project links it into the final assembly. `McmiscInlineSupport.cs` contributes private static character-map data on `StringUtils` for the transplanted utility bodies. C++ duplicate definitions of those four managed types are excluded for this build.

`IlMaxStack` is a proof-only post-link tool. It normalizes hidden helper names, relocates metadata when required, adds exact StandaloneSig entries, and transplants original Release method bodies and local signatures. The `SafeMutex(String, Int32)` transplant required real `std::wstring` conversion and replacement helpers (`ToWStrng` and `strrpl`) so its native string operations and metadata operands exist in the candidate. No CRT, startup, exception, or security-cookie pseudocode was copied. The code remains proof-only until the production project and the complete deployment path are migrated.

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

The Release build completed with zero compiler warnings and errors. The managed consumer successfully called `Str.RemoveWhite`, the C# netmodule `Pair` and `StringUtils` methods, `UtlNativeLogUtf8`, `SafeMutex(String, Int32)`, the explicit `IDictionary` and `IEnumerable` enumerators on `LabeledDictionary`, and `SafeMonitor.Enter/TryEnter/Exit`. Without the Prime install directory on `PATH`, Windows could not load the mixed-mode DLL because `UTL.DLL` and `Ijwhost.dll` were unresolved. This verifies loadability on the installed Prime machine with its native provider, .NET runtime, and C++/CLI host support.

## Release IL result

The maintainer's strict C++/CLI-aware validator was run against the current mixed-mode VCX candidate, using the integrated `main` commit as the base:

```powershell
python tools/validation/release_il.py --base main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlNetmoduleSpike/UtlNetmoduleSpike.vcxproj --results .mirror-cache/release-il-netmodule-hybrid-postmerge
```

It failed: **631 checked, 292 exact, 396 failures, 387 unfinished**. The latest ignored report is `.mirror-cache/release-il-netmodule-methodfocus-v14/release-il.json` (base `314e37faad02fdfc4d29d0674cce5daf4da93cce`, tip `0b65b4dc84c19634c27abe0e532618c9b2f01470`). The candidate inventory has 161 exact `utl-mgd` methods and 131 exact `mcmisc` methods with CIL bodies, plus 18 declaration-only methods whose Mathcad counterparts have no body. No mapped body difference remains. The reported failure groups are:

| Failure | Count | Detail |
|---|---:|---|
| Global `<Module>` methods without reviewed Mathcad bindings | 355 | MSVC/C++/CLI runtime and standard-library support generated for this build. The unmodified validator requires reviewed bindings for them. |
| Methods without a Mathcad counterpart | 32 | C++/CLI metadata helpers and CRT implementation support; this group also includes the candidate-owned `_RTL_CRITICAL_SECTION*` type identity. |
| Methods reported removed from the C# baseline | 9 | Six C# helper names (`_bang*`/`_tilde*`) are replaced by C++/CLI destructor/finalizer entries; two explicit interface methods are represented by MethodImpl rows; the C# `IntPtr` SafeMonitor signature is replaced by the native pointer signature. |
| Mapped method-body differences | 0 | The `SafeMutex(String, Int32)` constructor now transplants exactly; the other mapped bodies remain exact. |

ILSpy 11 decompiles the candidate's `SafeMutex(String, Int32)` using the same `ToWStrng`, `strrpl`, `std::wstring`, and field assignment sequence as the Mathcad reference. It reconstructs the explicit `IDictionary.GetEnumerator` and `IEnumerable.GetEnumerator` entries from the candidate's MethodImpl rows, and shows `SafeMonitor.GetCriticalSection` returning `_RTL_CRITICAL_SECTION*`. The consumer invoked both enumerators and the SafeMonitor methods successfully.

No validation rules were weakened or bypassed. The strict report compares the complete C++/CLI candidate with the C# Release baseline, so compiler-generated helpers and C++/CLI method metadata forms remain visible failures. The focused Mathcad-bound inventory and ILSpy inspection show exact bodies and matching visible signatures for the mapped methods, including `SafeMutex`, `SafeMonitor`, and the explicit `LabeledDictionary` interface methods. The whole PE is not a byte-for-byte metadata match to `utl-mgd.dll`.

## Identity, API, and references

The latest project-local DLL inspected by `System.Reflection.Metadata` has SHA-256 `0cee9ee46366110e2ff4289a85614f089b06dc4e848a4ecb259ed14a23f5c491`, MVID `b16927ae-46f3-44ae-8c46-259933e97ba6`, assembly `Quire.Engine.Utl`, version `0.1.0.0`, and .NET 10 metadata. Its CLI flags are `NativeEntryPoint` (`ILOnly` is clear). It has 817 TypeDefs, 697 MethodDefs, 1,228 Fields, 168 AssemblyRefs, and 47 public types. The production C# assembly has 38 TypeDefs, 310 MethodDefs, 168 Fields, 8 AssemblyRefs, and 31 public types. The candidate retains the Quire assembly name/version and adds C++/CLI runtime references and public compiler support types.

The current production C# Release assembly has the same simple assembly name, version, and target framework. Its inspected output is AnyCPU and contains 38 TypeDefs, 310 MethodDefs, 168 Fields, and 8 AssemblyRefs. A metadata-name comparison found all 31 production public type names and all 260 production public methods in the candidate; the candidate adds 16 public C++/CLI/compiler types and 26 public support methods. The candidate is x64 and introduces 160 additional AssemblyRefs plus native implementation and import metadata. The consumer exercises representative APIs, not every Quire consumer, so the extra references and architecture still need a complete deployment review before production replacement.

The Mathcad reference itself has a different identity: assembly `utl-mgd`, SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`. The proof preserves Quire's current simple name/version, not the original Mathcad assembly identity.

## Native code and dependencies

`dumpbin /headers` reports an x64 PE32+ DLL, a COM descriptor, and `_CorDllMain` at `0x180040AC1`. The latest build's sections are:

| Section | Virtual address range | Role |
|---|---|---|
| `.text` | `0x180001000–0x180040BB7` | Executable native code, including C++/CLI transitions and the unmanaged logging adapter. |
| `.nep` | `0x180041000–0x1800422AF` | Compiler-generated native entry support. |
| `.ilnew` | `0x18007D000–0x18017CFFF` | Reserved IL bodies and relocated metadata; this is not native code. |

`dumpbin /exports` reports `UtlNativeLogUtf8` at RVA `0xB950`. `dumpbin /dependents` reports `UTL.DLL`, `MSVCP140.dll`, `VCRUNTIME140.dll`, `api-ms-win-crt-runtime-l1-1-0.dll`, `api-ms-win-crt-heap-l1-1-0.dll`, `KERNEL32.dll`, and `mscoree.dll`. Deployment requires Windows x64, .NET 10, a matching x64 VC++ runtime, a compatible `Ijwhost.dll`, and the Mathcad `UTL.DLL` provider in the Windows DLL search path.

## REA and Ghidra inspection

REA reports in `.mirror-cache/netmodule-hybrid-g6-final-rea.json`, `.mirror-cache/netmodule-hybrid-g6-final-members.json`, and `.mirror-cache/netmodule-hybrid-g6-final-native-boundaries.json` cover the preceding proof artifact, not the latest hash recorded above. They establish that the earlier x64 PE/CLI image was classified as `mixed-clr-native` and its metadata parsed completely. REA was not rerun on this latest build.

Ghidra 12.1.4 imported and auto-analyzed the final candidate hash in temporary target `quire-utl-methodparity-final` with `ghidra-agent-cli ghidra import`. The loader recognized x64 PE native code and analyzed its executable ranges. It reported custom-attribute, StandaloneSig, and MethodSpec markup errors for the post-link metadata, plus C++/CLI constructor decompilation warnings. The configured `ghidra auto-analyze` and `ghidra export-baseline` commands still fail because their Java scripts are absent. Ghidra did not resolve `UTL.DLL`; that library was not inspected. `dumpbin` independently confirms executable ranges and dependencies.

## Remaining migration work and risks

- The strict whole-candidate Release IL gate still fails on compiler-generated globals and C++/CLI metadata forms. It was not weakened. The Mathcad-bound methods with readable IL are exact in the latest candidate inventory.
- Assembly-level compatibility is still unproven: the candidate is x64, adds public compiler/runtime types, and has 168 AssemblyRefs versus 8 in the C# production DLL. The current consumer exercises representative public types, not every Quire consumer.
- A full C# netmodule build hit MSVC C1001 in earlier experiments. The current smaller netmodule plus C++/CLI port and proof-local metadata transplant builds, but that architecture has not been wired into the production project.
- Native and mixed-mode runtime deployment needs Windows x64, .NET 10, `Ijwhost.dll`, the matching VC++ runtime, and Mathcad's `UTL.DLL` provider.
- Consumer loading was demonstrated only on x64 Windows with Prime's native provider and the required VC++/.NET runtime components available.
- All Quire consumers still reference the production C# project. Their production project references and runtime deployment have not been migrated or verified against this VCX candidate.

Keep the production C# assembly in place until the mixed-mode build is integrated into the production project and the added assembly references, public runtime types, and full consumer set are reviewed.
