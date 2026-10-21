# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e4`.
- Selected members:
  - `Quire.Mpl.Run.case_keyword`: `NewTok`, `walk_expression_as` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.do_keyword`: `NewTok`, `walk_expression_as` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.try_keyword`: `NewTok`, `walk_expression_as` brought to exact Release IL match (4/4 exact).
  - `Quire.Mpl.Run.if_keyword`: `NewTok`, `walk_expression_as`, `MakeIf(uint, Sexpr)`, `MakeIf(uint, Sexpr, Sexpr, Sexpr)` brought to exact Release IL match (6/6 exact).
  - `Quire.Mpl.Run.lambda_keyword`: `NewTok`, `walk_expression_as`, `MakeLambda(uint, Sexpr)`, `MakeLambda(uint, Sexpr, Sexpr)` brought to exact Release IL match (5/5 exact).
  - `Quire.Mpl.Run.define_keyword`: `NewTok`, `walk_declaration_as`, `walk_definition_as`, `MakeDefine(uint, Sexpr)`, `MakeDefine(uint, Sexpr, Sexpr)` brought to exact Release IL match (7/7 exact).
  - `Quire.Mpl.Run.defun_keyword`: `NewTok`, `walk_declaration_as`, `walk_definition_as`, `MakeDefun(uint, Sexpr)`, `MakeDefun(uint, Sexpr, Sexpr, Sexpr)` brought to exact Release IL match (6/6 exact).
  - `Quire.Mpl.Run.declare_keyword`: `NewTok`, `walk_declaration_as`, `IsMacro`, `IsSymMacro` brought to exact Release IL match (6/6 exact).
  - `Quire.Mpl.Run.let_keyword`: `ExpandDefs`, `ExpandLet`, `walk_expression_as` brought to exact Release IL match (6/6 exact).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,716 checked (`.mirror-cache/release-il-c1b5a8c673d9/release-il.json`).
- Remaining managed members: 90 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal`, `Quire.Mcd.MatDim`, `Quire.Mpl.Run.DynamicType`, `Quire.Mpl.Run.Function`, `Quire.Mpl.Run.Delegates`, macros `MplForeignMacro`/`MplLocalFunctionMacro`/`MplDestructuringBind`/`LocalEnv`/`Sexpr`, and `Mcd.NumVal::LoadNaN` structure-only).
