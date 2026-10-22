# `Quire.Engine.Utl` C++/CLI mixed-mode spike

## Result

The proof now emits a Windows x64 mixed-mode `Quire.Engine.Utl.dll` containing managed metadata and native machine code. A managed .NET 10 consumer references and loads it on the build machine when Mathcad Prime's install directory is on `PATH`. The C# production project remains unchanged. The spike is reviewable, but it does **not** meet full managed API or Release IL parity and must not replace the production assembly yet.

The proof uses genuine native behavior at the existing logging boundary: `Logging.Log` and `LogLine` construct native `std::string` values, call the `utl::Log(Substring<char const>)` import, and stream the message through the returned `std::ostream`. The import-library definition is generated locally from the decorated dependency name observed in the original `utl-mgd.dll` import metadata. No implementation or startup pseudocode was copied from Ghidra. The original `utl-mgd.dll` remained read-only and was never executed. `UTL.DLL` was not inspected or analyzed.

## Release IL result

The documented strict gate was rerun against current `origin/main` (report tip `ce913339cd99cad409260fe6ac1f3ee9a0c114a5`):

```powershell
python tools/validation/release_il.py --base origin/main --project Quire.Engine.Utl --candidate-vcxproj proofs/utl-mixed-mode/UtlPortSpike.vcxproj --results .mirror-cache/release-il-lane-d-final
```

It failed closed: 548 checked, 291 exact, 308 failures. The report is `.mirror-cache/release-il-lane-d-final/release-il.json`; its recorded base is `987703a6b6af7d722dce18a9f6786ff0f8e34da3`. The failure groups are:

| Failure | Count | Detail |
|---|---:|---|
| Global `<Module>` methods without reviewed Mathcad bindings | 267 | C++/CLI and MSVC generated support methods are counted as candidate methods without a reviewed counterpart. |
| Methods without a Mathcad counterpart | 37 | Includes generated/support methods and proof helpers. |
| Methods reported removed | 3 | `SafeMonitor.GetCriticalSection` and the two qualified `LabeledDictionary` enumerator method names; this candidate spells the latter `_e1` and `_e2`. |
| Mapped IL difference | 1 | `SafeMutex::.ctor(String,Int32)` still calls `String.Format` where the reference body calls a `<Module>` helper. |

There was no validator suppression or bypass. The validator now accepts the `.vcxproj` candidate, builds it in Release, and checks its methods, but it cannot accept the generated global methods without reviewed Mathcad bindings. Its fail-closed result is a concrete blocker to claiming same-DLL Release IL parity. The proof-local transplant helper additionally maps selected original method tokens and local signatures, adjusts metadata and preserves method bodies. It now transplants `Logging.Log` and `LogLine` as well as the previously handled utility bodies. The reference bodies are exact in the candidate where the scanner can bind them; the whole candidate still fails the gate.

## Build design and changes

`proofs/utl-mixed-mode/UtlPortSpike.vcxproj` is an x64 Visual C++ Dynamic Library project using MSVC toolset v145, `CLRSupport=NetCore` (`/clr:netcore`), and `net10.0-windows`. It emits assembly name `Quire.Engine.Utl`, version `0.1.0.0`, with the same framework target as the production project. The project ports the existing managed utilities to C++/CLI, including synchronization, queues, collections, `LabeledDictionary`, and the selected `Engine.Misc` types.

The proof has two support pieces for bodies that C++/CLI does not naturally emit in the Roslyn shape required by the reference:

- `InlineHelpers` is a small C# Roslyn netmodule linked into the mixed-mode DLL. It supplies the original static array data and `<PrivateImplementationDetails>` inline-array helpers used by the transplanted methods.
- `IlMaxStack` is a proof-only post-link tool. It relocates metadata into the reserved `.ilnew` section when it must append exact standalone local-signature rows, normalizes hidden helper names, maps source metadata tokens, and copies selected Release method bodies. It does not alter the validator or relax its comparison.

`UTLLogging.def` and the `BuildUtlLoggingImportLibrary` target generate `UTLLogging.lib` with MSVC `lib.exe`. The declaration covers the existing `utl::Log` entry point referenced by the original managed assembly. The consumer avoids calling `Logging`; the static import is present in the candidate and requires the installed native provider to resolve at load time.

The separate `MixedModeProof.vcxproj` remains a minimal earlier demonstration of an unmanaged function called through a public C++/CLI wrapper by a managed consumer. `UtlPortConsumer` exercises the ported public utility types and their current behavior. Neither proof replaces the production C# project or updates its consumers.

## Identity, API, references, and deployment

REA inspected the current candidate at SHA-256 `d34978df5703ee2579d28bf8d847045101d86ae0a6f315351e1bb0e544eb353d`, MVID `d22cb1e0-1669-4996-a746-84c552205338`. It reports:

