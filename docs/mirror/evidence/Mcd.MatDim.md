# Mcd.MatDim

- **Original:** `mplrun.dll` 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`,
  MVID `17a0933b-4988-4c18-963d-563998321cc8`
- **Reference:** local `Mcd.MatDim` view from `python3 tools/reference/reference.py Mcd.MatDim`; ILSpy 11.1.0.9782;
  REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14)
- **Quire:** `src/Quire.Mpl/Mcd/MatDim.cs`; tests: `Quire.Mpl.Tests.Mcd.MatDimTests`,
  `Quire.Differential.Tests.MatDimDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `get_UniformDim()` | 0x0600060e | Base getter returns null. | Static view plus direct differential cases; base declaration is not directly invoked on derived objects. | Verified |
| `get_Rows()` | 0x0600060f | Return logical row count. | Static view and direct cases on five implementations. | Verified |
| `get_Cols()` | 0x06000610 | Return logical column count. | Static view and direct cases on five implementations. | Verified |
| `get_RowDimensions()` | 0x06000611 | Base getter returns null. | Static view plus direct differential cases; base declaration is not directly invoked on derived objects. | Verified |
| `get_ColDimensions()` | 0x06000612 | Base getter returns null. | Static view plus direct differential cases; base declaration is not directly invoked on derived objects. | Verified |
| `RowDimensionArray(int)` | 0x06000613 | Allocate one entry per column by querying `dim(row, column)`. | Static view and direct cases on five implementations. | Verified |
| `ColDimensionArray(int)` | 0x06000614 | Allocate one entry per row by querying `dim(row, column)`. | Static view and direct cases on five implementations. | Verified |
| `Eq_b(MatDim, MatDim)` | 0x06000615 | Dispatch to the left operand's virtual matrix-dimension equality. | Static view and direct cases across uniform, row, column, and row-column dimensions. | Verified |
| `Eq_b(MatDim, Dim)` | 0x06000616 | Compare matrix dimension with a scalar dimension using the matrix-dimension implementation's equality rule. | Static view and direct cases for every configured dimension name. | Verified |
| `Eq(MatDim, MatDim, uint)` | 0x06000617 | Return the left operand when equal; otherwise choose the mismatch loser and raise the dimension error for the supplied argument. | Static view and direct cases across dimension kinds. | Verified |
| `Eq(MatDim, Dim, uint)` | 0x06000618 | Return the matrix dimension when equal to the scalar dimension; otherwise raise the dimension error for the supplied argument. | Static view and direct cases for every configured dimension name. | Verified |
| `MMult(MatDim, MatDim)` | 0x0600061a | Dispatch matrix multiplication to the left operand's virtual implementation. | Static view and direct pairwise cases across dimension kinds. | Verified |
| `DMult(MatDim, MatDim)` | 0x0600061b | Dispatch dot multiplication to the left operand's virtual implementation. | Static view and direct pairwise cases across dimension kinds. | Verified |
| `Transpose(MatDim)` | 0x0600061c | Dispatch transpose to the operand's virtual implementation. | Static view and direct cases across dimension kinds. | Verified |
| `CMult(MatDim, MatDim)` | 0x0600061d | Return null unconditionally, including for null operands. | Static view and direct null-operand case. | Verified |
| `Det(MatDim, int)` | 0x0600061e | Dispatch determinant to the operand's virtual implementation with the requested size. | Static view and direct cases across dimension kinds. | Verified |
| `Pwr(MatDim, int)` | 0x0600061f | Positive powers dispatch directly; negative powers invert then power the inverse; zero checks whether squaring is supported, then combines the operand and inverse. | Static view and direct powers −2 through 3 across dimension kinds; null/error paths not exercised. | Verified |
| `make(RC, Dim[], int)` | 0x06000622 | Build a row- or column-axis descriptor, selecting a compact implementation where possible and otherwise retaining the axis dimensions. | Static view and direct cases for both axes, several lengths and shapes. | Verified |
| `makeRowDimension(Dim, Dim[], int)` | 0x06000624 | Build a row descriptor from one row dimension and the supplied column dimensions. | Static view and direct cases for both axes' test inputs and several lengths. | Verified |
| `makeColDimension(Dim, Dim[], int)` | 0x06000626 | Build a column descriptor from the supplied row dimensions and one column dimension. | Static view and direct cases for both axes' test inputs and several lengths. | Verified |
| `make(Dim[], Dim[])` | 0x06000627 | Recognize uniform, row-only, column-only, or separable dimensions; fall back to a matrix-backed `crazy_dim` when no compact representation applies. | Static view and direct separable-array case; fallback and malformed shapes are not covered here. | Verified |
| `make(Dim)` | 0x06000628 | Create a 1×1 uniform dimension from the scalar dimension. | Static view and direct case. | Verified |
| `Uniform(Dim[])` | 0x06000629 | Return the common reference-identical element; null input returns null, empty input throws. | Static view and direct cases over empty, singleton, equal and unequal arrays plus null. | Verified |
| `Eq_b(Dim[], Dim[])` | 0x0600062a | Compare lengths and then call `Dim.Eq_b` pairwise; empty arrays compare true. | Static view and direct cases over empty, equal and unequal lengths and values. | Verified |
| `IsScalarMultiple(Dim[], Dim[])` | 0x0600062b | Compute the first quotient and require reference-identical quotients for remaining left entries; return null on inconsistency. | Static view and direct cases over empty, matching and differing arrays. | Verified |
| `Mult(Dim[], Dim[])` | 0x0600062c | Multiply corresponding dimensions and require reference-identical products; return null on inconsistency. | Static view and direct cases over empty, matching and differing arrays. | Verified |
| `Mult(Dim, Dim[])` | 0x0600062d | Multiply the scalar by each array entry into a fresh result array. | Static view and direct cases over empty and populated arrays. | Verified |
| `Invert(Dim[])` | 0x0600062e | Return a fresh array containing each element raised to −1. | Static view and direct cases over empty and populated arrays. | Verified |
| `Product(Dim, Dim[])` | 0x0600062f | Fold `Dim.Mult` from the scalar through the array; an empty array returns the scalar. | Static view and direct cases over empty and populated arrays. | Verified |
| `Push(Dim, Dim[])` | 0x06000630 | Return a fresh array with the scalar prepended; preserve the input array. | Static view and direct cases over empty and populated arrays. | Verified |
| `uniform_dim.dim(int, int)` | 0x0600079e | Return the stored uniform dimension for every coordinate. | Static view and direct coordinate-grid cases. | Verified |
| `uniform_dim.matdim(int, int)` | 0x0600079f | Return the same uniform descriptor for every coordinate. | Static view and direct coordinate-grid cases. | Verified |
| `uniform_dim.get_UniformDim()` | 0x060007a0 | Return the stored uniform dimension. | Static view and direct cases. | Verified |
| `uniform_dim.get_IsLinear()` | 0x060007a1 | Return this descriptor as the linear dimension. | Static view and direct cases; renderer confirms object identity. | Verified |
| `rc_dim.dim(int, int)` | 0x060007b5 | Return the product of the row and column dimensions at the requested coordinates. | Static view and direct coordinate-grid cases. | Verified |
| `rc_dim.matdim(int, int)` | 0x060007b6 | Return a new 1×1 uniform descriptor for the coordinate's product dimension. | Static view and direct coordinate-grid cases. | Verified |
| `rc_dim.get_IsLinear()` | 0x060007b7 | Return null because row-column dimensions are not represented by one linear dimension. | Static view and direct cases. | Verified |
| `rc_dim.RowDimensionArray(int)` | 0x060007b8 | Return the stored row-dimension array. | Static view and direct cases; renderer observes its elements. | Verified |
| `rc_dim.ColDimensionArray(int)` | 0x060007b9 | Return the stored column-dimension array. | Static view and direct cases; renderer observes its elements. | Verified |
| `r_dim.dim(int, int)` | 0x060007c5 | Return the row dimension selected by the row coordinate. | Static view and direct coordinate-grid cases. | Verified |
| `r_dim.matdim(int, int)` | 0x060007c6 | Return a new 1×1 uniform descriptor for the selected row dimension. | Static view and direct coordinate-grid cases. | Verified |
| `r_dim.get_IsLinear()` | 0x060007c7 | Return null because row-varying dimensions are not represented by one linear dimension. | Static view and direct cases. | Verified |
| `r_dim.get_RowDimensions()` | 0x060007c8 | Return the stored row-dimension array. | Static view and direct cases; renderer observes its elements. | Verified |
| `r_dim.ColDimensionArray(int)` | 0x060007c9 | Return a fresh array containing the invariant column dimension for each row. | Static view and direct cases. | Verified |
| `c_dim.dim(int, int)` | 0x060007d7 | Return the column dimension selected by the column coordinate. | Static view and direct coordinate-grid cases. | Verified |
| `c_dim.matdim(int, int)` | 0x060007d8 | Return a new 1×1 uniform descriptor for the selected column dimension. | Static view and direct coordinate-grid cases. | Verified |
| `c_dim.get_IsLinear()` | 0x060007d9 | Return null because column-varying dimensions are not represented by one linear dimension. | Static view and direct cases. | Verified |
| `c_dim.get_ColDimensions()` | 0x060007da | Return the stored column-dimension array. | Static view and direct cases; renderer observes its elements. | Verified |
| `c_dim.RowDimensionArray(int)` | 0x060007db | Return a fresh array containing the invariant row dimension for each column. | Static view and direct cases. | Verified |

