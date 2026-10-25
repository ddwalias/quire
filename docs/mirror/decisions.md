# Current project decisions

## D17: five-step Release IL workflow (2026-10-20)

Claim unowned members, read original Mathcad IL, implement, compare Release IL, commit and
land. This is the complete managed mirroring workflow. Changed methods must match Mathcad;
previously exact methods in affected projects must remain exact. All IL inspection uses
Release builds. Debug assemblies are rejected. The [plan](PLAN.md) defines the commands.

Only the Release IL result establishes completion. Source annotations, old test results and
historical receipts do not. Calling an unfinished callee does not mark that callee complete
or prevent matching the caller's instructions.

The old workflow tools and records have been removed from the working tree. Their previous
versions remain in Git history; they are not instructions for current agents.

## D18: four assembly lanes (2026-10-21)

The user retired every old lane and assigned A to `Quire.Core`, B to `Quire.Mpl.Emit`,
C to `Quire.Utils`, and D to `Quire.Engine.Utl`. These are the only active assignments;
all earlier file reservations are released. Preserve old branch work locally for recovery.
The user starts each worker in a separate Codex/AGY/Claude thread; agents must not spawn
workers or create threads. Each creates its own worktree from current `main`.
Each assignment covers every managed member of its assembly, superseding D11's historical
C/D deferrals there. Native/unreadable bodies still cannot establish managed IL parity.
Use D17's five steps, preserve previously exact methods, land passing batches and continue.

## D19: utl-mgd is an honest C++/CLI port (2026-10-23)

`utl-mgd.dll` was compiled by MSVC as C++/CLI; Roslyn cannot emit its IL. The user chose to port
it to C++/CLI so that MSVC emits Mathcad's IL from Quire's own source. (The build restrictions this
decision first set were lifted by D27.)

For a mixed-mode candidate the gate reports, without failing, methods that MSVC emits for its own
support and Mathcad's build lacked (`std`, CRT, exception/startup helpers, `vc.cppcli` types) and
native bodies whose Mathcad counterpart is native too. Removing the C# stand-ins that had no
counterpart is allowed. Every method with a counterpart must match exactly, as before.
`mcmisc.dll` is plain C# and moves to its own project, `Quire.Engine.Misc`.

