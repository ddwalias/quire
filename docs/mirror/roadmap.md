# Phase roadmap

## 4. Phases: turning stubs into real code

Phases are ordered by dependency, from the engine up to the app. Each phase:
1. Moves existing Quire code into the mirrored types.
2. Implements the stubs that the phase's features need.
3. Adds tests.
4. Leaves the solution green.

Counts are current Mirror-disposition CSV entries by phase (2026-10-14), not implemented types or method completion. Assembly lists describe routing; native/script estimates are from the original survey. Apply PLAN.md §3c to every phase.

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
| **10. Native engine libraries** | `efi*`, `mcd*`, `mslisp`, `msgen`, `mcfront12`, `mccore12`, `mpl*`, `utl-mgd`, `engutils-mgd`, others | 1,359 | These are C++/CLI wrappers: the logic is native and doesn't appear in metadata. Phase 0 gives them names and signatures. Real behaviour comes function family by function family (Bessel, statistics, signal/FFT, wavelets, ODE solvers, fitting, finance, six-sigma, image I/O), each written from scratch. The symbolic engine (`efisymbolics*` plus Axiom) is by far the largest single item. |
| **11. Mathcad native and scripts** | Exported surfaces of the 10 native Mathcad DLLs, `creo3dfit`'s Mathcad entry point, 64 `MC*` Axiom packages and 2 scripts, the Lisp/Prolog front-end structure (14 files), the user-function C SDK | ≈ 42 classes + ≈ 1,390 exports, 64 packages, 14 files, 1 C API | The symbolic engine on FriCAS with Mathcad-shaped `MC*` packages (the `→` operator, keyword commands such as `simplify` and `solve`); the user-function SDK; the ODE/PDE front end (`ode.prolog`/`pde.prolog`-shaped `odesolve`). |


Original session estimates are retired: method-level analysis and verification now determine the work, and type presence does not predict behavioral completion. Phase 1 remains open; prioritize its engine contracts before building dependent calculation features.
