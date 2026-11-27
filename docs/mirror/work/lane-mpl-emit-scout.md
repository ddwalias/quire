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

## Extended prototypes: locals, tail calls, and standalone module PE

The ignored Release harness in `.mirror-cache/mpl-emit-prototype` now exercises two larger cases; no emitter source change was needed.

### `Hashtable.values::mlookupInv`

The harness used `Quire.Mpl.Emit` to produce `Quire.Mpl.Modules.dll` containing `Hashtable.Map::Field2`, a placeholder `hlookup`, and `Hashtable.values::mlookupInv`. The method description declares `object` and `Map` locals, emits the field read, `tail.`, same-module call, and return. MethodAudit was pointed directly at that generated candidate with `--assemblies .mirror-cache/mpl-emit-prototype/assemblies --project Quire.Mpl.Modules --diff Quire.Hashtable.values::mlookupInv`; it reported the method **exact** against `hashtable.dll` (`Hashtable` token `0x06000003`). Its locals and instruction sequence match the original, including `ldloc.0`, `stloc.1`, `.maxstack 48`, and `tail.`. This confirms the backend's existing local signature writer, local opcodes, field/method reference resolver, and tail prefix path compose correctly in one real method.

### `McdUnitSystemUS` standalone output

The next harness mode emits a PE named `Quire.McdUnitSystemUS.dll`, with module name `McdUnitSystemUS.dll`, containing the complete `McdUnitSystemUS.values` type from `mcdunitsystemus.dll`: one public static `system` field and five ordered methods (`Update`, `op_L`, `op_M`, `.cctor`, `.ctor`). It uses the `Quire.Mpl.Emit` type/field/method definitions and copies the original `ModuleText` and `ModuleInterface` custom-attribute blobs into `TypeBuilder.SetCustomAttribute(ConstructorInfo, byte[])`. Those two blobs compare byte-for-byte with the original (3 and 6,833 bytes); `ModuleInterface` is not reconstructed from a decompiler string. The candidate adds a final `MirrorOf` marker so the existing auditor can bind its mapped type.

MethodAudit was run directly on that candidate with `--project Quire.McdUnitSystemUS --diff Quire.McdUnitSystemUS.values`. It reported **all five methods exact**, including cross-assembly field and method references in `Update`, `op_L`, and `op_M`. The type flags, `system` field flags/type, custom attributes, and method/field ordering also match the original after applying the project's namespace/assembly mapping; `MirrorOf` is the only additional Quire-side type attribute. This candidate contains the standalone module type rather than the current combined C# host, so it exercises the D20 output identity and assembly-reference path.

## What build and gate integration still needs

`MethodAudit` already accepts an arbitrary `--assemblies` directory and a project name, and the direct invocation above successfully compared both generated candidates. It does not require the target DLL to have come from C# compilation. `release_il.py`, however, builds each named `.csproj` into its staging `assemblies` directory and then asks MethodAudit to inspect `<project>.dll`; the generated artifact must be the project `TargetPath` by the end of that normal Release build.

The minimal integration is:

- Add an Mpl target project (`src/Quire.McdUnitSystemUS/Quire.McdUnitSystemUS.csproj` for the prototype target) whose Release build runs the structured-description emitter and leaves `Quire.McdUnitSystemUS.dll` at `$(TargetPath)`. Its dependencies should be normal `ProjectReference`s so the existing solution build produces the graph in order.
- Update MirrorGen's Mpl routing so an Mpl target project counts as split when that project exists, and emit an Mpl assembly manifest/description input instead of C# `MirrorStub` declarations. The current `Rules.IsSplit` handles C# and C++/CLI only; `Language.Mpl` always falls back to its D2 C# host.
- Keep the same `release_il.py` staging contract. It should build the Mpl project normally, then MethodAudit should compare its emitted DLL using the current type-map binding and `[MirrorOf]`. No C# IL stand-in or parity exemption is needed.
- Extend MethodAudit's metadata model only where the new contract requires a hard assertion: retain ordered FieldDef/MethodDef metadata and raw custom-attribute blobs, then compare type flags, ordered fields/method signatures/flags, and custom-attribute constructor/value blobs for Mpl types. The current `TypeModel.CustomAttributes` keeps attribute names, not values/order, and the existing method report only proves method declarations and bodies. Assembly-name normalization should use the same reviewed type/assembly binding already used for method references.

## Decompiler-driven coverage and revised estimate

I scanned Release IL from all 51 Mpl assemblies listed in `Rules.AssembliesIn(Language.Mpl)` with `ilspycmd -il`, counted body-bearing methods/instruction lines, and compared opcode families with the current `ILCodeConverter`. This scan found 3,050 concrete method bodies and 103,076 instruction lines. After normalizing optimized `ldloc.*` / `stloc.*` forms into the emitter's existing local-index API, 3,049 of 3,050 methods use only opcode families the emitter already recognizes. The one exception is in `EfiSymbolicsEmitter`: it contains 2,356 `nop` instructions, so the opcode-family coverage is 100,720/103,076 (97.7%) even though only one method is affected (99.97% of methods). A decompiler can represent `nop` as a trivial structured instruction; the production writer would need a small supported encoding for it, but that change is intentionally not included here because it would alter Quire.Mpl.Emit's own mirrored IL and needs its own Release-parity design.

The opcode scan is an upper bound, not a proof that every operand is straightforward: it does not validate every token, custom modifier, attribute, branch encoding, closure, and static initializer across all assemblies. The two prototypes give evidence that local slots, tail prefixes, self/cross-assembly member references, custom-attribute raw blobs, and ordered metadata can be represented. I estimate **95–99%** of the roughly 4,000 reported bodies could be generated mechanically as structured records from original IL (about 3,800–3,960), leaving roughly 40–200 bodies for hand recovery or backend exceptions. The scan counted 3,050 concrete method bodies; the difference from the 4,000-stub estimate likely includes bodyless declarations or a broader inventory and should be reconciled when building the extractor.

This changes the effort estimate from hand-entering thousands of bodies to implementing and validating the IL-to-description extractor, metadata serializer, and build/gate bridge. A reasonable planning range is **3–5 engineer-months** for those pieces plus manual handling of outliers, assuming the rest of the method descriptions are generated from the installed Release IL and checked in manageable batches. This remains substantial reverse engineering: emitted records recover compiler output and do not restore the absent original Mpl source or its higher-level intent.

**Updated recommendation:** Proceed with the decompiler-to-structured-description route on top of Quire.Mpl.Emit. The `mlookupInv` and `McdUnitSystemUS` Release prototypes show that the backend can already reproduce the difficult body and metadata shapes without changing the Mpl.Emit source. Before broad batches, land the standalone-project/MethodAudit metadata assertions, add `nop` encoding without changing Quire.Mpl.Emit's own mirrored bodies, and run a small end-to-end `release_il.py` build of the standalone module project.
