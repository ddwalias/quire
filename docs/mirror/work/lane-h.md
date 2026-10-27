# Lane H: efistatsci_.dll → Quire.Native.EfistatsciImpl

**Status: incomplete.** 375 of 781 exports pass the linked `/GL` gate (`native_linked.py`, D30): the 78 below and 79 from the consumer path, `cumulative_sum` 7, 24 unsigned bit operations, `realloc` 8, `abs` 4, `range` 7, 35 binary arithmetic exports, 14 scalar ones, `number_equal_scalar` 7 `sum_cols`/`sum_rows` 14, `entropy` 7, `rescale` 7, `translate` 6, `threshold` 4, `histogram` 4 12 signal windows `map_invert` 4, `matuniv_histogram` all `generate_rectangle`, `matdbl_translate` two `binary_dilate`, the four typed `grayscale_erode` `matu8_write_raw` the `rectangular_to_polar` and `point_test` pairs, `range_robust` 7, `pgndbl_shape_features`, `mutil_rand_set_seed` `mutil_rand_begin` `matcpx_to_double` the `moment` pair, `pgnuniv_shape_features` `tile`/`untile` 16, `matdbl`/`matflt_cast_to_u32`, the `rectangular_from_polar` pair `mutil_rand_uniform` and `matset_resize` (`transpose` 8, `flip_left_right` 8, `flip_up_down` 7, `assign` 7, `extract` 8, `number_equal` 7, `assign_zeropad` 7, `permute` 7, clamped `cast_to_*` 17, `cast_to_cpx` 2, `matcpx_cast_to_dbl`). No regressions in the linked run.

- **Owner:** Lane H (native, `efistatsci_.dll`)
- **Branch:** `mirror/h-efistatsci-20261027` (from `main` at `adee6ba0`; first batch landed as `75ba2925`)
- **Worktree:** `C:/Projects/Quire-wt/lane-h-efistatsci-2`
- **Harness:** `tools/validation/native_bytes.py` and `native_targets.py` on `main`, unmodified.
- **Build:** MSBuild `Quire.Native.EfistatsciImpl.vcxproj`, Release|x64, toolset v145, `cl` 19.51.36246 from `C:\BuildTools\VS18.6-msvc`, `/O2 /Gy /GS /EHa /MD /std:c++17 /W4 /WX` with `_CRTIMP2=__declspec(dllimport)` (D28), and `WholeProgramOptimization` true (`/GL` + LTCG). The original objects are `/GL` (D25); this build reproduces its entry code (`mov r10, rcx`), where the per-object build gives `mov r9, rcx`. Output in `.mirror-cache/bin/`.
- **Cast family:** the six `matu8_cast_to_*` exports and twelve widening casts from `u16`, `s16`, `u32`, `s32` and `float` (`mutils_cast.cpp`) match in the linked `/GL` DLL. They use the validate forms that fixed `assign_submat` (`src` fresh-field, `dst` cols-first).
- **Original disassembly:** `.mirror-cache/orig/efistatsci.asm` (dumpbin, ignored, D15).

## Completed groups

