# Lane I: Quire.Msclr and Quire.Msgen

- Owner: Lane I. Branch `mirror/i-msclr-msgen-202610240256` from `main` 8f85894e. Worktree `C:/Projects/Quire-wt/lane-i-msclr-msgen`.
- Target project: `src/Quire.Msclr` (csproj wrapper that builds `MixedMode/Quire.Msclr.vcxproj` with `QuireMsvcInstallDir`, MSVC 19.51.36246, D19/D20).
- `src/Quire.Msgen` is scaffolded in the worktree but not landed (see Remaining 4).

## Landed: batch 1

- Members: msclr `Mpl.IRegion` (abstract class, `IRegion+Symbol` struct, ctor, abstract `GetImports`/`GetExports`/`Evaluate`).
- Release IL: `release_il.py --base main --project Quire.Msclr` PASS, 85 checked, 80 exact, 0 failures (`.mirror-cache/release-il-f283e23520be`). The exact set is the MSVC `<Module>` runtime support and `IRegion..ctor`; the abstract declarations are declaration-only.

## Original inventory (msclr.dll, Release IL)

- 424 methods: 396 in `<Module>`, 12 in managed classes (`Mpl.Reader` 2, `Mpl.IRegion` 5, `Mpl.Region` 5).
- Most of `<Module>` is MSVC CRT/std/`<CrtImplementationDetails>` support, and the candidate reproduces it exactly without authored code.
- Non-toolset `<Module>` globals are the Mpl loader (`LoadModule` x2, `GetModuleAttribute`, `Get<T>`/`Set<T>`, `ToString`/`ToStrng`/`ToWStrng`, `MakeModule`, `SaveModule`, `LoadDynamicModule`, `pack`, `free`, `SetAssemblyPath`, `GetAssemblyPath`, `GetAssemblyName`, `GetTypeName`, `GetModuleState`/`Heap`/`Table`/`Handle`) and the Heavyweight/Lightweight/Loaded classes. Most depend on `Hex.*`.

## Remaining (msclr)

- `Mpl.Reader`: MSVC emits an internal ref class only when something uses it, so it lands with `ReadAttribute`. That needs `Mpl.LoadModule(string)`, `Mpl.Get<ModuleInterfaceAttribute>`/`ModuleTextAttribute`, and `Mpl.module_attribute`.
- `Mpl.Region`: needs `NewSymbols`, which calls `Hex.Signature.GetName`/`GetType` (blocker 1). Its constructor and fields depend on those too.
- `<Module>` `Mpl.pack`, `free`, `SetAssemblyPath`, `GetTypeName`: shapes compiled and checked against the IL (`GetTypeName` is `ScopeName + ".values"`). MSVC emits free functions only when referenced, so they land with callers (`Mpl.Loaded`, `Mpl.Region`).
- `IRegion.GetRegion(Hex.Module*)` and `<Module>` `Hex.*`: blocked (blocker 1).

## Blockers and decisions for the maintainer

1. Quire.Native.Mshex (shared Hex types). msclr imports 27 `mshex.dll` exports. `Hex.Signature`, `Hex.Module`, `Hex.Heap`, `Hex.Environment`, `Hex.Value`, `Hex.Error` are "duplicate (shared native header)" in the type map and need a reviewed `[MirrorOf]` binding before their `<Module>` natives pair. `Hex.Signature.GetName`/`GetType` (pinvokeimpl to mshex.dll) fail the gate with `original: null` (checked: candidate `[Quire.Msclr]Hex.Signature` vs original `[msclr]Hex.Signature`). There are no mshex headers or import library in the repository. Not fixed here; this is the native port, outside the lane.
2. Quire.Engine.Native: decided 2026-10-24 (maintainer). The stubs `src/Quire.Engine.Native/Generated/Mirror/Mpl.IRegion.g.cs`, `Mpl.Reader.g.cs` and `Mpl.Region.g.cs` stay in place for now. They duplicate `Quire.Msclr` types, but nothing references both assemblies, so there is no conflict. Removing them requires Engine.Native to reference `Quire.Msclr`, and that breaks because its mccore12 consumers (`Engine.DetectRedefs`, `Engine.MplCompile`, `Engine.MplEval`, `Engine.RegEval`) use `Quire.Mpl.IRegion`. Those consumers belong to mccore12, which is wave 16 in the assembly map. So the removal, and Engine.Native dropping its msclr view, happens with the mccore12 split. Not edited.
3. `Quire.slnx` does not list `Quire.Msclr` or `Quire.Msgen`. It is shared root tooling and was not edited.
4. `Quire.Msgen` is not landed. An empty C++/CLI target has no msgen-bound type, so the gate reports its `<CrtImplementationDetails>` `<Module>` methods as "no reviewed Mathcad binding" (`release-il-7b81821c7d1b`, 81 failures). It lands with its first msgen-bound type.

## msgen inventory (msgen.dll, Release IL): not started

- 890 methods: 683 in classes (73 non-toolset classes, almost all `gen.*`) and 191 in `<Module>`.
- Largest: `gen.IlAsm` 112, `gen.IlEmit` 74, `gen.MetaObj` 45, `gen.Closure` 31, `gen.Callable` 30, `gen.Ctxt` 26.
- Five internal enums (`gen.Key`, `gen.Decoration`, `gen.CallConv`, `gen.DataTypeImplementationCategory`, `gen.ftype`). `gen.CallConv` has no enumerator fields in the original.
