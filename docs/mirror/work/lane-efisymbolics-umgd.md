# Quire.Native.EfisymbolicsUmgd checkpoint

- Owner: native EfisymbolicsUmgd lane; branch `feature/efisymbolics-umgd-agy-20261116`.
- Original exports: 4. `_AxiomEval` is at RVA `0xC030`, 45 bytes (`?_AxiomEval@Efi@@YA_NPEA_WPEAPEA_WW4MuPadFilter@1@KK_N@Z`); calls private target `sub_9390` (`?sub_9390@Efi@@YA_NPEA_W0PEAPEA_WW4MuPadFilter@1@KK_N@Z`) at RVA `0x9390` (625 bytes, through RVA `0x9601`) via call at RVA `0xC053`.
- `_AxiomEval` implementation: exact 45 bytes achieved (`return sub_9390(expr, expr, result, filter, p4, p5, p6);`). The export is verified by RVA at its call site, but is not described as fully matched because its callee `sub_9390` is unverified under Decision D29.
- `sub_9390` is marked parked under Decision D29 in `docs/mirror/native/internal-targets.json` (`"parked": true`, RVA `0x9390`). Its bytes are not compared and its own targets are not followed, so it stays unverified.
- Source forms tried for `sub_9390`:
  * Form 1: Idiomatic C++ with `std::deque<Response> local_queue`, `CreateEventW`, `InitializeCriticalSection`, queue push to `g_queue`, wait loop, pop front: length 740 != 625. First differing offset: +0x00 (pushed 8 non-volatiles vs 5; inlined deque destructor).
  * Form 2: Out-of-line `ResponseQueue` destructor to match original `call 0xac40`: length 576 != 625. First differing offset: +0x00.
  * Form 3: Immediate `Request` initialization matching original parameter spill sequence: length 574 != 625. First differing offset: +0x00 (candidate pushes rbp, rbx, rsi, rdi, r12, r13, r14, r15 with `sub rsp, 0xa8`; original pushes rbp, rsi, rdi, r14, r15, spills rbx to `[rsp+0x20]` and rcx to `[rsp+8]`, with `sub rsp, 0xc0`).
- Gate result:
  * Export `_AxiomEval` passes (45 bytes exact, 1 relocation site verified against parked RVA `0x9390`).
  * Parked entry `?sub_9390@Efi@@YA_NPEA_W0PEAPEA_WW4MuPadFilter@1@KK_N@Z` passes by RVA under D29 (bytes not compared, targets not followed).
