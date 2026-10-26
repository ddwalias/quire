# Lane H: efistatsci_.dll → Quire.Native.EfistatsciImpl

**Status: incomplete.** 333 of 781 exports pass the linked `/GL` gate (`native_linked.py`, D30): the 78 below and 79 from the consumer path, `cumulative_sum` 7, 24 unsigned bit operations, `realloc` 8, `abs` 4, `range` 7, 35 binary arithmetic exports, 14 scalar ones, `number_equal_scalar` 7 `sum_cols`/`sum_rows` 14, `entropy` 7, `rescale` 7, `translate` 6, `threshold` 4, `histogram` 4 12 signal windows `map_invert` 4, `matuniv_histogram` all `generate_rectangle`, `matdbl_translate` two `binary_dilate` `matu8_write_raw` and the `rectangular_to_polar` pair (`transpose` 8, `flip_left_right` 8, `flip_up_down` 7, `assign` 7, `extract` 8, `number_equal` 7, `assign_zeropad` 7, `permute` 7, clamped `cast_to_*` 17, `cast_to_cpx` 2, `matcpx_cast_to_dbl`). No regressions in the linked run.

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
| Clamped `cast_to_*` | 17 of the 23 narrowing pairs with a `clamp` flag | 17/17 OK | `mutils_cast_clamp.cpp`. Float sources round with `floor(x + 0.5)`; low and high checks only where the source range exceeds the destination's; signed minimum is symmetric (-32767, -2147483647). Out of range: throw "overflow" unless `clamp`, then saturate. Work `size * 5.0`. |
| `cast_to_cpx`, `matcpx_cast_to_dbl` | 3 | 3/3 OK | `mutils_cast_cpx.cpp`. Work `size * 4.0`; `matcpx_cast_to_dbl` loops separately for `clamp` (real part) and not (nonzero imaginary throws "overflow"). |
| `cumulative_sum` | 7 typed | 7/7 OK | `mutils_cumsum.cpp`. Calls the typed `assign` export, then `d[i] = d[i - 1] + d[i]` over `dst`; work `0.0 + size`. `src` uses `check_src_f`; `dst` uses `validate` for dbl/flt/s32/u32 and `check_src_f` for s16/u16/u8. |
| Bit operations | `bit_shift_left/right`, `bit_and/or/xor_scalar`, `bit_and/or/xor` for u8/u16/u32 | 24/24 OK | `mutils_bits.cpp`. Data and `dst->size` in locals before the shape test; the scalar has the element type; work `size * 3.0`. The binary forms check `a` with `check_src_f` and `b`/`dst` cols-first. This resolves the binary forms parked earlier. |
| `realloc` | 8 typed | 8/8 OK | `mutils_realloc.cpp`. `check_src_f`; new dims stored before the byte comparison; grows by `allocate_array` + `memcpy` of the new size. |
| `abs` | dbl, flt, s32, s16 | 4/4 OK | `mutils_abs.cpp`. Bit-operation shape, `x >= 0 ? x : -x`. |
| `range` | 7 typed | 7/7 OK | `mutils_range.cpp`. `check_src_f`; data/size locals; `max` tested for null before `min`; `*max = *min = data[0]`; updates through the pointers; work `size * 5.0`. Resolves the parked `range` family. |
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
| `img*_generate_rectangle` | dbl, s32, u16, u8, univ | 5/5 OK | `img_generate.cpp`. `height`/`width < 1` returns "Illegal data type", `fg == bg` "Illegal data value", a rectangle fully outside returns the mismatch message; the progress locals come before the typed `assign_scalar(bg)` call; the clip is `row_end > rows ? rows : row_end`; work `height * 3.0 * width`. |
| `matuniv_histogram` | univ | 1/1 OK | Compare chain over dbl/s32/u16/u8; `hist` must be `EFI_UINT32`. |
| `img*_binary_dilate` | s32, u16 | 2/2 OK | `img_morph.cpp`. `src`/`se`/`dst` checks, shape and alias tests, `fg == bg` returns "Illegal data value"; `src->data` taken before the `assign_scalar(bg)` call; stamps `se` around each `fg` pixel; work `((se_cols * 3 + 3.0) * se_rows + 2.0) * cols`. |
| `matu8_write_raw` | u8 | 1/1 OK | `mutils_io.cpp`. Null `fp` returns "null pointer"; `fwrite` straight from `m->data` in chunks of at most `0xFFFFFFFF`; a short write returns "Input-output error"; work `size * 1000`. |
| `pgn*_rectangular_to_polar` | dbl, univ | 2/2 OK | `pgn_polar.cpp`. n×2 points to `(r, θ)`; `θ = atan(dy/dx)`, `+π` when `dx < 0`, then `+2π` when negative; `dx == 0` gives `π/2` or `3π/2`; work `rows * 20.0`. |
| `validate` | `mat{dbl,flt,s32,s16,u32,u16,u8,cpx}_validate` | 8/8 OK | One body (linker-folded in the original, `0x86460`). Reads `rows`/`cols` into locals first; the direct form picked another register. |
| `wrap_data` | 7 typed exports (no `cpx`) | 7/7 OK | Same body for all seven (`0x859e0`). |
| `malloc` | 8 typed exports | 8/8 OK | `allocate_mat<T>`; the allocator is `EfiCallContext`'s pointer at +80h. |
| `mat_set` | `matset_malloc`, `matset_free`, `matset_free_matrices`, `matset_validate` | 4/4 OK | `mutils_set.{h,cpp}`. `matset_free_matrices` is `validate` alone. |
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

