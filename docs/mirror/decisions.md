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
it to C++/CLI so that MSVC emits Mathcad's IL from Quire's own source. Every body must come from
source in this repository and the compiler run on it: copying, transplanting or patching original
IL, signatures, metadata or bytes into a build output is forbidden, and builds must not read the
Mathcad install (the gate hides it). Only reference and validation tools read originals.

For a mixed-mode candidate the gate reports, without failing, methods that MSVC emits for its own
support and Mathcad's build lacked (`std`, CRT, exception/startup helpers, `vc.cppcli` types) and
native bodies whose Mathcad counterpart is native too. Removing the C# stand-ins that had no
counterpart is allowed. Every method with a counterpart must match exactly, as before.
`mcmisc.dll` is plain C# and moves to its own project, `Quire.Engine.Misc`.

The C++/CLI build uses the compiler that built `utl-mgd.dll`. Its Rich header shows MSVC 19.51.36246
(Visual Studio 2026 18.6.2). The gate builds with that exact instance and rejects other linker builds
([validation](validation.md#ccli-candidate)).

## D20: one Quire project per Mathcad assembly, in its original language (2026-10-23)

The user replaced D2's consolidation with a strict 1:1 layout. Every in-scope Mathcad assembly gets its own Quire
project holding only that assembly's members, built from source in the original's language with the original's
compiler. Classification comes from PE data (CLR `ILONLY` flag, Rich header, linker version, assembly attributes),
not from file names: the `-mgd` suffix does not mean C++/CLI.

| Language | Count | Build | Assemblies |
|---|---:|---|---|
| C# | 30 | Roslyn `.csproj` | `Ptc.*`, `MathcadPrime`, `automation`, `automationmgd`, `componentsmgd`, `efiio-csharp`, `efiplot_`, `efisixsigma_`, `mcmisc`, `mplasb`, `mplrun`, `utilsmgd` |
| C++/CLI | 31 | `.vcxproj`, MSVC 19.51.36246 (D19) | `efi*_` except `efiplot_`/`efisixsigma_`, `efiutl`, `engutils-mgd`, `iengine`, `mccore12`, `mcdepman`, `mceng`, `mcfront12`, `msclr`, `msgen`, `mslisp`, `mspcons-mgd`, `utl-mgd` |
| Mpl | 51 | Quire's own Mpl toolchain | `efi*` without `_` except `efiio-csharp`, `efiutl`, `efisymbolics-umgd`; `mpl*` except `mplasb`/`mplrun`, `mcd*` except `mcdepman`, `hashtable` |
| Native | 10 | native `.vcxproj`, outside the IL gate (D10) | `efistatsci_`, `efisymbolics-umgd`, `engutils`, `mcaduser`, `mctranslator`, `mshex`, `msmpl`, `mspcons`, `msprolog`, `utl` |

`Ptc.License.Plpf` and `Ptc.OleInterop` (C++/CLI) and `creo3dfit` (native) stay deferred (D5).

- **Mpl modules** are emitted by Mathcad's Mpl compiler: every method has `.maxstack 48` and many calls carry
  `tail.`, which Roslyn never emits. They are written as Quire-owned Mpl source and compiled at build time by the
  mirrored toolchain: `msmpl` and `mshex` (native ports), `msgen` and `msclr` (C++/CLI), `mplasb` and `mplrun`
  (C#). C# stand-ins for the 51 modules can never pass the gate. They finish after that chain exists.
- **Native DLLs** are PTC closed source and ship no import libraries. The end state follows the likely original:
  Quire headers declare the exports with `UTL_API`-style `__declspec(dllexport/dllimport)` macros (`extern "C"`
  for `mcaduser`'s documented C API), and the import library is a by-product of linking the native project.
  As a stopgap, until a native DLL is ported, its import library may be generated with `lib.exe` from a `.def`
  in the repository, and the mirrors load Mathcad's DLL at run time. This is a narrow amendment to D19: the
  stopgap import library is the only build output allowed to carry original export names, and each one is
  replaced when its DLL is ported. Third-party imports link against their vendors' libraries (MKL); Knitro stays
  with D6.
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
  no exemption. Lanes A–C (`Quire.Core`, `Quire.Mpl.Emit`, `Quire.Utils`) are already split. Lane D's
  `Quire.Engine.Utl` is 1:1 but still C#; it splits when its `.vcxproj` lands in `src/`. A C++/CLI target that
  still hosts other assemblies (`Quire.Engine` hosts `engutils-mgd`) splits after them; MirrorGen refuses otherwise.

## Mapping and scope

- **D1:** `Mcd` maps to `Quire.Mcd`.
- **D2 (superseded by D20):** The engine assemblies were consolidated into shared projects. Those projects remain
  hosts only until each assembly's D20 target exists; `Rules.ProjectFor` owns routing ([mapping.md](mapping.md)).
- **D3:** Preserve visibility, including internal and private members.
- **D8:** `System.Reflection.Emit.PTC.Mathcad.Mpl` maps to `System.Reflection.Emit.Quire.Mpl`.
- **D4:** Unimplemented features stay hidden in the shipping UI.
- **D5:** Creo platform and licensing libraries remain deferred ([scope](scope.md)).
- **D10:** Native engine internals have no managed IL counterpart and remain outside this
  workflow. Their integration behavior is a separate engineering concern. Under D20 each native DLL is a native
  project that the C++/CLI mirrors link against.
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
