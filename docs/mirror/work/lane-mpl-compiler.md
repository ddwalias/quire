# Mpl compiler research and prototype lane

Owner: task `task_c969696e91de`; branch `feature/luna-mplc` (this worker worktree; shared `C:/Projects/Quire` remains on `main`). This document records evidence for all 51 Mpl assemblies before any interface-emitter implementation.

## Step 1: all-module evidence

Reproduction from the repository plus the Prime install:

```powershell
python tools/mpl/dump_interfaces.py --workers 2
python tools/mpl/analyze_modules.py --workers 2
```

The tools enumerate `language: Mpl` entries from `docs/mirror/assembly-map.json`. They decode each type's serialized custom-attribute strings, save each ModuleInterface and its declaration index, and save the IL dumps plus numeric analysis to ignored `.mirror-cache/mpl-interfaces/<install-key>/` and `.mirror-cache/mpl-il-families/<install-key>/`. No original binaries, decoded strings, IL, or decompiler output are committed. `--install` can point both tools at a different Prime install; the JSON report key is a hash of all 51 input DLLs.

Method buckets below use an explicit reproducible heuristic: `empty ret` is exactly the one-opcode `ret` body; `thin` is 2–6 IL opcodes; `real` is more than 6. “Interface-only” further requires exactly the common `.ctor`, `.cctor`, and `Update` bodies, at least one type signature in ModuleInterface, and no body over six opcodes. This bytecode threshold is useful for triage, not a claim about source-level complexity. All 3,050 methods have bodies. The `types` count includes `<Module>`.