- **`pgndbl_rectangular_from_polar`**: instructions match. The original converts `dst` in place (it only checks `src`). It fails on the pooled-constant neighbour of its `999999999.9999999` bound (Harness notes).

- Helper pattern for functions that return a message (added after the parked `sum`/`range` work): the exports call `validate` themselves, return the message literal themselves, and put only void/bool shared logic in `__forceinline` helpers (`same_shape`, `binary_elementwise`, `running_sum`). A message returned from an inlined template comes back in `rcx` and is copied to `rax`; the original returns it in `rax`. The helper pattern removes that copy.

## Remaining work

- 431 exports not yet implemented, plus the 51 parked above.
- The `siguniv_window_*` dispatchers (11) call typed `sigdbl_window_*` exports, so they cannot link until those exist; they wait for their typed exports.
- `matuniv_wrap_data` and `matuniv_wrap_matrix` dispatch through a jump table over a nine-case type enum, with different field copies per case.
- `mutil_realloc`: its `.pdata` extent conflicts with the code address (18-byte extent for a 71-byte body). Not implemented; parked for the maintainer.

## Harness notes

- **Data extents of pooled constants (2026-10-27).** For data, `native_linked.py` compares `max(candidate map-symbol extent, original object_end)`, and the original's `object_end` is the next referenced address. For 8-byte `__real@` constants, which the linker pools sorted by name, that span takes in the following constant. An export then passes or fails depending on whether an unrelated function brings the same neighbour constant into the candidate. Example: `matdbl_cast_to_s32` reads `__real@c1dfffffffc00000` (`-2147483647.0`, `0xAC9A8`). It matches while `matdbl_cast_to_flt` (which brings in `-1e37` at the next slot) is present and fails with `data +0x8: bytes differ` without it. Comparing a `__real@`/`__xmm@` constant over its own size would remove the dependence.
- **Blocker for the `*univ_*` dispatchers (2026-10-27): jump tables.** A dispatcher such as `matuniv_transpose` switches on the type tag (`EFI_UINT8` 0, `EFI_SINT8` 1, `EFI_UINT16` 2, `EFI_SINT16` 3, `EFI_UINT32` 4, `EFI_SINT32` 5, `EFI_FLOAT` 6, `EFI_DOUBLE` 7, `EFI_DCOMPLEX` 8; read from `matuniv_cast`'s tables) through `lea r10,[__ImageBase]` and `mov ecx,[r10+rax*4+<table RVA>]`, with the table of case-label RVAs placed right after the function. `native_linked.py` matches the `ADDRESS` pattern only for absolute image addresses, so it compares that `disp32` as raw bytes (`+0x37: bytes differ`). It also decodes the inline table as instructions, and the case RVAs differ in any relinked DLL. A fix would read a `disp32` used with the `__ImageBase` register as an RVA (local when inside the function), and compare the table entries as local offsets. Until then no switch-based dispatcher can pass. The source shape is found: `matuniv_transpose` agrees up to that instruction with a null check of both, a tag-equality check returning "Illegal data type", then `case EFI_DOUBLE: { if (const char *e = matdbl_transpose(...)) return e; } break;` per case in the order dbl, flt, u8, u16, u32, s16, s32, cpx, `default: return "Illegal data type";`, and `return nullptr;` after the switch. Similarity 0.85; the rest is the table. The dispatchers stay out of the DLL until the harness handles the table.
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
