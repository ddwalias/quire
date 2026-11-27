# Mpl compiler research and prototype lane

Owner: task `task_080923c9b603`; branch `feature/luna-mplc` (this worker worktree; shared `C:/Projects/Quire` remains on `main`). Scope: research and prototype for the 51 Mpl-produced assemblies (3,050 method bodies). This is compiler research, not an exact-IL implementation batch.

## Evidence and recovery boundary

The reference install is Mathcad Prime 12.0.1.0. Release IL and decompiler artifacts were generated with `python tools/reference/reference.py <type> --assembly <assembly> --il`; the tool keys local references by binary SHA-256 under ignored `.mirror-cache`. Representative references used here are `Hashtable.values` from `hashtable.dll`, `McdUnitSystemUS.values` from `mcdunitsystemus.dll`, and `MplSexpr.values` from `mplsexpr.dll`. The complete original modules are the 51 `language: Mpl` entries in `docs/mirror/assembly-map.json`; source IL and interface blobs must remain in `.mirror-cache` (D15).

Previously recorded all-assembly Release scan in [the Mpl emitter scout](lane-mpl-emit-scout.md) counted 3,050 concrete bodies and 103,076 instruction lines. 3,049 bodies use opcode families represented by `Quire.Mpl.Emit`; the remaining body is in `EfiSymbolicsEmitter` and contains 2,356 `nop`s. The count is an instruction-opcode scan, not proof that every metadata operand or signature is serializable.

No `.mpl`, `.mli`, `.ml`, `.mps`, or `.src` file was found in the Prime install. `ModuleTextAttribute` is empty in sampled modules. `ModuleInterfaceAttribute` is a nonempty UTF-8 declaration string encoded in the custom-attribute blob; ILSpy prints “Could not decode attribute arguments.” For example, `Hashtable.values`'s IL blob starts `01 00 89 38` and then contains the interface beginning `module Hashtable (Hashtable.Hashtable(..), Hashtable.make_hash, ...) where`. `McdUnitSystemUS.values` declares `system`, `op_M`, `op_L`, and imports `McdUnitSystemSI`, `McdStaticUnitSystemUS`, `McdRun`, `Mpl`, and `McdStaticUnitSystem`. These interfaces describe exported names/types/imports, not bodies.

## Recovered language model (provisional)

The interface grammar is ML-like and typed, but has distinctive module/data declarations and a runtime-erased object ABI:

```text
module <ModuleName> (<exports...>) where
  data <Name> = <Constructor> <field types...> | ...
  <name> :: <type expression> @ "<optional external/member name>";
  import <ModuleName>;
```

Observed type notation includes `->` (right-associative function arrows), bracketed list/function domains such as `[(a,b)]`, tuples, parametric variables (`a`, `b`), concrete module-qualified names, and unit-like/constructor values. `Hashtable` declares `data Hashtable`, `Dups = Error | Allow`, and record-like `HashSet`, `Map`, `BijectiveMap`, and `ManyToOneMap` constructors. Its declarations use e.g. `make_hash_map :: [(a,b)]->Hashtable.Hashtable a b @ "Hashtable.make_hash_map"`. `McdUnitSystemUS` exports operations over `Mpl.Value {}` and imports other unit modules. `MplSexpr` has a family of S-expression constructors (`Nul`, `Cons`, `Tok`, `Qt`, `Bq`, `Cm`, `Ca`, `SAppend`, `SList`, `Sym`, `IntCon`, `StrCon`, `RealCon`, `CharCon`, `Key`). The module names `MplMacro`, `MplPrelude`, `MplPrimitives`, and `MplConstants`, combined with interface declarations, indicate macro expansion, standard higher-order functions, primitive operations, and literal definitions; the precise phase boundaries need more body analysis.

The type system can be recovered only partially from interfaces and CLR signatures: interfaces preserve parametric types and data constructors, while the compiled CLR bodies commonly expose `object` for Mpl values. `Mpl.Run.Value` and concrete runtime classes distinguish booleans, functions, tuples/lists, symbols and numeric values. The source type checker and inference/generalization rules are not in the runtime and cannot be established from an interface alone. A full interface dump and a grammar corpus should be made as a cache-only extractor artifact; do not check the proprietary binaries/blobs into the repository.

