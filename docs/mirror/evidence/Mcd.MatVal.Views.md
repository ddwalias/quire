# Mcd.MatVal view and export members

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`); REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`, inspect_managed_members.
- **Quire:** `src/Quire.Mpl/Mcd/MatVal.Views.cs`; tests: `MatValViewDifferentialTests`.
- Contracts recorded before implementation. Units conversion is a representation cast, not physical scaling. Original unexpected mutations and indexing are intentional fidelity requirements.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| Mcd.MatVal.asTuple(u4, u4, u4) | 0x0600046d | Require requested prefix shape no larger than matrix; incompatible shape uses supplied handle as argument position. Return column-major elements. | Direct cases: requested prefix, oversized shape, and oversized columns. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.ToString() | 0x06000471 | MatVal[rows,cols] header, then up to ten rows. Show up to three elements per row, append ellipsis for more columns; zero-column matrices add no rows. | Direct cases: ordinary shape, truncation beyond ten rows/three columns, and zero columns. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Equals(object) | 0x06000472 | Require exact MatVal runtime type and same shape; compare each element in column-major order using reusable temporary numbers. | Direct cases: same values, other type, null, shape mismatch, and element mismatch. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.nearly_equal(class:584, r8) | 0x06000473 | Require MatVal and same shape; compare each element through DimVal.NearlyEqual using reusable numbers and given fuzz. | Direct cases: nonmatrix, same shape, outside fuzz, and shape mismatch. Limits: these direct inputs only. | Verified |
| Mcd.MatVal..ctor(class:532, i4) | 0x06000493 | Alias source arrays for one column, Flags.StringTag, source dimension, source/result Shared; trim physical pointer keys. | Direct case: private column-view constructor trims pointer entries. Aliasing and source/result state rendered. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.TrimPtrs(class:201) | 0x06000494 | Copy only live PtrNan entries in view logical extent, keeping physical backing keys; missing source entries remain null. | Direct case: TrimPtrs copies only live pointer NaNs; receiver state and pointer table rendered. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.mcd_sym_column(object, i4) | 0x06000495 | Require MatVal and in-range column; raise must be array / index out of bounds without argument position, then Column. | Direct cases: successful column, invalid type, and indices -1 and 2. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.mcd_column(object, i4) | 0x06000496 | Require MatVal and in-range column; argument positions 1 for type and 2 for bounds, then Column. | Direct cases: successful column, invalid type, and indices -1 and 2. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Column(i4) | 0x06000497 | Evaluate IsUniformScalar and Immutable without short-circuit. Uniform immutable source returns aliased column view, marking source and result Shared; otherwise pack arrays/table and canonicalize at source dimension. Raw method has no explicit bounds check. | Direct cases: mutable mixed storage, mutable uniform storage, mixed immutable storage, raw out-of-range, and immutable uniform scalar alias. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.real_column(i4) | 0x06000498 | Uniform scalar path packs real column; mixed path calls mixed-real accessor row by row. | Direct cases: mixed rows and uniform-scalar rows. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.mcd_sym_row(object, i4) | 0x06000499 | Require MatVal and in-range row; raise must be array / index out of bounds without argument position, then Row. | Direct cases: successful row, invalid type, and indices -1 and 2. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.mcd_row(object, i4) | 0x0600049a | Require MatVal and in-range row; argument positions 1 for type and 2 for bounds, then Row. | Direct cases: successful row, invalid type, and indices -1 and 2. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Row(i4) | 0x0600049b | Use half-open submatrix for one row and all columns; source becomes Immutable. | Direct case: row delegates to half-open submatrix. Limits: these direct inputs only. | Verified |
| Mcd.MatVal..ctor(class:532, i4, i4, i4, i4) | 0x0600049c | Alias half-open source submatrix arrays with source stride and selected offset; Flags.StringTag and Shared, preserve default dimension; transform pointer table. | Direct case: private half-open submatrix constructor; aliased arrays, shape, offset and pointer state rendered. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.SubMatrix(u4, u4, u4, u4) | 0x0600049d | Require nonempty half-open bounds within logical shape; otherwise index out of bounds argument 1. Mark source Immutable, then return aliased arrays with original stride, selected offset and Shared/StringTag flags; trim pointer table through coordinate setter. | Direct cases: aliased valid submatrix, empty bounds, and four invalid bound sets. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.get_elements(u4, u4) | 0x060004c4 | Return newly allocated object array for requested top-left shape, column-major; no explicit bounds check. Parameterless overload supplies logical shape. | Direct case: requested top-left prefix. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.get_elements() | 0x060004c5 | Return newly allocated object array for requested top-left shape, column-major; no explicit bounds check. Parameterless overload supplies logical shape. | Direct case: logical shape. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Transpose() | 0x06000523 | Allocate packed transposed arrays, preserve default dimension, transform live pointer coordinates. | Direct case: packed transpose with pointer mapping and receiver state. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.asImmutable() | 0x06000528 | Set Immutable and return input. | Direct case: sets Immutable and returns self; identity and backing aliases rendered. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Fix() | 0x06000529 | Set Immutable and return input; object overload returns null for nonmatrices. | Direct case: sets Immutable and returns self; identity and backing aliases rendered. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.Fix(object) | 0x0600052a | Set Immutable and return input; object overload returns null for nonmatrices. | Direct cases: matrix, null, and nonmatrix; return identity rendered. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.asResult() | 0x0600052b | Pack-copy only unpacked input, canonicalize chosen result, fix null default dimension, set Immutable; packed input can mutate and is returned. | Direct cases: pack unpacked matrix, retain packed matrix, and fix null dimension; return identity and backing aliases rendered. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.FixForExport(object) | 0x0600052c | Wrap plain strings; other nondimensional objects unchanged. Fix null default units on DimVal; matrices only Fix without packing/canonicalizing, other DimVal use asResult. | Direct cases: string, plain object, null-dimension matrix, dimensioned matrix, null, dimensionless dimensional value, and dimensioned value; matrix identity and backing aliases rendered. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.TransposeArr(r8[]) | 0x06000533 | Transpose only logical extent using source offset/allocated stride, retaining zero imaginary storage if present. | Direct case: logical rows/columns with nonzero offset and padded stride. Limits: these direct inputs only. Structure mismatch recorded; see reconciliation below. | Verified |
| Mcd.MatVal.TransposeReal() | 0x06000548 | Transpose real backing. | Direct case: transposes real backing. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.TransposeImag() | 0x06000549 | Return null only if imaginary backing absent; otherwise transpose it without zero suppression. | Direct cases: transposes present imaginary backing and preserves absent backing. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.SubMatrixPtrs(class:532, i4, i4, i4, i4) | 0x0600054e | Transform source live pointers using TrimSubMatrixPtrs callback. | Direct case: private pointer-coordinate transform over selected source coordinates. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.set_ptrs_2_transpose(i4, i4, object) | 0x06000551 | Pointer setter at (column,row), including nested immutability effects. | Direct case: pointer setter maps row to column. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.TransposePtrs(class:532) | 0x06000552 | Transform live source pointer coordinates through transpose callback. | Direct case: pointer source mapped through transpose. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.set_ptrs_2_reverse(i4, i4, object) | 0x06000553 | Pointer setter at (Rows-row,column); the original uses Rows-row, even for row zero. | Direct case: reverse pointer setter uses original Rows minus row. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.ReversePtrs(class:532) | 0x06000554 | Transform live source pointers using original Rows-row coordinate (no subtraction of one). May throw or write into padding. | Direct case: source pointers use original row coordinate. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.TrimSubMatrixPtrs..ctor(class:532, i4, i4, i4, i4) | 0x0600073c | Store destination and four half-open bounds. | Direct case: constructor stores destination and bounds. Limits: these direct inputs only. | Verified |
| Mcd.MatVal.TrimSubMatrixPtrs.set_submatrix_ptrs(i4, i4, object) | 0x0600073d | Ignore out-of-range source coordinates; pointer-write selected values at coordinates shifted by start bounds. | Direct cases: in-range shifted write and out-of-range ignored. Limits: these direct inputs only. | Verified |

