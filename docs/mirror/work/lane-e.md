# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e3`.
- Selected members:
  - `Quire.Mcd.Type`: `Take` brought to exact Release IL match (14/14 exact).
  - `Quire.Mcd.Type+Factory`: `NewApplication(Type, Type[])`, `NewFunction`, `NewList`, `NewProduct` brought to exact Release IL match (14/14 exact).
  - `Quire.Mcd.Type+Parser`: `Parse`, `ParseAtomic`, `GetNextTokenIf` brought to exact Release IL match (9/9 exact).
  - `Quire.Mcd.Type+Printer`: `Visit(Application)` brought to exact Release IL match (5/5 exact).
  - `Quire.Mcd.Type+Formatter`: `OnFunction`, `Visit(Product)` brought to exact Release IL match (5/5 exact).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,677 checked (`.mirror-cache/release-il-460a3e2c8737/release-il.json`).
- Remaining managed members: 113 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal`, `Quire.Mcd.MatDim`, `Quire.Mpl.Run.DynamicType`, `Quire.Mpl.Run.Function`, `Quire.Mpl.Run.Delegates`, remaining compiler keywords/macros, and `Mcd.NumVal::LoadNaN` structure-only).
