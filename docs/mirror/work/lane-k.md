# Lane K: utl.dll → Quire.Native.Utl

**Status: incomplete.** 24 of 100 functions pass strict byte validation, plus 1 data export (`s_tls_index`) present by name. Byte-identical but not strict-passing: `IsLoggingEnabled` (its two flags sit in zero-filled data, limitation 1) and `toBSTR` (ordinal import, limitation 3). Not matching: the `PerformanceTimer` constructor and `Start`. Export parity: 70 missing, 0 extra.

- **Owner:** Lane K (native, `utl.dll`)
- **Branch:** `mirror/k-utl-202610241230`
- **Worktree:** `C:/Projects/Quire-wt/lane-k-utl`
- **Base commit:** `c11fb045` (current `main`)
- **Harness:** `tools/validation/native_bytes.py` on `main` (changed since `c11fb045`; see the maintainer response below). Not `utl-mgd` (Lane D). `src/Quire.Engine.Utl` not touched.
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
| `PerformanceTimer::GetLast`, `GetElapsed`, `GetAverage` (batch 6) | 3 | 3/3 OK, 0 unverified; the `1000.0` constant is verified through the manifest | `utl_timer.cpp`. Reads `current`, `total`, `count` and `frequency`. |
| `BoostThreadPriority` (batch 6, `utl_system.cpp`) | 1 | **not matching**: 65 bytes, original 66 | The original keeps the second call as a branch with `mov al,1` on the success path. Our forms fold it into `sete`. Only the original's shape remains to find. The branch logic as written returns true only when both calls return zero. |
| `IsLoggingEnabled` (batch 5, `utl_logging.cpp`) | 1 | **not counted**: bytes match, `--strict` fails on the two zero-filled flags (limitation 1) | Reads `HKCU`, then `HKLM` (`ReadRegistryFlag`, an anonymous-namespace helper kept out of line). The helper and its three strings are recorded in `docs/mirror/native/internal-targets.json` and verified through it. |
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
| `Quire.Native.EfisymbolicsUmgd` | 10 exports (see below) | **Blocked.** Byte-identical but not strict-passing: `IsLoggingEnabled` (zero-filled flags, limitation 1) and `toBSTR` (ordinal import, limitation 3). Not implemented: 8. Three of those reference zero-filled data (`SafeTerminateProcess`, `SafeTerminateOnExit`, and `IsLoggingEnabled`). The rest use large STL code (`GetAppRelativeFS`, `GetDirectoryName`, `Win32ErrorText`, `mcGetEnvironmentVariable`) or the post-call `nop` shape (`getInstanceDirW`). |

`EfisymbolicsUmgd` imports: `getInstanceDirW`, `SafeTerminateProcess`, `mcGetEnvironmentVariable` (wide), `SafeTerminateOnExit`, `Log` (char), `IsLoggingEnabled`, `GetDirectoryName`, `GetAppRelativeFS` (wide), `Win32ErrorText`, `toBSTR`.

## Remaining work

- 74 exports missing from export parity. Written but not counted: `IsLoggingEnabled`, `toBSTR`, the `PerformanceTimer` constructor, and `Start`.
- **Open:** the constructor and `Start`. Our build is 9 and 8 bytes shorter. The original has a `this` spill and `nop`s after `CRITICAL_SECTION` calls that our build lacks. `/EHa` (built and validated, all 21 functions unchanged) does not produce them, so the cause is a source shape not yet found. Not guessed.
- Then the non-consumer groups: `TimeIt`, `WordData`, `WordChoice` (`BestGuess*`, `GetSuggestions`, `GetScore`), `with_purecall_handler` (ctor/dtor), `mcGetSetting`, timers, logging, paths, and the `std::` string/stream exports.
- Static initialisers (`s_tls_init`, the TLS index allocation) are not yet implemented. The DLL will not set `s_tls_index` until they are.

## Harness limitations (report to the maintainer; not worked around)

