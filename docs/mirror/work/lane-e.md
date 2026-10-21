# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e1`.
- Selected members:
  - `Quire.Mcd.Rational`: `.ctor(double, double)`, `Equals(object)`, `GCD(int, int)` brought to exact Release IL match (23/23 exact).
  - `Quire.Mcd.RngVal`: `Equals(object)` brought to exact Release IL match (25/25 exact).
  - `Quire.Mcd.UnitSystemGroup`: `resetMember(int)` brought to exact Release IL match (9/9 exact).
  - `Quire.Mcd.UnitSystem+Default`: `GetHashCode()` brought to exact Release IL match (5/5 exact).
  - `Quire.Mcd.UnitSystem`: `set_Basis(Unit[])`, `GetCoordinates(Product)`, `GetUnits(double[])`, `GetDimensions(int[])`, `ChangeBasis(int[])` brought to exact Release IL match (29/29 exact).
  - `Quire.Mpl.MplSignature`: `GetHashCode()` brought to exact Release IL match (5/5 exact).
  - `Quire.Mpl.Run.Tuple`: `TupleType(Type[])` brought to exact Release IL match (12/12 exact).
  - `Quire.Mpl.Run.Rts`: `GetTypeInitializer(TypeBuilder)` implemented and brought to exact Release IL match (19/19 exact).
  - `Quire.Mcd.DimVal`: `dyn(object, Dim)`, `NearlyEqual(object, object, double)`, `GetUnits(Dim, bool)`, `as_result(object)` brought to exact Release IL match (105/105 exact).
  - `Quire.Mcd.Dim`: `intern(Dim)`, `IsDimensioned(Dim)`, `FixDimension(Dim)`, `Mult(Dim, Dim)`, `Div(Dim, Dim)`, `Pwr(Dim, double)` brought to exact Release IL match (36/36 exact).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,653 checked (`.mirror-cache/release-il-888943b3630d/release-il.json`).
- Remaining managed members: 137 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal`, `Quire.Mcd.MatDim`, `Quire.Mpl.Run.DynamicType`, `Quire.Mpl.Run.Function`, `Quire.Mpl.Run.Delegates`, compiler keywords, and `Mcd.NumVal::LoadNaN` structure-only).