## Test cases and open work

Focused validation run `cdbd9b2c-00a7-4105-895a-21264b76b63e` passed 27 Harness tests, 159 Mpl tests, and 71 focused differential tests (zero failures or skips). The direct comparison cases exercise 49 unique original tokens. `Dimension_array_helpers` covers the array helpers
with empty, singleton, equal, unequal, and polymorphic dimensions. `Matrix_dimension_dispatch` compares the base
dispatchers and factories across uniform, row, column, and row-column descriptors, with coordinate grids, array
access, equality, transpose, determinant, powers −2 through 3, and pairwise multiplication. The renderers include
shape, uniform/linear dimension, every element dimension, and reference identity where relevant; target callbacks
capture mutations. No whole-type equivalence is claimed. `MatDimTests` supplies the independent behavior tests.
Sharper cases include equal-length `Eq_b` arrays differing only at index zero and length-two arrays with a
nonuniform quotient or product at the second element.

Base abstract `dim(int,int)` and `matdim(int,int)`, the constructor, `get_IsLinear()` base implementation, nested
dimension equality and linear algebra overrides, and the `crazy_dim` implementation remain open where no direct
comparison token is listed above. Invalid coordinate behavior and malformed factory inputs are not fully covered.

## Recorded structure mismatches

The focused method audit reports direct behavioral comparisons for these members but finds the following IL-shape
differences; they are retained here rather than treated as structural equivalence:

