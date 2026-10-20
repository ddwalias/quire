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

## Mapping and scope

- **D1:** `Mcd` maps to `Quire.Mcd`.
- **D2:** Project consolidation follows [mapping.md](mapping.md); `Rules.ProjectFor` owns routing.
- **D3:** Preserve visibility, including internal and private members.
- **D8:** `System.Reflection.Emit.PTC.Mathcad.Mpl` maps to `System.Reflection.Emit.Quire.Mpl`.
- **D4:** Unimplemented features stay hidden in the shipping UI.
- **D5:** Creo platform and licensing libraries remain deferred ([scope](scope.md)).
- **D10:** Native engine internals have no managed IL counterpart and remain outside this
  workflow. Their integration behavior is a separate engineering concern.
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
