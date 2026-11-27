# Mpl compiler and Quire.Mpl.Emit scout

## Finding

The `Mcd` and `Mpl.Modules` stubs in scope are **Mpl output**, not C++/CLI. The assembly map classifies the module DLLs as `Mpl`; their methods use Mpl compiler conventions such as fat method headers with `.maxstack 48`, initialized local signatures, and `tail.` prefixes. This is not an MSVC or Roslyn output quirk. The module's runtime/compiler dependencies include several other languages, but those do not change the language which produced these module DLLs.

This is consistent with [D20](../decisions.md#d20-one-quire-project-per-mathcad-assembly-in-its-original-language-2026-10-23): the eventual Mpl chain has `mplasb` and `mplrun` in C#, `msgen` and `msclr` in C++/CLI, and `msmpl` and `mshex` as native code. `mplasb` is the compiler/front end; the Mpl module output itself is produced by that chain. The compiler/runtime binaries are dependencies, not the modules' source compiler language. The `Quire.Mpl` project mirrors `mplrun.dll` in C#, and `Quire.Mpl.Emit` mirrors `mplasb.dll` in C#; neither one is itself a C++/CLI implementation of these module assemblies.

## Assemblies and evidence

The two current host projects contain the following Mpl assembly targets. These are the members that need to move from the present C# hosts into one generated output per original assembly, as D20 requires.

| Current host | Mpl assembly targets |
|---|---|
| `Quire.Mcd` | `McdTranslator`, `McdDynamicEfi`, `McdEfi`, `McdUnitSystemCGS`, `McdUnitSystemMKS`, `McdUnitSystemNone`, `McdUnitSystemSI`, `McdUnitSystemUS` |
| `Quire.Mpl.Modules` | `Hashtable`, `McdRun`, `McdStaticUnitSystem`, `McdStaticUnitSystemCGS`, `McdStaticUnitSystemMKS`, `McdStaticUnitSystemNone`, `McdStaticUnitSystemSI`, `McdStaticUnitSystemUS`, `Mpl`, `MplConstants`, `MplMacro`, `MplPrelude`, `MplPrimitives`, `MplSexpr`, `MplUnitSystemSI` |

The [assembly map](../assembly-map.md) identifies each row's original DLL and language. It records `Mpl` for every target above, versus `CSharp` for `mplrun.dll` (`Quire.Mpl`) and `mplasb.dll` (`Quire.Mpl.Emit`). The [Mcd lane checkpoint](lane-quire-mcd.md) records 259 Mpl-generated `values` stubs; the [Mpl.Modules checkpoint](lane-quire-mpl-modules.md) records 530 skipped stubs, including a 530-member `--all` failure. The older [Mcd checkpoint](lane-mcd-20261109.md) shows the representative Roslyn mismatch: same body instructions, but `.maxstack 8` / `.initlocals False` versus Mpl's `.maxstack 48` / `.initlocals True`.

I disassembled installed Release originals with `python tools/reference/reference.py <type> --assembly <dll> --il` and `ilspycmd -il`. For `hashtable.dll`, `Hashtable.values` has 23 methods with `.maxstack 48` and seven `tail.` occurrences; the `Update` method alone is `public static void`, `.maxstack 48`, `ret`. Other sampled module assemblies show the same producer fingerprint: `mcdtranslator.dll` has 116 `.maxstack 48` bodies and 55 `tail.` occurrences; `mcddynamicefi.dll` has 118 `.maxstack 48` and 115 `tail.` occurrences (plus two other maxstack values); `mcdrun.dll` has 265 `.maxstack 48` and 104 `tail.` occurrences; `mplprelude.dll` has 152 `.maxstack 48` and 55 `tail.` occurrences; `mplmacro.dll` has 37 `.maxstack 48` and 33 `tail.` occurrences. This distribution is much stronger evidence than any filename or `-mgd` suffix.

The module types also carry `Mpl.Run.ModuleInterfaceAttribute` and `Mpl.Run.ModuleTextAttribute`, and their IL references `[mplrun]Mpl.Run.*`, Mpl prelude assemblies, and other Mpl modules. In sampled `hashtable.dll` and `mcdtranslator.dll`, `ModuleTextAttribute` has an empty string; `ModuleInterfaceAttribute` contains the module's declaration/interface text. It does not contain the implementation bodies. A search of the Mathcad Prime 12 install found no `.mpl`, `.mli`, `.ml`, `.mps`, or `.src` files. Therefore, unless a separate source package is located, method bodies must be recovered from original Release IL; interface text and preserved attributes are useful metadata but are not source for the bodies.

## What Quire.Mpl.Emit can emit today

`Quire.Mpl.Emit` is a custom Reflection.Emit-style writer backed by `System.Reflection.Metadata`, not just a wrapper around Roslyn:

- `AssemblyBuilder.Save` / `EmitAssembly` serialize a PE assembly; `ModuleBuilder.AppendMetadata` writes type, field, method, and reference metadata.
- `TypeBuilder.DefineMethod` and `MethodBuilder.GetILGenerator` collect method signatures and instructions. The IL converter resolves method, field, type, string, local, and branch operands for the implemented instruction cases.
- `StackDepthCalculator` deliberately has a floor of 48. `MetadataHelper.AddMethodDefinitionBody` supplies `MethodBodyAttributes.InitLocals` whenever there are locals, giving the required `.maxstack 48` and initialized locals for Mpl-style bodies.
- `ILCodeConverter` maps `OpCodes.Tailcall` to the CLI `tail.` prefix.

This is enough for a useful structured method-description layer, but it is not yet a complete offline compiler backend. Many `ILGenerator`, `MethodBuilder`, `TypeBuilder`, and module operations still throw `NotImplementedException`; opcode conversion and metadata/reference support are selective. The builder's metadata emission also needs to be exercised against the range of method attributes, custom attributes, signatures, generic/nested types, and cross-assembly references in all module DLLs. In particular, the current stack floor is `max(48, calculated depth)`, which matches the sampled Mpl convention but must be checked for every target if some bodies exceed 48.

## One-method Release prototype

I used `python tools/reference/reference.py Hashtable.values --assembly hashtable --il` to obtain the original Release listing. In an ignored `.mirror-cache/mpl-emit-prototype` harness, I called Quire's own `AssemblyBuilder`, `DefineDynamicModule`, `DefineType`, `DefineMethod`, `GetILGenerator().Emit(OpCodes.Ret)`, and `Save` to emit `Hashtable.values::Update` into a `Quire.Mpl.Modules` candidate PE. `ilspycmd -il` on the emitted PE shows the same method header and body as Mathcad:

```il
.method public static void Update () cil managed
{
    .maxstack 48
    IL_0000: ret
}
```

The method flags, signature, instruction stream, and maxstack agree exactly. `TypeBuilder.CreateType()` also inserted a normal instance constructor into the scratch type; that constructor was not part of this single-method comparison. This proves the output path can match a simple real Mpl Release method. It does **not** yet prove the existing `release_il.py` project build/gate can consume a generated DLL, or that the current writer can emit a representative method with locals, a field access, and a tail call without further work.

## Concrete build and authoring plan

1. Keep `Quire.Mpl.Emit` as the PE/method-body backend and add an explicit structured input format for module assembly metadata and bodies. Hand-written Mpl descriptions should specify types, signatures, fields, attributes, ordered locals, and typed instruction/operand references; they should not be copied as untyped IL text. Where readable Mpl source semantics can be recovered, keep that source as the authored representation and lower it to this format.
2. Extend the writer only as the first real method batches require. A prototype using `Hashtable.values::mlookupInv` is a useful next gate because it needs initialized locals and a `tail.` call; further batches should exercise cross-assembly calls, field references, exception regions, nested/closure types, and custom attributes.
3. Make each target assembly its own build product (`Quire.Hashtable`, `Quire.McdRun`, etc.) per D20. An MSBuild target should run the emitter to produce the project's `TargetPath` (with dependencies built in assembly-map order). C# source generated for the present combined D2 hosts must not also define those types in the final PE.
4. Teach MirrorGen to route Mpl assemblies to the structured Mpl manifest/body inputs instead of emitting C# stubs for those assemblies. It should continue generating signatures/inventories and stable identities; the emitted assembly is the build artifact. Keep generated descriptions under the generated tree and hand-authored source under a non-generated Mpl source directory. The existing Release IL gate must inspect the emitted assembly as the target project output, with no Mpl-specific exemption, and continue checking unchanged exact members.
5. Land the toolchain, generator/build integration, and gate-path support before large body batches. Then implement and compare small batches per target using `python tools/reference/reference.py <type> --assembly <original> --il` and `python tools/validation/release_il.py --base main --project <target>`.

## Estimate, risks, recommendation

At the task's estimate of about 4,000 stubs, the simple `Update` prototype is only a backend smoke test. Even if structured descriptions reduce authoring to an average of 15–30 minutes per method, body recovery alone is roughly 1,000–2,000 hours (about 6–12 full-time engineer-months), before completing emitter features, build integration, metadata, and review. Complex closures, exception handling, compiler-generated names, uncommon Mpl ops, and cross-assembly order can push this higher. The main risk is treating the project as bulk IL transcription: the absent Mpl bodies require careful behavior/source recovery as well as matching compiler conventions, while one metadata/signature mistake can invalidate an entire assembly.

**Recommendation:** Treat `Quire.Mpl.Emit` as the right technical foundation, not as a finished compiler. Land the build/gate bridge and a second, nontrivial prototype (`mlookupInv`) before committing to the full 4,000-method effort. If that prototype can match locals, `tail.`, field/method references, and assembly metadata, proceed with structured Mpl source/description batches; otherwise first extend or replace the backend. Do not port these assemblies to C++/CLI: it would still be the wrong source compiler and would not produce Mpl's characteristic IL.
