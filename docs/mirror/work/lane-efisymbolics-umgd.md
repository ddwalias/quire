# Quire.Native.EfisymbolicsUmgd checkpoint

- Owner: native EfisymbolicsUmgd lane; branch `feature/native-efisymbolics-umgd-20261114`.
- Original exports found: 4. `_AxiomEval` is at RVA `0xC030`, 45 bytes; the other three are `_AxiomReinitialize`, `_RequestInterruptAxiom`, and `_UnRequestInterruptAxiom`.
- `_AxiomEval` calls private target `sub_9390` at call site RVA `0xC053`; target RVA `0x9390`, size 625 bytes (`0x271`).
- Source forms tried for the exported wrapper/helper: one failed compilation (the `void**` conversion); two built but the gate reported `length 14 != original 45`. The gate did not provide a differing byte offset because it stopped at the length check.
- The 625-byte `sub_9390` body is not yet written. No native source is retained in this lane.
