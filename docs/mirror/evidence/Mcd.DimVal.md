# Mcd.DimVal

- **Original:** `mplrun.dll` 12.0.1.0 (IL-only, net10.0), SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Inspection record:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (managed-member inspection, 2026-10-14);
  decompiled view: ilspycmd 11.1.0.9782
- **Quire:** `src/Quire.Mpl/Mcd/DimVal.cs`; tests: `Quire.Differential.Tests.DimValDifferentialTests`

An abstract base class for dimensioned values, providing conversion methods, capability queries,
error handling, and dynamic call routing.

| Member | Token | Normalized IL | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `ApplyMatVal`0([Quire.Mpl]Quire.Mcd.DimVal,[Quire.Mpl]Quire.Mcd.DimVal)` | 0x060006b0 | — | Convert `mv` with `asMatrix(2u)`, pack its elements with `PackObjects()`, and pass that array to virtual `fv.Apply`; conversion exceptions occur before application. | Same-artifact ilspycmd/IL; five direct cases in Lane B checkpoint | Verified |
| `.ctor`0()` | 0x060006ce | IL | .ctor`0() contract | High | Verified |
| `get_IsPolymorphic`0()` | 0x06000660 | IL | get_IsPolymorphic`0() contract | High | Verified |
| `get_PhysicalUnit`0()` | 0x06000661 | IL | get_PhysicalUnit`0() contract | High | Verified |
| `get_PhysicalDimension`0()` | 0x06000662 | IL | get_PhysicalDimension`0() contract | High | Verified |
| `get_UniformDimension`0()` | 0x06000663 | IL | get_UniformDimension`0() contract | High | Verified |
| `get_IsString`0()` | 0x06000665 | IL | get_IsString`0() contract | High | Verified |
| `get_IsReal`0()` | 0x06000669 | IL | get_IsReal`0() contract | High | Verified |
| `get_IsDimless`0()` | 0x06000674 | IL | get_IsDimless`0() contract | High | Verified |
| `get_IsDimlessReal`0()` | 0x06000675 | IL | get_IsDimlessReal`0() contract | High | Verified |
| `get_IsMatrix`0()` | 0x06000680 | IL | get_IsMatrix`0() contract | High | Verified |
| `get_IsFunction`0()` | 0x0600068b | IL | get_IsFunction`0() contract | High | Verified |
| `get_Rsize`0()` | 0x06000692 | IL | get_Rsize`0() contract | High | Verified |
| `get_Rfinal`0()` | 0x06000693 | IL | get_Rfinal`0() contract | High | Verified |
| `get_Isize`0()` | 0x06000694 | IL | get_Isize`0() contract | High | Verified |
| `dyn`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x06000653 | IL | dyn`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `dv`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x06000654 | IL | dv`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `NearlyEqual`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Double)` | 0x06000655 | IL | NearlyEqual`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Double) contract | High | Verified |
| `nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double)` | 0x06000656 | IL | nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double) contract | High | Verified |
| `ToString`0([System.Runtime]System.IFormatProvider)` | 0x06000657 | IL | ToString`0([System.Runtime]System.IFormatProvider) contract | High | Verified |
| `GetUnits`0([Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Boolean)` | 0x06000658 | IL | GetUnits`0([Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Boolean) contract | High | Verified |
| `GetUnits`0([System.Runtime]System.Boolean)` | 0x06000659 | IL | GetUnits`0([System.Runtime]System.Boolean) contract | High | Verified |
| `DimOf`0([System.Runtime]System.Object)` | 0x0600065a | IL | DimOf`0([System.Runtime]System.Object) contract | High | Verified |
| `IsDimOf`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065c | IL | IsDimOf`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `DimCast`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065d | IL | DimCast`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `SetDim`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065e | IL | SetDim`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `IsUniformDimOf`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065f | IL | IsUniformDimOf`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `asString`0()` | 0x06000666 | IL | asString`0() contract | High | Verified |
| `asString`0([System.Runtime]System.UInt32)` | 0x06000667 | IL | asString`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000668 | IL | asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asReal`0()` | 0x0600066a | IL | asReal`0() contract | High | Verified |
| `asReal`0([System.Runtime]System.UInt32)` | 0x0600066b | IL | asReal`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600066c | IL | asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `isInt`0([System.Runtime]System.Int32*)` | 0x0600066d | IL | isInt`0([System.Runtime]System.Int32*) contract | High | Verified |
| `asInt`0()` | 0x0600066e | IL | asInt`0() contract | High | Verified |
| `asInt`0([System.Runtime]System.UInt32)` | 0x0600066f | IL | asInt`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000670 | IL | asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessInt`0()` | 0x06000671 | IL | asDimlessInt`0() contract | High | Verified |
| `asDimlessInt`0([System.Runtime]System.UInt32)` | 0x06000672 | IL | asDimlessInt`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000673 | IL | asDimlessInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asDimless`0()` | 0x06000676 | IL | asDimless`0() contract | High | Verified |
| `asDimless`0([System.Runtime]System.UInt32)` | 0x06000677 | IL | asDimless`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessReal`0([System.Runtime]System.UInt32)` | 0x06000678 | IL | asDimlessReal`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000679 | IL | asDimlessReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asVector`0()` | 0x0600067a | IL | asVector`0() contract | High | Verified |
| `asVector`0([System.Runtime]System.UInt32)` | 0x0600067b | IL | asVector`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600067c | IL | asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRealVector`0()` | 0x0600067d | IL | asRealVector`0() contract | High | Verified |
| `asRealVector`0([System.Runtime]System.UInt32)` | 0x0600067e | IL | asRealVector`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600067f | IL | asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRealMatrix`0()` | 0x06000681 | IL | asRealMatrix`0() contract | High | Verified |
| `asRealMatrix`0([System.Runtime]System.UInt32)` | 0x06000682 | IL | asRealMatrix`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000683 | IL | asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asMatrix`0()` | 0x06000684 | IL | asMatrix`0() contract | High | Verified |
| `asMatrix`0([System.Runtime]System.UInt32)` | 0x06000685 | IL | asMatrix`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000686 | IL | asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32)` | 0x06000687 | IL | asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRange`0()` | 0x06000688 | IL | asRange`0() contract | High | Verified |
| `asRange`0([System.Runtime]System.UInt32)` | 0x06000689 | IL | asRange`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600068a | IL | asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asFunction`0()` | 0x0600068c | IL | asFunction`0() contract | High | Verified |
| `asFunction`0([System.Runtime]System.UInt32)` | 0x0600068d | IL | asFunction`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600068e | IL | asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `IsFunctionN`0([System.Runtime]System.Int32)` | 0x0600068f | IL | IsFunctionN`0([System.Runtime]System.Int32) contract | High | Verified |
| `asFunctionN`0([System.Runtime]System.Int32)` | 0x06000690 | IL | asFunctionN`0([System.Runtime]System.Int32) contract | High | Verified |
| `asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x06000691 | IL | asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `range_get`0([System.Runtime]System.Int32)` | 0x06000695 | IL | range_get`0([System.Runtime]System.Int32) contract | High | Verified |
| `iterator_get`0([System.Runtime]System.Int32)` | 0x06000696 | IL | iterator_get`0([System.Runtime]System.Int32) contract | High | Verified |
| `asIterator`0()` | 0x06000697 | IL | asIterator`0() contract | High | Verified |
| `matrix_get_1`0([System.Runtime]System.Int32)` | 0x06000698 | IL | matrix_get_1`0([System.Runtime]System.Int32) contract | High | Verified |
| `sym_matrix_get_1`0([System.Runtime]System.Int32)` | 0x06000699 | IL | sym_matrix_get_1`0([System.Runtime]System.Int32) contract | High | Verified |
| `matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069a | IL | matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069b | IL | sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `asImmutable`0()` | 0x060006ac | IL | asImmutable`0() contract | High | Verified |
| `asResult`0()` | 0x060006ad | IL | asResult`0() contract | High | Verified |
| `as_result`0([System.Runtime]System.Object)` | 0x060006ae | IL | as_result`0([System.Runtime]System.Object) contract | High | Verified |
| `Apply`0([System.Runtime]System.Object[])` | 0x060006af | IL | Apply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b1 | IL | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32)` | 0x060006b2 | IL | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Object)` | 0x060006b3 | IL | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Object) contract | High | Verified |
| `WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x060006b4 | IL | WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b5 | IL | WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x060006b6 | IL | BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b7 | IL | BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `DynCall`0()` | 0x060006b8 | IL | DynCall`0() contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object)` | 0x060006b9 | IL | DynCall`0([System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006ba | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bb | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bc | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bd | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006be | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bf | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c0 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c1 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c2 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c3 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c4 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c5 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c6 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c7 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c8 | IL | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynApply`0([System.Runtime]System.Object[])` | 0x060006c9 | IL | DynApply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object)` | 0x060006ca | IL | VecCall`0([System.Runtime]System.Object) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006cb | IL | VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006cc | IL | VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `VecApply`0([System.Runtime]System.Object[])` | 0x060006cd | IL | VecApply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `get_MatrixDimension`0()` | 0x06000664 | IL | get_MatrixDimension`0() contract | High | Analyzed |
| `MatDimOf`0([System.Runtime]System.Object)` | 0x0600065b | IL | MatDimOf`0([System.Runtime]System.Object) contract | High | Analyzed |
| `extend_1`0([System.Runtime]System.Int32)` | 0x0600069c | IL | extend_1`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069d | IL | extend_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_1u`0([System.Runtime]System.Int32)` | 0x0600069e | IL | extend_1u`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_2u`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069f | IL | extend_2u`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set0_1`0([System.Runtime]System.Int32)` | 0x060006a0 | IL | matrix_set0_1`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set0_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006a5 | IL | matrix_set0_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a1 | IL | matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006a2 | IL | matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal)` | 0x060006a3 | IL | matrix_set_1`0([System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal) contract | High | Analyzed |
| `matrix_set_1_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006a4 | IL | matrix_set_1_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a6 | IL | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double,[System.Runtime]System.Double)` | 0x060006a7 | IL | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2i`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a8 | IL | matrix_set_2i`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006aa | IL | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal)` | 0x060006a9 | IL | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal) contract | High | Analyzed |
| `matrix_set_2_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006ab | IL | matrix_set_2_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |

## Test cases

**Observed** (`DimValDifferentialTests`, against the installed `mplrun.dll`):
- Capability queries and conversions over polymorphic null and interned dimensions
- Dynamic and vector call dispatch
- Static conversion and unit helpers
- Matrix extension and setter operations
- Inherited `PhysicalUnit`, `IsDimlessReal` and `asDimlessReal` bodies on real, complex, polymorphic and dimensioned `NumVal`s
- `extend_1` and `matrix_set0_1` at indices 0 and 5, `extend_2` at (1, 0) and (0, 0), `matrix_set0_2` at (1, 0),
  with the matrix shape, allocated rows, stored parts and imaginary storage shown
- `Rfinal` on `Range(1, 5)`, `IncrRange(4, 3, 1)` and `IncrRange(-2, 0.5, 3)`, and `NearlyEqual` of two numbers and
  of a number and a string

**Unreachable statements:** these bodies end in a default return (`null`; `NaN` for `asReal`, `0` for `asInt`)
after a call that always throws (`AppException.Raise`, `OnError`, `BadCall(-1, …)` or `WrongNumberOfArguments`), as
the original's tails do: `asString(string, uint)`, `asReal(string, uint)`, `asInt(string, uint)`,
`asVector(string, uint)`, `asRealVector(string, uint)`, `asRealMatrix(string, uint)`, `asMatrix(string, uint)`,
`asTuple`, `asRange(string, uint)`, `asFunction(string, uint)`, `asFunctionN(int, uint)`, `matrix_get_1`,
`sym_matrix_get_1`, `matrix_get_2`, `sym_matrix_get_2`, `Apply`, `WrongNumberOfArguments(int, int, uint)` (the
closing braces after each `OnError` too), `BadCall(int, int, uint)` and `DynCall()`. Every raising call is compared;
coverage reports the tails as unexecuted because no input reaches them.

## Not yet comparable

- `Quire.Mcd.DimVal..ctor()`: Protected constructor of an abstract class; cannot be invoked directly via ConstructorInfo.Invoke without concrete instantiation.

## ApplyMatVal

The original public static method (`0x060006b0`) is artifact-bound to the Prime 12.0.1.0 assembly identified above. Its IL loads `mv`, calls virtual `DimVal.asMatrix(uint)` (`0x06000685`) with `2u`, stores the returned `MatVal`, calls virtual `MatVal.PackObjects()` (`0x06000539`), then calls virtual `fv.Apply(object[])` and returns that result. The overload delegates to `DimVal.asMatrix(string,uint)` (`0x06000686`), whose base implementation raises at argument position 2; the `MatVal` override (`0x0600046b`) returns itself. `PackObjects()` traverses columns outer and rows inner, calling `MatVal.get_2(int,int)` (`0x060004d1`); a live pointer is returned as an object, while a stale pointer marker with no table entry can be replaced by NaN in real storage. The result, conversion exception and source-matrix effects are directly compared in `DimValApplyMatValDifferentialTests`.

## Release IL shape follow-up (2026-10-19)

`dyn(object,Dim)` (`0x06000653`), `NearlyEqual(object,object,double)` (`0x06000655`), `GetUnits(Dim,bool)` (`0x06000658`)
and `as_result(object)` (`0x060006ae`) now report exact Release instruction matches in `python tools\validation\il_diff.py`
against the Mathcad artifact above. The changes preserve the original branch direction, cast locals, fallback return and
null-dimension selection. `DimValDifferentialTests` directly compare these paths. For `as_result`, a new
`RngValInterpolate` case observes that virtual dispatch returns a plain `RngVal`, distinguishing it from the unchanged
fallback object path. The required Release class passed 6/6 before this case was added and must be rerun. Other methods
and untested cases remain as documented above.

