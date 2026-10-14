# Mcd.MatVal growth and assignment

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 (tools/reference); REA `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members).
- **Quire:** `src/Quire.Mpl/Mcd/MatVal.Growth.cs`; tests: `MatValGrowthTests`, `MatValGrowthDifferentialTests`.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Mcd.MatVal..ctor(class:532,i4,i4)` | 0x06000491 | Copy enlarged logical numeric storage, dimension and live pointers; initialize fresh flags and offset. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.realloc(object,i4,i4)` | 0x06000492 | Clamp indices to zero. Nonmatrix input yields zero logical rows and requested column/row capacity. Matrix input is copied with enlarged capacity, then logical shape restored and newly allocated unit zeros tidied. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.extend_1(i4)` | 0x060004f3 | Reject negative index (argument 2) and multicolumn input (argument 1). Immutable matrix returns enlarged copy; otherwise stretch in place. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.mcd_setm_1(object,i4,object)` | 0x060004f4 | Dispatch to DimVal.matrix_set_1 for DimVal values and matrix_set_1_ob for other values; null/non-DimVal destination naturally fails. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_set_1(i4,object)` | 0x060004f5 | Validate index and vector shape; immutable input copies before writing, mutable input stretches and writes. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.mcd_upd_1(i4,object)` | 0x060004f6 | Validate index, stretch and write in place even when immutable; double overload writes with identity dimension. Does not reject multiple columns. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_1(i4,r8)` | 0x060004f7 | Validate index, stretch and write in place even when immutable; double overload writes with identity dimension. Does not reject multiple columns. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.mcd_setm_2(object,i4,i4,object)` | 0x060004f8 | Dispatch to DimVal.matrix_set_2 for DimVal values and matrix_set_2_ob for other values; null/non-DimVal destination naturally fails. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.extend_2(i4,i4)` | 0x060004f9 | Reject negative row/column (arguments 2/3). Immutable matrix returns enlarged copy; otherwise stretch in place. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.mcd_set_2(i4,i4,object)` | 0x060004fa | Validate both indices; immutable input copies before writing, mutable input stretches and writes. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.mcd_upd_2(i4,i4,object)` | 0x060004fb | Validate both indices, stretch and write in place even when immutable; double overload writes with identity dimension. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_2(i4,i4,r8)` | 0x060004fc | Validate both indices, stretch and write in place even when immutable; double overload writes with identity dimension. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setcolumn(object,i4,object)` | 0x060004fd | Reject negative index. Convert nonmatrix value with Singleton. Create matrix for nonmatrix destination, copy immutable destination, otherwise mutate; setcolumn checks vector shape. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.setcolumn(i4,object)` | 0x060004fe | Convert scalar to Singleton, require one source column (argument 2), stretch destination and copy column. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.setcol(i4,class:532)` | 0x060004ff | Adopt source dimension if destination dimension null; conflicting nonnull dimension assigns via objects. Otherwise copy real/imaginary rows and live source pointers. Does not remove stale pointer table entries. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setrow(object,i4,object)` | 0x06000500 | Reject negative index. Convert nonmatrix value with Singleton. Create matrix for nonmatrix destination, copy immutable destination, otherwise mutate; row assignment accepts blocks. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.setrow(i4,object)` | 0x06000501 | Delegate to setblock at column zero. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setblock(i4,i4,object)` | 0x06000502 | Grow to contain scalar or full source rectangle, then assign scalar or matrix without immutable check. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.setmatrix(i4,i4,class:532)` | 0x06000503 | Adopt null destination dimension, otherwise use object element assignment on dimension mismatch. Copy each source column and imaginary zeros; assign source live pointers afterward. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setmatrix_ptrs(i4,i4,class:532)` | 0x06000504 | Read each source element and assign at translated row/column positions. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAlloc(object,i4)` | 0x06000505 | Delegate matrices to ReAllocate; otherwise create requested zero matrix. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAlloc(object,i4,i4)` | 0x06000506 | Delegate matrices to ReAllocate; otherwise create requested zero matrix. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAllocate(i4)` | 0x06000507 | Return independent enlarged storage with maxima of requested and current shape; preserve dimension and live pointer mapping. Never shrink. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAllocate(i4,i4)` | 0x06000508 | Return independent enlarged storage with maxima of requested and current shape; preserve dimension and live pointer mapping. Never shrink. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Stretch(i4)` | 0x0600053a | Mutate logical shape and grow capacity as needed. Vector capacity doubles; matrix row capacity doubles subject to 16777216 element threshold. Column growth may exactly allocate requested rows. Remap pointer keys when matrix stride changes. Existing offset remains unchanged. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.TidyZeros(i4,i4,i4,i4)` | 0x0600053b | Only live-pointer representations are tidied. Newly exposed numeric zero cells become identity-dimension zero pointers for a nonidentity matrix dimension; null dimension becomes identity. Existing cells are excluded. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Stretch(i4,i4)` | 0x0600053c | Mutate logical shape and grow capacity as needed. Vector capacity doubles; matrix row capacity doubles subject to 16777216 element threshold. Column growth may exactly allocate requested rows. Remap pointer keys when matrix stride changes. Existing offset remains unchanged. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.StretchWithColumnFill(i4,i4,class:584)` | 0x0600053d | Grow shape, then fill new columns only over the original row count. | Observed direct Mathcad cases listed below; equivalence is limited to those inputs and mutations. | Analyzed |
| `Mcd.MatVal.Resize(r8[],i4)` | 0x0600053e | Allocate zero numeric storage and copy logical source rows by old stride; equal strides copy padding too. Vector overload copies Rows starting at Offset. Offset is not reset by callers. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.CopyResize(r8[],i4,i4)` | 0x0600053f | Resize with requested shape bounded below by source shape. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Resize(r8[],i4,i4)` | 0x06000540 | Allocate zero numeric storage and copy logical source rows by old stride; equal strides copy padding too. Vector overload copies Rows starting at Offset. Offset is not reset by callers. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.AssignPtrs(class:532,i4,i4)` | 0x0600054f | Transform live source pointers through translated row/column callback. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ResizePtrs(i4,i4)` | 0x06000550 | Create table of live pointers keyed by specified stride and offset; return null when empty. | Static IL and decompilation; runtime cases pending | Analyzed |

| `MatVal.AssignMatrixPtrs..ctor(class:532,i4,i4)` | 0x0600073e | Store destination matrix and row/column translation. | Static IL/decompilation | Analyzed |
| `MatVal.AssignMatrixPtrs.assign_matrix_ptrs(i4,i4,object)` | 0x0600073f | Write pointer at translated coordinates using set_ptrs_2. | Static IL/decompilation | Analyzed |
| `MatVal.ResizeMatrixPtrs..ctor(class:201,i4,i4)` | 0x06000740 | Store pointer table, stride and offset. | Static IL/decompilation | Analyzed |
| `MatVal.ResizeMatrixPtrs.resize_matrix_ptrs(i4,i4,object)` | 0x06000741 | Write pointer table entry at offset + row + column * stride. | Static IL/decompilation | Analyzed |
| `MatVal.AssignMatrixPtrs.m_m` | 0x0400017c | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.AssignMatrixPtrs.m_r` | 0x0400017d | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.AssignMatrixPtrs.m_c` | 0x0400017e | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_parr` | 0x0400017f | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_anr` | 0x04000180 | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_off` | 0x04000181 | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |

## Test cases

Observed direct comparisons in `MatValGrowthDifferentialTests` cover vector growth, immutable/shared copy-on-write, `mcd_upd_1(int,double)` mutation on an immutable matrix, row and column growth, matrix and null-input `realloc`, and `extend_2` mutable growth, zero-based column, both negative-index branches and immutable-copy behavior across the named dimensions. Assignment cases compare `setblock`, `setcolumn`, `mcd_setrow`, immutable scalar `mcd_setcolumn`, and `StretchWithColumnFill`; `mcd_set_2` cases include zero-based column, growth, negative first and second indices, and immutable copy; other errors cover multicolumn `extend_1`, negative `mcd_set_1`, and a multicolumn `setcolumn` source. State renderers include shape, stride, offset, flags, dimension, real/imaginary backing and pointer entries. The observations establish only these inputs and mutations.

## Open

Direct cases pass for the listed inputs, but reconstruction integrity leaves these first structural differences unresolved:

- `realloc(object,int,int)` (original token `0x06000492`): calls differ at #4 (`Quire get_Rows` vs `Mathcad TidyZeros`); field accesses differ at #1 (`Quire m_rows` vs `Mathcad m_cols`).
- `extend_1(int)` (`0x060004f3`): calls differ at #1 (`Quire get_Cols` vs `Mathcad AppException.Raise`); field accesses differ at #0 (`Quire end` vs `Mathcad ldflda MatVal.Data`).
- `mcd_set_1(int,object)` (`0x060004f5`): same first call and field differences as `extend_1`.
- `mcd_upd_1(int,double)` (`0x060004f7`): calls differ at #2 (`Quire get_Offset` vs `Mathcad set(int,double,Dim)`); fields differ at #0 (`Quire ldsfld Dim.identity` vs `Mathcad ldfld MatVal.m_offset`).
- `mcd_setcolumn(object,int,object)` (`0x060004fd`): calls differ at #4 (`Quire call setcolumn` vs `Mathcad callvirt setcolumn`).
- `setcolumn(int,object)` (`0x060004fe`): calls differ at #1 (`Quire get_IsVector` vs `Mathcad get_Rows`).
- `mcd_setrow(object,int,object)` (`0x06000500`): calls differ at #4 (`Quire call setrow` vs `Mathcad callvirt setrow`).
- `setblock(int,int,object)` (`0x06000502`): calls differ at #0 (`Quire get_Rows` vs `Mathcad Stretch`).
- `StretchWithColumnFill(int,int,DimVal)` (`0x0600053d`): calls differ at #0 (`Quire get_Rows` vs `Mathcad get_Cols`).

The mutation audit found that replacing the `j < 0` comparison with `j < 1` survived for both `extend_2(int,int)` and `mcd_set_2(int,int,object)`. The direct cases now include `j == 0` to distinguish these boundary mutations, as well as the negative-index and immutable-copy paths. `set_ptrs_2` is used only to construct pointer-bearing inputs; token `0x06000518` is not listed in this record, so it is not compared under this contract. The other methods in this record remain outside these cases.

Wrapper dispatch depends on DimVal matrix_set_1/2 and matrix_set_1/2_ob. Shared Singleton, FillArr, numeric and pointer setter implementations are validated with the combined batch. Nonzero offset growth intentionally retains the original offset behavior; bounded cases do not establish all malformed storage cases.