| Assembly | Methods | Types | DLL bytes | Empty ret | Thin (2–6 ops) | Real (>6 ops) | Shape |
|---|---:|---:|---:|---:|---:|---:|---|
| EfiAffine | 3 | 2 | 13808 | 1 | 2 | 0 | interface-only |
| EfiBessel | 3 | 2 | 16880 | 1 | 2 | 0 | interface-only |
| EfiCalculus | 20 | 2 | 17392 | 1 | 17 | 2 | real-code |
| EfiCore | 32 | 2 | 28144 | 0 | 13 | 19 | real-code |
| EfiData | 3 | 2 | 17904 | 1 | 2 | 0 | interface-only |
| EfiDebug | 7 | 2 | 14832 | 1 | 2 | 4 | real-code |
| EfiDiffEQ | 15 | 2 | 22000 | 1 | 1 | 13 | real-code |
| EfiFinance | 3 | 2 | 15344 | 1 | 2 | 0 | interface-only |
| EfiFitting | 4 | 2 | 16880 | 1 | 2 | 1 | real-code |
| EfiFourier | 3 | 2 | 14320 | 1 | 2 | 0 | interface-only |
| EfiGraphics | 18 | 2 | 18928 | 0 | 3 | 15 | real-code |
| EfiImage | 3 | 2 | 26608 | 1 | 2 | 0 | interface-only |
| EfiIO | 3 | 2 | 17904 | 1 | 2 | 0 | interface-only |
| EfiMatrix | 18 | 2 | 25584 | 0 | 6 | 12 | real-code |
| EfiPlot | 9 | 2 | 17392 | 0 | 2 | 7 | real-code |
| EfiSignal | 3 | 2 | 21488 | 1 | 2 | 0 | interface-only |
| EfiSixsigma | 3 | 2 | 17392 | 1 | 2 | 0 | interface-only |
| EfiSolver | 28 | 2 | 20976 | 0 | 5 | 23 | real-code |
| EfiSpecial | 3 | 2 | 14832 | 1 | 2 | 0 | interface-only |
| EfiStatistics | 3 | 2 | 24560 | 1 | 2 | 0 | interface-only |
| EfiSymbolics | 88 | 2 | 57328 | 0 | 16 | 72 | real-code |
| EfiSymbolicsEmitter | 416 | 12 | 530416 | 0 | 120 | 296 | real-code |
| EfiSymbolicsLabels | 30 | 7 | 18928 | 0 | 17 | 13 | real-code |
| EfiSymbolicsLexer | 1150 | 326 | 133616 | 0 | 1017 | 133 | real-code |
| EfiSymbolicsMacros | 23 | 2 | 20976 | 0 | 2 | 21 | real-code |
| EfiSymbolicsMap | 27 | 3 | 56816 | 0 | 7 | 20 | real-code |
| EfiSymbolicsParser | 229 | 30 | 163824 | 0 | 55 | 174 | real-code |
| EfiWavelets | 3 | 2 | 21488 | 1 | 2 | 0 | interface-only |
| Hashtable | 23 | 4 | 17392 | 1 | 8 | 14 | real-code |
| McdDynamicEfi | 120 | 2 | 192496 | 0 | 11 | 109 | real-code |
| McdEfi | 3 | 2 | 35824 | 1 | 2 | 0 | trivial-only |
| McdRun | 265 | 43 | 68592 | 0 | 185 | 80 | real-code |
| McdStaticUnitSystem | 27 | 2 | 46064 | 0 | 25 | 2 | real-code |
| McdStaticUnitSystemCGS | 13 | 2 | 54768 | 0 | 12 | 1 | real-code |
| McdStaticUnitSystemMKS | 12 | 2 | 27120 | 0 | 11 | 1 | real-code |
| McdStaticUnitSystemNone | 4 | 2 | 16880 | 0 | 3 | 1 | real-code |
| McdStaticUnitSystemSI | 5 | 2 | 63472 | 0 | 4 | 1 | real-code |
| McdStaticUnitSystemUS | 12 | 2 | 27120 | 0 | 11 | 1 | real-code |
| McdTranslator | 116 | 2 | 59888 | 0 | 55 | 61 | real-code |
| McdUnitSystemCGS | 12 | 2 | 39920 | 0 | 11 | 1 | real-code |
| McdUnitSystemMKS | 4 | 2 | 19952 | 0 | 4 | 0 | thin-only |
| McdUnitSystemNone | 12 | 2 | 16368 | 0 | 11 | 1 | real-code |
| McdUnitSystemSI | 16 | 2 | 48624 | 0 | 10 | 6 | real-code |
| McdUnitSystemUS | 5 | 2 | 19952 | 0 | 5 | 0 | thin-only |
| Mpl | 12 | 6 | 19440 | 1 | 7 | 4 | real-code |
| MplConstants | 3 | 2 | 13296 | 0 | 2 | 1 | real-code |
| MplMacro | 37 | 2 | 27120 | 0 | 9 | 28 | real-code |
| MplPrelude | 152 | 7 | 41968 | 0 | 25 | 127 | real-code |
| MplPrimitives | 10 | 2 | 15856 | 1 | 2 | 7 | real-code |
| MplSexpr | 34 | 17 | 16368 | 1 | 12 | 21 | real-code |
| MplUnitSystemSI | 3 | 2 | 21488 | 0 | 2 | 1 | real-code |

The 12 interface-only assemblies are `EfiAffine`, `EfiBessel`, `EfiData`, `EfiFinance`, `EfiFourier`, `EfiImage`, `EfiIO`, `EfiSignal`, `EfiSixsigma`, `EfiSpecial`, `EfiStatistics`, and `EfiWavelets`. Each has exactly three methods: an empty `Update`, a static initializer that calls `Update`, and an object constructor. `McdEfi` has the same three tiny method shapes but its interface is a large export/re-export list with no `::` signatures, so it is classified `trivial-only`; `McdUnitSystemMKS` and `McdUnitSystemUS` have only 2–6-op bodies and are `thin-only`, not empty-Update modules. The remaining 36 assemblies contain at least one body over six opcodes.

