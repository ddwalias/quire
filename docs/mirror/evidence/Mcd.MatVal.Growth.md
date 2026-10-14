# Mcd.MatVal growth and assignment

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **REA:** `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8` (inspect_managed_members); same-artifact ILSpy decompilation retained outside repository at `/tmp/quire-matval-decompiled.cs`.
- **Quire:** `src/Quire.Mpl/Mcd/MatVal.Growth.cs`; tests: `MatValGrowthTests`, `MatValGrowthDifferentialTests`.

| Member/signature | Token | Normalized IL prefix | Contract | Confidence / limits | Status |
|---|---|---|---|---|---|
| `Mcd.MatVal..ctor(class:532,i4,i4)` | 0x06000491 | 0d7fbdfc4c64855b | Copy enlarged logical numeric storage, dimension and live pointers; initialize fresh flags and offset. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.realloc(object,i4,i4)` | 0x06000492 | 910c08ce8d1b0bcf | Clamp indices to zero. Nonmatrix input yields zero logical rows and requested column/row capacity. Matrix input is copied with enlarged capacity, then logical shape restored and newly allocated unit zeros tidied. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.extend_1(i4)` | 0x060004f3 | 27216af5c205714b | Reject negative index (argument 2) and multicolumn input (argument 1). Immutable matrix returns enlarged copy; otherwise stretch in place. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setm_1(object,i4,object)` | 0x060004f4 | 025b50ccc90fec31 | Dispatch to DimVal.matrix_set_1 for DimVal values and matrix_set_1_ob for other values; null/non-DimVal destination naturally fails. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_set_1(i4,object)` | 0x060004f5 | 0b7a968751e4f2a1 | Validate index and vector shape; immutable input copies before writing, mutable input stretches and writes. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_1(i4,object)` | 0x060004f6 | 06ecca2fd189906b | Validate index, stretch and write in place even when immutable; double overload writes with identity dimension. Does not reject multiple columns. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_1(i4,r8)` | 0x060004f7 | 70240016b5c7b1a4 | Validate index, stretch and write in place even when immutable; double overload writes with identity dimension. Does not reject multiple columns. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setm_2(object,i4,i4,object)` | 0x060004f8 | d4f66c2d963ddf5b | Dispatch to DimVal.matrix_set_2 for DimVal values and matrix_set_2_ob for other values; null/non-DimVal destination naturally fails. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.extend_2(i4,i4)` | 0x060004f9 | b2ec4690be5d0ca8 | Reject negative row/column (arguments 2/3). Immutable matrix returns enlarged copy; otherwise stretch in place. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_set_2(i4,i4,object)` | 0x060004fa | 888eef42ecb0399f | Validate both indices; immutable input copies before writing, mutable input stretches and writes. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_2(i4,i4,object)` | 0x060004fb | 8aeb1a8dc1ef2f0c | Validate both indices, stretch and write in place even when immutable; double overload writes with identity dimension. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_upd_2(i4,i4,r8)` | 0x060004fc | f51379778701d582 | Validate both indices, stretch and write in place even when immutable; double overload writes with identity dimension. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setcolumn(object,i4,object)` | 0x060004fd | b6d869fd85678518 | Reject negative index. Convert nonmatrix value with Singleton. Create matrix for nonmatrix destination, copy immutable destination, otherwise mutate; setcolumn checks vector shape. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setcolumn(i4,object)` | 0x060004fe | 7cc8c284890cf27b | Convert scalar to Singleton, require one source column (argument 2), stretch destination and copy column. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setcol(i4,class:532)` | 0x060004ff | 2720a575d1d2778a | Adopt source dimension if destination dimension null; conflicting nonnull dimension assigns via objects. Otherwise copy real/imaginary rows and live source pointers. Does not remove stale pointer table entries. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.mcd_setrow(object,i4,object)` | 0x06000500 | ec9dc30eaba4f458 | Reject negative index. Convert nonmatrix value with Singleton. Create matrix for nonmatrix destination, copy immutable destination, otherwise mutate; row assignment accepts blocks. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setrow(i4,object)` | 0x06000501 | 2597a9af2d8f99fb | Delegate to setblock at column zero. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setblock(i4,i4,object)` | 0x06000502 | 513a1fdf5173ea53 | Grow to contain scalar or full source rectangle, then assign scalar or matrix without immutable check. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setmatrix(i4,i4,class:532)` | 0x06000503 | d5e980d92e107a9e | Adopt null destination dimension, otherwise use object element assignment on dimension mismatch. Copy each source column and imaginary zeros; assign source live pointers afterward. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.setmatrix_ptrs(i4,i4,class:532)` | 0x06000504 | b467e75b1806362f | Read each source element and assign at translated row/column positions. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAlloc(object,i4)` | 0x06000505 | 60ea4ff749dd0a26 | Delegate matrices to ReAllocate; otherwise create requested zero matrix. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAlloc(object,i4,i4)` | 0x06000506 | 80ce23d33e8571de | Delegate matrices to ReAllocate; otherwise create requested zero matrix. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAllocate(i4)` | 0x06000507 | 9a24e4f979b4fc88 | Return independent enlarged storage with maxima of requested and current shape; preserve dimension and live pointer mapping. Never shrink. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ReAllocate(i4,i4)` | 0x06000508 | acba04f5ac8a0e5a | Return independent enlarged storage with maxima of requested and current shape; preserve dimension and live pointer mapping. Never shrink. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Stretch(i4)` | 0x0600053a | 7acfb0f07e180270 | Mutate logical shape and grow capacity as needed. Vector capacity doubles; matrix row capacity doubles subject to 16777216 element threshold. Column growth may exactly allocate requested rows. Remap pointer keys when matrix stride changes. Existing offset remains unchanged. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.TidyZeros(i4,i4,i4,i4)` | 0x0600053b | 51094a8711db0448 | Only live-pointer representations are tidied. Newly exposed numeric zero cells become identity-dimension zero pointers for a nonidentity matrix dimension; null dimension becomes identity. Existing cells are excluded. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Stretch(i4,i4)` | 0x0600053c | 9585b4125f1bb17a | Mutate logical shape and grow capacity as needed. Vector capacity doubles; matrix row capacity doubles subject to 16777216 element threshold. Column growth may exactly allocate requested rows. Remap pointer keys when matrix stride changes. Existing offset remains unchanged. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.StretchWithColumnFill(i4,i4,class:584)` | 0x0600053d | 7bc38115d25cd02d | Grow shape, then fill new columns only over the original row count. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Resize(r8[],i4)` | 0x0600053e | a62803a494a62606 | Allocate zero numeric storage and copy logical source rows by old stride; equal strides copy padding too. Vector overload copies Rows starting at Offset. Offset is not reset by callers. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.CopyResize(r8[],i4,i4)` | 0x0600053f | 5cbc3dbee58fa3fd | Resize with requested shape bounded below by source shape. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.Resize(r8[],i4,i4)` | 0x06000540 | 1a01bb1b821a6145 | Allocate zero numeric storage and copy logical source rows by old stride; equal strides copy padding too. Vector overload copies Rows starting at Offset. Offset is not reset by callers. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.AssignPtrs(class:532,i4,i4)` | 0x0600054f | 2f9a8e6f2e53a589 | Transform live source pointers through translated row/column callback. | Static IL and decompilation; runtime cases pending | Analyzed |
| `Mcd.MatVal.ResizePtrs(i4,i4)` | 0x06000550 | e0a64fefa99aa1e4 | Create table of live pointers keyed by specified stride and offset; return null when empty. | Static IL and decompilation; runtime cases pending | Analyzed |

| `MatVal.AssignMatrixPtrs..ctor(class:532,i4,i4)` | 0x0600073e | f5e0b57feeb03f26 | Store destination matrix and row/column translation. | Static IL/decompilation | Analyzed |
| `MatVal.AssignMatrixPtrs.assign_matrix_ptrs(i4,i4,object)` | 0x0600073f | 9517a3dcbed9eec6 | Write pointer at translated coordinates using set_ptrs_2. | Static IL/decompilation | Analyzed |
| `MatVal.ResizeMatrixPtrs..ctor(class:201,i4,i4)` | 0x06000740 | 75f6da523d089a29 | Store pointer table, stride and offset. | Static IL/decompilation | Analyzed |
| `MatVal.ResizeMatrixPtrs.resize_matrix_ptrs(i4,i4,object)` | 0x06000741 | edc85db3193be57f | Write pointer table entry at offset + row + column * stride. | Static IL/decompilation | Analyzed |
| `MatVal.AssignMatrixPtrs.m_m` | 0x0400017c | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.AssignMatrixPtrs.m_r` | 0x0400017d | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.AssignMatrixPtrs.m_c` | 0x0400017e | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_parr` | 0x0400017f | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_anr` | 0x04000180 | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |
| `MatVal.ResizeMatrixPtrs.m_off` | 0x04000181 | field | Private mutable callback context, initialized by its constructor. | Metadata and constructor IL | Analyzed |

## Test cases

Inferred regression cases cover doubling, matrix stride changes, copy on write, identity zero pointer fill, scalar/column/block assignment and argument positions. Direct comparison scenarios are supplied but not yet executed.

## Open

Wrapper dispatch depends on DimVal matrix_set_1/2 and matrix_set_1/2_ob. Shared Singleton, FillArr, numeric and pointer setter implementations are validated with the combined batch. Nonzero offset growth intentionally retains the original offset behavior; bounded tests do not establish all malformed storage cases.
