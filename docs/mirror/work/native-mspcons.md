# Quire.Native.Mspcons (mspcons.dll) checkpoint

- **Owner:** AGY, Orca task `task_eb24dce5b51c`, dispatch `ctx_967a2773b576`.
- **Branch:** `feature/native-mspcons-agy`, from `main`.
- **Project:** `src/Quire.Native.Mspcons/Quire.Native.Mspcons.vcxproj`.
- **Gate:** `python tools/validation/native_gate.py --base main --project Quire.Native.Mspcons`.
- **Status:** PASS; 26 checked, 0 failures, 0 regressions, 1 unfinished (`??_7alloc@Unmanaged@Engine@@6B@`).

## Landed Exports in this Batch

1. `?get_tag@untagged@Unmanaged@Engine@@UEBAPEAVanyval@23@XZ`: RVA `0x11A0`, 8 bytes, 1 relocation site masked. Matches exact bytes returning `&notag_object` verified via manifest entry `notag_object`.
2. `?notag@untagged@Unmanaged@Engine@@SAPEAVanyval@23@XZ`: RVA `0x11A0`, 8 bytes, 1 relocation site masked. Exported and matched byte-for-byte.
3. `??_7anyval@Unmanaged@Engine@@6B@`: RVA `0x3210`, 56 bytes, 7 relocation sites masked. Exported and matched byte-for-byte.
4. `??_7untagged@Unmanaged@Engine@@6B@`: RVA `0x3290`, 56 bytes, 7 relocation sites masked. Exported and matched byte-for-byte.

Total exports in candidate `.def`: 25 (out of 28 original exports).
Export parity: 3 missing (`??_7alloc@Unmanaged@Engine@@6B@`, `?equals@anyval@Unmanaged@Engine@@QEBA_NPEBD@Z`, `?write@tagged@Unmanaged@Engine@@UEBAXAEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@@Z`), 0 extra.

## Remaining Exports and Concrete Blockers

1. `??_7alloc@Unmanaged@Engine@@6B@` (RVA `0x32D0`):
   - **Blocker:** Slot 0 references `??_Galloc@Unmanaged@Engine@@UEAAPEAXI@Z` (scalar deleting destructor of `alloc` at RVA `0x1030`), which is not exported by `mspcons.dll`. In the original binary, MSVC emitted a combined vector/scalar deleting destructor (`f6 c2 02 74 42`, testing bit 2) requiring whole-program LTCG or manifest entry, leaving slot 0 unverified under per-object compilation without manifest updates.

2. `?equals@anyval@Unmanaged@Engine@@QEBA_NPEBD@Z` (RVA `0x13C0`, 68 bytes):
   - **Blocker:** Inlines `dynamic_cast<pstring const*>(this)` and references internal RTTI type descriptor `??_R0?AVpstring@Unmanaged@Engine@@@8` at RVA `0x5138` and `pstring` layout, which are unexported internal targets not in `internal-targets.json`.

3. `?write@tagged@Unmanaged@Engine@@UEBAXAEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@@Z` (RVA `0x1410`, 269 bytes):
   - **Blocker:** Calls `operator<<(ostream&, char)` at RVA `0x16E0` and `operator<<(ostream&, char const*)` at RVA `0x1520`. As documented under Decision D25, these standard library ostream template instantiations diverge between MSVC 19.51.36246 and modern toolset inlines, preventing strict byte matching.