- Assembly `Quire.Engine.Utl`, version `0.1.0.0`, unsigned, targeting `.NETCoreApp,Version=v10.0`.
- x64 PE32+, mixed CLR/native classification, `ILONLY` clear, native-entry-point CLI flag, native entry RVA `0xb370`.
- Complete metadata parse: 822 TypeDefs, 616 MethodDefs, 168 AssemblyRefs, 375 StandaloneSig rows.
- 51 P/Invoke imports and 64 native/runtime implementation boundaries in the managed metadata inventory.

This preserves the simple assembly name, version, and .NET target that existing Quire project references expect. It changes the architecture from the production C# project's AnyCPU output to x64 and adds C++/CLI runtime references and native deployment dependencies. REA's original artifact identity remains SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd`, MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`; the original is assembly `utl-mgd`, version `0.0.0.0`.

The strict report identifies three removed method definitions. Two are the explicit `IDictionary` and `IEnumerable` enumerators whose candidate metadata names are `_e1` and `_e2`; the other is `SafeMonitor.GetCriticalSection`. The first two are interface implementation methods and the consumer still exercises the collection operations, but the name/body comparison does not consider them exact. Thus this proof does not yet establish complete managed API compatibility. The other Quire projects still reference the production C# project; their project references were not migrated or rebuilt against the candidate as production dependencies.

`dumpbin /headers` reports `_CorDllMain` at `0x18000DA75` and the following current image ranges:

| Section | Virtual address range | Meaning |
|---|---|---|
| `.text` | `0x180001000–0x18000DB77` | Executable code, including compiler-generated and C++/CLI routines. |
| `.nep` | `0x18000E000–0x18000ED6F` | Native entry-point support. |
| `.ilnew` | `0x18003B000–0x18013AFFF` | Reserved managed IL bodies and relocated metadata; not counted as native machine code. |

`dumpbin /dependents` reports `VCRUNTIME140.dll`, `MSVCP140.dll`, UCRT API sets, `KERNEL32.dll`, `mscoree.dll`, and `UTL.DLL`. The direct logging import means Mathcad's `UTL.DLL` must be discoverable by Windows when the assembly loads. A consumer run without the Mathcad install directory on `PATH` failed with `FileNotFoundException` for the mixed-mode module; the same consumer ran after adding the Prime install directory to `PATH`. The consumer did not invoke either logger method. This confirms dependency resolution and loadability in the installed Prime environment, not on a clean machine.

Production deployment would require x64, the .NET 10 runtime, `Ijwhost.dll`, compatible MSVC/UCRT runtime components, and the required Mathcad native provider on the DLL search path. Building requires Windows, Visual Studio's C++ workload, the C++/CLI component for the selected MSVC toolset, and Windows SDK x64 libraries. The current output directory contains `Ijwhost.dll`.

## REA and Ghidra inspection

REA statically parsed the final candidate's managed metadata completely and classified it as mixed CLR/native. Ghidra 12.1.4 imported that exact SHA-256 through `ghidra-agent-cli`; its import log says automatic analysis succeeded and shows the native PE sections and imports. Ghidra logged CLR markup errors for several custom attributes, `StandaloneSig`, and `MethodSpec` records in the relocated metadata, followed by decompiler p-code warnings around C++/CLI constructors. The Ghidra `auto-analyze` and baseline-export CLI commands could not run because this installation lacks their configured `AutoAnalyze.java` and `ExportBaseline.java` scripts. No Ghidra scripts were created. `dumpbin` section ranges above are the address-range evidence; `.ilnew` is not presented as native code.

The Ghidra loader could not find `UTL.DLL` in its library search paths. No attempt was made to inspect that library. The managed consumer's PATH-based load test only confirms that Windows resolved the candidate's static dependency; it does not establish the internals or behavior of `UTL.DLL`.

## Remaining migration decision

The spike proves that MSVC can build a same-file mixed-mode assembly that a Quire .NET consumer can load. It does not prove the proposed production migration preserves the complete API, AnyCPU deployment behavior, all project references, or the Release IL acceptance gate. The C++/CLI compiler's generated global methods and the remaining API/name differences need a reviewed policy or implementation path before a production project replacement can be claimed. Keep the production C# project in place until the strict gate can accept the candidate and the consuming projects can use its x64 deployment contract.

## Reproduction

Build the candidate and consumer on Windows from the repository root:

```powershell
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe'
& $msbuild proofs\utl-mixed-mode\UtlPortSpike.vcxproj /m /p:Configuration=Release /p:Platform=x64 /p:SolutionDir="$PWD\proofs\utl-mixed-mode\" /v:minimal
dotnet build proofs\utl-mixed-mode\UtlPortConsumer\UtlPortConsumer.csproj -c Release
$env:PATH = "C:\Program Files\PTC\Mathcad Prime 12.0.1.0;$env:PATH"
dotnet proofs\utl-mixed-mode\UtlPortConsumer\bin\Release\net10.0-windows\UtlPortConsumer.dll
```

The original binary and generated IL remain in `.mirror-cache`; generated build and Ghidra workspaces are ignored. No original Mathcad binary or decompilation is committed.
