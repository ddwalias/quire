# Lane mcaduser: mcaduser.dll memory and interrupt exports

Owner: mcaduser worker. Branch: `native-mcaduser-20261116-work`. Project: `Quire.Native.Mcaduser`.
Native gate: PASS (5 checked, 5 exact).

Matched: `MathcadAllocate`, `MathcadFree`, `MathcadArrayAllocate`, `MathcadArrayFree`, `isUserInterrupted`.

Not yet matched (left out of the `.def`): `CreateUserErrorMessageTable` (RVA 0x6490, 1789 bytes, wide-string table
building with a stack cookie), `CreateUserFunction` (0x5930, 2905 bytes), `LoadAndRegisterUserEfis` (0x47F0, 4407 bytes;
directory scan with `FindFirstFileW` and `LoadLibraryW`).

Notes:
- `isUserInterrupted` calls `utl::InterruptControl::IsInterrupted` through the IAT (`__declspec(dllimport)`). The import
  library `utl.lib` is generated from `utl.def` by a pre-link `lib.exe` step (the EfistatsciImpl stopgap, D10).
- `MathcadAllocate` keeps the null path as a separate return (`if (address) { ...; return address; } return nullptr;`).
  The merged-return form is 11 bytes shorter than the original.
- `MathcadArrayFree` stores zero into the first column pointer before freeing each block, as the original does.
- `MathcadArrayAllocate` is in its own translation unit so `MathcadAllocate` is called, not inlined. It zeroes `cols`
  before `rows` in the no-allocation case, which is the original's `xor edi, edi` before `xor r15d, r15d`.

## Parked: the three remaining exports (checkpoint on main 107b0b324)

Decision (owner): do not port the helpers below and do not change the build to `/GL`. The three exports stay
unmatched, left out of the `.def`, and no source changes are made for them. Remaining mismatches: 3 of 8 exports
(`LoadAndRegisterUserEfis`, `CreateUserFunction`, `CreateUserErrorMessageTable`). Native gate on main: PASS, 5 checked,
5 exact, 0 unfinished.

Blocker 1: 31 internal call targets of the three exports are not in `docs/mirror/native/internal-targets.json`, which has
no `mcaduser.dll` entry. Each must be recorded, and its candidate definition must match, before any caller can verify.
Targets (original RVAs, hex): 13f0, 1460, 1670, 1720, 1900, 1b30, 1de0, 1e50, 2590, 26e0, 2a50, 2b40, 2c30, 2ee0,
3110, 3300, 3350, 3360, 3780, 37a0, 37b0, 3b90, 475c, 4762, 6e40, 7200, 76f0, 8420, 9078, 907e, 9084. Of these, 22
have bodies (from 18 to 658 bytes in `.pdata`) and 9 are import thunks or jump stubs (3300, 3350, 37a0, 3b90, 475c,
4762, 9078, 907e, 9084). Several bodies use Mathcad object-model access through virtual-base tables, not only STL.

Blocker 2: the Rich header of `mcaduser.dll` carries the `0x0109` product entry with build 36246, which D25 ties to
`/GL` objects. Per-object parity may not be reachable for these functions. The question of linked-mode comparison
(D30, `WholeProgramOptimization`) is open and is not decided here; the project stays per-object `/O2` for now.

What the exports do, from the original's strings: `LoadAndRegisterUserEfis` scans a directory for user DLLs and
writes Mpl binding text (`// Mpl bindings for UserEfi:`, `import McdRun;`, `function_registry`, registration into
`[efiutl]Efi.User.Function`); `CreateUserFunction` writes Mpl `user_function` declarations; `CreateUserErrorMessageTable`
writes the `err_table_` text. Original bytes stay in the ignored `.mirror-cache`.