1. **Zero-filled data cannot be recorded.** `native_targets.py` rejects a data entry outside the original's raw section bytes (`PeImage.in_section` uses `min(vsize, raw_size)`). The `.data` section here has raw size `0x800` but virtual size `0xA20`, so the statics in its tail are not recordable. Affected: `IsLoggingEnabled` (`0x22862`, `0x22863`), `IsTimingEnabled` and `IsTimingInclusive` (`0x229D1`, `0x229D2`), and zero-filled references in `SafeTerminateProcess` and `SafeTerminateOnExit`. Request: accept zero-filled bytes as zeros, or record them from the section's virtual size. The latest `main` (`70c6f8e5`) reads uninitialised candidate `.bss` as zeros, but it still rejects original zero-filled data, so this remains open.
2. **Data exports are not byte-checked.** `s_tls_index`, `s_tls_init` and the `??_7TimeIt` vtable are exports of the original, but the validator checks only functions. Their names pass export parity; their contents are not verified.
3. **Ordinal imports cannot match.** `toBSTR` calls `SysAllocStringLen`, which the original imports from `OLEAUT32.dll` **by ordinal 4** (IAT slot `0x180016430`, shown as `#4`). The validator matches only `__imp_<name>` symbols, so a name-based candidate cannot match `#4`. Request: resolve ordinal imports through the imported system DLL's export table (`C:/Windows/System32/OLEAUT32.dll`, ordinal 4 = `SysAllocStringLen`), or allow a candidate to name the ordinal.

**Maintainer response ([D24](../decisions.md)):**

1. Fixed: section tails past the raw size read as zeros on both sides, so the zero-filled statics are recordable.
   Record `IsLoggingEnabled`'s `initialized`/`enabled` statics with `native_targets.py`; they are its last 4
   unverified targets.
2. Fixed: data exports the original exports are byte-compared at the export's RVA for the candidate's size.
3. Fixed: an ordinal import is named after the system DLL's export with that ordinal; retry `toBSTR`.

Also changed: strict is the default (`--allow-unverified` for exploration; `--strict` is accepted and does
nothing), export/import targets check the relocation addend, manifest names match regardless of the
`?A0x<hash>` tag (the `ReadRegistryFlag` entry now resolves from any checkout), and `lane_merge.py` runs
`native_gate.py` for changed native projects, so batches land through the normal merge.

## Internal targets (manifest)

Recorded in `docs/mirror/native/internal-targets.json` with `native_targets.py`, using the reviewed procedure: each entry is a name the candidate defines, recorded at an RVA that a function which already matches references. Batch 6 adds `__real@408f400000000000` (`1000.0`, 8 bytes at `0x194a8`).

- `?ReadRegistryFlag@?A0x…@utl@@YA_NPEB_W@Z`: RVA `0x88e0`. The helper's bytes match the original.
- `??_C@_1BM@…` (`EngineLogging`): RVA `0x190d0`, 28 bytes.
- `??_C@_1FO@…` (`Software\Mathsoft\Mathcad Engine 1\AppSettings`): RVA `0x19210`, 94 bytes.
- `??_C@_1GO@…` (`Software\Mathsoft\Mathcad Engine 1\Default\AppSettings`): RVA `0x191a0`, 110 bytes.

**D19 question for the maintainer:** these string constants are the registry key and value names the original reads. Matching their bytes needed their exact content, which I read from the original into `.mirror-cache` and typed into source. Please confirm that registry names count as interface constants, not original bytes copied into a build output. If they do not, `IsLoggingEnabled` and the other registry-reading exports cannot be matched.

## Corrected field naming (batch 6)

`GetStdDev` reads `[rcx]` and `[rcx+8]` as the begin and end of a vector of doubles, and `Reset` clears it (`end = begin`). So `PerformanceTimer`'s first three words are a sample vector (begin, end, capacity), not the placeholder names used before. `GetStdDev` (121 bytes) also tail-calls an internal CRT routine at `0x12E73` for a negative variance. It is not ported yet.

## Open shape question (not resolved)

Several original functions have a `nop` after a call: the `CRITICAL_SECTION` calls in the `PerformanceTimer` constructor and `Start`, and the `std::wstring` builder call in `getInstanceDirW`. Our builds have no such `nop`s, and `/EHa` does not add them. `BoostThreadPriority` has a different branch shape with no `nop`. Until the source shapes are identified, these functions will not match.

## Landing

Not landed. Batch 1 is `c318361a`, batch 2 is `96321fac`, batch 3 (`toBSTR`) is `1d6b254b`, batch 4 (`PerformanceTimer`) is `db9dcb57`, and batch 5 (`IsLoggingEnabled`, manifest) is `5a1d652d`, and batch 6 (timer getters, `BoostThreadPriority`) is committed after it. Ask the maintainer to land the branch. Do not use `lane_merge.py` (D10: its Release IL gate does not cover native code).