- `Uniform(Dim[])` (0x06000629): Quire's generated delegate construction and captured `candidate` field access have
  no corresponding original call/field accesses; Quire has 2 conditional branches and the original has 3.
- `Push(Dim, Dim[])` (0x06000630): Quire calls `Array.CopyTo`; the original calls the five-argument `Array.Copy`.
- `Eq(MatDim, MatDim, uint)` and `Eq(MatDim, Dim, uint)` (0x06000617–0x06000618): Quire calls the static
  `MatDim.Eq_b` overloads; the original uses virtual `eq_b` / `eq2_b` dispatch.
- `make(RC, Dim[], int)` (0x06000622): Quire's ROW factory dispatch differs; its branch calls `uniform_dim.make`,
  while the corresponding original branch calls `c_dim.make`.
- `make(Dim[], Dim[])` (0x06000627): Quire calls `rc_dim.make_rc`; the original calls `MatDim.Mult` at the
  corresponding factoring point.

The singleton array cases snapshot public thread-static `Dim.s_vec` before formatting the result. They exercise fresh
dimensions whose first quotient/product may be interned, so the extra loop iteration is checked for observable
scratch changes. The zero-power `Pwr` case uses a built-in row-column descriptor with fresh row/column dimensions
and snapshots both `s_vec` and the interning-table count immediately after the call; its observer performs no further
`Dim` operation. The scoped mutation run killed both loop-start mutants and the `Pwr` exponent mutant. `MatDim` is
public and abstract with protected virtual `pwr(uint)`, so custom derived-type dispatch remains outside these cases;
no equivalence claim is made for custom overrides or untested inputs.
## Full focused method-audit mismatch register — 2026-10-15

This register records every member of this type that the current focused matrix audit classified as `structure_mismatch`. The member-level contracts above retain input and behavior limits; each row below records the first structural distinction reported by that run.