Across the 51 DLLs, the scan found 3,050 methods, 537 types, 3,767 fields, and 2,296,528 bytes. Exactly 21 methods are a lone `ret`; 1,736 other bodies are 2–6 opcodes; 1,293 exceed six opcodes. Every assembly has one `ModuleTextAttribute` and one `ModuleInterfaceAttribute` on its `values` type: all 51 ModuleText strings decode to empty, and all 51 ModuleInterface strings are nonempty (607,230 decoded UTF-8 characters total). The interface corpus contains 4,390 `::` signature lines, 79 `data` lines, 223 `import` lines, 4,390 `@` annotations, and 5,553 `->` tokens. These are literal corpus counts, not a claim that every occurrence has identical grammar. The full strings and declaration indexes are reproduced only into `.mirror-cache`.

### Grammar: what the corpus establishes

The following surface forms are directly visible in the interface texts:

```text
module <Module> (<comma-separated exports>) where
{
  import <Module>;
  data <Type> = <Constructor> | <Constructor> <field-type> ...;
  <name> :: <type-expression> @ "<external-name>";
}
```

There are module headers, imports, data declarations with constructor alternatives, type signatures, and foreign-name annotations. Type texts visibly use `->`, square-bracketed types, tuples, type variables, qualified names, constructor/type application, and braces (including empty braces). Interfaces combine forms familiar from ML/Haskell (`::`, `data`, arrows, type variables) with an explicit brace/semicolon body and `@` foreign targets; this is not enough to claim Haskell layout or Haskell typechecking semantics. Certain grammar is limited to those tokens and their observed ordering. Guessed grammar includes precedence/associativity of type application/arrows, whether declarations can omit annotations, the exact meaning of brace types, pattern/expression syntax, operator precedence, and type inference/generalization rules. The runtime erases many source-level values to `object`, and CLR signatures do not recover the source checker.

### Codegen regularity across five real-code modules

The analyzer assigns each method one coarse, exclusive IL family in this precedence order: thin wrapper/init (≤6 opcodes), exception, switch dispatch, backward branch loop, other conditional branch, array access/creation, field access, call, arithmetic, allocation, then straight-line. The heuristic uses IL opcodes and labels only; it is not an AST reconstruction. Selected modules have these counts:

| Module | Thin wrapper/init | Fields | Calls | Conditions | Loops | Arrays | Dispatch | Exceptions | Other |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Hashtable | 9 | 7 | 0 | 5 | 1 | 1 | 0 | 0 | 0 |
| McdRun | 185 | 7 | 29 | 36 | 5 | 2 | 0 | 1 | 0 |
| McdTranslator | 55 | 14 | 30 | 11 | 0 | 3 | 0 | 0 | 3 |
| MplPrelude | 25 | 14 | 24 | 53 | 7 | 16 | 0 | 0 | 13 |
| EfiSymbolicsLexer | 1017 | 70 | 13 | 30 | 0 | 13 | 7 | 0 | 0 |

Across those modules, 1,291 of 1,706 methods fall into the thin wrapper/init family. Normalizing each full method to its opcode sequence (discarding operands and branch destinations) gives 384 distinct sequences; the ten most common cover 1,170/1,706 bodies (68.6%). Three account for most of that: `ldstr; ret` (356), `newobj; stsfld; ret` (356), and `ldarg.0; ldc.i4; call; ret` (351). Thus a small set explains many accessors, constant-return functions, and initialization wrappers, but the substantive bodies use diverse branches, fields, arrays, calls, loops, and dispatch. This is IL-shape coverage, not a percentage of source constructs recovered.

### Large-module and compiler evidence

