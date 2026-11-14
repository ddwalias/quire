# Quire.Msclr checkpoint

- Owner: Msclr lane (WORK row `Msclr`). Branch `mirror/msclr-20261114` from `main` 8f43a887e. Worktree `C:/Users/DuyDang/orca/workspaces/Quire/msclr-20261114`.
- Target: `src/Quire.Msclr` (C++/CLI `MixedMode/MsclrPort.cpp`, MSVC 19.51.36246, D19/D20). Predecessor checkpoint: [lane-i.md](lane-i.md).
- Landed in this batch: no code change. `IRegion` (abstract members and ctor) stays as on main.

## Release IL report

- `release_il.py --base main --project Quire.Msclr` on the base: PASS, 82 checked, 80 exact, 0 failures, 330 Mathcad members not paired (`no Quire member pairs with this Mathcad member`).

## Attempted this batch: `Mpl.Reader`, `Mpl.LoadModule(string)`, `Mpl.Get<…>`

Members (msclr Release IL): `Mpl.Reader.ReadAttribute(string, module_attribute)`, `Mpl.Reader..ctor`, `<Module>` `Mpl.LoadModule(string)` (105 bytes, managed only), `<Module>` `Mpl.Get<class Mpl::Run::ModuleTextAttribute>` and `Mpl.Get<class Mpl::Run::ModuleInterfaceAttribute>`. `module_attribute` is an enum with no enumerators in the original, only `value__`.

Four source forms were tried; forms 1, 2 and 4 reached the gate, form 3 failed to build (candidate source kept outside the repo, not committed):

1. `Reader` internal, `if`-chain: the candidate DLL contains none of the group. Mathcad's `Reader` is internal and reached only from `GetModuleAttribute`, and the vcxproj links with `OptimizeReferences`, so the linker drops unreferenced internal types and free functions. Gate PASS only because nothing was emitted (no new members checked).
2. `Reader` public, `if`-chain: group emitted; gate FAIL (4): `LoadModule` calls `Module::op_Inequality` where Mathcad has `brfalse`, plus a `dup` vs `stloc.2` at #2; `Get<…>` instantiations have no counterpart; `ReadAttribute` branch shape differs (`brtrue.s` vs `brfalse.s`).
3. `Reader` public, `switch` on the enum, `if (module)`, named `assembly_name` local, `"c:\io.sys"` escape typo: build failed (`C4063` case on an enumerator-less enum, `C4129` escape; warnings are errors). No gate result.
4. Same as 3 with `switch (safe_cast<int>(attribute))` and the escape fixed: gate FAIL, 4 failures, 87 checked, 81 exact. Remaining mismatches:
   - `Mpl.Get<class Mpl::Run::ModuleInterfaceAttribute>` and `Mpl.Get<class Mpl::Run::ModuleTextAttribute>` have no counterpart. MSVC spells the type argument of a mapped Quire type as `Quire::Mpl::Run::…`, and MethodAudit pairs `<Module>` globals by exact name (`tools/Quire.Tools.MethodAudit/Program.cs`, the `<Module>` branch of the pairing loop). No source shape can change the spelling.
   - `LoadModule` and `ReadAttribute` call targets differ only by that spelling.
   - `LoadModule`: Mathcad stores the `".values"` literal in local 4 before loading `ScopeName` (first shape difference at #17); the candidate does not yet reproduce that, and it cannot be verified until the spelling is fixed.

Visibility: Mathcad keeps `Reader` internal. Internal lands nothing from this group, because the linker (`OptimizeReferences`) drops unreferenced internal types and free functions. Public emits the group, but exposes a type Mathcad keeps internal and fails the gate above, so neither is landed. The candidate source (form 4) is kept outside the repo, not committed.

## Remaining (not attempted; blocked)

- `Mpl.Reader`, `Mpl.LoadModule`, `Mpl.Get<…>`: blocked on the MethodAudit template-instance naming above (maintainer, shared tool). After that, `Reader` must stay internal, so it needs a live referencing root; Mathcad's root is `GetModuleAttribute` (native `std::string`, `Substring`).
- `<Module>` `Mpl.GetTypeName(Module)` (`ScopeName + ".values"`), `Mpl.pack`, `Mpl.free`: small managed bodies, but unreferenced in Quire, so the linker drops them. Their Mathcad callers are `Mpl.Loaded`, `Mpl.Region` and native code.
- `Mpl.GetAssemblyPath`, `Mpl.SetAssemblyPath`, `Mpl.ToString(Substring<…>)`, `Mpl.ToStrng`, `Mpl.ToWStrng`, `Mpl.GetModuleStrng`: take `std::string`/`Substring<T>` (native template instances).
- `Mpl.Heavyweight`, `Mpl.Lightweight`, `Mpl.Loaded`, `Mpl.Table`, `Mpl.GetModuleState`/`Heap`/`Table`/`Handle`, `Mpl.LoadModule(Hex.Module*, …)`, `Mpl.MakeModule`, `Mpl.SaveModule`, `Mpl.LoadDynamicModule`, `Mpl.NewTransientBuilder`, `Mpl.NewPersistentBuilder`: use `Hex.*` native types. Blocked by lane-i blocker 1 (`mshex.dll` binding; `Hex.Signature.GetName`/`GetType` pinvoke).
- `Mpl.Region` and `IRegion.GetRegion(Hex.Module*)`: blocked on `Hex.Signature.GetName` (lane-i blocker 1).
- `Hex.*`, `Attachment`, `Hex.Module`: native shared-header types (lane-i blocker 1).
- No Quire-only members in this assembly were found that lack a Mathcad counterpart.

## Observation for review

`<Module>` `Mpl.LoadModule(string)` in the original ends its failure path with `Assembly.LoadFrom("c:\\io.sys")` before throwing `BadImageFormatException("", path)`. A faithful port needs that call to match Release IL exactly; the maintainer should confirm it is intended behaviour of the original rather than a stray.

## Remaining mismatches

- Template-instance naming for mapped managed type arguments (see above). This is the only blocker for the `Reader`/`LoadModule`/`Get` group.
