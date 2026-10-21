# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e2`.
- Selected members:
  - `Quire.Mpl.Run.Instruction`: `EmitCode` brought to exact Release IL match (7/7 exact).
  - `Quire.Mpl.Run.InstructionSeq`: `EmitCode` brought to exact Release IL match (2/2 exact).
  - `Quire.Mpl.Run.Instruction+InstructionArgType`: `EmitCode` brought to exact Release IL match (2/2 exact).
  - `Quire.Mpl.Run.Instruction+InstructionArgInt`: `EmitCode` brought to exact Release IL match (2/2 exact).
  - `Quire.Mpl.Run.Instruction+InstructionArgField`: `EmitCode` brought to exact Release IL match (2/2 exact).
  - `Quire.Mpl.Run.Instruction+InstructionArgCtor`: `EmitCode` brought to exact Release IL match (2/2 exact).
  - `Quire.Mpl.Run.defdata_keyword`: `NewTok` brought to exact Release IL match (5/5 exact).
  - `Quire.Mpl.Run.deftype_keyword`: `NewTok` brought to exact Release IL match (5/5 exact).
  - `Quire.Mpl.Run.locally_keyword`: `walk_expression_as` brought to exact Release IL match (7/7 exact).
  - `Quire.Mpl.Run.Keyword`: `key`, `keytok` brought to exact Release IL match (8/8 exact).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,666 checked (`.mirror-cache/release-il-ddf7cf563a27/release-il.json`).
- Remaining managed members: 124 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal`, `Quire.Mcd.MatDim`, `Quire.Mpl.Run.DynamicType`, `Quire.Mpl.Run.Function`, `Quire.Mpl.Run.Delegates`, remaining compiler keywords/macros, and `Mcd.NumVal::LoadNaN` structure-only).
