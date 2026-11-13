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
