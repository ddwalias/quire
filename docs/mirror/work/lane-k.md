# Lane K: utl.dll → Quire.Native.Utl

**Status: incomplete.** 17 of 100 functions pass strict byte validation, plus 1 data export (`s_tls_index`) present by name. Export parity: 82 missing, 0 extra.

- **Owner:** Lane K (native, `utl.dll`)
- **Branch:** `mirror/k-utl-202610241230`
- **Worktree:** `C:/Projects/Quire-wt/lane-k-utl`
- **Base commit:** `c11fb045` (current `main`)
- **Harness:** `tools/validation/native_bytes.py` on `main`, unchanged. Not `utl-mgd` (Lane D). `src/Quire.Engine.Utl` not touched.
- **Original:** `C:/Program Files/PTC/Mathcad Prime 12.0.1.0/utl.dll`, 100 exports, 191 imports.

## Completed groups

| Group | Functions | Strict result | Notes |
|---|---|---|---|
| `InterruptControl` (`RequestInterrupt`, two `operator=`) | 3 | 3/3 OK, 0 unverified | `utl_interrupt.cpp`. `interrupted` is the `bool` at offset 0. |
| `WordDatum` (ctor, `Name`, two `operator=`) | 4 | 4/4 OK, 0 unverified | `utl_word.cpp`. `name` is at offset 0. |
| `WordChoice` (ctor, `Datum`, `Score`, `CompareScores`, three `operator=`) | 6 | 6/6 OK, 0 unverified | `utl_word.cpp`. Layout `{WordDatum*, double}`. The copy `operator=` uses `memcpy` so MSVC emits the original's 16-byte `movups` form. |
| `with_purecall_handler::operator=` | 1 | 1/1 OK, 0 unverified | `utl_word.cpp`. |
| `InterruptControl::IsInterrupted`, `ThreadInitialize` (batch 2) | 2 | 2/2 OK, 0 unverified | `utl_interrupt.cpp`. Reads and writes `s_tls_index`, which the original exports, so the reference is verified. `Quire.Native.Mcaduser`'s only `utl.dll` import. |
| `GetNumberOfProcessors` (batch 2) | 1 | 1/1 OK, 0 unverified | `utl_system.cpp`. `GetSystemInfo` on a local `SYSTEM_INFO`. |
| `s_tls_index` (data, batch 2) | 0 | not byte-checked | Name present in export parity. The validator has no data-export check. Its TLS allocation is not yet ported. |

Totals: **17/17 functions OK, 0 unverified** under `--strict`. Command:
`python tools/validation/native_bytes.py --original "<install>/utl.dll" --candidate .mirror-cache/obj/utl/<file>.obj --strict --symbol <name>...`.

Export parity: `python tools/validation/native_bytes.py --original "<install>/utl.dll" --built-dll .mirror-cache/bin/Quire.Native.Utl.dll` gives **82 missing, 0 extra** (exit 1, expected until complete).

## Build

MSBuild, Release|x64, toolset v145, MSVC 14.51.36231. `/O2 /Gy /GS /EHsc /MD /std:c++17` (from the project's Release settings). Output in `.mirror-cache/bin/` and `.mirror-cache/obj/utl/`. Exports come from `Quire.Native.Utl.def` (not `__declspec`), because the original exports data symbols (`s_tls_index`) and compiler-generated members. The `.def` lists only implemented names.

## Toolchain evidence

- Original's Rich header: C++ objects are product 265 at build 36246 (`cl` 19.51.36246, same as D19's `utl-mgd`).
- This build's Rich header: product 261 at 36260 (`cl` 14.51.36231).
- Batch 1 bytes match despite the difference. Recorded per the brief, as Lane H did.

## Imports (D10)

`utl.dll` imports no Quire-owned DLL. Its 191 imports are all system or runtime DLLs (`KERNEL32`, `USER32`, `ADVAPI32`, `SHELL32`, `OLEAUT32`, `MSVCP140`, `VCRUNTIME140`, UCRT API sets). I interpret D10's `.def` pinning as applying to Quire-owned DLLs (as with `efiutl.def` for Lane H), so these bind to the SDK and toolset import libraries. Please confirm this is the intended reading. If you want per-DLL `.def` pins for system DLLs too, that is a follow-up.

## Remaining work

- 82 exports, in groups: `PerformanceTimer`, `TimeIt`, `WordData`, `WordChoice` (`BestGuess*`, `GetSuggestions`, `GetScore`), `with_purecall_handler` (ctor/dtor), `InterruptControl` (`IsInterrupted`, `ThreadInitialize`, `s_tls_index`, `s_tls_init`), `mcGetSetting`, timers, logging, paths, and the `std::` string/stream exports.
- Consumers (per the brief's priority):
  - `Quire.Native.Mcaduser` imports `?IsInterrupted@InterruptControl@utl@@SA_NXZ`. Done (batch 2).
  - `Quire.Native.EfisymbolicsUmgd` imports 10 exports. Among them, `?toBSTR@utl@@YAPEA_WIPEBD_K@Z` is strict-feasible (see below). The rest reference utl-internal data or helpers.
- Static initialisers (`s_tls_init`, the TLS index allocation) are not yet implemented. The DLL will not set `s_tls_index` until they are.

## Harness limitations (report to the maintainer; not worked around)

1. **Internal references block `--strict`.** Any function that references a utl-internal (non-exported) data item or a call to an internal helper is reported as unverified. Strict fails it, and the original's symbols are not available. Examples: `IsLoggingEnabled` (flag at `0x180022862`, helper `0x1800088E0`), `IsTimingEnabled`, `SafeTerminateProcess`, `getInstanceDirW`. Their byte content is not in question, but they cannot pass strict as the harness stands.
2. **Data exports are not byte-checked.** `s_tls_index`, `s_tls_init` and the `??_7TimeIt` vtable are exports of the original, but the validator checks only functions. Their names pass export parity; their contents are not verified.
3. **Ordinal imports cannot match.** `toBSTR` calls `SysAllocStringLen`, which the original imports from `OLEAUT32.dll` **by ordinal 4** (IAT slot `0x180016430`, shown as `#4`). The validator matches only `__imp_<name>` symbols, so a name-based candidate cannot match `#4`. Request: resolve ordinal imports through the imported system DLL's export table (`C:/Windows/System32/OLEAUT32.dll`, ordinal 4 = `SysAllocStringLen`), or allow a candidate to name the ordinal.

## Landing

Not landed. Commit only owned files on this branch. Ask the maintainer to land it. Do not use `lane_merge.py` (D10: its Release IL gate does not cover native code).
