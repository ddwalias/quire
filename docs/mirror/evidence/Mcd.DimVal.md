# Mcd.DimVal

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference`); REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (`inspect_managed_members`).
- **Quire:** src/Quire.Mpl/Mcd/DimVal.cs and src/Quire.Mpl/Mcd/DimVal.ApplyMatVal.cs; tests: DimValDifferentialTests and DimValApplyMatValDifferentialTests.

An abstract base class for dimensioned values, providing conversion methods, capability queries, error handling, and dynamic call routing.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `ApplyMatVal(Mcd.DimVal, Mcd.DimVal)` | 0x060006b0 | Convert mv to MatVal with error position 2; pack its elements, pass that array to fv.Apply, and return the result or propagate the first error. | High; same-artifact C# control flow confirms this call order. Current run `/tmp/quire-b-dimval-safe-renderer-9ccce8f` passed both direct cases, matched structure, and covered every line and branch. The wider DimVal audit still has unrelated gaps. | Verified |
| `.ctor`0()` | 0x060006ce | .ctor`0() contract | High | Verified |
| `get_IsPolymorphic`0()` | 0x06000660 | get_IsPolymorphic`0() contract | High | Verified |
| `get_PhysicalUnit`0()` | 0x06000661 | get_PhysicalUnit`0() contract | High | Verified |
| `get_PhysicalDimension`0()` | 0x06000662 | get_PhysicalDimension`0() contract | High | Verified |
| `get_UniformDimension`0()` | 0x06000663 | get_UniformDimension`0() contract | High | Verified |
| `get_IsString`0()` | 0x06000665 | get_IsString`0() contract | High | Verified |
| `get_IsReal`0()` | 0x06000669 | get_IsReal`0() contract | High | Verified |
| `get_IsDimless`0()` | 0x06000674 | get_IsDimless`0() contract | High | Verified |
| `get_IsDimlessReal`0()` | 0x06000675 | get_IsDimlessReal`0() contract | High | Verified |
| `get_IsMatrix`0()` | 0x06000680 | get_IsMatrix`0() contract | High | Verified |
| `get_IsFunction`0()` | 0x0600068b | get_IsFunction`0() contract | High | Verified |
| `get_Rsize`0()` | 0x06000692 | get_Rsize`0() contract | High | Verified |
| `get_Rfinal`0()` | 0x06000693 | get_Rfinal`0() contract | High | Verified |
| `get_Isize`0()` | 0x06000694 | get_Isize`0() contract | High | Verified |
| `dyn`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x06000653 | dyn`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `dv`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x06000654 | dv`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `NearlyEqual`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Double)` | 0x06000655 | NearlyEqual`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Double) contract | High | Verified |
| `nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double)` | 0x06000656 | nearly_equal`0([Quire.Mpl]Quire.Mcd.DimVal,[System.Runtime]System.Double) contract | High | Verified |
| `ToString`0([System.Runtime]System.IFormatProvider)` | 0x06000657 | ToString`0([System.Runtime]System.IFormatProvider) contract | High | Verified |
| `GetUnits`0([Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Boolean)` | 0x06000658 | GetUnits`0([Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Boolean) contract | High | Verified |
| `GetUnits`0([System.Runtime]System.Boolean)` | 0x06000659 | GetUnits`0([System.Runtime]System.Boolean) contract | High | Verified |
| `DimOf`0([System.Runtime]System.Object)` | 0x0600065a | DimOf`0([System.Runtime]System.Object) contract | High | Verified |
| `IsDimOf`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065c | IsDimOf`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `DimCast`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065d | DimCast`0([System.Runtime]System.Object,[Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `SetDim`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065e | SetDim`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `IsUniformDimOf`0([Quire.Mpl]Quire.Mcd.Dim)` | 0x0600065f | IsUniformDimOf`0([Quire.Mpl]Quire.Mcd.Dim) contract | High | Verified |
| `asString`0()` | 0x06000666 | asString`0() contract | High | Verified |
| `asString`0([System.Runtime]System.UInt32)` | 0x06000667 | asString`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000668 | asString`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asReal`0()` | 0x0600066a | asReal`0() contract | High | Verified |
| `asReal`0([System.Runtime]System.UInt32)` | 0x0600066b | asReal`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600066c | asReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `isInt`0([System.Runtime]System.Int32*)` | 0x0600066d | isInt`0([System.Runtime]System.Int32*) contract | High | Verified |
| `asInt`0()` | 0x0600066e | asInt`0() contract | High | Verified |
| `asInt`0([System.Runtime]System.UInt32)` | 0x0600066f | asInt`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000670 | asInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessInt`0()` | 0x06000671 | asDimlessInt`0() contract | High | Verified |
| `asDimlessInt`0([System.Runtime]System.UInt32)` | 0x06000672 | asDimlessInt`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000673 | asDimlessInt`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asDimless`0()` | 0x06000676 | asDimless`0() contract | High | Verified |
| `asDimless`0([System.Runtime]System.UInt32)` | 0x06000677 | asDimless`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessReal`0([System.Runtime]System.UInt32)` | 0x06000678 | asDimlessReal`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asDimlessReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000679 | asDimlessReal`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asVector`0()` | 0x0600067a | asVector`0() contract | High | Verified |
| `asVector`0([System.Runtime]System.UInt32)` | 0x0600067b | asVector`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600067c | asVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRealVector`0()` | 0x0600067d | asRealVector`0() contract | High | Verified |
| `asRealVector`0([System.Runtime]System.UInt32)` | 0x0600067e | asRealVector`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600067f | asRealVector`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRealMatrix`0()` | 0x06000681 | asRealMatrix`0() contract | High | Verified |
| `asRealMatrix`0([System.Runtime]System.UInt32)` | 0x06000682 | asRealMatrix`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000683 | asRealMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asMatrix`0()` | 0x06000684 | asMatrix`0() contract | High | Verified |
| `asMatrix`0([System.Runtime]System.UInt32)` | 0x06000685 | asMatrix`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x06000686 | asMatrix`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32)` | 0x06000687 | asTuple`0([System.Runtime]System.UInt32,[System.Runtime]System.UInt32,[System.Runtime]System.UInt32) contract | High | Verified |
| `asRange`0()` | 0x06000688 | asRange`0() contract | High | Verified |
| `asRange`0([System.Runtime]System.UInt32)` | 0x06000689 | asRange`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600068a | asRange`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `asFunction`0()` | 0x0600068c | asFunction`0() contract | High | Verified |
| `asFunction`0([System.Runtime]System.UInt32)` | 0x0600068d | asFunction`0([System.Runtime]System.UInt32) contract | High | Verified |
| `asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32)` | 0x0600068e | asFunction`0([System.Runtime]System.String,[System.Runtime]System.UInt32) contract | High | Verified |
| `IsFunctionN`0([System.Runtime]System.Int32)` | 0x0600068f | IsFunctionN`0([System.Runtime]System.Int32) contract | High | Verified |
| `asFunctionN`0([System.Runtime]System.Int32)` | 0x06000690 | asFunctionN`0([System.Runtime]System.Int32) contract | High | Verified |
| `asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x06000691 | asFunctionN`0([System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `range_get`0([System.Runtime]System.Int32)` | 0x06000695 | range_get`0([System.Runtime]System.Int32) contract | High | Verified |
| `iterator_get`0([System.Runtime]System.Int32)` | 0x06000696 | iterator_get`0([System.Runtime]System.Int32) contract | High | Verified |
| `asIterator`0()` | 0x06000697 | asIterator`0() contract | High | Verified |
| `matrix_get_1`0([System.Runtime]System.Int32)` | 0x06000698 | matrix_get_1`0([System.Runtime]System.Int32) contract | High | Verified |
| `sym_matrix_get_1`0([System.Runtime]System.Int32)` | 0x06000699 | sym_matrix_get_1`0([System.Runtime]System.Int32) contract | High | Verified |
| `matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069a | matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069b | sym_matrix_get_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `asImmutable`0()` | 0x060006ac | asImmutable`0() contract | High | Verified |
| `asResult`0()` | 0x060006ad | asResult`0() contract | High | Verified |
| `as_result`0([System.Runtime]System.Object)` | 0x060006ae | as_result`0([System.Runtime]System.Object) contract | High | Verified |
| `Apply`0([System.Runtime]System.Object[])` | 0x060006af | Apply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b1 | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32)` | 0x060006b2 | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Int32) contract | High | Verified |
| `OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Object)` | 0x060006b3 | OnError`0([System.Runtime]System.UInt32,[System.Runtime]System.String,[System.Runtime]System.Object) contract | High | Verified |
| `WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x060006b4 | WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b5 | WrongNumberOfArguments`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)` | 0x060006b6 | BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32) contract | High | Verified |
| `BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006b7 | BadCall`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Verified |
| `DynCall`0()` | 0x060006b8 | DynCall`0() contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object)` | 0x060006b9 | DynCall`0([System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006ba | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bb | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bc | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bd | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006be | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006bf | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c0 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c1 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c2 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c3 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c4 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c5 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c6 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c7 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006c8 | DynCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `DynApply`0([System.Runtime]System.Object[])` | 0x060006c9 | DynApply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object)` | 0x060006ca | VecCall`0([System.Runtime]System.Object) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006cb | VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object)` | 0x060006cc | VecCall`0([System.Runtime]System.Object,[System.Runtime]System.Object,[System.Runtime]System.Object) contract | High | Verified |
| `VecApply`0([System.Runtime]System.Object[])` | 0x060006cd | VecApply`0([System.Runtime]System.Object[]) contract | High | Verified |
| `get_MatrixDimension`0()` | 0x06000664 | get_MatrixDimension`0() contract | High | Analyzed |
| `MatDimOf`0([System.Runtime]System.Object)` | 0x0600065b | MatDimOf`0([System.Runtime]System.Object) contract | High | Analyzed |
| `extend_1`0([System.Runtime]System.Int32)` | 0x0600069c | extend_1`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069d | extend_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_1u`0([System.Runtime]System.Int32)` | 0x0600069e | extend_1u`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `extend_2u`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x0600069f | extend_2u`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set0_1`0([System.Runtime]System.Int32)` | 0x060006a0 | matrix_set0_1`0([System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set0_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32)` | 0x060006a5 | matrix_set0_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a1 | matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006a2 | matrix_set_1`0([System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_1`0([System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal)` | 0x060006a3 | matrix_set_1`0([System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal) contract | High | Analyzed |
| `matrix_set_1_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006a4 | matrix_set_1_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a6 | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double,[System.Runtime]System.Double)` | 0x060006a7 | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2i`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double)` | 0x060006a8 | matrix_set_2i`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Double) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006aa | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |
| `matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal)` | 0x060006a9 | matrix_set_2`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[Quire.Mpl]Quire.Mcd.DimVal) contract | High | Analyzed |
| `matrix_set_2_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object)` | 0x060006ab | matrix_set_2_ob`0([System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.Object) contract | High | Analyzed |

## Test cases

**Observed** (`DimValDifferentialTests`, against the installed `mplrun.dll`):
- Capability queries and conversions over polymorphic null and interned dimensions.
- `IsDimlessReal` and `asDimlessReal` on real, complex and unitful `NumVal` instances; `Rfinal` on singleton and multi-element ranges; and `NearlyEqual` on nearby numeric values.
- Dynamic and vector call dispatch.
- Static conversion and unit helpers.
- Matrix extension and setter operations, including zero and larger row indices; result rendering checks dimensions, stride, real and imaginary elements, and handles invalid mutated backing lengths without masking the value difference.

**Verified this run** (`DimValApplyMatValDifferentialTests`, direct `MethodComparison.Compare` cases; run `/tmp/quire-b-dimval-safe-renderer-9ccce8f`):
- A four-argument delegate returns all four values, checking that a 2×2 matrix is packed in row-major order and passed to `Apply`.
- A scalar second argument fails during `asMatrix(2)` conversion before function invocation.

## Open

- Current scoped validation `/tmp/quire-b-dimval-safe-renderer-9ccce8f` passed Harness 29/29, Mpl 159/159 and Differential 79/79; lint 0 and ratchet ok. `ApplyMatVal` has matching structure and no uncovered lines or branches. The type-wide audit reports 21 uncovered members and one missing constructor. Mutation killed 92/111; the only 19 remaining mutants are proposed as equivalent because each base `DynCall`/`VecCall` overload calls `OnError(..., "bad_function", this)` before its `actual` count can reach `WrongNumberOfArguments`. The run used `/tmp/quire-dimval-proposed-mutants.txt`, so the merge gate cannot accept it until the proposal is reviewed and approved. See the Lane B checkpoint for the separate reviewer result and proposal list.
