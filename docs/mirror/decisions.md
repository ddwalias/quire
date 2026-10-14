# Decisions

## 6. Decisions

### Decided (2026-10-13)

- **D1:** `Mcd` → `Quire.Mcd` (literal).
- **D2:** Consolidated projects follow the implemented assembly layers in [mapping.md](mapping.md). `Quire.Efi.Common` was not needed and does not exist. `Rules.ProjectFor` is the current routing source.
- **D3:** visibility is faithful. Mathcad's `internal` types stay `internal`, and test projects get
  `InternalsVisibleTo`.
- **D8:** `System.Reflection.Emit.PTC.Mathcad.Mpl` → `System.Reflection.Emit.Quire.Mpl` (literal).
- **D9 (2026-10-14):** Method analysis, implementation, evidence, tests, and deviations follow the authoritative [PLAN.md §3c](PLAN.md#3c-method-by-method-fidelity). User approval for original-counterpart analysis is already given; no per-method renewal. Applicable licensing/legal constraints still need confirming.

### Still open (none of these block Phase 0)

- **D4: stubs in the shipping build.** Either exclude stubbed features from the UI until they're real
  (recommended), or let them surface and throw.
- **D5: Creo platform libraries (§1.4).** Mirror them as PTC code, or defer them with licensing (recommended)?
- **D6: substitutes for commercial components** (§1.2). Fluent.Ribbon + AvalonDock for DevComponents, IPOPT or
  NLopt for Knitro, MathNet.Numerics for MKL. Are these acceptable, or do you prefer others?
  *Checked on net10.0-windows:* `Fluent.Ribbon` 11.0.2, `Dirkster.AvalonDock` 5.0.1,
  `Microsoft.Xaml.Behaviors.Wpf` 1.1.161 and `MathNet.Numerics` 5.0.0 all restore, build and run. `NLoptNet` 1.4.3
  also solved a constrained problem through its native `nlopt.dll`. `ipopt-net` 3.14.20.32 builds, but no solve was
  run with it. Knitro has no NuGet package; it is licensed directly by Artelys.
- **D7: copyleft dependencies.** NRtfTree (LGPL) and possibly IPOPT (EPL-2.0) bring licence obligations. Are
  copyleft dependencies acceptable for Quire's intended distribution?