| Family | Exports | Strict result | Notes |
|---|---|---|---|
| `assign_scalar` | `matdbl`, `matflt`, `mats32`, `mats16`, `matu32`, `matu16`, `matu8`, `matcpx` | 8/8 OK | `assign_scalar<T>` in `mutils_mat.h`. `matcpx` has its own body with the same plain loop: through the template the value splits into two doubles (206 bytes), in the export it stays one 16-byte copy and the fill becomes the original's `rep movs` (219). |
| `transpose` | 8 typed (`dbl` … `u8`, `cpx`) | 8/8 OK | `mutils_transpose.cpp`. Dst's `rows`/`cols` in locals before the in-place test; the in-place case swaps a vector's dimensions or returns the mismatch message. |
| `flip_left_right`, `flip_up_down` | 8 + 7 typed | 15/15 OK | `mutils_flip.cpp`. Both inputs use the fresh-field check (`check_src_f`); per-row offset locals (`left`/`right`, `top`/`bottom`); `ceil(n * 0.5)` stays in the loop condition (hoisting it by hand changes the code). |
| `assign` | 7 typed (no `cpx` in the original) | 7/7 OK | `mutils_assign.cpp`. `memmove` into `int bytes`; `matu8` alone checks `dst` cols-first; `matdbl` needs `void *` casts, or the compiler emits `memcpy`. |
| `extract` | 8 typed | 8/8 OK | `mutils_extract.cpp`. Inclusive `last_row`/`last_col` bounds; the outer loop runs over columns with paired source and destination indices. |
| `number_equal` | 7 typed (no `cpx` in the original) | 7/7 OK | `mutils_number_equal.cpp`. `b` uses the cols-first check (`check_dst_f`); data and `size` in locals before the `count` null test, as in `sum`. Work `size * 3.0`. |
| `assign_zeropad` | 7 typed (no `cpx`) | 7/7 OK | `mutils_zeropad.cpp`. Calls the typed `assign_submat` export and ignores its result; `total`/`progress` declared before that call (they live in callee-saved registers); two separate row loops, the second starting at `src->rows`. Work `cols * 4`. |
| `permute` | 7 typed (index is `_sint32_mat`) | 7/7 OK | `mutils_permute.cpp`. Checks `src`, `dst`, `index` in that order (`dst`/`index` cols-first); an index outside `src->size` returns "Illegal data value". |
| Clamped `cast_to_*` | 19 of the 23 narrowing pairs with a `clamp` flag | 19/19 OK | `mutils_cast_clamp.cpp`. Float sources round with `floor(x + 0.5)`; low and high checks only where the source range exceeds the destination's; signed minimum is symmetric (-32767, -2147483647). Out of range: throw "overflow" unless `clamp`, then saturate. Work `size * 5.0`. |
| `cast_to_cpx`, `matcpx_cast_to_dbl` | 3 | 3/3 OK | `mutils_cast_cpx.cpp`. Work `size * 4.0`; `matcpx_cast_to_dbl` loops separately for `clamp` (real part) and not (nonzero imaginary throws "overflow"). |
| `matcpx_to_double` | cpx | 1/1 OK | In `mutils_cast_cpx.cpp`. Either output may be null (both null throws "null pointer"); two submat-style progress steps whose work `size + size` is recomputed, not accumulated. |
| `cumulative_sum` | 7 typed | 7/7 OK | `mutils_cumsum.cpp`. Calls the typed `assign` export, then `d[i] = d[i - 1] + d[i]` over `dst`; work `0.0 + size`. `src` uses `check_src_f`; `dst` uses `validate` for dbl/flt/s32/u32 and `check_src_f` for s16/u16/u8. |
| Bit operations | `bit_shift_left/right`, `bit_and/or/xor_scalar`, `bit_and/or/xor` for u8/u16/u32 | 24/24 OK | `mutils_bits.cpp`. Data and `dst->size` in locals before the shape test; the scalar has the element type; work `size * 3.0`. The binary forms check `a` with `check_src_f` and `b`/`dst` cols-first. This resolves the binary forms parked earlier. |
| `realloc` | 8 typed | 8/8 OK | `mutils_realloc.cpp`. `check_src_f`; new dims stored before the byte comparison; grows by `allocate_array` + `memcpy` of the new size. |
| `abs` | dbl, flt, s32, s16 | 4/4 OK | `mutils_abs.cpp`. Bit-operation shape, `x >= 0 ? x : -x`. |
| `range` | 7 typed | 7/7 OK | `mutils_range.cpp`. `check_src_f`; data/size locals; `max` tested for null before `min`; `*max = *min = data[0]`; updates through the pointers; work `size * 5.0`. Resolves the parked `range` family. |
| `range_robust` | 7 typed | 7/7 OK | In `mutils_range.cpp`. `range` restricted to `[lo, hi]` with a `found` flag; `hi < lo` returns "Illegal data value"; the first hit is stored by `*max = *min = data[i]`; later tests read `data[i]` again; it ends with `if (!found) return "Illegal data value"; return nullptr;` (the ternary gives another `cmov`). Work `size * 8.0`. |
| `max`, `min`, `add`, `subtract`, `multiply_elem` | 7 typed each | 35/35 OK | `mutils_binary.cpp`. The binary bit-operation shape with the context first; `min` is `a > b ? b : a`. The s16/u16/u8 `subtract` check `b` with `check_src_f`, the rest cols-first. The vectorizer reproduces its alias-checked loops from this plain source. |
| `add_scalar`, `multiply_scalar` | 7 typed each | 14/14 OK | `mutils_scalar.cpp`. The scalar bit-operation shape with the context first; scalar of the element type. |
| `number_equal_scalar` | 7 typed | 7/7 OK | In `mutils_number_equal.cpp`; the one-matrix form of `number_equal`. |
| `sum_cols`, `sum_rows` | 7 typed each | 14/14 OK | `mutils_sum_dims.cpp`. `src` dims in locals; per-line sums with the submat-style progress; work `cols * 4.0` per line in both. s16/u16/u8 check `dst` with `check_src_f`. |
| `entropy` | 7 typed | 7/7 OK | `mutils_entropy.cpp`. Two submat-style progress steps of `size`. dbl/flt: probabilities, a negative value or `!(fabs(sum - 1.0) <= (fabs(sum) + 1.0) * 1e-05)` returns "Illegal data value"; `matflt` uses `logf`. Integer types: counts summed as `int`, with the `< 0` test kept even for unsigned types (it prevents vectorization), `*out -= v * log(v / (double)sum)`, then `/ sum` and `/ log(2.0)`. |
| `rescale` | 7 typed | 7/7 OK | `mutils_rescale.cpp`. Checks `dst`, then calls the typed `range` export (LTCG inlines it; its check is the `src` check), shape test, `lo > hi` returns "Illegal data value", `double scale = min == max ? 0.0 : (hi - lo) / (max - min)` in doubles. `matdbl` checks `dst` with `validate`. |
| `translate` | 7 typed | 7/7 OK | `mutils_translate.cpp`. `(0, 0)` calls the typed `assign`; otherwise `assign_scalar(value)` fills `dst`, then the overlapping block is copied with per-row work `(cols + 1) * 8`. The progress locals are declared before the range branches; the inner loop starts `k` before `j`, and `out_row` is incremented after the work update. |
| `threshold` | dbl, s32, u16, u8 | 4/4 OK | `mutils_threshold.cpp`. `lo > hi` returns "Illegal data value" before the checks; `on` inside `[lo, hi]`, `off` outside; work `size * 3.0`. |
| `histogram` | dbl, s32, u16, u8 (into a `_uint32_mat`) | 4/4 OK | `mutils_histogram.cpp`. One-column `hist`, `lo >= hi` returns "Illegal data value", zeroed by `matu32_assign_scalar` (now `__declspec(noinline)`, as the original never inlines it), bin `ceil((v - lo) * bins / (hi - lo)) - 1`; a value at `lo` counts in bin 0 when `include_lo`; the `bin < bins` test is shared by both paths. |
| Signal windows | `sigdbl_`/`siguniv_window_` `rectangle`, `hamming`, `hanning`, `gaussian`, `triangle`, `born_jordan` | 12/12 OK | `sig_window.cpp`. `validate`, vector test ("array size mismatch"), the textbook formula over `w->size`, work `0.0 + size * k` (5, 50 or 100). The `siguniv_` form tests null, then `type == EFI_DOUBLE` ("Illegal data type"), then `if (const char *error = sigdbl_...(...)) return error;` (the `cmov` shape). |
| `img*_map_invert` | dbl, u16, u8, univ | 4/4 OK | `img_map.cpp`. Calls the typed `range` (inlined for u16/u8), then `min + max - v` over a `size` local (dbl: `max - v + min`); u16/u8 check `dst` with `check_src_f`. `imguniv_` is a compare-chain dispatcher with the per-case `if (const char *error = ...)` form. |
| `img*_moment` | dbl, univ | 2/2 OK | `img_moment.cpp`. `Σ iᵖ·jᵠ·v` into `*out`, `pow` only for a nonzero exponent; dims and data in locals; work `cols * 10.0` per row. |
| `mat*_tile`, `mat*_untile` | 8 typed each | 16/16 OK | `mutils_tile.cpp`, `mutils_untile.cpp`. Aliasing returns "Illegal memory address", a bad tile size "Illegal data value"; the tile index is one inline expression with plain signed `/` and `%` (the compiler emits unsigned `div` for `tile` once it proves both operands positive); work `15.0` per element of a row. |
| `pgnuniv_shape_features` | univ | 1/1 OK | Pass-through dispatcher. |
| `img*_generate_rectangle` | dbl, s32, u16, u8, univ | 5/5 OK | `img_generate.cpp`. `height`/`width < 1` returns "Illegal data type", `fg == bg` "Illegal data value", a rectangle fully outside returns the mismatch message; the progress locals come before the typed `assign_scalar(bg)` call; the clip is `row_end > rows ? rows : row_end`; work `height * 3.0 * width`. |
| `matuniv_histogram` | univ | 1/1 OK | Compare chain over dbl/s32/u16/u8; `hist` must be `EFI_UINT32`. |
| `img*_binary_dilate` | s32, u16 | 2/2 OK | `img_morph.cpp`. `src`/`se`/`dst` checks, shape and alias tests, `fg == bg` returns "Illegal data value"; `src->data` taken before the `assign_scalar(bg)` call; stamps `se` around each `fg` pixel; work `((se_cols * 3 + 3.0) * se_rows + 2.0) * cols`. |
| `img*_grayscale_erode` | dbl, s32, u8, u16 | 4/4 OK | `img_grayscale.cpp`, after `img_generate.cpp` as in the original. `check_src_f` on `src`, `se`, `dst`; size and alias tests; `switch (boundary)` with zero (0, out-of-image pixels read 0) and replicate (3, clamped), others "Illegal data value", written inline at both fetches (a helper function does not match); minimum of pixel(`r + k - se_row`, `c + l - se_col`) - `se`, the `(0, 0)` element taken first; work `se_rows * 8.0 * se_cols * cols + 8.0` per row. |
| `matu8_write_raw` | u8 | 1/1 OK | `mutils_io.cpp`. Null `fp` returns "null pointer"; `fwrite` straight from `m->data` in chunks of at most `0xFFFFFFFF`; a short write returns "Input-output error"; work `size * 1000`. |
| `pgn*_rectangular_to_polar`, `pgn*_rectangular_from_polar` | dbl, univ | 4/4 OK | `pgn_polar.cpp`. n×2 points to `(r, θ)`; `θ = atan(dy/dx)`, `+π` when `dx < 0`, then `+2π` when negative; `dx == 0` gives `π/2` or `3π/2`; work `rows * 20.0`. |
| `pgn*_point_test` | dbl, univ | 2/2 OK | `pgn_point.cpp`. Winding-number test with the boundary cases (crossing edge through the point, horizontal edge on the line); the last vertex is read as `data[(n - 1) * poly->cols]`; the final classification is an `if`/`else` on `*winding & 1` (the bit forms compile to `not`/`and`). |
| `pgndbl_shape_features` | dbl | 1/1 OK | In `pgn_point.cpp`. Shoelace sums started from the closing edge, running bounding box and perimeter; the loop updates are ordered `sy`, `sum`, `sx`, perimeter; the first perimeter term is written `dx² + dy²`, the loop's `dy² + dx²`. Work `rows * 50.0`. |
| `mutil_rand_set_seed`, `mutil_rand_begin` | | 2/2 OK | `mutils_rand.cpp`. The state is Numerical Recipes' `ran1` (seed, 32-entry shuffle table, last value) plus a saved normal deviate (0x98 bytes). `set_seed` takes `*seed` or `time(nullptr)`, writing `st->seed` directly (a local gives another register); `begin` allocates the state and seeds it with 1. |
| `validate` | `mat{dbl,flt,s32,s16,u32,u16,u8,cpx}_validate` | 8/8 OK | One body (linker-folded in the original, `0x86460`). Reads `rows`/`cols` into locals first; the direct form picked another register. |
| `wrap_data` | 7 typed exports (no `cpx`) | 7/7 OK | Same body for all seven (`0x859e0`). |
| `malloc` | 8 typed exports | 8/8 OK | `allocate_mat<T>`; the allocator is `EfiCallContext`'s pointer at +80h. |
| `mat_set` | `matset_malloc`, `matset_free`, `matset_free_matrices`, `matset_validate` | 4/4 OK | `mutils_set.{h,cpp}`. `matset_free_matrices` is `validate` alone. |
| `matset_resize` | | 1/1 OK | `mutils_set.cpp`. Uses `check_set_f`, the set check with every field read where it is used (the `validate` form keeps `count`/`total` in locals and gives other registers); `count < 1` throws "Illegal data value"; the new dims' product must equal `total`. |
| `pgn_chain_free` | `pgn_chain_free` | 1/1 OK | Early return for a null or empty chain. |
| `mutil_interrupt` | `mutil_interrupt` | 1/1 OK | Over the shared inline in `efi_common.h`. |
| Trivial (`ret 0` / `xor eax,eax; ret`) | `mutil_abort`, `mutil_msg_print`, `mutil_msg_print_line`, `mutil_msg_print_time`, `mutil_msg_set_level`, `mutil_rand_end` | 6/6 OK | `mutils_msg.{h,cpp}`. `_mutil_msg_level` values not inferred yet. |
| Allocator wrappers | `mutil_malloc`, `mutil_calloc` | 2/2 OK | `mutils_alloc.{h,cpp}`. |