`EfiSymbolicsLexer` is 133,616 bytes with 326 types, 471 fields, and 1,150 methods; it has 323 `newarr`, 7 `switch`, 115 `ldftn`, and no `.data` directive in ILSpy's IL listing. `EfiSymbolicsParser` is 163,824 bytes with 30 types, 127 fields, and 229 methods; it has 33 `newarr`, 150 `switch`, 73 `ldftn`, and no `.data`. `EfiSymbolicsEmitter` is 530,416 bytes with 12 types, 190 fields, and 416 methods; it has 84 `newarr`, 703 `switch`, 146 `ldftn`, and no `.data`. The pattern shows arrays, closures, fields, and switch-heavy dispatch, but no single static blob or obvious generated-table representation. Large maxstack requirements vary by method, which fits sizeable lowering routines but does not identify a generator.

The installed `msgen.dll` lists 71 managed/native C++ types, including `gen.Asm`, `gen.AsmProxy`, `gen.Visitron`, `gen.xVisitron`, `gen.AstInfo`, and `gen.umgdCompiler`. Its two PE exports demangle to `gen::Expand` and `gen::NewAsm`. ILSpy presents `gen.umgdCompiler` as an 8-byte NativeCppClass marker with no managed methods. Those names are evidence of a native generator/assembler subsystem, not evidence that `msgen.dll` contains the missing Mpl source compiler. No `mplc` executable or authored `.mpl` files were found in the Prime install.

Evidence for a common Mpl build pipeline: all 51 map entries point to the same `mplrun` runtime and `System.Runtime`; all `values` types carry the same pair of Mpl attributes; all 3,050 methods have IL bodies and `.maxstack` at least 48; 3,007 use exactly 48 and 43 use higher values; all are .NETCoreApp v10 modules in this install and no sibling PDB exists. This is strong common-toolchain evidence, not proof that every module was built by the identical compiler version or options.

## Evidence and recovery boundary

The reference install is Mathcad Prime 12.0.1.0. Release IL and decompiler artifacts were generated with `python tools/reference/reference.py <type> --assembly <assembly> --il`; the tool keys local references by binary SHA-256 under ignored `.mirror-cache`. Representative references used here are `Hashtable.values` from `hashtable.dll`, `McdUnitSystemUS.values` from `mcdunitsystemus.dll`, and `MplSexpr.values` from `mplsexpr.dll`. The complete original modules are the 51 `language: Mpl` entries in `docs/mirror/assembly-map.json`; source IL and interface blobs must remain in `.mirror-cache` (D15).

Previously recorded all-assembly Release scan in [the Mpl emitter scout](lane-mpl-emit-scout.md) counted 3,050 concrete bodies and 103,076 instruction lines. 3,049 bodies use opcode families represented by `Quire.Mpl.Emit`; the remaining body is in `EfiSymbolicsEmitter` and contains 2,356 `nop`s. The count is an instruction-opcode scan, not proof that every metadata operand or signature is serializable.

No `.mpl`, `.mli`, `.ml`, `.mps`, or `.src` file was found in the Prime install. The analyzer decoded every `ModuleTextAttribute` and `ModuleInterfaceAttribute` from all 51 `values` types; ModuleText is empty in every assembly, while ModuleInterface is a nonempty UTF-8 string. These interfaces describe exports, imports, and signatures, not function bodies.

## Recovered language model (provisional)

The interface grammar is ML-like and typed, but has distinctive module/data declarations and a runtime-erased object ABI. The evidence and confidence boundary are in the preceding “Grammar” section; the type system can be recovered only partially from interfaces and CLR signatures. Interfaces preserve type variables and data constructors, while compiled method signatures commonly expose `object` for Mpl values. `Mpl.Run.Value` and concrete runtime classes distinguish booleans, functions, tuples/lists, symbols, and numeric values. The source type checker and its inference/generalization rules cannot be established from the runtime alone.

```text
module <ModuleName> (<exports...>) where
  data <Name> = <Constructor> <field types...> | ...
  <name> :: <type expression> @ "<optional external/member name>";
  import <ModuleName>;
```

