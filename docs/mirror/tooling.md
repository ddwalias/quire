# Mirror tooling

## 3. Phase 0: tooling and skeleton (done)

### What was built

| Piece | What it does |
|---|---|
| `tools/Quire.Tools.MirrorGen` | Reads Mathcad's assemblies with `System.Reflection.Metadata` (as data; nothing is loaded or run, no method bodies are read), maps names by §2, and plans and writes the stubs. `type-map.csv` is the reviewed scope it reads. Commands: `emit`, `manifest`, `native-inventory`, plus diagnostics (`scope-diff`, `collisions`, `graph`, `stats`, `dump`). |
| `src/Quire.Mirror` | `[MirrorOf("Ptc…, Assembly")]` on each mirrored type, `[MirrorStub]` on each unimplemented member, and `MirrorStub.NotImplemented()`. |
| `src/*/Generated/Mirror/*.g.cs` | 4,652 files of `partial` declarations: one per top-level type, nested types inside. |
| `src/*/Generated/Mirror.props` | Per project: project and package references, WPF/WinForms/unsafe settings and `InternalsVisibleTo`, all derived from Mathcad's own references. |
| `docs/mirror/manifest/*.txt` | The mirrored shape of each project, one line per type and member. |
| `docs/mirror/coverage.md` (now generated, not committed) | Mirrored types, members and stubs per project: 5,385 types, 57,774 members, 56,114 stubs. The non-stub 3% are hand-written members, including Quire-only ones. |
| `docs/mirror/notes.txt` | Every place the generated C# differs from Mathcad's metadata, and why. |
| `docs/mirror/native/` | The §1.3 inventory: 1,389 exports from 11 native DLLs (47 exported classes), 464 Lisp definitions, 712 Prolog predicates, 86 Axiom packages with 2,497 operations, and the `MCADINCL.H` API. Names and signatures only. |
| `tests/Quire.Mirror.Tests` | Fails when a manifest line changes or disappears, or when an in-scope Mathcad type has no Quire type. It guards against drift; fidelity to Mathcad is the generator's job, and `notes.txt` lists every known difference. |

The reader was cross-checked with REA: for `Ptc.Controls.SolveBlock.dll` the method counts of all 23 named types
match REA's managed-member inventory, and for `mshex.dll` the 380 exports match exactly.

### Which members are mirrored

Every member Mathcad declares is mirrored, private ones included: fields, constructors, methods, properties and events.
Only compiler-generated members are left out, because their names come from a compiler rather than from Mathcad's
design. These are lambdas, local functions, iterator and async state machines, auto-property and event backing
fields, and the Mpl compiler's `$` gensyms. C# has no private virtual members, so private `newslot` virtual methods
are declared non-virtual.

### How stubs behave

- Methods and properties throw `NotImplementedException` naming the member.
- Nothing that frameworks call implicitly throws:
  - Constructors succeed, so objects can be created by WPF or as the base of hand-written classes. On a type with a
    hand-written part, they chain to its constructor so its initialisation runs.
  - Events are field-like, so subscribing (as WPF does to `PropertyChanged`) works.
  - `Dispose` and finalizers do nothing.
- An override of a concrete base member calls the base, so stubs don't break inherited behaviour (for example
  `OnTemplateChanged`).
- Interface members get throwing default bodies, so hand-written classes that implement a mirrored interface keep
  compiling until the member is implemented.

### Merging with hand-written code

The generator never edits hand-written files. It reads a build of the hand-written code alone
(`dotnet build -p:MirrorStubs=false`, which leaves `Generated/Mirror` out) and skips every member already declared
there. Where hand-written code differs, it wins and the difference goes into `notes.txt`. This covers a different
base class (four region controls), a different access, and overloads covered by optional parameters.

Existing code that clashed with Mathcad's design was moved or renamed, not merged:

| Change | Why |
|---|---|
| `ResultFormat` → `Quire.Controls.Core.ResultFormatOptions` | Mathcad's `ResultFormat` is a `DependencyObject`; Quire's is an options record |
| `MathNode` → `MathExpr` | Mathcad's `MathNode` is a class; Quire's linear-editor tree is records |
| `EngineResult` → `CalculationResult` | Mathcad's `EngineResult` is a struct |
| `Quire.Mpl.Run.Sexpr`/`Sym`/`Atom`/`SList` → `Quire.Mpl.Rl` | Mathcad's `Mpl.Run` types have a different design (`Atom` is abstract, `Sexpr` takes a tag) |
| Persistent data holding equation trees → `Quire.Controls.EquationControl`; `WorksheetPackage`/`WorksheetConverter` → `Quire.Controls.Worksheet` | Mathcad's `Ptc.Core` is the bottom layer; Quire's `Quire.Core` depended on the equation editor |

The four §5 "moves into mirrored types" stay for each phase; Phase 0 only made the names coexist.

### Things the metadata needed