Shared foundation (`src/Quire.Native.EfistatsciImpl/efi_common.h`):
- `Efi::Throw`: `[[noreturn]] __declspec(dllimport)`, import `?Throw@Efi@@YAXPEBD_K@Z` (IAT `0xac220`), pinned in `efiutl.def`.
- `typed_mat<T>`: `rows` @0, `cols` @4, `size` @8, `data` @10h. Each element type derives from it (`_double_mat`, ...).
- `Efi::EfiCallContext`: only the allocator pointer at +80h is inferred.
- `mutil_interrupt_step` (inline `double`, 1e6 in the image) and `mutil_interrupt_inline` (returns `unsigned char`).

Manifest entries recorded with `native_targets.py` (`docs/mirror/native/internal-targets.json`, `efistatsci_.dll`): the literals `"array size mismatch"` (`0xac368`), `"null pointer"` (`0xac3d8`), `"Illegal data value"` (`0xac398`), the step static (`0xcc0c0`) and the `1e7` constant (`0xac908`).

## Facts from machine code

- **Build mode decides register choice.** The `assign_submat` entry copy is `mov r10, rcx` in the original and in the `/GL` + LTCG build; `/O1`, `/Os`, `/Ot`, `/GS-`, `/Gy-`, `/EHsc`, `/Oi-` and compiler build 36260 all give `mov r9, rcx` per object.
- **Source shapes still matter under `/GL`:** the bounds error test is `dst->rows < src->rows + row`; the source validate uses fresh field reads (`m->rows * m->cols`) and the destination reads `cols` first. The interrupt work is `cols * 8` for every element type.

