# Quire.Msclr checkpoint

- Owner: Msclr lane (WORK row `Msclr`). Branch `mirror/msclr-20261116b` from `main` fff67b092. Worktree `C:/Users/DuyDang/orca/workspaces/Quire/lane-msclr`.
- Target: `src/Quire.Msclr` (C++/CLI under `MixedMode/`, MSVC 19.51.36246, D19/D20). C# stand-ins for the Debug build are in `Mirrored/`.

## Landed in this batch

- `Mpl.Region`: constructor, `GetImports`, `GetExports`, `Evaluate`, `NewSymbols`, fields `m_ldr`, `m_imp`, `m_exp`.
- `Mpl.IRegion.GetRegion(Hex.Module*)`. The abstract members, constructor and `Symbol` were already in the C++ source; their generated stubs are now removed too.
- `<Module>` `Mpl.GetModuleState`, and the `mshex.dll` imports `Hex.Module.GetState`, `Hex.Signature.GetName`, `Hex.Signature.GetType`.
- Build: `MixedMode/mshex.def` makes the import library for those three exports (D20 stopgap; Mathcad's `mshex.dll` is loaded at run time). `GetModuleState` is in its own source file (`MplLoaded.cpp`) because the original does not inline it into `GetRegion`.
- The earlier blocker "`Hex.*` types need a reviewed binding" no longer holds: the gate pairs the native `Hex.Module`, `Hex.Signature`, `Mpl.Loaded`, `ms.Range<…>` and `Substring<…>` types.

## Release IL report

- `release_il.py --base main --project Quire.Msclr`: PASS, 92 checked, 90 exact, 0 failures, 9 native pairs exact, 325 Mathcad members not paired. Base: 82 checked, 80 exact.

## Parked: `Mpl.Reader` (`ReadAttribute`, constructor)

`Mpl.Reader.g.cs` keeps its two stubs. A candidate for the whole group is saved, uncommitted, as
`.mirror-cache/msclr-reader-candidate.patch` in the lane worktree (applies on this batch's commit).

Exact with that candidate (gate run `release-il-481907d8212d` and a local IL diff):
`Reader.ReadAttribute`, `Reader..ctor`, `Mpl.LoadModule(string)`, both `Mpl.Get<…>` instances,
`Mpl.GetModuleAttribute(Substring<wchar_t const>, module_attribute, bool)`, `strutf8`, `Mpl.ToString(Substring<wchar_t const>)`,
both `Mpl.ToStrng`, and the `Hex.Error` imports. 138 checked.

Blockers (the gate fails on these three, so the group cannot land):

1. `<Module>` `strconv(std::string*, Substring<wchar_t const>, uint)` is native code in the original (RVA 0x78E0, 380 bytes).
   The candidate is 173 bytes: MSVC calls `std::string(count, ch)` out of line where the original inlines it. Tried `/EHa`,
   `/EHsc` and `/Ob3` on the native file with `std::string out(size, '\0')`; none inlines it. The original's source form or
   flags for that file are not recovered.
2. `<Module>` `new`: `native body differs: +0x38: +0x9: +0x0: data +0x10: a pointer on one side only` (CRT `operator new`,
   reached once managed `std::string` code is instantiated). Not investigated.
3. `<Module>` `delete[]`: `native body differs: +0x0: length 48 != original 176`. Not investigated.

`Reader` is internal in Mathcad and MSVC emits it only when something references it. Its only referrer is
`GetModuleAttribute`, whose error paths call `strutf8` and so `strconv`; that is why the blockers above hold the type.
A public `Reader` passes the gate without them (run `release-il-5607e2b2b118`, 97 checked) but exposes a type Mathcad keeps
internal, so it was not landed.

For review: the candidate instantiates `Mpl::Get` on native tag classes `Mpl::Run::ModuleTextAttribute` and
`Mpl::Run::ModuleInterfaceAttribute` that map to the mirrored attribute types. MSVC spells a template argument by its C++
name, the mirrored types are `Quire::Mpl::Run::…`, and MethodAudit pairs `<Module>` globals by exact name; without the tags
the instances have no counterpart. The original's `LoadModule(string)` failure path calls `Assembly.LoadFrom("c:\io.sys")`
before throwing; the candidate reproduces it.

## Remaining (not attempted)

- `Mpl.GetModuleAttribute(Substring<char const>, …)` and native `strconv(Substring<wchar_t>, Substring<char const>, uint)`.
- `Mpl.GetTypeName`, `Mpl.pack`, `Mpl.free`, `Mpl.GetAssemblyPath`, `Mpl.SetAssemblyPath`, `Mpl.GetModuleHeap`/`Table`/`Handle`,
  `Mpl.GetModuleStrng`/`String`, `Mpl.GetAssemblyName`.
- `Mpl.Heavyweight`, `Mpl.Lightweight`, `Mpl.Loaded` members, `Mpl.MakeModule`, `Mpl.LoadModule(Hex.Module&, …)`,
  `Mpl.SaveModule`, `Mpl.LoadDynamicModule`, `Mpl.NewTransientBuilder`, `Mpl.NewPersistentBuilder`, `Mpl.Set<…>`, `Mpl.unpack<…>`.
- The other `mshex.dll` imports and the STL instantiations those bodies pull in.

## Remaining mismatches

- None in the landed members. The three native bodies above block `Mpl.Reader`.
