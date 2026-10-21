# C++/CLI mixed-mode feasibility proof for `Quire.Engine.Utl`

## Outcome

The isolated Windows proof builds and runs: a managed .NET 10 consumer references a C++/CLI DLL, calls its public managed wrapper, and receives a result from native x64 code in that same DLL. REA identifies the output as `cpp-cli-mixed`, and Ghidra locates the native function in an executable section.

This does **not** establish a viable same-DLL migration of `Quire.Engine.Utl`. The production C# project was left intact. No exact Release IL claim is made for a C++/CLI replacement. The existing C# project passed the documented Release IL validator against `origin/main` at `f2f4282c6d0b9a3afaae4320049e004f8c4e4fb0`: 129 methods checked, 129 exact, zero failures. That report validates the existing C# assembly only. A later remote update occurred before this note was written; `lane_merge.py` will refresh and validate again during integration.

The reference REA investigation found 55 native P/Invoke declarations, two IL P/Invokes, eight runtime-provided delegate methods, and 57 analyzed internal native procedures. Those procedures are C++/CLI, CRT, exception, allocation, security, and generated wrapper support; it identified no application-authored native algorithm to implement in Quire. The proof's addition routine exists only to exercise the native-to-managed call path. It is not proposed for production.

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

## Quire compatibility assessment

The literal mixed-mode output can carry both native machine code and managed metadata, and the proof confirms that an ordinary managed .NET 10 consumer can call its managed surface. Microsoft's .NET C++/CLI model requires a Windows `.vcxproj` DLL using `CLRSupport=NetCore` / `/clr:netcore`; mixed managed and native functions are supported in one assembly. See [C++/CLI migration guidance](https://learn.microsoft.com/en-us/dotnet/core/porting/cpp-cli) and [mixed native and managed assemblies](https://learn.microsoft.com/en-us/cpp/dotnet/mixed-native-and-managed-assemblies?view=msvc-170).

What remains unproven for Quire is material:

1. **Assembly identity and API:** the proof has another assembly name and version. A production project could configure the output name/version, but this work did not port Quire's 89 types and their managed declarations or compare its full public surface.
2. **Project references:** `Quire.Engine`, `Quire.Engine.Native`, `Quire.Efi`, and `Quire.Mcd` currently reference `Quire.Engine.Utl.csproj`. Replacing that project with `.vcxproj` requires updating or deliberately preserving those reference paths. Those consumers are outside Lane D's edit scope, so no integration claim is made.
3. **Exact Release IL:** C++/CLI emits compiler-authored managed wrappers and runtime support, while this proof's wrapper IL is already different in shape from ordinary C# compilation. The proof did not rebuild the existing C# members into the mixed-mode assembly, and exact parity for them has not been demonstrated.
4. **Validator support:** `tools/validation/release_il.py` discovers production projects only from `src/*/*.csproj` (`production_projects`, line 34) and `build_and_scan` constructs `<project>.csproj` and invokes `dotnet build` (lines 113–116). It has no `.vcxproj` build path or candidate-project override. Therefore it cannot validate a C++/CLI replacement as written. This is a concrete validation blocker, not a failed IL comparison.

The safe disposition for this batch is to retain the C# project, its passing Release IL state, and this proof. A future same-DLL migration needs an explicit way to retain the existing managed methods and compatible references, then a validator path that builds the `.vcxproj` and inspects the resulting assembly. Until those conditions are satisfied, do not describe `Quire.Engine.Utl.dll` as mixed-mode or claim same-DLL Release IL parity.

## Original artifact handling

The reference was only read from `C:\Program Files\PTC\Mathcad Prime 12.0.1.0\utl-mgd.dll`. The available REA evidence is bound to SHA-256 `589523c705e8766324fad449cd29e318e440415b37c7e9142190495af6e21dfd` and MVID `14a15303-d932-4fc8-a991-4ef95fe284ba`. This work did not modify or execute that DLL and did not analyze `UTL.DLL`. The original binary and detailed REA cache remain local and uncommitted.
