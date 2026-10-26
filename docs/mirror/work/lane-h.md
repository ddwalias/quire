# Lane H: efistatsci_.dll → Quire.Native.EfistatsciImpl

**Status: incomplete.** 77 of 781 exports pass the linked `/GL` gate (`native_linked.py`, D30): the 70 earlier exports and the seven `sum` exports (`mutils_sum.cpp`, shape from the maintainer brief). No regressions in the linked run.

- **Owner:** Lane H (native, `efistatsci_.dll`)
- **Branch:** `mirror/h-efistatsci-20261025h1` (from `main` at `31b4238b`)
- **Worktree:** `C:/Projects/Quire-wt/lane-h-efistatsci-2`
- **Harness:** `tools/validation/native_bytes.py` and `native_targets.py` on `main`, unmodified.
- **Build:** MSBuild `Quire.Native.EfistatsciImpl.vcxproj`, Release|x64, toolset v145, `cl` 19.51.36246 from `C:\BuildTools\VS18.6-msvc`, `/O2 /Gy /GS /EHa /MD /std:c++17 /W4 /WX` with `_CRTIMP2=__declspec(dllimport)` (D28), and `WholeProgramOptimization` true (`/GL` + LTCG). The original objects are `/GL` (D25); this build reproduces its entry code (`mov r10, rcx`), where the per-object build gives `mov r9, rcx`. Output in `.mirror-cache/bin/`.
- **Cast family:** the six `matu8_cast_to_*` exports and twelve widening casts from `u16`, `s16`, `u32`, `s32` and `float` (`mutils_cast.cpp`) match in the linked `/GL` DLL. They use the validate forms that fixed `assign_submat` (`src` fresh-field, `dst` cols-first).
- **Original disassembly:** `.mirror-cache/orig/efistatsci.asm` (dumpbin, ignored, D15).

## Completed groups

| Family | Exports | Strict result | Notes |
|---|---|---|---|
| `assign_scalar` | `matdbl`, `matflt`, `mats32`, `mats16`, `matu32`, `matu16`, `matu8` | 7/7 OK | `assign_scalar<T>` in `mutils_mat.h`. |
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

- **`matcpx_assign_scalar`** (`0x493c0`, 219 bytes): candidate 206 bytes with the plain loop. Shapes tried, all differ: plain loop 206; pointer loop 217; `std::fill` 217; `std::fill_n` 218; store element 0 then copy forward 218; `_dcomplex` as `double v[2]` 206. The original stores the value as one 16-byte `movups` and fills the rest with `rep movs` over overlapping element pairs. Flags not searched (D25).
- **`sum` family** (7: `matdbl_sum` `0x7c650`, `matflt_sum` `0x7c780`, `mats{32,16}_sum`, `matu{32,16,8}_sum`): the body matches except for the epilogue. The candidate is 2 bytes longer for the integer forms (219 vs 217) and for `dbl` (299 vs 297), because the two `const char *` returns merge into one register (`lea rcx` + `mov rax, rcx`; `xor ecx` + `mov eax, ecx`). The original returns `lea rax` / `xor eax` directly. Five declaration orders of the locals were tried, all 219. `flt` is 302 vs 287 (the original unrolls by four).
- **`range` family** (5: `mats{32,16}_range`, `matu{32,16,8}_range`): the same return-merge (`lea rcx`, `mov rax, rcx`, +3 bytes per return), plus the original places its null-pointer return block inline, so its jumps are short (rel8); the candidate puts that block at the end, with rel32 jumps. Both forms tried; no reordering of the checks is allowed.
- **`matu8_bit_and/or/xor`, `matu16_*`, `matu32_*`** (9): same length as the original (662/664/661) and the vectorized loop matches; the candidate differs in one register choice (`b`'s `rows * cols` operand order in `validate`) and carries an unverified `3.0` constant (`__real@4008000000000000`). That constant can only be recorded through a matching function that uses it, and none does yet. Parked; the helpers stay in `mutils_mat_ops.h`.
- **`matset_verify_allsame`** (`0x70fb0`, 302 bytes): 276 bytes with the `-1` guards written as default-then-override. The original keeps the guards and its loop layout differs; not resolved.
- **`range`, `mats32`/`mats16`/`matu32`/`matu16`/`matu8`** (5): with the message return moved into the export, the candidate is 264-280 bytes against 257-277. The original keeps `size` in `rbx` (`push rbx`) and `max` in `eax`; the candidate uses different registers from the first instruction. A sweep over the validate form, null-check form, loop form, value locals and load order (48 combinations) found no match.
- **`matu8_cast_to_dbl`** (sweep over loop-bound and local-size forms, 4 combinations): still unrolls or differs; no match.
- **`matu8_cast_to_*`** (6: `dbl`, `flt`, `s16`, `u16`, `s32`, `u32`): forced inlining gives the same loop as a template, but the candidate unrolls the loop four times (437 bytes against the original's 285). The original keeps the loop un-unrolled, because it reloads `src->size` each iteration. Binding the loop to `src->size` directly still unrolled, so the compiler proved the bound invariant where the original did not. Not solved by source shape yet.

- Helper pattern for functions that return a message (added after the parked `sum`/`range` work): the exports call `validate` themselves, return the message literal themselves, and put only void/bool shared logic in `__forceinline` helpers (`same_shape`, `binary_elementwise`, `running_sum`). A message returned from an inlined template comes back in `rcx` and is copied to `rax`; the original returns it in `rax`. The helper pattern removes that copy.

## Remaining work

- 700 exports not yet implemented, plus the 33 parked above.
- The `siguniv_window_*` dispatchers (11) call typed `sigdbl_window_*` exports, so they cannot link until those exist; they wait for their typed exports.
- `matuniv_wrap_data` and `matuniv_wrap_matrix` dispatch through a jump table over a nine-case type enum, with different field copies per case.
- The `add`/`subtract`/`multiply_elem` families (about 680 bytes each for `int` types) are auto-vectorized with runtime alias checks, so the source shape has to reproduce the vectorizer's output.
- `mutil_realloc`: its `.pdata` extent conflicts with the code address (18-byte extent for a 71-byte body). Not implemented; parked for the maintainer.

## Harness notes

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