- `mutil_interrupt` returns `unsigned char` (mangled `E`), not `bool`.
- The step static's file image holds **1000000.0** (`00 00 00 00 80 84 2E 41`), not zero. The `1e7` fallback runs only when the step reads zero. The maintainer note says it is "lazily set to 10000000.0"; the image says otherwise, so the initializer is 1e6.
- `_univ_mat`'s type tag is a 32-bit `int` at `+18h` (`movsxd rax, dword [r8+18h]` in `matuniv_assign_scalar`).
- `_mat_set`: `count` (int) @0, dims pointer @8, `total` (int) @10h, a pointer at @18h (non-null-checked).
- `pgn_chain` (in `mutils`): count (int) @8, pointers at @10h and @18h.
- Matrix checks reach `Efi::Throw` in this order: null matrix, `rows <= 0 || cols <= 0`, null data, `size != rows * cols`.
- Work constants differ per operation: `assign_scalar` uses `size + size`, `sum` `size * 3.0`, `range` `size * 5.0`, `cast_to` `size + size`.

## Parked (D25): present in the source's history, not exported

Parked exports are removed from the DLL, because a committed new function that fails would block `native_gate.py` (new functions must pass). They return when they match.

Re-tested under D28 (`/EHa`, `_CRTIMP2`): every parked function keeps its size and mismatch (`cpx` 206 vs 219; `sum` 219/217 for the integer forms, 299/297 `dbl`, 302/287 `flt`; `range` 274/257 and 285/277 and 279/273; `cast_to` diverges at +0x73). D28 does not resolve them.

