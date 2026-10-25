# Lane H: efistatsci_.dll → Quire.Native.EfistatsciImpl

**Status: incomplete.** 16 of 781 exports pass strict byte validation (0 unverified targets). Export parity: 764 missing, 0 extra.

- **Owner:** Lane H (native, `efistatsci_.dll`)
- **Branch:** `mirror/h-efistatsci-20261025h1` (from `main` at `31b4238b`)
- **Worktree:** `C:/Projects/Quire-wt/lane-h-efistatsci-2`
- **Harness:** `tools/validation/native_bytes.py` and `native_targets.py` on `main`, unmodified.
- **Build:** MSBuild `Quire.Native.EfistatsciImpl.vcxproj`, Release|x64, toolset v145, `cl` 19.51.36246 from `C:\BuildTools\VS18.6-msvc`, `/O2 /Gy /GS /EHsc /MD /std:c++17 /W4 /WX`. Output in `.mirror-cache/bin/`.
- **Original disassembly:** `.mirror-cache/orig/efistatsci.asm` (dumpbin, ignored, D15).

## Completed groups

| Family | Exports | Strict result | Notes |
|---|---|---|---|
| `assign_scalar` | `matdbl`, `matflt`, `mats32`, `mats16`, `matu32`, `matu16`, `matu8` | 7/7 OK, 0 unverified | One template `assign_scalar<T>` in `mutils_mat.h`; one-line exports in `mutils_mat.cpp`. |
| `mutil_interrupt` | `mutil_interrupt` | 1/1 OK | `mutils_interrupt.cpp`, over the shared inline in `efi_common.h`. |
| Trivial (`ret 0` / `xor eax,eax; ret`) | `mutil_abort`, `mutil_msg_print`, `mutil_msg_print_line`, `mutil_msg_print_time`, `mutil_msg_set_level`, `mutil_rand_end` | 6/6 OK | `mutils_msg.{h,cpp}`. `_mutil_msg_level` values not inferred yet. |
| Allocator wrappers | `mutil_malloc`, `mutil_calloc` | 2/2 OK | `mutils_alloc.{h,cpp}`. |

Shared foundation (`src/Quire.Native.EfistatsciImpl/efi_common.h`):
- `Efi::Throw`: `[[noreturn]] __declspec(dllimport)`, import `?Throw@Efi@@YAXPEBD_K@Z` (IAT `0xac220`), pinned in `efiutl.def`.
- `typed_mat<T>`: `rows` @0, `cols` @4, `size` @8, `data` @10h. Each element type derives from it (`_double_mat`, ...).
- `mutil_interrupt_step` (inline `double`, 1e6 in the image) and `mutil_interrupt_inline` (returns `unsigned char`).

Manifest entries recorded with `native_targets.py` (`docs/mirror/native/internal-targets.json`, `efistatsci_.dll`): the literals `"array size mismatch"` (`0xac368`) and `"null pointer"` (`0xac3d8`), the step static (`0xcc0c0`) and the `1e7` constant (`0xac908`).

## Facts from machine code

- `mutil_interrupt` returns `unsigned char` (mangled `E`), not `bool`.
- The step static's file image holds **1000000.0** (`00 00 00 00 80 84 2E 41`), not zero. The `1e7` fallback runs only when the step reads zero. The maintainer note says it is "lazily set to 10000000.0"; the image says otherwise, so the initializer is 1e6.
- `_univ_mat`'s type tag is a 32-bit `int` at `+18h` (`movsxd rax, dword [r8+18h]` in `matuniv_assign_scalar`).
- Matrix layout checks are identical across all eight element types: null, `rows <= 0 || cols <= 0`, data null, and `size != rows * cols` each reach `Efi::Throw`.

## Parked (D25)

- **`matcpx_assign_scalar`** (`0x493c0`, 219 bytes): the candidate is 206 bytes with the plain loop, and every shape tried differs.
  - `for (int i) data[i] = value` → 206 bytes.
  - Pointer loop → 217.
  - `std::fill` → 217.
  - `std::fill_n` → 218.
  - Store element 0, then copy it forward → 218.
  - `_dcomplex` declared as `double v[2]`, with the loop → 206.
  - The original stores the value as one 16-byte `movups`, then fills the rest with `rep movs` over overlapping element pairs. No flags searched (D25).
  - The export stays in the DLL for name parity; its bytes are unverified.

## Remaining work

- 764 exports unimplemented. Next families in maintainer order: `siguniv_window_*` (11 exports, 56–62 bytes each), then `range`, `min`/`max`, `entropy`, then the `*univ_*` dispatchers and the internal-helper bodies.
- `mutil_realloc`: its `.pdata` extent conflicts with the code address (18-byte extent for a 71-byte body). Not implemented; parked for the maintainer.

## Harness notes

- Strict default and manifest route work as documented in `native_bytes.py` (D23–D26). No harness changes made.
- Product ID (Rich header) not compared; every matching function here matched without it.

## Landing

`lane_merge.py` runs `native_gate.py` for native projects (D24). See the checkpoint commit for the result.

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
`s16` 172, `u8` 173 bytes) under `/O2 /Gy /GS /EHsc /MD /std:c++17`, with the BuildTools 36246 toolset:

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