Observed type notation includes `->`, bracketed list/function domains, tuples, parametric variables, concrete module-qualified names, and unit-like/constructor values. The `Hashtable` interface exposes several generic map/set abstractions and a duplicate policy. `McdUnitSystemUS` exports operations over runtime values and imports other unit modules. `MplSexpr` exposes a family of S-expression constructors for lists, tokens, quoted forms, symbols, and literals. The `MplMacro`, `MplPrelude`, `MplPrimitives`, and `MplConstants` interfaces suggest their broad roles, but do not establish exact compiler phase boundaries.

## Runtime and emitter model

`Quire.Mpl` mirrors `mplrun.dll`, the runtime; `Quire.Mpl.Emit` mirrors `mplasb.dll`, the Reflection.Emit-like assembler. `Mpl.Run.ModuleInterfaceAttribute` and `ModuleTextAttribute` attach module interface/source payloads to a generated `values` class. In the observed shipped modules the latter is empty. Module functions are static methods, values are often static fields, and Mpl closures become generated static methods plus `ldftn`/delegate construction, stored in runtime `Mpl.Run.F`/`DelegateFunction` values. Runtime call helpers (`Mpl.Run` function/value classes) implement invocation and dynamic values; exact tail-call/trampoline semantics require further `Mpl.Run` call-path review. `tail.` is emitted before eligible terminal calls.

Emitter evidence in `src/Quire.Mpl.Emit`: the custom writer builds PE metadata with `System.Reflection.Metadata`, stores methods and locals, resolves type/field/method operands, encodes branch targets and tail-call prefixes, and selects max stack at a floor of 48 (`StackDepthCalculator`). It is not a source compiler: its API resembles `Reflection.Emit`, and several type/member/exception APIs remain unimplemented. Prior scratch-harness results recorded in `lane-mpl-emit-scout.md` show it emitted `Hashtable.values::Update`, `mlookupInv`, and the standalone `McdUnitSystemUS.values` metadata/members; MethodAudit reported the prototype `mlookupInv` and all five `McdUnitSystemUS.values` methods exact. That scratch harness was cache-only and is not a committed, reproducible compiler/test project; the current lane does not claim otherwise.

## Output conventions and method families

Observed Mpl method bodies use `.maxstack` at least 48: 3,007 of 3,050 use exactly 48, and 43 use larger values (up to 3,015). Only 716 methods have initialized local signatures, so locals are emitted when needed, not universally. Calls returning the enclosing result often have a `tail.` prefix; `call` and `callvirt` both occur. Constructors, static initializers, and `Update` methods use the same shared module shape. A previously inspected Hashtable closure helper uses a generated method name and is bound by `ldftn` and a runtime function wrapper, but that isolated example does not establish the closure-naming algorithm across modules.

Member and attribute ordering remain important for exact assembly reconstruction. The all-module scan gives a coarse IL family distribution across representative methods, but no source ASTs are available; these families describe bytecode shapes only. A real compiler still needs to recover source expressions, static/member order, generated helper naming, and all metadata signatures. ModuleInterface and empty ModuleText are attached to every observed `values` type.

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

The field-pattern syntax above is only suggestive; the interface does not expose private field mappings or expression grammar. The methods' IL is evidence for expected output, not validation of these source strings. The follow-on prototype tests its own limited source syntax and lowering; it does not establish that these are Mathcad's original expressions.

## Proposed compiler, build, and gate shape