- **Obfuscation.** `Ptc.EquationEditor` is obfuscated: 268 of its 491 types, and many internal members, have names
  such as `a` and `g+a`. They are mirrored verbatim.
  - Where C# forbids a clash that IL allows, the member or the nested type gets a `_` suffix (217 cases).
  - Overrides that the obfuscator renamed are declared under their base member's name.
  - Properties and events whose metadata rows were stripped are rebuilt from their special-name accessors.
  - Interface accessors implemented without a property become explicit properties (251 notes).
- **C++/CLI.** The wrappers only list direct interfaces, and some public types are built on internal ones. The
  planner walks interface closures, and lowers access where C# requires it (notes say where).
- **Excluded types in signatures.**
  - Members whose signatures use deferred types (COM interop, licensing), NRtfTree (pending D7) or unlisted C++
    templates are left out with a note (about 120 members).
  - Pointers to native toolchain types become `nint`.
  - The DevComponents base classes map to their Fluent.Ribbon/AvalonDock equivalents (D6).

### Regenerating

`dotnet build -p:MirrorStubs=false` → `MirrorGen emit` → `dotnet build` → `MirrorGen manifest` → `dotnet test`.

- `emit` needs the Mathcad install (`MATHCAD_DIR` overrides its path); the tests do not.
- `emit` refuses to read a build that includes the stubs, which `Directory.Build.targets` stamps.
- `manifest` refuses to drop lines from the checked-in manifest unless given `--accept-removals`, so every shape
  change is reviewed.
- Hand-written partials declare their constructors explicitly. Once the generated part declares Mathcad's
  constructors, C# no longer supplies the implicit one.

`PositionComparer.Compare` is the first stub made real: Quire's worksheet ordering moved there from
`WorksheetItemPositionComparer`, which now inherits it, as in Mathcad.

Coverage counts in the original tooling inventory above are historical. Current totals and per-member classifications are generated, uncommitted, in `coverage.md` and `status/` (see [status/README.md](status/README.md); definitions in [statuses.md](statuses.md)). MirrorGen reads metadata for structure; behavior analysis is a separate workflow in [evidence/README.md](evidence/README.md).

For repeatable batch commands, strict result reporting, harness checks and platform limits, see [validation.md](validation.md).

## MemberGen

`tools/Quire.Tools.MemberGen` reconstructs the mechanical stubs: members whose Mathcad body is straight-line code
without locals, branches or exception handling (accessors, auto-properties, field-like events, forwarding calls,
constant returns, `throw new …`, constructors that chain and store their arguments). It reads Mathcad's IL with
`System.Reflection.Metadata` and turns it into C# in Quire's names: `=> expression;` for one statement, a block
otherwise, short type names where unambiguous. Each member moves from `Generated/Mirror` (its `[MirrorStub]` line
pair deleted, as PLAN step 2 allows) into the hand-written partial that declares its type, or a new file named after
it, with `[MirrorVerified("<Type>MechanicalDifferentialTests")]`; the stub fields its body uses move with it. Each
Mathcad type gets a test class of one fact per method (`Mechanical.Compare(typeof(T), token)`: zero/null and
distinct values for every parameter and target field, targets created without a constructor) and evidence rows
with the tokens.

Only Tier B members are generated (the maintainer's tier lists in `docs/mirror/tiers/`): a member is written
only when every method key it has (`Owner::Key`, the format of `tier-a.txt`) is listed in `tier-b.txt`. With
`--include-tier-a`, keys in `tier-a.txt` and `tier-a2.txt` are allowed as well, for a lane generating its own Tier A
members under normal review. C, D and unlisted members are never generated. `emit` fails when `tier-b.txt` is
missing.

```sh
dotnet run --project tools/Quire.Tools.MemberGen -- survey --repo . --install <Mathcad dir> --output .mirror-cache/membergen/survey.md
python3 tools/Quire.Tools.MemberGen/accept.py --project Quire.Mpl [--include-tier-a] [--no-mutate] [--work DIR]
```

`survey` classifies every remaining stub by the shape of Mathcad's body (from the Debug builds under `src/*/bin`).
`emit` is internal to `accept.py`: it marks what it writes `[MirrorVerified]` and Verified in the evidence before
anything is verified, and only `accept.py` keeps what then passes. `accept.py` keeps a member only when the merge
gate's method checks pass for it. Each round it restores the tree, emits everything not yet rejected, and builds;
a compile error rejects the member whose lines (or moved fields) it is in. It then runs MethodAudit on the Release
build and the generated comparisons, and rejects every member whose IL is not an exact match (`exact_match`) or
whose test fails; it fails outright when MethodAudit fails or a generated test does not run. When that rejects
nothing, it runs the scoped validation (`validate.py --profile focused --method-type` the touched types, with
`--mutate` unless `--no-mutate`) and rejects every member whose methods are not `compared` there (uncovered,
verified but unsupported, ...) or have a surviving mutant. It repeats until a round rejects nothing and leaves the
accepted members in the working tree, with `summary.json` (kept, rejected with reasons) in its work directory
(default `.mirror-cache/membergen/<project>-<time>`). On any failure or interruption it restores the tree. A lock
file, `.mirror-cache/membergen.lock`, keeps a second run out of the same worktree, and builds run without build
servers. Only `Quire.Mpl` has a portable differential host; other projects need the Windows differential project.
