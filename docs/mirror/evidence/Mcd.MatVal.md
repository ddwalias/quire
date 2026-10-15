# Mcd.MatVal — batch 2b foundations

- **Original:** mplrun.dll, Prime 12.0.1.0; SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`; MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members, 2026-10-14).
- **Decompiled view:** ILSpy 11.1.0.9782, run under Linux .NET 10 against the same installed file; raw view remains in /tmp.
- **Quire:** src/Quire.Mpl/Mcd/MatVal.cs; behavior tests MatValTests; direct scenarios MatValDifferentialTests.
- Scope is the exact rows below. Other methods, including nested MatDim implementations, remain open.

| Member (metadata parameter types) | Token | Normalized IL SHA-256 | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| get_Rows() | 0x0600042f | d08b2d90b428e395f7696c13c47a4655dd1856dde64c5d96811e426d35a68055 | Return logical row count. | Static observed; direct cases below only | Verified |
| get_Cols() | 0x06000430 | 826757426bdea790c07a11cd39b31c5ec57cdada08ed8ccdd848847fd4549a46 | Return logical column count. | Static observed; direct cases below only | Verified |
| get_Anr() | 0x06000431 | c643e9ac55cf1634eb2a038dee2d6cd4bfa0b1721b9bdabc68975bae748792e5 | Return allocated row stride. | Static observed; direct cases below only | Verified |
| get_Offset() | 0x06000432 | 121b2466849d876cdd4b4d0bc81ad05f6bda54ee116b3fe86121923232c0aa4d | Return storage offset. | Static observed; direct cases below only | Verified |
| get_Packed() | 0x06000433 | 072dfcb76af2c3a6073ea5606c4bc81ee7aa3580dc4a1c22628280af1be2db69 | Compare logical area with real backing length; null backing throws. | Static observed; direct cases below only | Verified |
| get_HasImag() | 0x06000436 | 297a49ecb2bc12266bb2b31c30b7fe0a8881e2b5b8e07d5f2d82487526cef578 | Scan entire imaginary backing, including padding. Any nonzero/NaN returns true; all zero replaces backing with null and returns false. | Static observed; direct cases below only | Verified |
| get_Immutable() | 0x06000460 | 1c322eed8f5942cb05bf0a205c796c12c0bfa26586ed28918499091f06b90373 | Test CopyOnWrite bit. | Static observed; direct cases below only | Verified |
| set_Immutable(bool) | 0x06000461 | 28e4de1be30cf84fad0ab38e56f8f3fde534481b6566aee733d48344c2d804a7 | Set/clear CopyOnWrite while preserving every other flag. | Static observed; direct cases below only | Verified |
| get_Shared() | 0x06000462 | f89b308ee455ba77f1f8e120f6d39b97e7707448683e685dd02685200dec292b | Test Shared bit. | Static observed; direct cases below only | Verified |
| set_Shared(bool) | 0x06000463 | 5fc149ea5cbb0aa13bbf64eea48535104cf1c9ec8559ddaa953a1e66dca8bd95 | Set/clear Shared while preserving every other flag. | Static observed; direct cases below only | Verified |
| GetHashCode() | 0x06000474 | 9c0154cb0bb34c3e248d50807040d05f7d9a1fcb5246e9940b345944df1c4194 | Return unchecked rows × 64 + cols. | Static observed; direct cases below only | Verified |
| .cctor() | 0x06000475 | 609a2e624f7d93b214db56f43bae597b6c03c31d62b7c84d737434d2a5f59db0 | Initialize NanMask to 2146959360 and PtrNan to the all-one-bit sentinel. | Static observed; direct cases below only | Verified |
| .ctor(i4, i4, bool, i4) | 0x06000476 | 7551e4072850bd0e9fe630eb9171498be4587fda578c19c9402f5bb2f95bb72b | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(i4) | 0x06000477 | 56cbdbd539a49ac6a5bed0b5412306352bed9851647d730d1bc007d37f2a466f | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(i4, i4) | 0x06000478 | 3d956c40fe1064e64b426666b3e7a0f7afce6621ad9c0a817004d41fcc3582e5 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(i4, i4, bool) | 0x06000479 | ecaf2b77c4417b521b088555be1c49848819c61b75532ed13ac9e3887840d86a | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[], i4, i4, i4, class:201, valuetype:512, i4) | 0x0600047a | cdcbce31faf50a01e4bb97b74617706702a5613cb4f85774ad45c676f24402fa | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[], i4, i4, i4, class:201) | 0x0600047b | ad38ef471f6afd6715c6f52af7280a87943f7bd4515674029b9d7be84efa9894 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[], i4, i4, i4) | 0x0600047c | afb23ff93f8ff392a13c4b4bec0fb54d29a6391c67e22bd8e05385fe3e50b729 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[], i4, i4) | 0x0600047d | 6805a27c0e8a4b7832c740a12284a3605bc94cfce4b1ce0221a9dfa2ce032f9f | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[], i4) | 0x0600047e | 37e5a30957db6af08eb7cb290f021fad5532386384198941563147634a6077c2 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[], r8[]) | 0x0600047f | c127e5a589ce34487138e847be815a10fd313e43708f4841b75d328c56200ca8 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor(r8[]) | 0x06000480 | 827bcf8b314115c5df88cd162cc7d6e9fcfd5c530e6dc5ba0914e0556a0f7bf8 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| .ctor() | 0x06000481 | 17b6e0f1990f7964af2bc4f02ee30d2de6186eec702fceab7f9cf2e86ccabd96 | Allocated overloads allocate cols × stride, preserve shape without validation, set MatrixTag and leave dimension null; wrapping overloads alias arrays/table without validation, set identity dimension and supplied flags/offset; default wraps null arrays and zero shape. | Static observed; direct cases below only | Verified |
| get_Isize() | 0x060004bc | d7a7eeddaf947da79672065501cfd4977d04d0cbfb0447372c694edfb1cbf3ac | Return logical area. | Static observed; direct cases below only | Verified |
| Index(i4) | 0x060004cc | 2c2d29f9788b9bdaa31bd0d1f0602b3aea5e4db896b6c7f3ec2eda463c161e83 | Single index adds offset; two indices add offset + row + stride × column; no bounds validation. | Static observed; direct cases below only | Verified |
| Index(i4, i4) | 0x060004cd | bc4c982fbd5fb30ac4f09727952e2056f3a90e2e38de13eb09f755232b567766 | Single index adds offset; two indices add offset + row + stride × column; no bounds validation. | Static observed; direct cases below only | Verified |
| RowOf(i4) | 0x060004ce | b42e9af81177c14e833f722c7aeee4af559250b3a729b3b62df04a38f9573b29 | Remainder of (index − offset) divided by allocated stride; zero stride throws. | Static observed; direct cases below only | Verified |
| ColOf(i4) | 0x060004cf | 42f99bb01a4878e3e5a883494b8ddd6c22ed574dc5e011116ff1e96ee98347e6 | Integer division of (index − offset) by allocated stride; zero stride throws. | Static observed; direct cases below only | Verified |
| IsPtrNan(r8&) | 0x0600052d | a62b749506c591b9cd7439e3c059ae4b05df9f437d5f290f140a91a41d99d5ac | By-reference overload tests upper 32 bits equal 0xffffffff without changing input; private two-index overload tests indexed real backing. | Static observed; direct cases below only | Verified |
| ThePtrNan() | 0x0600052e | bc5d51b552b5ac23d06e569ccbfc1511bd060b45acbabba89c7569718ab9b433 | Return double with all 64 bits set. | Static observed; direct cases below only | Verified |
| IsPtrNan(i4, i4) | 0x0600052f | 36abb07aca75a638661364cc8d75713b0e53196acd75b7f89472e32566310b83 | By-reference overload tests upper 32 bits equal 0xffffffff without changing input; private two-index overload tests indexed real backing. | Static observed; direct cases below only | Tested |
| AllocImaginary() | 0x06000530 | ab164ac11450fc305fd20ab7bf9386dc8b5cea91bb534259564d201ff54c94c8 | Replace imaginary backing with newly allocated zero array of real backing length; null real backing throws. | Static observed; direct cases below only | Verified |
| Canonicalize() | 0x0600043b | — | If the pointer table is nonnull, call `CanonicalizePtrs`; always return this matrix. Canonicalization may change the matrix dimension, pointer table, backing arrays, stride, offset, Shared flag, pointer keys, and pointed-to zero dimensions. | Same-artifact ILSpy/IL; 10 direct cases; finite cases in Lane B checkpoint | Verified |
| TidyPtrs(int32, int32, Mcd.Dim, bool) | 0x06000442 | — | With zero compatible pointers, retain the table and return false when another live pointer is incompatible; otherwise clear it and return true. For full logical coverage, adopt a nonnull common dimension and reinsert pointers, using zero-aware reinsertion only when every pointer is zero. For partial coverage, require matrix/common-dimension compatibility, merge dimensions, and reinsert; otherwise return false without reinsertion. Reinsertion may repack shared storage and mutate zero-pointer dimensions. | Same-artifact IL/parameters; 15 direct comparisons; internal count arguments are explicit | Verified |
| IsUniformPtrs() | 0x06000458 | — | Inspect only entries whose real slot has the pointer-NaN marker; merge compatible dimensions, allow `mcd_zero` values, return false on other incompatible dimensions, then pass the live count and common dimension to `TidyPtrs`. May canonicalize pointer storage and dimensions. | Same-artifact IL; 15 direct cases; finite cases in Lane B checkpoint | Verified |
| IsUniformPtrsNoChange() | 0x06000459 | — | Return true for a null table; ignore stale entries; for each live pointer return false when its dimension is nonscalar or differs from the matrix dimension and is not special. Does not change matrix or pointer state. | Same-artifact IL; 6 direct cases including nonscalar nested-matrix dimensions; finite cases in Lane B checkpoint | Verified |
| get_HasPtrs() | 0x06000434 | — | Return false for a null pointer table; otherwise return the negation of IsUniformScalar, which may inspect and canonicalize live pointers. | Same-artifact ILSpy; 9 direct comparisons; finite cases in lane checkpoint | Verified |
| get_RepHasPtrs() | 0x06000435 | — | Return false for a null table; return true for any PtrNan-marked live offset; if none are live, clear the table, set it to null, and return false. | Same-artifact ILSpy; 4 direct comparisons; finite cases in lane checkpoint | Verified |
| get_IsUniformScalar() | 0x0600043e | — | Return true for a null table; otherwise delegate to IsUniformScalarPtrs, which may canonicalize pointer storage and dimensions. | Same-artifact ILSpy; 8 direct comparisons; finite cases in lane checkpoint | Verified |
| IsUniformScalarPtrs() | 0x0600043f | — | Inspect only PtrNan-marked entries; return false for a live non-NumVal or incompatible nonspecial dimension, accept mcd_zero dimensions, and pass the live count/common dimension to TidyPtrs. May mutate matrix and pointer state. | Same-artifact ILSpy; 9 direct comparisons; finite cases in lane checkpoint | Verified |
| get_IsUniform() | 0x06000457 | — | Return true for a null table; otherwise delegate to IsUniformPtrs, which may canonicalize pointer storage and dimensions. | Same-artifact ILSpy; 10 direct comparisons; finite cases in lane checkpoint | Verified |
| IsUniformDimOf(Mcd.Dim) | 0x06000437 | — | Inspect only live PtrNan entries and compare their values against the current matrix dimension; if live count equals logical area, assign the exact requested dimension reference and return true, including null and zero-area requests; otherwise fall back to equality with the current matrix dimension. | Same-artifact ILSpy; 10 direct comparisons; finite cases in lane checkpoint | Verified |
| get_IsDimless() | 0x0600043a | — | First require IsUniformScalar; return false without checking m_dim when that fails; otherwise return Dim.Id_b(m_dim). | Same-artifact ILSpy; 7 direct comparisons; finite cases in lane checkpoint | Verified |
| get_PhysicalDimension() | 0x06000452 | — | Return the virtual UniformDimension property result. | Same-artifact ILSpy; 6 direct comparisons; finite cases in lane checkpoint | Verified |
| get_UniformDimension() | 0x06000453 | — | Return mixed_matrix_dim when IsUniform is false; otherwise return m_dim. May canonicalize pointer storage through IsUniform. | Same-artifact ILSpy; 8 direct comparisons; finite cases in lane checkpoint | Verified |
| CanonicalizePtrs() | 0x06000441 | — | Normalize `string_dim` to identity, seed the common dimension from `m_dim` only when logical cells include non-pointer data, then classify only PtrNan-marked table entries. Merge compatible NumVal dimensions, accept `mcd_zero`, flag other live values/dimensions, and pass live/zero counts plus the common dimension to `TidyPtrs`; may mutate matrix and pointer state through reinsertion. | Same-artifact IL; 18 direct comparisons | Verified |
| ReinsertPtrs() | 0x06000443 | — | Save the original pointer table/backing and view geometry; if Shared, pack storage and reset offset/stride/Shared; then detach the table, skip stale entries, remap live indices when packed, and reinsert each original object through `set`. | Same-artifact IL; 11 direct comparisons | Verified |
| ReinsertPtrs1() | 0x06000444 | — | Perform the same shared repack and live-entry remapping as `ReinsertPtrs`, but first mutate each live `NumVal` with `mcd_zero` dimension to `identity` before reinsertion. | Same-artifact IL; 12 direct comparisons | Verified |

The static initializer is explicit (the original type is not beforefieldinit). Fields marked Verified below are compared by the constructor/storage scenarios.

## Fields

| Owner / field | Token | Contract | Status |
|---|---|---|---|
| Mcd.mat_val.m_anr | 0x040000e8 | Storage/shape field; preserve original declaration and constructor assignment. | Verified |
| Mcd.mat_val.m_flags | 0x040000e9 | Storage/shape field; preserve original declaration and constructor assignment. | Verified |
| Mcd.mat_val.m_rows | 0x040000ea | Storage/shape field; preserve original declaration and constructor assignment. | Verified |
| Mcd.mat_val.m_cols | 0x040000eb | Storage/shape field; preserve original declaration and constructor assignment. | Verified |
| Mcd.mat_val.m_rv | 0x040000ec | Native backing pointers; allocation/pinning lifecycle not implemented. | Analyzed |
| Mcd.mat_val.m_iv | 0x040000ed | Native backing pointers; allocation/pinning lifecycle not implemented. | Analyzed |
| Mcd.MatVal.PtrNan | 0x040000ee | Mutable static sentinel; all bits one. | Verified |
| Mcd.MatVal.NanMask | 0x040000ef | Mutable static mask initialized to 2146959360. | Verified |

## Test cases and open work

MatValTests: 4/4 passed in process. MatValDifferentialTests: 2/2 passed against the installed assembly in process (0 skipped), covering 16 constructor scenarios, negative/short/inconsistent storage without validation, shape/hash/index queries (−1,0,1,7), zero-stride errors, flag set/clear, padding NaN/zero imaginary arrays, and four NaN/finite payload patterns. Wrapping alias behavior and private IsPtrNan(i,j) also have independent assertions. No whole-type equivalence is claimed. Pointer fields are declaration-only analysis; their native lifecycle remains open. The manifest review also checked earlier pending changes: NumVal.Zero (0x040000fb) and One (0x040000fe) have field flags 22 (public static, mutable), and the complete REA Mcd.RngVal method inventory contains no declared ToString override. The three previous manifest lines were removed/replaced on that structural evidence; scalar/range behavior is not certified here.

Matrix pointer canonicalization, access/mutation, dimension factories, nested dimensions, arithmetic, copy-on-write, and native lifecycle remain outside this foundation contract.

## Access / pointer representation contracts (implementation in progress)

Same-artifact REA inventory and ILSpy view establish these contracts before implementation:

- Raw access uses backing offset/allocated stride without logical bounds checks. One-index access uses the raw
  offset, not row-major or logical flattening. get returns a live pointer value only when the slot has PtrNan;
  missing/null pointer entries normalize that slot to ordinary NaN. get_dim/get_matdim dereference pointer entries
  without repairing them. get_na mutates and returns its supplied number on numeric slots, returns the pointer
  on pointer slots. Raw real/imag access reads arrays directly; mixed-real access raises the original message
  and handle when a live pointer is not a NumVal.
- Numeric object setters adopt a non-special dimension when the matrix is polymorphic; a differing actual
  dimension becomes a pointer. Strings are wrapped as StrVal. Nested matrices become Immutable, and a value
  containing the destination is copied first. Pointer writes preserve imaginary backing. Raw-double setters
  overwrite real backing only: stale pointers/imaginary entries remain. Dimension-aware scalar setters clear
  imaginary backing; zero/NaN with mcd_zero has special pointer-preservation rules. The complex overload does
  not apply that zero rule. setspec preserves any explicit differing dimension, including special dimensions.
- HasPtrs/IsUniform/IsUniformScalar may canonicalize representation and dimensions. Only PtrNan-marked table
  entries participate. RepHasPtrs discards a table with no live pointers. Scalar/real checks inspect live pointer
  types; mixed-real checks pointer imaginary parts and the full imaginary backing. IsVector means one column.
- Canonicalization merges compatible dimensions and polymorphic zeros, preserves incompatible/non-numeric
  pointers, and discards stale entries. TidyPtrs distinguishes no pointers, all logical entries pointer-backed,
  and partial pointer coverage. Reinsertion on Shared matrices first packs storage, resets offset/stride and
  Shared, remaps pointer indices, and may mutate mcd_zero NumVal pointers to identity. Reinsertion uses the
  original setter semantics and may leave incompatible pointers in the new table.
- Pack helpers allocate packed copies, omit all-zero imaginary copies, and preserve pointer NaN payloads.
  Rectangular packing intentionally counts columns c2−c1 but starts its numeric copy at column zero;
  pointer packing uses the actual c1. The original Copy skips n<=0, otherwise pins destination and calls Marshal.Copy.
  Copy constructor clears flags to MatrixTag, packs storage, preserves the shared dimension, and shallow-copies
  live pointer objects/table (nested pointed-to values are not recursively copied).
- DimVal.MatrixDimension builds a 1×1 uniform MatDim from m_dim; MatDimOf dereferences the value and delegates.
  These dependency contracts are bound to the same REA artifact and will be recorded separately.

## Indexed access, axis enumeration and scalar arithmetic contracts

- mcd_get validates logical shape, unlike raw get. Vector access requires exactly one column, reports must be
  vector on argument 1, then out-of-bounds on argument 2. Matrix access reports argument 2 for either axis.
  Symbolic variants use argument 0. Signed overloads cast to uint; DynCall converts indices through asInt with
  arguments 1/2. iterator_get maps n by logical Rows and then calls checked matrix access (zero rows divide by zero).
- Axis offsets enumerate the chosen row across columns or chosen column across rows. Element classification
  tests pointer StrVal/NumVal first, numeric NaN before imaginary threshold, then actual units; a present pointer
  table with a null/missing entry classifies Other. An orphan marker without any table is replaced with NaN.
  A NaN threshold requires an exactly zero imaginary part; a finite threshold uses strict absolute < threshold.
- Axis double enumeration reads raw backing divided by scale only when imaginary parts meet threshold;
  pointer data enumeration returns pointed-to objects without scaling. Missing-table markers become raw NaN.
- Matrix division mutates a Packed, mutable matrix; otherwise it copies first. Scale scans the full real backing
  in descending order, dividing live pointer values through Utilities.Divide when not uniform-scalar; numeric
  backing and all imaginary backing are divided in place. Empty real backing errors, matching ScaleVector.
- Utilities.Divide returns input unchanged for scalar 1, rejects scalar 0 before value-type dispatch, divides
  NumVal and MatVal otherwise, and preserves other objects. Multiply delegates using reciprocal (including
  infinity/zero/NaN consequences). NumVal scalar division is already implemented and is a same-artifact dependency.

DimVal matrix assignment dependencies preserve original ownership: checked extend allocates a fresh matrix for
non-matrix values, rejects negative indices on arguments 2/3; unchecked extend variants allocate without those
checks. Object-typed matrix_set casts to DimVal and delegates (non-values therefore fail), while *_ob accepts
any pointer. Value-typed assignment first calls asImmutable on the incoming value, then extends the destination
and uses the original setters. Numeric overloads use identity dimensions; zero and imaginary-only overloads
retain their raw-write semantics.

## Integration checkpoint — 2026-10-14

All original MatVal and MatDim generated method stubs have been replaced in this batch, including additional
partial files and the four subsection evidence records. New contracts retain Analyzed classifications until a
successor completes the exact per-member contract/status review; passing whole-class test groups does not imply
unbounded equivalence or automatically promote every private member.

Combined independent in-process run: 123 passed, 0 failed, 0 skipped. Focused direct comparison run: 24 passed,
0 failed, 0 skipped (21 matrix/function scenarios plus 3 pre-existing UnitSystem scenarios). The root core and
remaining MatDim methods need their exact token/hash rows appended from the retained REA inventory; child
records already contain their selected rows. This is the next evidence bookkeeping task, not missing code.

Three old Quire-only helpers were removed after their callers were replaced with original owning members:
Check(int,int), CopyElement(MatVal,int,int,int,int), Like(int,int). Complete same-artifact REA MatVal method
inventory has no methods with these names. Removing their old manifest rows is intentional; no original
member was removed. Native pointer default/layout checks now pass, but their statuses have not been promoted.