## Test cases

Direct comparisons in `MatValViewDifferentialTests.Direct_views_and_private_helpers` invoke 34 selected view, transpose, tuple, equality, formatting, export, constructor and pointer-callback members on both artifacts. Constructor cases include the private column-view, private half-open submatrix and nested pointer-trim callback constructors. Instance renderers compare shape, stride, offset, flags, dimension, backing arrays and pointer tables, including target mutation and alias state. Conclusions are limited to the inputs and branches exercised in the current focused validation run.

## Structure mismatches observed in the focused run

The following IL structure differences remain between the implementations. The direct cases above compare each method’s result and relevant state, including mutations and backing aliases; these differences are recorded as reconstruction limits rather than treated as equivalent IL. The first difference is listed for each method.

| Member | First structure difference |
|---|---|
| Mcd.MatVal..ctor(class:532, i4) | call #4: Quire direct call to `set_Shared`; original `callvirt set_Shared`. |
| Mcd.MatVal.TrimPtrs(class:201) | call #0: Quire `Index`; original `get_Rows`. Field #0: Quire `m_rarr`; original `m_offset`. |
| Mcd.MatVal.SubMatrix(u4, u4, u4, u4) | call #2: Quire `AppException.Raise`; original `set_Immutable`. |
| Mcd.MatVal.TransposeArr(r8[]) | call #2: Quire `get_Cols`; original `get_Anr`. |
| Mcd.MatVal.Equals(object) | call #0: Quire `callvirt Object.GetType`; original `call Object.GetType`. Field #0: original accesses `MatVal.Data`. |
| Mcd.MatVal.nearly_equal(class:584, r8) | call #0: Quire `get_Rows`; original constructs `NumVal`. Field #0: original accesses `MatVal.Data`. |
| Mcd.MatVal.ToString() | call #0: Quire interpolated-string handler constructor; original `get_Rows`. Other differences: Quire has six conditional branches vs original one, Quire has no switch vs original four switch targets, Quire uses constant 3 vs original 2, and original allocates `String[]`. |
| Mcd.MatVal.Fix(object) | call #0: Quire direct `Fix`; original `callvirt Fix`. |
| Mcd.MatVal.FixForExport(object) | call #2: Quire `DimVal.asResult`; original `MatVal.Fix`. |

## Nested fields

| Member | Token | Contract | Status |
|---|---|---|---|
| TrimSubMatrixPtrs.m_m | 0x04000177 | Private mutable destination reference or half-open bound; initialized by its constructor. | Analyzed |
| TrimSubMatrixPtrs.m_r1 | 0x04000178 | Private mutable destination reference or half-open bound; initialized by its constructor. | Analyzed |
| TrimSubMatrixPtrs.m_r2 | 0x04000179 | Private mutable destination reference or half-open bound; initialized by its constructor. | Analyzed |
| TrimSubMatrixPtrs.m_c1 | 0x0400017a | Private mutable destination reference or half-open bound; initialized by its constructor. | Analyzed |
| TrimSubMatrixPtrs.m_c2 | 0x0400017b | Private mutable destination reference or half-open bound; initialized by its constructor. | Analyzed |