| Member | Original token | First structural distinction |
|---|---|---|
| `Uniform`0([Quire.Mpl]Quire.Mcd.Dim[])->[Quire.Mpl]Quire.Mcd.Dim` | 0x06000629 | calls differ at #0: Quire newobj Mcd.MatDim+<generated>::.ctor`0()->System.Void, Mathcad (end); field accesses differ at #0: Quire stfld Mcd.MatDim+<generated>::candidate, Mathcad (end); conditional branches: Quire 2, Mathcad 3 |
| `Push`0([Quire.Mpl]Quire.Mcd.Dim,[Quire.Mpl]Quire.Mcd.Dim[])->[Quire.Mpl]Quire.Mcd.Dim[]` | 0x06000630 | calls differ at #0: Quire callvirt System.Array::CopyTo`0(System.Array,System.Int32)->System.Void, Mathcad call System.Array::Copy`0(System.Array,System.Int32,System.Array,System.Int32,System.Int32)->System.Void |
| `Eq`0([Quire.Mpl]Quire.Mcd.MatDim,[Quire.Mpl]Quire.Mcd.MatDim,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000617 | calls differ at #0: Quire call Mcd.MatDim::Eq_b`0(Mcd.MatDim,Mcd.MatDim)->System.Boolean, Mathcad callvirt Mcd.MatDim::eq_b`0(Mcd.MatDim)->System.Boolean |
| `Eq`0([Quire.Mpl]Quire.Mcd.MatDim,[Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000618 | calls differ at #0: Quire call Mcd.MatDim::Eq_b`0(Mcd.MatDim,Mcd.Dim)->System.Boolean, Mathcad callvirt Mcd.MatDim::eq2_b`0(Mcd.Dim)->System.Boolean |
| `make`0([Quire.Mpl]Quire.Mcd.MatDim+RC,[Quire.Mpl]Quire.Mcd.Dim[],[System.Runtime]System.Int32)->[Quire.Mpl]Quire.Mcd.MatDim` | 0x06000622 | calls differ at #1: Quire call Mcd.MatDim+uniform_dim::make`0(Mcd.Dim,System.Int32,System.Int32)->Mcd.MatDim+uniform_dim, Mathcad call Mcd.MatDim+c_dim::make`0(Mcd.Dim[],System.Int32)->Mcd.MatDim+c_dim |
| `make`0([Quire.Mpl]Quire.Mcd.Dim[],[Quire.Mpl]Quire.Mcd.Dim[])->[Quire.Mpl]Quire.Mcd.MatDim` | 0x06000627 | calls differ at #4: Quire call Mcd.MatDim+rc_dim::make_rc`0(Mcd.Dim[],Mcd.Dim[])->Mcd.MatDim+rc_dim, Mathcad call Mcd.MatDim::Mult`0(Mcd.Dim,Mcd.Dim[])->Mcd.Dim[] |
| `makeArrayDim`0([Quire.Mpl]Quire.Mcd.MatVal)->[Quire.Mpl]Quire.Mcd.MatDim` | 0x0600064b | calls differ at #19: Quire call Mcd.MatDim+rc_dim::make_rc`0(Mcd.Dim[],Mcd.Dim[])->Mcd.MatDim+rc_dim, Mathcad callvirt Mcd.MatVal::get_Cols`0()->System.Int32 |
| `OnError`0([System.Runtime]System.String,[Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000641 | calls differ at #2: Quire call System.String::Concat`0(System.String,System.String,System.String)->System.String, Mathcad call System.String::Format`0(System.String,System.Object,System.Object)->System.String; constants differ at #0: Quire str 	, Mathcad str {0}	{1} |
| `OnError`0([System.Runtime]System.String,[Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000644 | calls differ at #0: Quire call System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor`0(System.Int32,System.Int32)->System.Void, Mathcad call Mcd.Dim::FixDimension`0(Mcd.Dim)->Mcd.Dim; type operations differ at #0: Quire (end), Mathcad box System.Int32; constants differ at #0: Quire i4 2, Mathcad str {0}	{1}	{2} |
| `OnError`0([System.Runtime]System.String,[Quire.Mpl]Quire.Mcd.Dim,[System.Runtime]System.Int32,[System.Runtime]System.Int32,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000643 | calls differ at #0: Quire call System.Runtime.CompilerServices.DefaultInterpolatedStringHandler::.ctor`0(System.Int32,System.Int32)->System.Void, Mathcad call <generated>::InlineArrayElementRef`2(!!0&,System.Int32)->!!1&<System.Runtime.CompilerServices.InlineArray4`1<System.Object>,System.Object>; type operations differ at #0: Quire (end), Mathcad box System.Int32; constants differ at #0: Quire i4 3, Mathcad i4 2 |
| `PickLoser`0([Quire.Mpl]Quire.Mcd.MatDim,[Quire.Mpl]Quire.Mcd.MatDim,[System.Runtime]System.UInt32)->[System.Runtime]System.Void` | 0x06000645 | calls differ at #0: Quire callvirt Mcd.MatDim::dim`0(System.Int32,System.Int32)->Mcd.Dim, Mathcad callvirt Mcd.MatDim::get_Rows`0()->System.Int32; constants differ at #1: Quire str bad matrix element dimension, Mathcad str array unit mismatch; conditional branches: Quire 4, Mathcad 6 |