- **`sum` family** (7: `matdbl_sum` `0x7c650`, `matflt_sum` `0x7c780`, `mats{32,16}_sum`, `matu{32,16,8}_sum`): the body matches except for the epilogue. The candidate is 2 bytes longer for the integer forms (219 vs 217) and for `dbl` (299 vs 297), because the two `const char *` returns merge into one register (`lea rcx` + `mov rax, rcx`; `xor ecx` + `mov eax, ecx`). The original returns `lea rax` / `xor eax` directly. Five declaration orders of the locals were tried, all 219. `flt` is 302 vs 287 (the original unrolls by four).
- **`matu8/16/32_bit_not`** (3): instructions match; fails only on the data of its all-ones vector constant, which is compared together with its neighbour in the pool (Harness notes).
- **`matset_verify_allsame`** (`0x70fb0`, 302 bytes): 276 bytes with the `-1` guards written as default-then-override. The original keeps the guards and its loop layout differs; not resolved.
- **`matu8_cast_to_dbl`** (sweep over loop-bound and local-size forms, 4 combinations): still unrolls or differs; no match.
- **`matu8_cast_to_*`** (6: `dbl`, `flt`, `s16`, `u16`, `s32`, `u32`): forced inlining gives the same loop as a template, but the candidate unrolls the loop four times (437 bytes against the original's 285). The original keeps the loop un-unrolled, because it reloads `src->size` each iteration. Binding the loop to `src->size` directly still unrolled, so the compiler proved the bound invariant where the original did not. Not solved by source shape yet.

- **`matcpx_flip_up_down`** (500 bytes, same length): only register assignment differs: the original keeps `ceil`'s result in `xmm9` and the `1e7` constant in `xmm5`, while the candidate swaps them. `native_permute.py` stopped at 0.984 after 579 attempts. By hand: `half` local before or after the progress locals (476 bytes), swap through `bottom` first (0.895), `progress` declared first (0.911). The other 15 flips match with the same source.

- **`matdbl_cast_to_flt`**: instructions match (bounds ±1e37 as doubles, saturating to ±1e37f). It fails only on data: in the original the float constants `__real@fcf0bdc2`/`__real@7cf0bdc2` follow the two double bounds, and the comparison reads into them. Only code after `matuniv_range_robust` (`0x7FB40`) references those floats, so this export should match unchanged once that function exists. `matdbl`/`matflt_cast_to_s32`, `_s16` and `_u32` (the clamped shape; bounds ±2147483647.0, ±32767.0, 0/4294967295.0) are parked with it. All six match while `matdbl_cast_to_flt` is in the DLL and fail without it, one after another: each bound constant is followed in the original's sorted pool by the next one, ending at `-1e37`/`1e37` (see Harness notes). Retry them together once `matuniv_range_robust` (a dispatcher) can land, or once the harness compares constants over their own size. Source: the clamped-cast shape with `double v = src->data[i]`, bounds `v < -1e37` / `v > 1e37`, work `size * 4.0`.

- **`sig*_window_blackman`, `_nuttall`, `_raised_cosine`** (6 with their dispatchers): formulas reconstructed. blackman differs only in which register holds `(i + 1)` around its second `cos` (permuter: no improvement in 500 attempts). nuttall's three `cos` calls are scheduled in another order than written; its dispatcher returns `nullptr` for other element types. raised_cosine differs only in epilogue placement.

- **`matuniv_bit_*`** (8 compare-chain dispatchers over u8/u16/u32): the original keeps one shared `test rax,rax; jne` after the calls. The candidate duplicates the tail per case with `cmov` (`if (error) return error;` after the switch, per-case `if (const char *e = ...)`, or a null-initialized `error`), or drops the test (`return error;`, 70 bytes against 96).

- **`matdbl_assign_scalar` is `__forceinline`**: the original never calls it (0 call sites); its link inlines it into every user (`matdbl_translate`, `imgdbl_generate_rectangle`, ...). With the attribute those callers match and the export still matches. **`img*_generate_disk`** (s32, u16, u8): reconstructed (`ceil(radius)`, clipped rows, `floor(sqrt(r^2 - dy^2))` half-width, `painted` flag, work `radius * 5.0 + 3.0`); the original re-reads `dst->cols` where the candidate keeps it in a register. **`imgdbl`/`imgu8_binary_dilate`**: register assignment only. **`mat*_multiply`** (7): the plain triple loop with `dst`/`a`/`b` checks, the alias test and work `dst->cols * a->cols * 8.0` gets 0.76-0.96; the remaining difference is register assignment around the unrolled inner loop (permuter: no improvement on `mats16`).

- **`mat*_write_raw` (dbl, flt, s32, s16, u32, u16), `mat*_read_raw`, `mat*_binary_erode`, `matdbl_to_complex`**: reconstructed in `.mirror-cache/lane-h-tools/parked_*.cpp` (local). write_raw reaches 0.985-0.993 (with `*(in + i + k)` for the byte swap); only the order of `i = 0` against the bound computation differs. read_raw differs in which register the final work addition lands in.

- **`mutil_rand_uniform`, `mutil_rand_normal`**: both match while `normal` is in the DLL, but `normal` fails on the pooled-constant neighbour of `-2.0`: in the original `-π` (only from `cls*_scalar_quantize_angle`) follows it. `uniform` alone, as a leaf placed before CRT code, takes the alignment `nop` padding into its extent (the harness trims only `int3`). Land both with `cls*_scalar_quantize_angle`. 

- **Pooled-constant parks move as the pool fills (2026-10-27).** Re-adding the parked data-only failures after more exports landed: `matdbl`/`matflt_cast_to_u32`, `rectangular_from_polar` `mutil_rand_uniform` and `matset_resize` now pass. `dbl`/`flt` to s16/s32, `dbl_to_flt` and `mutil_rand_normal` pass only while `mat*_bit_not` is present. `bit_not` itself still fails: its all-ones vector constant (`0xACA80`, shared only with `binary_erode`) is compared over about 0x110 bytes. Retry these whenever the pool changes. `pgndbl_rectangular_from_polar` converts `dst` in place, as the original does (it only checks `src`).

- **`mat*_print`, `mat*_write_ascii`, `mat*_read_ascii`** (21 typed, 2026-10-27): reconstructed in `.mirror-cache/lane-h-tools/parked_io/` (local). In the original these sit in one file with `write_raw`/`read_raw` (`0x60150`-`0x66150`: univ dispatchers, then `print`, `parse`, `write_ascii`, `read_ascii`, `write_raw`, `read_raw`; types dbl, flt, u8, u16, u32, s16, s32). `read_ascii` (`%lf`/`%f`/`%hu`/`%lu`/`%hd`/`%ld`; u8 rejects >255, s16 < -32767, s32 < -2147483647) is 1.000 with all instructions equal. Its empty-matrix guard jumps past the `total > 0` test in the candidate (to the original's `+0x134`) instead of to it (`+0x10e`). Its skip helper (`0x9FCE0`: white space and the separator character are skipped, the comment character skips a 100-byte `fgets` line, then `ungetc`) matches, once `/GS` is off. `print` (default width 75, `(int)floor(log10(rows))`, header `fprintf`s joined with `||`, `format = format == nullptr ? default : format`) is 0.976. Only the row-loop guard comes before the loop-invariant set-up instead of after it. `write_ascii` (field = `strlen(separator)` + 18/3/6/11; `.data` `" "` default, `.bss` `""` for the last element) is 0.982: the original keeps `size - 1` in `ecx` around the loop. Adding the file also moves `"Input-output error"`'s pool neighbour (`matu8_write_raw` then fails on `data +0x14`), and putting the file at the end of the compile list reorders the `"array size mismatch"` neighbours for 19 exports. Place it beside `mutils_io.cpp`.
- **`mat*_sort_index_partial`** (7 typed, 2026-10-27): reconstructed in `.mirror-cache/lane-h-tools/parked_sort/` (local); in the original it follows the typed `permute` exports in their file. The export (`check_src_f(src)`, `validate(index)`, `validate(ranks)` when given, `size` cached, ranks outside `[0, size)` return "Illegal data value", `index[i] = i`, then `if (error == "interrupted") return error;`) matches all seven at 1.000. Its recursive per-type helper (`0x79A90` dbl ... `0x7AB90` u32) is a partial quicksort. It runs the interrupt step check first, returns early when no requested rank lies in `[lo, hi]` (a found flag, no `k == size` test), and insertion-sorts 9 or fewer elements. Otherwise it swaps `index[lo + 1]` with `index[(lo + hi) / 2]`, orders (lo, hi), (lo, lo+1), (lo+1, hi) with `>=`, and partitions with `++i; while (data[index[i]] < v) ++i;`. The smaller side recurses first. With NR's `t = index[i]; index[i] = index[j]; index[j] = t` the compiler hoists the `index[j]` load above the `i >= j` test (4 bytes short). With `t = index[j]` first the length is exact, but the two stores come out in the other order. 18 combinations of break test, swap and final swap tried; none matches.
- **`img*_grayscale_dilate`** (dbl, s32, u8, u16; 2026-10-27): the same source as `erode` with `+`, `>` and `r - k + se_row` (`.mirror-cache/lane-h-tools/parked_gray/`). It reaches 0.993 (s32/u8/u16) and 0.984 (dbl). The original loads the pixel before `se->data[0]` for the first element (dbl: it adds after the boundary branches merge). Because `+` commutes, the candidate loads `se->data[0]` first and sinks the add into each branch. `v + s[0]`, `s[0] + v`, `best = v; best += s[0]` and the `if`/`else` zero forms all compile the same way.
- **Moment chain** (`imgdbl/imguniv_moment_central`, `_normalized`, `_invariant`; 2026-10-27; `.mirror-cache/lane-h-tools/parked_moment/`). `normalized` (`*out /= pow(|mu00|, (p + q + 2) * 0.5)` after `central(p, q)` and `central(0, 0)`, "Illegal data value" for mu00 == 0) and the three univ wrappers match. `imguniv_moment_invariant` checks `out`'s tag before `img`'s. They wait on `imgdbl_moment_central`, which is 6 bytes short at 0.63: `p`/`q` checks, `imgdbl_moment(0, 0)` for p = q = 0, otherwise three inlined `imgdbl_moment` calls for m00/m10/m01, "divide by zero", then the moment loop over `i - xc`, `j - yc`. The original spills `p`/`q` to their home slots and converts `cols` with `movd`+`cvtdq2pd` where the candidate keeps `q` in `r13` and emits `cvtsi2sd`. A 30-minute permute reached 0.73 only with unnatural types. `imgdbl_moment_invariant` (Hu's seven, with the original's operand forms: `(n03 - 3n21)^2 + (n30 - 3n12)^2`, `h[2]`, `h[3]`, `h[5]` negated when mu00 < 0) needs `__declspec(noinline)` on `normalized` and `validate(img)`. It differs only in FP scheduling (the original keeps 3.0 in `xmm6`).
- **`imgdbl_local_mean`** (`.mirror-cache/lane-h-tools/parked_local/`): a running box mean (odd window checks, zero/replicate boundary written inline, the column-0 window sum then add/subtract per column, work `n * 8.0 + 3.0 + (win_rows * 13.0 + 3.0) * cols`). The length is within a byte (1339 against 1338), but registers differ throughout (0.42). The replicate clamps come out as different `cmov` forms in the original for rows and columns. A permute reached 0.49. `local_variance` (sum in a register, sum of squares in `dst`, then `sumsq / n - mean^2`), `filter_wiener` and the univ wrappers build on it.
- **`pgn_chain_malloc`, `pgn_chain_realloc`** (`.mirror-cache/lane-h-tools/parked_pgn_chain_set.cpp`): `malloc` (null "null pointer", zero count gives null arrays, negative "Illegal data value", `count * 4` and `count` byte allocations) reaches 0.933: the original stores `count` before `codes` in the merged tail and writes the null `points` as an immediate. `realloc` (0.945, `count == 0` returns early, an empty chain goes to `malloc`, each array grows by `allocate_array` + `memcpy` when not smaller) differs only through the inlined `malloc` tail. `boundary_chain_from_region` (5 typed) needs both.
- **`mat*_multiply`** (re-tried 2026-10-27): the checks now match (`check_src_f(dst)`, `check_dst_f` on both operands). s16/u16/u8 differ in one register (0.96), s32/u32 in the loop-index register (0.88). dbl/flt (0.76) differ in the 4x unrolled sum, where the original loads all four products before adding them.
- **Pool chain of the cast parks (2026-10-27):** `matdbl`/`matflt_cast_to_s16` need `cast_to_s32`'s `-2147483647.0` after their `-32767.0`. The s32 casts need `matdbl_cast_to_flt`'s `-1e37`. `matdbl_cast_to_flt` needs the float `-1e37f` (`0xAC9B8`) that only the `matuniv_range_robust` jump-table dispatcher brings, and `mutil_rand_normal` needs the same group. With all six added, five pass and `cast_to_flt` fails, so none can land until that dispatcher can.
- **`img*_transform_distance`** (5 typed), **`sig*_correlate`** (typed, 5 of 10 with `convolve`), **`sigdbl_kernel_choi_williams`** (2026-10-27; `.mirror-cache/lane-h-tools/parked_td/`, `parked_sigfilter/`, `parked_timefreq/`). `transform_distance` is a two-pass chamfer (switch on the metric for the diagonal weight and the far value, forward then backward pass with `minsd`): 0.73 at 2431 against 2536 bytes, the passes differ in registers only. `correlate` (zero/periodic/reflect/replicate boundary written inline, `src` check rows x cols): 0.51; the original keeps the sum in `dst` and re-reads it, the candidate promotes it to a register. `kernel_choi_williams` is 0.97 (168 against 170 instructions): the original stores `rows` before computing `rows * cols`, and a permute found nothing. The boundary enum has four values (0 zero, 1 periodic, 2 reflect, 3 replicate) in `efi_common.h`.
- **`/GS-` (2026-10-27):** the original has no stack cookies (no `xor rax,rsp` anywhere), so the project now sets `BufferSecurityCheck` false. No existing match changed.

- Helper pattern for functions that return a message (added after the parked `sum`/`range` work): the exports call `validate` themselves, return the message literal themselves, and put only void/bool shared logic in `__forceinline` helpers (`same_shape`, `binary_elementwise`, `running_sum`). A message returned from an inlined template comes back in `rcx` and is copied to `rax`; the original returns it in `rax`. The helper pattern removes that copy.

## Remaining work

- 360 exports not yet implemented, plus the 88 parked above.
- The `siguniv_window_*` dispatchers (11) call typed `sigdbl_window_*` exports, so they cannot link until those exist; they wait for their typed exports.
- `matuniv_wrap_data` and `matuniv_wrap_matrix` dispatch through a jump table over a nine-case type enum, with different field copies per case.
- `mutil_realloc`: its `.pdata` extent conflicts with the code address (18-byte extent for a 71-byte body). Not implemented; parked for the maintainer.

## Harness notes

- **Data extents of pooled constants (2026-10-27).** For data, `native_linked.py` compares `max(candidate map-symbol extent, original object_end)`, and the original's `object_end` is the next referenced address. For 8-byte `__real@` constants, which the linker pools sorted by name, that span takes in the following constant. An export then passes or fails depending on whether an unrelated function brings the same neighbour constant into the candidate. Example: `matdbl_cast_to_s32` reads `__real@c1dfffffffc00000` (`-2147483647.0`, `0xAC9A8`). It matches while `matdbl_cast_to_flt` (which brings in `-1e37` at the next slot) is present and fails with `data +0x8: bytes differ` without it. Comparing a `__real@`/`__xmm@` constant over its own size would remove the dependence.
- **Blocker for the `*univ_*` dispatchers (2026-10-27): jump tables.** A dispatcher such as `matuniv_transpose` switches on the type tag (`EFI_UINT8` 0, `EFI_SINT8` 1, `EFI_UINT16` 2, `EFI_SINT16` 3, `EFI_UINT32` 4, `EFI_SINT32` 5, `EFI_FLOAT` 6, `EFI_DOUBLE` 7, `EFI_DCOMPLEX` 8; read from `matuniv_cast`'s tables) through `lea r10,[__ImageBase]` and `mov ecx,[r10+rax*4+<table RVA>]`, with the table of case-label RVAs placed right after the function. `native_linked.py` matches the `ADDRESS` pattern only for absolute image addresses, so it compares that `disp32` as raw bytes (`+0x37: bytes differ`). It also decodes the inline table as instructions, and the case RVAs differ in any relinked DLL. A fix would read a `disp32` used with the `__ImageBase` register as an RVA (local when inside the function), and compare the table entries as local offsets. Until then no switch-based dispatcher can pass. The source shape is found: `matuniv_transpose` agrees up to that instruction with a null check of both, a tag-equality check returning "Illegal data type", then `case EFI_DOUBLE: { if (const char *e = matdbl_transpose(...)) return e; } break;` per case in the order dbl, flt, u8, u16, u32, s16, s32, cpx, `default: return "Illegal data type";`, and `return nullptr;` after the switch. Similarity 0.85; the rest is the table. The dispatchers stay out of the DLL until the harness handles the table.
- **Leaf functions without `.pdata` that sit next to each other (2026-10-27).** A leaf's extent runs to the next export or `.pdata` start, minus trailing `int3`. `__local_stdio_printf_options` and `__local_stdio_scanf_options` (`lea rax,[...]; ret`, 8 bytes each) are emitted next to each other when one TU uses both `fprintf` and `fscanf`, so the first one's extent covers both (24 bytes against the original's 8): `+0x21: length 24 != original 8` inside `fprintf`. In the original the printf one comes from the TU of `imgset_region_grow_klm` (`sprintf`, `0x1F950`), so it sits before a `.pdata` function. Ending a leaf's inferred extent at its first `ret` (or at the next map symbol) would remove the dependence on TU grouping.
- **Incremental builds can lay out the pool differently from clean builds.** The same source and compile order gave different literal-pool neighbours depending on build history. Clean rebuilds (`-t:Rebuild`, about 16 s) are the reference; the gate's fresh cache agrees with them.
- Strict default and the manifest route work as documented in `native_bytes.py` (D23–D26). No harness changes made.
- Product ID (Rich header) not compared; every matching function here matched without it.

## Landing

Landed on `main` through `lane_merge.py` (Release IL gate PASS; native gate PASS). The branch was rebased onto `f61c3dc0` first; the checkpoint text was kept over main's copy of this file, and the maintainer section was restored from main.

## Maintainer update 2026-10-25

The remaining exports are mostly one pattern across element types, and per-object `/O2` reproduces it despite D25's
`/GL` note. Rebase onto `main` first: the branch is 121 commits behind, and D23–D27 changed the harness (strict by
default, zero-filled data, leaf targets, `lane_merge.py` gates native projects).

**Shape of the DLL.** 781 exports on 699 bodies. 597 bodies call no internal function, only exports and imports.
482 exports are one operation in 4–9 element types (`matdbl_`, `matflt_`, `mats16_`, `matu8_` … `matuniv_`). 539 bodies
call `Efi::Throw`. The `*univ_*` functions check `_univ_mat`'s type tag at `+18h` (7 = double) and call the typed
export, so they verify once the typed exports match.

**Shared vocabulary**, found from `matdbl_assign_scalar` and `mutil_interrupt`:

- `_double_mat` (and the other typed matrices): `int rows` @0, `int cols` @4, `int size` @8, `T *data` @10h.
- Validation is `Efi::Throw("<message>", ~0ull)` (noreturn import from `efiutl.dll`). The messages are string literals
  at `0xac368` "array size mismatch" (560 sites), `0xac3d8` "null pointer" (521), `0xac398` "Illegal data value"
  (49), `0xac350` "Illegal data type", `0xac380`, `0xac3b0`, `0xac3c0`.
- `mutil_interrupt`'s step is a static `double` at `0xcc0c0` (1,573 references), lazily set to the constant
  `10000000.0` at `0xac908`. The exports inline it, so define it `inline` in a shared header; a per-object
  compile cannot inline it across objects. Its out-of-line copy has the same instructions as the exported
  `mutil_interrupt` (compared by eye, not yet byte-checked).

**Trial (scratch, not committed).** This source matches 5/5 `assign_scalar` variants (`dbl` 168, `flt` 168, `s32` 170,
`s16` 172, `u8` 173 bytes) under `/O2 /Gy /GS /EHsc /MD /std:c++17`, with the BuildTools 36246 toolset. It also
matches under D28's `/EHa` and `_CRTIMP2=__declspec(dllimport)`, which the project must adopt first; the 8 existing
exports still match under them:

```cpp
template <typename T, typename M> inline const char *assign_scalar(T value, void *context, M *m)
{
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->rows <= 0 || m->cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    T *data = m->data;
    if (data == nullptr) Efi::Throw("null pointer", ~0ull);
    int size = m->size;
    if (size != m->rows * m->cols) Efi::Throw("array size mismatch", ~0ull);
    for (int i = 0; i < size; ++i) data[i] = value;
    double work = (double)size + (double)size;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}
```

The `data` and `size` locals matter: reading `m->data[i]` in the loop gives a reloading loop instead of the original's
`rep stos`. `__int64 size` moves the sign extension and fails. Each match left 5 unverified targets (the two strings,
the step static, the constant), so record the shared literals and the step static once with `native_targets.py
--name`; every later batch then verifies through them.

**Suggested order.** (1) Shared header: matrix structs, `Efi::Throw`, inline `mutil_interrupt`, and the manifest
entries. (2) Whole type families per batch, small operations first (`assign_scalar`, `siguniv_window_*`, then
`range`, `min`/`max`, `entropy`). (3) The `*univ_*` dispatchers after their typed exports. (4) The 102 bodies with
internal helpers last; `0x267a0` and `0x26750` (69 bytes each) are shared by 35 exports. Park per D25 any function
that still differs after the source shapes and flags tried are recorded.

## Maintainer update 2026-10-26

Answers to the scope questions, a solved family, and a search tool for near misses.

**Scope.** All 781 exports stay in scope, because `scope.md` mirrors the whole exported surface. Work on the consumer path first:
- Four Mathcad DLLs import from `efistatsci_`: `efiimage_` (68 imports), `efisignal_` (14), `efiio_` (10) and `efistatistics_` (4). Together that is 78 exports.
- Those 78 reach 317 exports by direct call or jump, listed in
  [`efistatsci-consumer-path.txt`](../native/efistatsci-consumer-path.txt). 43 of the 70 landed exports are on it, so 274 remain.
- The list follows direct references only, so treat it as a lower bound.

**Order on the consumer path.** Work bottom-up, because a dispatcher only verifies once its typed exports match:
1. typed `mat*` operations (`extract`, `transpose`, `assign`, `permute`, `flip_*`, `read_raw`/`write_raw`/`read_ascii`, `sort_index_partial`, `assign_zeropad`, `number_equal`, `table_lookup`, `cast_to_*`, `histogram`);
2. the `matuniv_*` dispatchers;
3. `sigdbl`/`sigcpx`, then `siguniv`;
4. the typed `img*` exports, then `imguniv` and `imgset`.

The other 464 exports follow in the order of the 2026-10-25 note.

**Parked families.** Keep them parked with their evidence (D25/D29), and don't drop them. Retry them with `native_permute.py` (below) before any more hand rounds. `range` (dbl, flt), `transpose`, `assign_zeropad` and `matdbl_assign` are on the consumer path; the integer `range` forms are not.

**The `sum` family is solved.** All seven exports match in the linked run, 77/77 with the 70 landed. Two shapes made the difference:
- In the input check, read `cols` into a local and read `rows` from the matrix.
- Accumulate into `*out`; a separate `total` local gives different code.

```cpp
template <typename M>
__forceinline void check_sum_input(const M *m) {
    if (m == nullptr) Efi::Throw("null pointer", ~0ull);
    int cols = m->cols;
    if (m->rows <= 0 || cols <= 0) Efi::Throw("array size mismatch", ~0ull);
    if (m->data == nullptr) Efi::Throw("null pointer", ~0ull);
    if (m->size != m->rows * cols) Efi::Throw("array size mismatch", ~0ull);
}

template <typename T>
__forceinline void running_sum(const T *data, int size, T *out) {
    *out = 0;
    for (int i = 0; i < size; ++i) *out += data[i];
}

const char *matdbl_sum(Efi::EfiCallContext *, const _double_mat *m, void *context, double *out) {
    check_sum_input(m);
    double *data = m->data;
    int size = m->size;
    if (out == nullptr) return "null pointer";
    running_sum(data, size, out);
    double work = (double)size * 3.0;
    if (work > 0.0) mutil_interrupt_inline(work, nullptr, context);
    return nullptr;
}
```

The other six sum exports use the same body with their own types (`float`, `long`, `short`, `unsigned int`, `unsigned short`, `unsigned char`). `matflt_sum` no longer needs the four-way unroll the parked note expected. The shared `validate` template stays unchanged, because the 8 `validate` exports match with its current form.

**Near misses: `tools/validation/native_permute.py`.** See [validation.md](../validation.md) for the full contract.
1. Write the export in the shape you would land, so the project builds.
2. Run:

   ```
   python tools/validation/native_permute.py --project Quire.Native.EfistatsciImpl --source mutils_mat.cpp \
       --function matdbl_sum --inline validate --symbol matdbl_sum --minutes 20
   ```

   - `--function` and `--symbol` repeat; a family can be searched together.
   - `--inline` gives the searched bodies their own copy of a shared helper. The helper's other users are left alone.
   - No attempt that breaks a landed export is kept.
3. On `MATCH`, apply `.mirror-cache/permute/<name>-<time>/match.patch` with `git apply` from the repository root, then land as usual. The patch is already reduced to the lines the match needs.

From the helper-pattern source, it found `matdbl_sum` and `mats16_sum` in 250–350 attempts, about 6 minutes on 14 workers. Its patch passed `native_gate.py`.

When it stalls, read its improvement diffs and `native_linked.py --score`. A structural difference still needs a hand-written shape, which `--minutes 0` scores without searching. Examples are a loop form, a helper boundary, or a message returned from inside a template (the helper pattern exists for that case). Its `report.txt` quotes original instructions, so keep it out of commits and PRs (D15).