## Runtime and emitter model

`Quire.Mpl` mirrors `mplrun.dll`, the runtime; `Quire.Mpl.Emit` mirrors `mplasb.dll`, the Reflection.Emit-like assembler. `Mpl.Run.ModuleInterfaceAttribute` and `ModuleTextAttribute` attach module interface/source payloads to a generated `values` class. In the observed shipped modules the latter is empty. Module functions are static methods, values are often static fields, and Mpl closures become generated static methods plus `ldftn`/delegate construction, stored in runtime `Mpl.Run.F`/`DelegateFunction` values. Runtime call helpers (`Mpl.Run` function/value classes) implement invocation and dynamic values; exact tail-call/trampoline semantics require further `Mpl.Run` call-path review. `tail.` is emitted before eligible terminal calls.

Emitter evidence in `src/Quire.Mpl.Emit`: the custom writer builds PE metadata with `System.Reflection.Metadata`, stores methods and locals, resolves type/field/method operands, encodes branch targets and tail-call prefixes, and selects max stack at a floor of 48 (`StackDepthCalculator`). It is not a source compiler: its API resembles `Reflection.Emit`, and several type/member/exception APIs remain unimplemented. Prior scratch-harness results recorded in `lane-mpl-emit-scout.md` show it emitted `Hashtable.values::Update`, `mlookupInv`, and the standalone `McdUnitSystemUS.values` metadata/members; MethodAudit reported the prototype `mlookupInv` and all five `McdUnitSystemUS.values` methods exact. That scratch harness was cache-only and is not a committed, reproducible compiler/test project; the current lane does not claim otherwise.

## Output conventions and method families

Observed Mpl method bodies use fat headers, `.maxstack 48`, and initialized local signatures, even for empty bodies (`Hashtable.values::Update`: one `ret`, no locals). Local slots commonly preserve compiler lowering temporaries. Calls that return the enclosing function's result often have a `tail.` prefix. Calls are a mix of `call` and `callvirt` according to target semantics; fields and members are emitted in compiler order. Constructors, `.cctor`s and `Update` methods are ordinary Mpl-emitted bodies and obey the same header policy. The generated closure pattern in `Hashtable.values` names a helper `$$5d_64c00f8`, then the type initializer binds it with `ldftn`, a runtime `F` delegate constructor, and `DelegateFunction.make`; generated names encode compiler state and must be retained for exact parity.

Metadata ordering is significant: module `values` type, fields and methods appear in a stable source/compiler order; static initialization calls `Update` and initializes closures. `ModuleInterfaceAttribute` and empty `ModuleTextAttribute` are attached to the module type. Mpl-produced names can contain punctuation and encoded text. The full family distribution across the 3,050 bodies has not been classified by AST construct because Mpl source/ASTs are absent. The instruction scan is a useful near-complete opcode-family inventory (99.97% of methods use recognized opcodes), but it is not a construct-family census. A recoverable census needs a high-level IL recognizer for: straight-line forwarding/tail calls; field/static access; constructors/static initialization; branch/loop bodies; pattern dispatch; exception paths; array/list/tuple construction; closure allocation and invocation; and generated helper methods. No evidence supports claiming regularity beyond the strong shared header and opcode conventions.

## Hand-authored source sketches for prototype targets

These are intentionally marked as *hypotheses*, not recovered original source. They use the syntax suggested by module interfaces and source-visible names. The original bodies are not in the install.

```mpl
module Hashtable (mlookupInv) where
  mlookupInv table key = hlookup (table.MapField2) key

module McdUnitSystemUS (Update) where
  Update = system := McdStaticUnitSystemUS.system

module McdUnitSystemUS (op_L) where
  op_L value = McdRun.times_unit value McdUnitSystemSI.mc_ft
```

Expected Release lowering inferred from references:

* `Hashtable.values::Update`: `ret`; header `.maxstack 48`.
* `Hashtable.values::mlookupInv(object, object)`: two initialized locals, local copies/type-binding for `Map`, field read of `Map::Field2`, second argument, `tail. call values::hlookup`, `ret`.
* `McdUnitSystemUS.values::Update`: `ldsfld McdStaticUnitSystemUS.values::system`, `stsfld McdUnitSystemUS.values::system`, `ret`.
* `McdUnitSystemUS.values::op_L(object)`: argument, static `mc_ft` field, `tail. call McdRun.values::times_unit`, `ret`.

The field-pattern syntax above is only suggestive; the interface does not expose private field mappings or expression grammar. The methods' IL is evidence for expected output, not validation of these source strings. A proper proof requires a small Mpl parser/lowerer and an executable test. No such compiler/test project is committed by this research lane; the earlier scratch proof was a direct structured-emitter harness, not a source-to-IL compiler. This gap is an explicit remaining task, rather than an implied success.

## Proposed compiler, build, and gate shape

Add `src/Quire.Mpl.Compiler` (C#) with four stages: lexer; parser and typed AST for module/data/import/declaration/expression forms; type checker (initially rank-1 polymorphism and module signatures, later exact Mpl rules); and IL lowering/metadata generation atop `Quire.Mpl.Emit`. Keep source under `src/<ModuleProject>/mpl/<module>.mpl`. An MSBuild target should invoke the compiler during Release build to write the original module assembly as `$(TargetPath)`, with normal project references preserving dependency order. Compiler output must retain ordered module types, fields, methods, parameters, attributes, local slots, generated closure names and method bodies.

The build product is the candidate assembly per D20. MirrorGen should inventory mapped original members and signatures but must not emit substitute C# bodies for Mpl projects. MethodAudit/release_il should continue loading the built target path and compare the produced assembly with the original Release image; the Mpl build must not use a parity exemption. The current gate stages project build products and can inspect a directly supplied assembly with MethodAudit, but integration needs an explicit Mpl project/output contract and metadata checks for custom-attribute blobs, member order, and compiler-generated members. Shared tools are out of scope for this lane; these are proposals only.

## Effort, risks, authoring recommendation

The 3,050-body count supersedes the broader ~4,000 stub estimate. If hand-written and typechecked, 3,050 implementations at 15–30 minutes each alone are roughly 760–1,525 hours. A source-to-source recovery tool plus hand normalization is likely the practical path: decompile original Release IL into a cache-only control-flow/AST sketch, infer readable expressions where possible, then hand-normalize and commit authored `.mpl` sources. Do not commit decompiler output or original IL (D15). Structured IL descriptions can recover emitted behavior efficiently but would not meet the stated goal of repository source resembling original Mpl source; use them only as a private recovery aid or as temporary compiler tests.

Planning range: 2–4 months for a robust extractor/recovery workflow and a useful lexer/parser/type system; 2–3 further months for an IL backend and assembly metadata support through the full instruction/closure/exception surface; 1–2 months for MSBuild/MirrorGen/MethodAudit integration and validation. These phases overlap, giving about 4–7 engineer-months before body-by-body normalized source recovery, which can be several more months. High risks: recovering exact expression semantics from IL, loss of source-level type information, exotic module/attribute/signature metadata, closure/member naming determinism, cross-module imports, and making generated Release output stable across compiler/runtime versions. The recommendation is cache-only IL/source reconstruction followed by committed, reviewed idiomatic Mpl source, compiled by Quire's own compiler; fully manual source entry is less feasible at this scale, while checked-in IL transcription does not satisfy the source-recovery goal.

## Checkpoint

- Owner: task `task_080923c9b603`; branch `feature/luna-mplc`.
- Members: Mpl compiler research/prototype lane; no original module members changed.
- Release IL report: reference samples inspected; prototype claims above point to prior cache-only scout results. No new compiler-to-IL exactness test was run in this branch.
- Remaining: cache full interfaces for all 51 modules; reproducibly implement the Mpl source compiler proof for `Update`, `mlookupInv`, and `McdUnitSystemUS` operation; then design/test full output metadata and gate integration.
