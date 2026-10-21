# Lane E — Quire.Mpl

- Owner: Lane E (Claude).
- Branch: `mirror/e-mpl-20261021-e6`.
- Selected members:
  - `Quire.Mcd.MatDim` (65/65 exact managed bodies): `Uniform(Dim[])`, `Eq_b(Dim[], Dim[])`, `IsScalarMultiple(Dim[], Dim[])`, `Mult(Dim[], Dim[])`, `Mult(Dim, Dim[])`, `Invert(Dim[])`, `Product(Dim, Dim[])`, `Pwr(MatDim, int)`, `make(Dim[], Dim[])`, `makeArrayDim(MatVal)`.
  - `Quire.Mpl.Run.DynamicType` (30/30 exact managed methods): `DefineTuple(ModuleBuilder, string)`, `DefineTuple(ModuleBuilder, Type[])`, `DefineNullaryRecord`, `DefineRecord` (3 overloads), `DefineConstructor`, `DefinePrinter` (2 overloads), `DefineFields`, `RemovePrefix`, `NameDemangle`, `TupleNameDemangle`, `Ldarg`.
  - `Quire.Mpl.Run.Function`: `IsFunctionN(int)`, `DynApply(object[])`, `AssertFullApp(int)`, `AssertFullApp(int, uint)`, `VecCall(object)`, `VecCall(object, object)`, `VecCall(object, object, object)` brought to exact Release IL match (7 methods).
  - `Quire.Mcd.MatVal`: `.ctor(object[])`, `FillArr(double[], double, int, int, int)`, `IsUniformPtrs()`, `IsUniformPtrsNoChange()`, `DivideByDimension(Dim)`, `ApplyLinearDimension(MatDim)`, `MatCreateDim(object, object, object)` brought to exact Release IL match (7 methods).
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Mpl` PASS; 1,776 checked (`.mirror-cache/release-il-fbab7bddc254/release-il.json`).
- Remaining managed members: 39 unfinished methods with Mathcad originals (primarily `Quire.Mcd.MatVal` (18), `Quire.Mpl.Run.Delegates` (13), `Quire.Mpl.Run.Function` (7 closures), and `Mcd.NumVal::LoadNaN` structure-only).
