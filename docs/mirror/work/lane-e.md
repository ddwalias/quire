# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e5`.
- Selected members:
  - `Quire.Mpl.Run.Fn_papp`: `.ctor`, `get_Name`, `invVec<R>` brought to exact Release IL match (3/3 exact).
  - `Quire.Mpl.Run.MethFunction`: all three constructors and `invVec<R>` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.MplForeignMacro`: `.ctor`, `get_Macro`, `ExpandCall`, `ExpandRef` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.MplLocalFunctionMacro`: `.ctor`, `ExpandCall`, `ExpandRef` brought to exact Release IL match (3/3 exact).
  - `Quire.Mpl.Run.MplDestructuringBind`: `.cctor`, `.ctor`, `ExpandCall` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.LocalEnv`: `AddFunctionMacro`, `AddForeignMacro` brought to exact Release IL match (10/10 exact).
  - `Quire.Mpl.Run.Sexpr`: `Walk`, `WalkModule`, `walk_definitions` brought to exact Release IL match (31/31 exact).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,738 checked (`.mirror-cache/release-il-f2359b116f9f/release-il.json`).
- Remaining managed members: 77 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal` (25), `Quire.Mcd.MatDim` (10), `Quire.Mpl.Run.DynamicType` (14), `Quire.Mpl.Run.Function` (14), `Quire.Mpl.Run.Delegates` (13), and `Mcd.NumVal::LoadNaN` structure-only).
