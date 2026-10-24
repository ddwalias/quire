# Lane K: utl.dll → Quire.Native.Utl

**Status: incomplete.** 21 of 100 functions pass strict byte validation, plus 1 data export (`s_tls_index`) present by name. Two `PerformanceTimer` functions (constructor, `Start`) are present but do not match in length (see below). `toBSTR` is byte-identical but fails strict on the ordinal-import check (limitation 3), so it is not counted. Export parity: 75 missing, 0 extra.

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
| `PerformanceTimer`: `Pause`, `Resume`, `Reset`, `GetCount` (batch 4) | 4 | 4/4 OK, 0 unverified | `utl_timer.cpp`. Layout inferred from bodies; field names beyond `started`/`frequency`/`count`/`lock`/`depth` are guesses (`anchor`, `anchor_copy`, `current`, `total`). |
| `PerformanceTimer` constructor, `Start` (batch 4) | 2 | **not matching**: lengths 85 vs 94 and 90 vs 98 | The original has a `mov [rsp+8],rcx` spill of `this` and `nop`s after the `CRITICAL_SECTION` calls. The source shape producing these is not identified. `/EHa` was built and tested and changes nothing (see below), so the project stays on `/EHsc`. |
| `toBSTR` (batch 3, `utl_string.cpp`) | 1 | **not counted**: bytes identical, strict fails on the ordinal import `#4` (limitation 3) | Two-pass `MultiByteToWideChar` plus `SysAllocStringLen`. The reason text shows only the import-name mismatch, with no byte difference. |

Totals: **21 functions OK, 0 unverified** under `--strict`; 2 `PerformanceTimer` functions not matching. Command:
`python tools/validation/native_bytes.py --original "<install>/utl.dll" --candidate .mirror-cache/obj/utl/<file>.obj --strict --symbol <name>...`.

Export parity: `python tools/validation/native_bytes.py --original "<install>/utl.dll" --built-dll .mirror-cache/bin/Quire.Native.Utl.dll` gives **75 missing, 0 extra** (exit 1, expected until complete).

## Build

MSBuild, Release|x64, toolset v145, MSVC 14.51.36231. `/O2 /Gy /GS /EHsc /MD /std:c++17` (from the project's Release settings). Output in `.mirror-cache/bin/` and `.mirror-cache/obj/utl/`. Exports come from `Quire.Native.Utl.def` (not `__declspec`), because the original exports data symbols (`s_tls_index`) and compiler-generated members. The `.def` lists only implemented names.

## Toolchain evidence

- EH mode test: `/EHa` (ExceptionHandling `Async`) was built and every object revalidated. The 21 passing functions are unchanged, and the two `PerformanceTimer` mismatches are unchanged too. The project stays on `/EHsc`, as the brief specifies. The original does import `__C_specific_handler` and exports `se_trans_func`, so SEH translation is used somewhere in `utl.dll`.
- Original's Rich header: C++ objects are product 265 at build 36246 (`cl` 19.51.36246, same as D19's `utl-mgd`).
- This build's Rich header: product 261 at 36260 (`cl` 14.51.36231).
- Batch 1 bytes match despite the difference. Recorded per the brief, as Lane H did.

## Imports (D10)

`utl.dll` imports no Quire-owned DLL. Its 191 imports are all system or runtime DLLs (`KERNEL32`, `USER32`, `ADVAPI32`, `SHELL32`, `OLEAUT32`, `MSVCP140`, `VCRUNTIME140`, UCRT API sets). I interpret D10's `.def` pinning as applying to Quire-owned DLLs (as with `efiutl.def` for Lane H), so these bind to the SDK and toolset import libraries. Please confirm this is the intended reading. If you want per-DLL `.def` pins for system DLLs too, that is a follow-up.

## Consumer status

| Consumer | `utl.dll` imports | Status |
|---|---|---|
| `Quire.Native.Mcaduser` | `IsInterrupted` | **Done** (batch 2, strict OK). Its only `utl` import is present. |
| `Quire.Native.EfisymbolicsUmgd` | 10 exports (see below) | **Blocked.** 1 byte-identical but import-blocked (`toBSTR`); 9 not implemented. Most reference utl-internal data or helpers (limitation 1) or use large STL code (`GetAppRelativeFS`, `GetDirectoryName`, `Win32ErrorText`). |

`EfisymbolicsUmgd` imports: `getInstanceDirW`, `SafeTerminateProcess`, `mcGetEnvironmentVariable` (wide), `SafeTerminateOnExit`, `Log` (char), `IsLoggingEnabled`, `GetDirectoryName`, `GetAppRelativeFS` (wide), `Win32ErrorText`, `toBSTR`.

## Remaining work

- 75 exports. Written but not counted: `toBSTR`, the `PerformanceTimer` constructor, and `Start`.
- **Open:** the constructor and `Start`. Our build is 9 and 8 bytes shorter. The original has a `this` spill and `nop`s after `CRITICAL_SECTION` calls that our build lacks. `/EHa` (built and validated, all 21 functions unchanged) does not produce them, so the cause is a source shape not yet found. Not guessed.
- Then the non-consumer groups: `TimeIt`, `WordData`, `WordChoice` (`BestGuess*`, `GetSuggestions`, `GetScore`), `with_purecall_handler` (ctor/dtor), `mcGetSetting`, timers, logging, paths, and the `std::` string/stream exports.
- Static initialisers (`s_tls_init`, the TLS index allocation) are not yet implemented. The DLL will not set `s_tls_index` until they are.

## Harness limitations (report to the maintainer; not worked around)

Per the brief, these are reported and not worked around. Decisions needed from the maintainer before the remaining consumer exports can pass strict:

1. **Internal references block `--strict`.** Any function that references a utl-internal (non-exported) data item or a call to an internal helper is reported as unverified. Strict fails it, and the original's symbols are not available. Examples: `IsLoggingEnabled` (flag at `0x180022862`, helper `0x1800088E0`), `IsTimingEnabled`, `SafeTerminateProcess`, `getInstanceDirW`. Their byte content is not in question, but they cannot pass strict as the harness stands.
2. **Data exports are not byte-checked.** `s_tls_index`, `s_tls_init` and the `??_7TimeIt` vtable are exports of the original, but the validator checks only functions. Their names pass export parity; their contents are not verified.
3. **Ordinal imports cannot match.** `toBSTR` calls `SysAllocStringLen`, which the original imports from `OLEAUT32.dll` **by ordinal 4** (IAT slot `0x180016430`, shown as `#4`). The validator matches only `__imp_<name>` symbols, so a name-based candidate cannot match `#4`. Request: resolve ordinal imports through the imported system DLL's export table (`C:/Windows/System32/OLEAUT32.dll`, ordinal 4 = `SysAllocStringLen`), or allow a candidate to name the ordinal.

## Landing

Not landed. Batch 1 is `c318361a`, batch 2 is `96321fac`, batch 3 (`toBSTR`) is committed after it, and batch 4 (`PerformanceTimer`) is the last. Ask the maintainer to land the branch. Do not use `lane_merge.py` (D10: its Release IL gate does not cover native code).
