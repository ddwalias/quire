# Quire.Native.EfisymbolicsUmgd checkpoint

- Owner: native EfisymbolicsUmgd lane; branch `feature/efisymbolics-umgd-agy-20261116`.
- Original exports: 4. `_AxiomEval` is at RVA `0xC030`, 45 bytes (`?_AxiomEval@Efi@@YA_NPEA_WPEAPEA_WW4MuPadFilter@1@KK_N@Z`); calls private target `sub_9390` (`?sub_9390@Efi@@YA_NPEA_W0PEAPEA_WW4MuPadFilter@1@KK_N@Z`) at RVA `0x9390` (625 bytes, through RVA `0x9601`) via call at RVA `0xC053`.
- `_AxiomEval` implementation: exact 45 bytes achieved (`return sub_9390(expr, expr, result, filter, p4, p5, p6);`).
- `sub_9390` source forms tried:
  * Form 1: Idiomatic C++ with `std::deque<Response> local_queue`, `CreateEventW`, `InitializeCriticalSection`, queue push to `g_queue`, wait loop, pop front: length 740 != 625. First differing offset: +0x00 (pushed 8 non-volatiles vs 5; inlined deque destructor).
  * Form 2: Out-of-line `ResponseQueue` destructor to match original `call 0xac40`: length 576 != 625. First differing offset: +0x00.
  * Form 3: Immediate `Request` initialization matching original parameter spill sequence: length 574 != 625. First differing offset: +0x00 (candidate pushes rbp, rbx, rsi, rdi, r12, r13, r14, r15 with `sub rsp, 0xa8`; original pushes rbp, rsi, rdi, r14, r15, spills rbx to `[rsp+0x20]` and rcx to `[rsp+8]`, with `sub rsp, 0xc0`).
- Gate result:
  * `Quire.Native.EfisymbolicsUmgd::?_AxiomEval@Efi@@YA_NPEA_WPEAPEA_WW4MuPadFilter@1@KK_N@Z: call target differs: ?sub_9390@Efi@@YA_NPEA_W0PEAPEA_WW4MuPadFilter@1@KK_N@Z at +0x24: manifest entry does not match: length 574 != original 625`
  * `Quire.Native.EfisymbolicsUmgd::?sub_9390@Efi@@YA_NPEA_W0PEAPEA_WW4MuPadFilter@1@KK_N@Z: length 574 != original 625`
  * `Native bytes: FAIL; 2 checked`
- Blocker: The Rich header of `efisymbolics-umgd.dll` contains entry `0x0109` build 36246 (D25: original built with `/GL` LTCG WholeProgramOptimization). Per-object compilation under MSVC 14.51 cannot reproduce the compiler-internal register allocation and frame layout of MSVC 14.36 `/GL` for private routine `sub_9390`. When `sub_9390` is parked under D29, `_AxiomEval` passes 45 bytes exact (1 relocation site masked); without parking, the native gate fails on `length 574 != original 625`.