Build the production compiler as a tool project such as `tools/mpl/Quire.Mpl.Compiler` (C#) with four stages: lexer; parser and typed AST for module/data/import/declaration/expression forms; type checker (initially rank-1 polymorphism and module signatures, later exact Mpl rules); and IL lowering/metadata generation atop `Quire.Mpl.Emit`. Keep source under `src/<ModuleProject>/mpl/<module>.mpl`. An MSBuild target should invoke the compiler during Release build to write the original module assembly as `$(TargetPath)`, with normal project references preserving dependency order. Compiler output must retain ordered module types, fields, methods, parameters, attributes, local slots, generated closure names and method bodies. Keeping the compiler in `tools/mpl` excludes its own utility code from the product assembly inventory.

The build product is the candidate assembly per D20. MirrorGen should inventory mapped original members and signatures but must not emit substitute C# bodies for Mpl projects. MethodAudit/release_il should continue loading the built target path and compare the produced assembly with the original Release image; the Mpl build must not use a parity exemption. The current gate stages project build products and can inspect a directly supplied assembly with MethodAudit, but integration needs an explicit Mpl project/output contract and metadata checks for custom-attribute blobs, member order, and compiler-generated members. Shared tools are out of scope for this lane; these are proposals only.

## Effort, risks, authoring recommendation

The 3,050-body count supersedes the broader ~4,000 stub estimate. If hand-written and typechecked, 3,050 implementations at 15–30 minutes each alone are roughly 760–1,525 hours. A source-to-source recovery tool plus hand normalization is likely the practical path: decompile original Release IL into a cache-only control-flow/AST sketch, infer readable expressions where possible, then hand-normalize and commit authored `.mpl` sources. Do not commit decompiler output or original IL (D15). Structured IL descriptions can recover emitted behavior efficiently but would not meet the stated goal of repository source resembling original Mpl source; use them only as a private recovery aid or as temporary compiler tests.

Planning range: 2–4 months for a robust extractor/recovery workflow and a useful lexer/parser/type system; 2–3 further months for an IL backend and assembly metadata support through the full instruction/closure/exception surface; 1–2 months for MSBuild/MirrorGen/MethodAudit integration and validation. These phases overlap, giving about 4–7 engineer-months before body-by-body normalized source recovery, which can be several more months. High risks: recovering exact expression semantics from IL, loss of source-level type information, exotic module/attribute/signature metadata, closure/member naming determinism, cross-module imports, and making generated Release output stable across compiler/runtime versions. The recommendation is cache-only IL/source reconstruction followed by committed, reviewed idiomatic Mpl source, compiled by Quire's own compiler; fully manual source entry is less feasible at this scale, while checked-in IL transcription does not satisfy the source-recovery goal.

## Reproducible subset compiler follow-on

The follow-on lane added [Mpl language notes](mpl-language-notes.md), the interface dumper `tools/mpl/dump_interfaces.py`, and a deliberately narrow source compiler in `tools/mpl/Quire.Mpl.Compiler`. The dumper was run against all 51 Mpl assemblies; it wrote 51 decoded interface strings and per-type declaration JSON under the ignored `.mirror-cache/mpl-interfaces/fae88e37629bc218/` (content-addressed by installed artifacts). The compiler reads its own small `.mpl` examples, builds standalone candidate assemblies through `Quire.Mpl.Emit`, and carries `MirrorOf` metadata so MethodAudit can pair methods with the installed types. It recognizes only these three forms; it is not a general Mpl parser or type checker.

`python tools/mpl/test_prototype.py` builds in Release and runs MethodAudit against the original install. Result: **PASS**, two tests; exact Release IL for `Hashtable.values::Update`, `Hashtable.values::mlookupInv`, and `McdUnitSystemUS.values::op_L`. Source, compiler, and test are tracked; generated assembly outputs remain ignored. Original IL listings and decoded interface strings remain in `.mirror-cache`.

Updated follow-on checkpoint: branch `feature/mpl-compiler-prototype-3`, rebased onto current `origin/main` after `fe390ec45` landed. Lint (`python tools/lint/lint.py --base origin/main`) passed; the Release prototype test passed.

## Revised evidence-first checkpoint

- Owner: task `task_c969696e91de`; branch `feature/luna-mplc`.
- Members: Step 1 research and reproducible analyzers only; no mapped module code changed.
- Evidence: all 51 interfaces and IL scans are regenerated into `.mirror-cache`; report totals are above. No source-body compiler work was started.
- Remaining: land this evidence report, then build the interface-only declaration emitter in a separate Step 2 batch. Do not infer real-body compilation capability from this census.
