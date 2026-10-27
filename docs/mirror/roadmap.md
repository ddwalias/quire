# Phase roadmap

For assembly-by-assembly dependency order and the next assemblies unlocked by completion,
use the [assembly map](assembly-map.md). The phase groups below describe areas of the product.

## 4. Phases: turning stubs into real code

Phases group assemblies by dependency and help select work. Every managed batch follows
[the same five-step Release IL workflow](PLAN.md); phases add no validation or closure steps.
Counts below describe the original type inventory, not current IL completion.

| Phase | Mathcad assemblies | Types | Becomes real |
|---|---|---:|---|
| **1. Engine runtime** | `mplrun`, `iengine` | 203 | `Mpl.Run` S-expression types (`Sexpr`, `SCons`, `SList`, `Sym`, `Atom`, `Keyword`, `MplEnv`, `Function`…), `Mcd` values (`NumVal`, `MatVal`, `StrVal`, `RngVal`, `Dim`, `Unit`, `Units`, `UnitSystem`, `Rational`), `Engine.IEngine`/`IFactory`/`NodeCallback`/`NodeStatus`. Today's interpreter moves behind them. |
| **2. Calculation** | `Ptc.Calculation.Core`, `Ptc.Calculation` | 160 | First, the engine result-dictionary contract (§3b). Then the result node hierarchy, `EngineError`, `ResultManager`, `UnitDescriptor`, `CalculationController` with Mathcad's handler set, `EngineBuilder`, `CollapsibleDispatchQueue`, `DataExchangeHandler`. |
| **3. Core** | `Ptc.Core` | 679 | `Ptc.PersistentDataObjects` and `Ptc.Serialization` (Quire's `.qwx` and `.mcdx` I/O move here), `Ptc.Wpf` helpers that are actually used. Many `Ptc.Wpf` types stay stubs until a later phase needs them. |
| **4. Equation editor** | `Ptc.EquationEditor` | 491 | `EquationModel`, `MathNode` tree and descriptor tables, `Tree.Parsing` (Mathcad's rule-based parser shape around the existing grammar), `Tree.Transform` (MathNode→Rl, Xml→MathNode), the `View` visual-node hierarchy. **This is where direct 2D editing arrives**: the `IVisual*Editor`, navigator and typeover types. |
| **5. Control infrastructure** | `Ptc.Controls.Core`, `Ptc.Controls` | 575 | Region base classes, commands, adorners, settings (`ResultFormat`, `MathStyle`), `Ptc.Controls.IO`. |
| **6. Region controls** | `EquationControl`, `Plots`, `Text`, `Include`, `SolveBlock`, `CollapsibleArea` | 836 | Plot strategies, axes, trace types (lines, bars, stems, error bars…), the `SolveBlockModel`/`SolverAggregate` split, collapsible areas, include regions. |
| **7. Worksheet** | `Ptc.Controls.Worksheet`, `Whiteboard` | 249 | Persistent-data visitors, item-location strategies, header/footer, pages, print, whiteboard layout. |
| **8. App and automation** | `MathcadPrime`, `Ptc.MathcadPrime.Automation`, `EngNotebook*.Automation`, `automationmgd`, `componentsmgd`, `automation` | 605 | The `Spirit` shell → `Quire`: ribbon and backstage view models, MRU, message boxes, behaviours. The Automation API (its public surface is documented in the shipped `Ptc.MathcadPrime.Automation.xml`). |
| **9. Components** | `Ptc.Controls.ExcelComponent`, `Ptc.Media3D` | 215 | Excel component (via `DocumentFormat.OpenXml`) and 3D plots. |
| **10. Native engine libraries** | `efi*`, `mcd*`, `mslisp`, `msgen`, `mcfront12`, `mccore12`, `mpl*`, `utl-mgd`, `engutils-mgd`, others | 1,359 | 57 of these 87 assemblies are IL-only (MPL-compiled engine modules, plus the C# `efiplot_`, `efisixsigma_` and `efiio-csharp`); the other 30 are C++/CLI wrappers whose logic is native and doesn't appear in metadata. Tiers per [D11](decisions.md): MPL-compiled modules are Tier C, the C# libraries Tier B/C, native internals Tier D; engine behaviour is checked from outside under D10. Phase 0 gives them names and signatures. Real behaviour comes function family by function family (Bessel, statistics, signal/FFT, wavelets, ODE solvers, fitting, finance, six-sigma, image I/O), each written from scratch. The symbolic engine (`efisymbolics*` plus Axiom) is by far the largest single item. |
| **11. Mathcad native and scripts** (Tier D per [D10/D11](decisions.md)) | Exported surfaces of the 11 in-scope native DLLs, including `creo3dfit`, 64 `MC*` Axiom packages and 2 scripts, the Lisp/Prolog front-end structure (14 files), the user-function C SDK | ≈ 42 classes + ≈ 1,390 exports, 64 packages, 14 files, 1 C API | The symbolic engine on FriCAS with Mathcad-shaped `MC*` packages (the `→` operator, keyword commands such as `simplify` and `solve`); the user-function SDK; the ODE/PDE front end (`ode.prolog`/`pde.prolog`-shaped `odesolve`). |
