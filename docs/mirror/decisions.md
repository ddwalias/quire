# Decisions

## 6. Decisions

### Decided (2026-10-13)

- **D1:** `Mcd` → `Quire.Mcd` (literal).
- **D2:** Consolidated projects follow the implemented assembly layers in [mapping.md](mapping.md). `Quire.Efi.Common` was not needed and does not exist. `Rules.ProjectFor` is the current routing source.
- **D3:** visibility is faithful. Mathcad's `internal` types stay `internal`, and test projects get
  `InternalsVisibleTo`.
- **D8:** `System.Reflection.Emit.PTC.Mathcad.Mpl` → `System.Reflection.Emit.Quire.Mpl` (literal).
- **D9 (2026-10-14):** Method analysis, implementation, evidence, tests, and deviations follow the authoritative [PLAN.md §3c](PLAN.md#3c-method-by-method-fidelity). User approval for original-counterpart analysis is already given; no per-method renewal. Applicable licensing/legal constraints still need confirming.
- **D4 (2026-10-16):** Stubbed features are hidden in the shipping UI until they are real. They do not surface
  and throw.
- **D5 (2026-10-16):** The Creo platform libraries ([scope.md §1.4](scope.md#14-ptc-creo-platform-libraries-ptc-but-shared-across-creo-products-not-mathcad-specific))
  are deferred along with licensing.
- **D10 (2026-10-16):** Native engine internals (Phase 11 and the native side of Phase 10) are not mirrored
  method by method. Quire's engine sits behind `IEngine`, and an outside harness checks it by running the same
  expressions and worksheets through Mathcad's `IEngine` and Quire's. Each difference becomes a deviation
  proposal in [deviations.md](deviations.md).
- **D11 (2026-10-16): tiered scope and definition of done.** Each member is assigned one tier. The per-member
  lists and per-type table are in [tiers/](tiers/).

  | Tier | Members | What | Done means |
  |---|---:|---|---|
  | A | 3,789 | Reachable from the engine, calculation, `mplrun` and document I/O | PLAN §3c rule 7 |
  | A2 | 1,481 | Equation tree and parser | Stretch goal; same bar as A |
  | B | 17,002 | Mechanical members (accessors, constructors, forwarding, constants) | Generated; exact Release-IL match with Mathcad and every callee settled, in place of a per-member direct comparison |
  | C | 18,651 | UI and region behaviour, MPL-compiled libraries | Deferred; stays a stub, hidden under D4 |
  | D | 14,066, plus Phase 11 outside metadata | Shell, automation, Excel, 3D, whiteboard, OLE, licensing, mixed C++/CLI internals, Phase 11 | Out of scope; stays a stub, hidden under D4 |

  The 5,174 abstract members are already settled. A Tier B member the generator can't match exactly returns to
  Tier A if it is reached from the Tier A roots, and to Tier C otherwise.
- **D12 (2026-10-16):** The user accepts the legal risk of generating Tier B members to match Mathcad's
  Release IL, so Tier B work proceeds without waiting for D9's licensing confirmation. This is a risk
  acceptance, not a legal confirmation. Generated source still follows PLAN §3c rule 3 (no decompiler
  artifacts), and no Mathcad IL is committed.

- **D13 (2026-10-18):** The user accepts the licensing risk of copying the minimal set of Mathcad Prime files
  that the nightly audit loads (`mplrun.dll`, `mplasb.dll`) to the separate **private** repository
  `ddwalias/quire-mathcad-bin`, which the nightly workflow reads with a read-only token. This is a risk
  acceptance, not a legal confirmation. Those files are never committed to this repository, and Mathcad IL is
  still never committed.
- **D14 (2026-10-18):** The user removed the independent-review step from the workflow. The merge gate is the
  check before `main`; accepted mutants and unreachable lines need the lane's proof and the user's approval, with
  no reviewer proof. This supersedes every earlier requirement for an independent review or reviewer verdict.

- **D15 (2026-10-18):** The user makes this repository public, so the nightly runs on free 4-vCPU GitHub
  runners. The user waives the IL rule for the one historical dump of `mplrun`'s IL (`docs/mirror/il/`, added
  and removed on 2026-10-14), which stays in history; no history rewrite. Like D12, this is a risk acceptance,
  not a legal confirmation. New decompiled IL or C# is still never committed, and `.gitignore` now blocks
  `*.il`, `*.dll` and `*.pdb`.

### Still open (none of these block Phase 0)

- **D6: substitutes for commercial components** (§1.2). Fluent.Ribbon + AvalonDock for DevComponents, IPOPT or
  NLopt for Knitro, MathNet.Numerics for MKL. Are these acceptable, or do you prefer others?
  *Checked on net10.0-windows:* `Fluent.Ribbon` 11.0.2, `Dirkster.AvalonDock` 5.0.1,
  `Microsoft.Xaml.Behaviors.Wpf` 1.1.161 and `MathNet.Numerics` 5.0.0 all restore, build and run. `NLoptNet` 1.4.3
  also solved a constrained problem through its native `nlopt.dll`. `ipopt-net` 3.14.20.32 builds, but no solve was
  run with it. Knitro has no NuGet package; it is licensed directly by Artelys.
- **D7: copyleft dependencies.** NRtfTree (LGPL) and possibly IPOPT (EPL-2.0) bring licence obligations. Are
  copyleft dependencies acceptable for Quire's intended distribution?
