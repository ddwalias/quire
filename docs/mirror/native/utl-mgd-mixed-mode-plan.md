# Putting native code in Quire’s Utl DLL

## Goal

Produce one `Quire.Engine.Utl.dll` that contains both managed .NET code and native x64 machine code, in the same broad format as Mathcad’s mixed-mode `utl-mgd.dll`. This means a **mixed-mode C++/CLI assembly**. It does not mean copying Mathcad’s CRT pseudocode or putting a native DLL inside a managed DLL as a resource.

## What the repository builds today

- `Quire.Engine.Utl.csproj` is an SDK-style C# project targeting `net10.0`.
- `Quire.Engine.Native.csproj` is also an SDK-style C# project; its name does not make it a native C++ project.
- The repository has no `.vcxproj`, `.cpp`, `.h`, or CMake native project to extend.
- `Quire.Engine.Utl` is referenced by `Quire.Engine`, `Quire.Engine.Native`, `Quire.Efi`, and `Quire.Mcd`.
- The managed implementation calls native OS/runtime APIs through `NativeLibrary` and function pointers. That makes native calls at runtime, but the C# assembly itself contains IL, not those API implementations.

The original `utl-mgd.dll` native pass found compiler/runtime startup, exception, allocation, security, and C++/CLI wrapper support. It found no application-authored native algorithm that Quire needs to reproduce. A C++/CLI build would generate the toolchain’s own required startup/runtime support; it would not reproduce the same helper addresses or exact helper list as Mathcad.

## Build choices

### A. Put native code in `Quire.Engine.Utl.dll`

Convert the project that owns the `Quire.Engine.Utl` assembly identity to a Visual C++ **`.vcxproj` Dynamic Library** with .NET Runtime Support (`<CLRSupport>NetCore</CLRSupport>`) and a Windows .NET target such as `net10.0-windows`. Compile its C++/CLI portion with `/clr:netcore`; C++/CLI allows managed and unmanaged code in one assembly and can select compilation mode per file or function.[1][2]

Native implementation files can use normal C++ and unmanaged compilation regions. The C++/CLI-facing layer exposes managed `ref class` and value types and calls the native implementation. MSVC creates the mixed-mode assembly and the startup/transition support it needs; those compiler-generated routines should not be hand-transcribed from Ghidra output.

This is the only option here that meets the literal “native code in this same DLL” requirement. It is a migration, not a property change on the current C# project: C++/CLI projects for .NET use `.vcxproj`, are Windows-only, and produce DLLs.[1] The MSVC property pages support .NET 5+ target framework monikers and per-file CLR support in Visual Studio 2022 with the v143-or-later toolset.[3]

**Main cost:** the current managed API has already been mirrored as C# and passed its Release IL comparison. Rewriting its implementation in C++/CLI will not automatically preserve the exact CIL body comparison. Public namespaces, types, signatures, assembly identity, generated method bodies, and all four project references need an explicit migration plan and validation. The current Release IL workflow is designed around the C# SDK project and will need a proven way to build and inspect the C++/CLI candidate before this route can claim parity.

### B. Keep C# and add a native companion

Build a native C++ DLL or a C++/CLI bridge DLL and call it from the existing `Quire.Engine.Utl` C# assembly. This keeps current managed IL and is the smaller integration. The native instructions will be in the companion DLL, **not** in `Quire.Engine.Utl.dll`; this does not satisfy the same-file requirement.

### C. NativeAOT

NativeAOT compiles .NET code to native machine code, but Microsoft lists C++/CLI as unsupported by NativeAOT.[4] It does not provide a way to combine this existing C# assembly and C++/CLI native regions into the requested mixed-mode DLL. It is therefore not the route for this goal.

## Recommended proof before migration

1. Use a Windows machine with Visual Studio’s C++ workload and the C++/CLI component for the installed MSVC toolset. The current WSL shell cannot find `cl.exe` or `MSBuild.exe`, so it cannot perform this proof locally. Microsoft documents the C++/CLI component for Visual Studio 2022 separately from the regular C++ workload.[5]
2. Create a small, separate C++/CLI DLL targeting the same .NET version. Include one trivial unmanaged function and one public managed wrapper that calls it. Reference it from a throwaway C# `net10.0-windows` program.
3. Build Release and inspect the DLL’s CLI flags, native code ranges, dependencies (`ijwhost.dll`/VC++ runtime as applicable), and load behavior on Windows. Confirm the wrapper can be called from C# and that Ghidra identifies both the managed and native portions.
4. Use that result to estimate a `Quire.Engine.Utl` migration. Do not replace the existing project until the public API, assembly references, native deployment requirements, and Release IL acceptance path are all understood.

The C++/CLI .NET build must run on Windows. When building without the Visual Studio project system, Microsoft also documents `/clr:netcore`, the .NET app-host library path, and copying `ijwhost.dll`; a managed entry-point application supplies the runtime configuration for its referenced C++/CLI library.[1] Deployment must also match process architecture and the selected MSVC runtime model.

## Decision summary

- If the requirement is strictly **native machine code in `Quire.Engine.Utl.dll`**, use choice A and accept a C++/CLI project migration, subject to the Release IL compatibility spike.
- If preserving the current C# assembly and exact managed IL takes priority, use choice B and accept that native code ships in a separate DLL.
- Do not manually port the original CRT, exception, security-cookie, or loader startup routines. They are compiler/runtime support, and a C++/CLI toolchain supplies its own equivalents.

No Quire source files were changed while preparing this design note.

## References

1. [Migrate C++/CLI projects to .NET](https://learn.microsoft.com/en-us/dotnet/core/porting/cpp-cli)
2. [Mixed (Native and Managed) Assemblies](https://learn.microsoft.com/en-us/cpp/dotnet/mixed-native-and-managed-assemblies?view=msvc-170)
3. [Visual Studio C++/CLI project properties](https://learn.microsoft.com/en-us/cpp/build/reference/advanced-property-page?view=msvc-170)
4. [.NET Native AOT deployment overview](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/?tabs=net8plus)
5. [.NET programming with C++/CLI](https://learn.microsoft.com/en-us/cpp/dotnet/dotnet-programming-with-cpp-cli-visual-cpp?view=msvc-170)