The C++/CLI build uses the compiler that built `utl-mgd.dll`. Its Rich header shows MSVC 19.51.36246
(Visual Studio 2026 18.6.2). The gate builds with that exact instance and rejects other linker builds
([validation](validation.md#ccli-candidate)).

On 2026-10-24 the user confirmed that interface constants are source: the literals a function uses, such as the
registry key and value names `utl.dll` reads, are typed into Quire source as a managed method's `ldstr` operands are.
Reading them from an original during analysis is allowed.

## D20: one Quire project per Mathcad assembly, in its original language (2026-10-23)

The user replaced D2's consolidation with a strict 1:1 layout. Every in-scope Mathcad assembly gets its own Quire
project holding only that assembly's members, built from source in the original's language with the original's
compiler. Classification comes from PE data (CLR `ILONLY` flag, Rich header, linker version, assembly attributes),
not from file names: the `-mgd` suffix does not mean C++/CLI.

| Language | Count | Build | Assemblies |
|---|---:|---|---|
| C# | 34 | Roslyn `.csproj` | `Ptc.*`, `MathcadPrime`, `automation`, `automationmgd`, `componentsmgd`, `efiio-csharp`, `efiplot_`, `efisixsigma_`, `interop.*` (D22), `mcmisc`, `mplasb`, `mplrun`, `utilsmgd` |
| C++/CLI | 31 | `.vcxproj`, MSVC 19.51.36246 (D19) | `efi*_` except `efiplot_`/`efisixsigma_`, `efiutl`, `engutils-mgd`, `iengine`, `mccore12`, `mcdepman`, `mceng`, `mcfront12`, `msclr`, `msgen`, `mslisp`, `mspcons-mgd`, `utl-mgd` |
| Mpl | 51 | Quire's own Mpl toolchain | `efi*` without `_` except `efiio-csharp`, `efiutl`, `efisymbolics-umgd`; `mpl*` except `mplasb`/`mplrun`, `mcd*` except `mcdepman`, `hashtable` |
| Native | 10 | native `.vcxproj`, outside the IL gate (D10) | `efistatsci_`, `efisymbolics-umgd`, `engutils`, `mcaduser`, `mctranslator`, `mshex`, `msmpl`, `mspcons`, `msprolog`, `utl` |

`Ptc.License.Plpf` and `Ptc.OleInterop` (C++/CLI) and `creo3dfit` (native) stay deferred (D5). Every other binary in the
install has a recorded reason in `Rules.NotMirrored`, and the [assembly map](assembly-map.md) lists all of them.

- **Mpl modules** are emitted by Mathcad's Mpl compiler: every method has `.maxstack 48` and many calls carry
  `tail.`, which Roslyn never emits. They are written as Quire-owned Mpl source and compiled at build time by the
  mirrored toolchain: `msmpl` and `mshex` (native ports), `msgen` and `msclr` (C++/CLI), `mplasb` and `mplrun`
  (C#). C# stand-ins for the 51 modules can never pass the gate. They finish after that chain exists.
- **Native DLLs** are PTC closed source and ship no import libraries. The end state follows the likely original:
  Quire headers declare the exports with `UTL_API`-style `__declspec(dllexport/dllimport)` macros (`extern "C"`
  for `mcaduser`'s documented C API), and the import library is a by-product of linking the native project.
  As a stopgap, until a native DLL is ported, its import library may be generated with `lib.exe` from a `.def`
  in the repository, and the mirrors load Mathcad's DLL at run time. Each stopgap import library is replaced
  when its DLL is ported. The stopgap only lets a build link: every native DLL is still mirrored, and its
  port precedes its dependents in the [completion order](assembly-map.md). Third-party imports link against their
  vendors' libraries (MKL); Knitro stays with D6.
- **Shared-header types** follow each original's metadata: where an original defines a type, its project defines
  it; where it only references one, the project references the owner. C++/CLI projects include one shared header,
  so each DLL compiles its own copy, as Mathcad's build did. The C# public duplicates (`Mathcad.IValue`,
  `IMatrixValue`) are embedded COM interop types and come from embedding the interop types.
- **Support projects** with no Mathcad counterpart are allowed as an explicit list (`Quire.Mirror`,
  `Quire.Mpl.Runtime`, and a home for `Quire.Engine`'s own interpreter code). Mirrored method bodies never call
  into them; that would change the IL.
- **Cycles:** the original graph has none, counting managed references and native imports. A cycle is reported as
  an error, never resolved by merging assemblies.
- **Migration is progressive.** `Rules.ProjectFor` names each assembly's target. MirrorGen writes an assembly's
  members to its target once that project exists in the original's language, and to its D2 project until then.
  Splitting an assembly out is therefore creating its target project. The Release IL gate is unchanged and gains
  no exemption. All four lane projects are split: lane D's `Quire.Engine.Utl` builds
  `MixedMode/Quire.Engine.Utl.vcxproj` behind a `.csproj` wrapper ([mapping](mapping.md)). A C++/CLI target that
  still hosts other assemblies (`Quire.Engine` hosts `engutils-mgd`) splits after them; MirrorGen refuses otherwise.

## D21: own-assembly resource URIs and product-only members (2026-10-23)

The XAML compiler writes the building assembly's name into `InitializeComponent`'s pack URI
(`/Ptc.Calculation.Core;component/…`). Quire's assemblies have their own names, and renaming them would
collide with the Mathcad DLLs. MethodAudit therefore compares a string literal that starts with a pack URI
naming the assembly holding it as `<self>` on both sides; the rest of the literal must match, and a URI
naming another assembly is compared as written. Mirrored code names its own Quire assembly in such URIs.
The resources behind them must be Quire-authored XAML built from Quire's own data, never Mathcad's XAML
or BAML. On 2026-10-23 none exists yet: `defaultunitdescriptortable.xaml` (lane C, from `BuiltinUnits`),
`engineerrormessages.xaml`, `redefinitionwarningsdictionary.xaml` and
`functionalitieslimitation/functionalitieslimitationdictionary.xaml` still fail to load.

A member with no Mathcad counterpart before and after a change may be changed or removed: it cannot
affect parity, and callers must be able to move onto mirrored APIs. A new member without a counterpart,
a member that loses its counterpart, a compiler-generated helper of a mirrored type, and an explicitly
selected member still fail when changed.

## D22: the COM interop assemblies are standalone projects (2026-10-24)

The user chose to mirror the four PTC type-library interop assemblies as C# projects owned by lane G, amending D20:

| Original | TypeLib | Types | Project |
|---|---|---:|---|
| `interop.mathcadautomationprivate` | `MathcadAutomationPrivate` 1.2 | 18 | `Quire.AutomationPrivate` |
| `interop.mathcad` | `Mathcad` 1.0 | 91 | `Quire.Interop.Mathcad` |
| `interop.mathcadold` | `MathcadOld` 1.0 | 7 | `Quire.Interop.MathcadOld` |
| `interop.vsinterfaceslib` | `VSINTERFACESLib` 1.0 | 90 | `Quire.Interop.VSInterfacesLib` |

`automation`, `componentsmgd`, `iengine` and `mcfront12` reference them, so their signatures need these types.
Their types bind in the type map like any other assembly, and `Rules.ProjectFor` routes them. `Mathcad.*` maps to
the `Quire` namespace (`Mathcad.IValue` is `Quire.IValue`). Hand-written declarations carry their own `[MirrorOf]` so that their interop attributes keep the original's order;
MirrorGen writes no shell for a top-level type bound that way when hand-written code declares all its members.

`automationmgd.dll` defines 8 `MathcadAutomationPrivate` interfaces as NoPIA copies (`[TypeIdentifier]`,
`[CompilerGenerated]`, only the members it uses), so they are compiler output, not source. `automationmgd`,
`Ptc.Calculation` and `Ptc.Controls.ExcelComponent` embed `Mathcad.IValue` and its relatives the same way. The end state builds them as Mathcad did:
`Quire.Automation.Native` references `Quire.AutomationPrivate` with `EmbedInteropTypes`, which needs the original's
assembly attributes (`ImportedFromTypeLib`, `Guid`, `TypeLibVersion`) in `Quire.AutomationPrivate`, and MirrorGen
stops generating the 8 copies; the same applies to the embedded `Mathcad.*` types. Until that lands, the consumers
keep their generated copies, and no project may reference two assemblies that define the same type, or the build
fails with an ambiguous type.

For `interop.mathcadautomationprivate`, Roslyn cannot express the remaining differences, so they are accepted: `runtime managed internalcall` on the 50
interface members (Roslyn emits `cil managed`), `hidebysig` on the coclass constructor, the `instance` bit on 4
property signatures, `beforefieldinit` on 14 types, attribute order on 8 types, and the `IsModified` setter
parameter name `value` (the original's is `pIsModified`). The gate compares an abstract member's access and custom
modifiers, and also a runtime-supplied constructor's implementation flags. A C++/CLI port (D19) might emit the exact
flags, but that is untested.

## D23: native harness rules for thunks, toolchain symbols and chained unwind (2026-10-24)

Lane L (`mspcons.dll`) raised three targets that `native_bytes.py --strict` could not verify. The user chose these rules:

- **Import thunks:** a call to a non-exported name that lands on `jmp [slot]` (FF 25) verifies when the slot imports
  that name and is a mismatch when the slot imports another. No manifest entry is needed.
- **Toolchain entries:** a symbol that the toolset's static libraries supply, such as the C runtime's
  `??_7type_info@@6B@`, cannot be defined by a candidate. `native_targets.py --toolchain` records it as
  `{"rva": ..., "toolchain": true}`. Every site must resolve to that RVA, a candidate that defines it is a mismatch,
  and an unfiltered run lists each such entry. Only its RVA is checked, never its bytes.
- **Chained unwind:** a function's extent is its primary `.pdata` range plus the chained ranges
  (`UNW_FLAG_CHAININFO`) that continue it without a gap. Chained ranges are not function starts. In the installed
  Mathcad DLLs, every chained range continues its primary function. A detached one would be left out, so a candidate
  with the whole body fails on its length instead of matching in part.

## D24: gate scope, judge integrity and native merging (2026-10-25)

The maintainer tightened the gate where it could pass non-identical code, and moved validation out of the merge
lock. Details are in [validation](validation.md).

- **Scope:** a changed project also selects every project whose reference closure reaches it; the type map selects
  all projects; scope diffs list both sides of a rename.
- **Judge:** MethodAudit is built from the base commit and `lane_merge.py` runs main's gate scripts. Tool changes
  apply from the run after they land.
- **Strict IL:** custom modifiers, `pinned`, signature headers, function pointers, P/Invoke parameter marshaling and
  assembly-qualified non-framework types are compared. Unpairable or unreadable methods fail their own row.
- **Native:** `native_bytes.py` is strict by default, checks relocation addends on export/import targets, compares data
  exports and matches manifest names without the `?A0x` tag. Changed native projects are gated by `native_gate.py`:
  a function that passed must still pass; new or changed functions must pass.
- **Merging:** lanes validate outside the lock; under it, `main` unchanged or moved without touching the candidate's
  inputs lands without rebuilding, otherwise it is revalidated. Base inventories are cached in the main checkout.
- **Mapping:** `utl-mgd.dll,EHExceptionRecord` is a `c++ artefact`, like the other CRT exception types; the
  mixed-mode build emits it under its C++ name.

## D25: link-time code generation in the native originals (2026-10-25)

Lane J (`mshex.dll`) could not reproduce the stream-slot accessor at `0xa7c0`. The candidate differs in one register
(`eax` where the original uses `edx`) and lays out the static guard's slow path out of line. The maintainer found
the cause in the Rich headers, and the user chose to park such functions:

- **Build:** Mathcad's own objects in every native-only DLL were compiled with `/GL` (Rich product `0x0109`, build
  36246). In `mspcons` and `efistatsci_` all of them were; in `mshex`, 20 of 24. Thirteen of the 22 `efi*_` DLLs with
  a mirror project also contain `/GL` objects. No DLL shows profile-guided optimization. `native_bytes.py` compares
  COFF objects, and a `/GL` object holds no machine code, so the original's code was generated by the linker.
- **Existing matches stand.** Small functions match under `/O2` despite `/LTCG` (Lane L's `mspcons` functions,
  Lane J's 36).
- **Experiment:** a candidate DLL of the accessor, an approximation of its caller `0x99e0` and `operator<<`, linked
  with `/GL /LTCG` under 36246, still gives `eax` and the out-of-line slow path. So did `/O1`, `/Os`, `/EHa`, `/EHs`,
  `/GS-`, `/guard:cf`, `/Qspectre` and `/favor:INTEL64`. Only `/Od /Ob2` gives the original's `jle` layout, and it
  does not optimize the rest. The original's inlined guard in `0x99e0` uses the same inline layout, which no small
  link reproduces.
- **Rule:** a function that per-object compiles cannot reproduce, after the source shape and the flags tried are
  recorded in the lane's checkpoint, is parked with that evidence. Lanes do not search further through compiler flags.
  A harness mode that compares functions in a linked candidate DLL is reconsidered once a DLL's internal call graph is
  mostly mirrored. A small link does not reproduce the original's choices.
- **Parked:** `mshex` `operator<<(ostream&, Error const&)` (`0xa2d0`) and `Error::GetDetails` (`0x8780`); Engutils
  stays blocked on them.
- **SECREL is deferred** until a function needs it. In `mshex` the `.tls` data is merged into `.rdata` (RVA `0x12188`)
  and the image has no `.tls` section, so the base is the TLS directory's `StartAddressOfRawData`, not a section
  start. A SECREL whose target lies outside the TLS data stays unsupported.

## D26: leaf internal targets; mspcons' iostream instance is parked (2026-10-25)

Lane L (`mspcons.dll`) raised two blockers. The maintainer decided both:

- **Leaf functions as internal targets.** A manifest function entry may name a leaf function (no `.pdata`) when its
  RVA lies in an executable section and outside every `.pdata` range, primary or chained. Its extent is inferred as
  for a leaf export: up to the next export, `.pdata` range or manifest function entry, without trailing `int3`
  padding. `native_targets.py` records such entries from matching call and address sites, as before. An extent that
  runs on into an unrecorded neighbour fails on its length, never passes in part. This covers `pstring`'s
  `type` (`0x1260`), `write_raw` (`0x13A0`) and `tagged::get_tag` (`0x1190`).
- **`operator<<(ostream&, char)` at `0x16E0` is parked under D25, not a toolchain entry.** It is a header template
  instantiated in Mathcad's own `/GL` objects. The toolset's static libraries do not supply it, so D23 does not apply.
  A per-object `/O2` compile calls `fill`, `flags`, `rdbuf`, `tie` and `width` through import thunks (433 bytes).
  A small `/GL /LTCG` link inlines them (454 bytes) like the original (458 bytes, which also inlines `good`).
  `tagged::write`, which calls it, stays parked.
- `pstring::before` calls an out-of-line `type_info::before` where the original inlines it. That is the same
  pattern, so `before` is probably parked too. `notag` and its vftable verify only when all seven slots do.

## D27: builds may read the Mathcad install (2026-10-25)

The user lifted D19's build restrictions. Builds may read the Mathcad install, and build outputs may carry original
IL, metadata or bytes. The gates no longer hide the install (`MATHCAD_DIR`, `MathcadInstallDir`), no longer scan
build inputs for references to it, and no longer reject an `.ilnew` section. What stays from D19: the C++/CLI port,
the `toolset_support` reporting and the MSVC 19.51.36246 pin.

D15 is unchanged. This repository is public, so original binaries, decompiled source and IL stay out of commits, and
the native manifest still holds names, RVAs and sizes only. Without D19, an exact match shows only that the build
output equals the original. It no longer shows that Quire's source alone reproduces it.

## D28: native builds use /EHa and dllimport for msvcp140 (2026-10-25)

The maintainer investigated the shapes Lanes J and K could not reproduce, and the user adopted the rule below. Two build
settings explain most of them:

- **`/EHa`.** All four native originals spill `this` to `[rsp+8]` (`mshex` 73 sites, `utl` 89, `efistatsci_` 56,
  `mspcons` 4), which `/EHa` produces and `/EHsc` does not. With `/EHa`, `mshex`'s three `Error` constructors match
  without other changes. Lane J's "extra `std::string` byte store" was a misreading: the original has that store too.
  Lane K already builds with `/EHa`.
- **`_CRTIMP2=__declspec(dllimport)`.** The original calls msvcp140 functions such as `std::uncaught_exception`
  through `call [__imp_...]`. The pinned headers declare them without `dllimport`, so the candidate calls a thunk
  (`E8`, one byte shorter). With `/EHa` and `/D_CRTIMP2=__declspec(dllimport)`, `mspcons`'s `operator<<` instances
  at `0x1520` (446 bytes) and `0x16E0` (458 bytes) match. D26 parked both, and through them `pstring::write`,
  `write_raw`, `tagged::write` and `notag`.
- **No regressions.** With both settings, all exports that already match still match: `mshex` 77, `mspcons` 28,
  `utl` 34, `efistatsci_` 8, and the `efistatsci_` `assign_scalar` trial 5/5.
- **Source shapes, not flags**, for the remaining Lane K parks: `BoostThreadPriority` and `RestoreThreadPriority`
  match as `return !SetPriorityClass(...) && !SetThreadPriority(...);`. `RestoreThreadPriority`'s two globals are
  zero in the original, so declare them without initializers. `GetStdDev` matches when the mean comes from
  `GetAverage()` before the scale is computed, with `samples` a `std::vector<double>`. `mcGetSetting` stays parked:
  eight further forms gave 251–284 bytes against 297.

Rule: every native project builds with `/EHa` and `_CRTIMP2=__declspec(dllimport)`. Re-test functions parked under
D25/D26 with these settings before treating them as linker-generated.

**Amendment (2026-10-25): the exception model is per source file.** `mshex`'s `Attachment` (the virtual base of
`Component`, `Value`, `Environment` and the rest) matches only under `/EHsc`: constructor `0xC490` (102 bytes), destructor
`0xC500`, deleting destructor `0xC540` and slot 1 `0xC5F0` (`delete this`). Under `/EHa` the constructor is 84 bytes and
both destructors store the vptr before loading the lock. Its `Error` constructors in the same DLL match only under
`/EHa`. So Mathcad compiled some files of one DLL with `/EHsc` and others with `/EHa`. `/EHa` stays the project default.
A file whose functions differ only in the `this` spill, the order of the vptr store, or the cleanup path of a
`new`-expression is set to `/EHsc` per file (`<ExceptionHandling>Sync</ExceptionHandling>` on its `ClCompile`
item), and its source files are grouped as the original's objects were. Try both models for such a function before
parking it. `mcGetSetting` stays parked under both models.

## D29: parked functions verify their callers by RVA (2026-10-26)

`mshex`'s function-local statics lay out their first-time initialization inline after a `jle`. Per-object compilation
puts it out of line, under every exception model, optimization flag and source shape tried, and D25 showed that a
small `/LTCG` link does too. The stream-slot accessor (`0xA7C0`, `pword(xalloc())`) and `Formatter::Fetch` (`0x99E0`) are
such functions. Everything that reaches them stayed unverified: Component's `Insert` and `OnError`, their vtables, and
about 40 `Attachment`-derived constructors. The user chose to let parked functions verify their callers:

- **Manifest:** an entry may be marked `"parked": true`. It is a function entry (no size, not a toolchain entry) and
  is recorded with `native_targets.py --parked NAME` from a matching site, like any other entry.
- **Checking:** every site must resolve to the entry's RVA. Its bytes are not compared and its own targets are not
  followed. A candidate may define its attempt, and the attempt is not reported as a match.
- **Visibility:** a run without `--symbol` lists each parked entry, as it lists toolchain entries. Record the source
  shape and flags tried for each one in the lane's checkpoint, as D25 requires before parking.
- **Scope:** only a function D25 parks may be marked. Exported functions need no entry, because a reference to an
  export already verifies by RVA. Their own rows stay failing or absent until a linked-DLL comparison exists.

## Mapping and scope

- **D1:** `Mcd` maps to `Quire.Mcd`.
- **D2 (superseded by D20):** The engine assemblies were consolidated into shared projects. Those projects remain
  hosts only until each assembly's D20 target exists; `Rules.ProjectFor` owns routing ([mapping.md](mapping.md)).
- **D3:** Preserve visibility, including internal and private members.
- **D8:** `System.Reflection.Emit.PTC.Mathcad.Mpl` maps to `System.Reflection.Emit.Quire.Mpl`.
- **D4:** Unimplemented features stay hidden in the shipping UI.
- **D5:** Creo platform and licensing libraries remain deferred ([scope](scope.md)).
- **D10:** Native engine internals have no managed IL counterpart, so the Release IL gate does not cover them.
  Under D20 each native DLL is still mirrored as a native project that the C++/CLI mirrors link against, and it
  takes its place in the [completion order](assembly-map.md) like any other assembly.
- **D11:** [Tier lists](tiers/) select work: A covers runtime/calculation/I/O, A2 covers the
  equation tree, and B covers mechanical members. All three use the same exact Release IL
  acceptance rule. C is deferred and D is out of scope. Matching abstract/delegate declarations
  need no body. Tier membership does not establish completion.

## Original artifact storage

- Original-counterpart analysis is authorized; it needs no per-member renewal.
- **D12:** The user accepted the risk of generating members to match Mathcad Release IL.
- **D13:** The existing private `ddwalias/quire-mathcad-bin` repository is authorized to hold
  the previously approved `mplrun.dll` and `mplasb.dll` copies. No scheduled audit consumes them.
- **D15:** This repository is public. Original binaries, decompiled source and IL stay out
  of new commits. The historical IL dump added and removed on 2026-10-14 stays in Git history;
  the user authorized retaining that history.

## Open dependency choices

- **D6:** Commercial-component replacements remain undecided: Fluent.Ribbon/AvalonDock,
  IPOPT/NLopt, and MathNet.Numerics are candidates listed in [scope](scope.md).
- **D7:** Acceptance of copyleft dependency obligations remains undecided.

These choices do not add steps to a managed IL mirroring batch.
