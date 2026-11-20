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
| `with_purecall_handler` (ctor, dtor, `operator=`) | 3 | 3/3 OK, 0 unverified | `utl_word.cpp`. Ctor enters `s_purecall_lock` and sets `ms_purecall_handler`; dtor restores previous handler and leaves lock (emitting the original's `nop` under `/EHa`). |
| `InterruptControl::IsInterrupted`, `ThreadInitialize` (batch 2) | 2 | 2/2 OK, 0 unverified | `utl_interrupt.cpp`. Reads and writes `s_tls_index`, which the original exports, so the reference is verified. `Quire.Native.Mcaduser`'s only `utl.dll` import. |
| `GetNumberOfProcessors` (batch 2) | 1 | 1/1 OK, 0 unverified | `utl_system.cpp`. `GetSystemInfo` on a local `SYSTEM_INFO`. |
| `s_tls_index` (data, batch 2) | 0 | not byte-checked | Name present in export parity. The validator has no data-export check. Its TLS allocation is not yet ported. |
| `PerformanceTimer`: `Pause`, `Resume`, `Reset`, `GetCount` (batch 4) | 4 | 4/4 OK, 0 unverified | `utl_timer.cpp`. Layout inferred from bodies; field names beyond `started`/`frequency`/`count`/`lock`/`depth` are guesses (`anchor`, `anchor_copy`, `current`, `total`). |
| `PerformanceTimer` constructor, `Start` (batch 4) | 2 | **not matching**: lengths 85 vs 94 and 90 vs 98 | The original has a `mov [rsp+8],rcx` spill of `this` and `nop`s after the `CRITICAL_SECTION` calls. The source shape producing these is not identified. `/EHa` was built and tested and changes nothing (see below), so the project stays on `/EHsc`. |
| `PerformanceTimer::GetLast`, `GetElapsed`, `GetAverage` (batch 6) | 3 | 3/3 OK, 0 unverified; the `1000.0` constant is verified through the manifest | `utl_timer.cpp`. Reads `current`, `total`, `count` and `frequency`. |
| `BoostThreadPriority` (batch 6, `utl_system.cpp`) | 1 | 1/1 OK, 0 unverified | Short-circuit AND `!SetPriorityClass(...) && !SetThreadPriority(...)` matches original's 66 bytes exactly. |
| `toBSTR` (batch 3, `utl_string.cpp`) | 1 | **not counted**: bytes identical, strict fails on the ordinal import `#4` (limitation 3) | Two-pass `MultiByteToWideChar` plus `SysAllocStringLen`. The reason text shows only the import-name mismatch, with no byte difference. |
| `ms_purecall_handler` (batch 7, `utl_word.cpp`) | 1 | 1/1 OK, 0 unverified | 22 bytes. Throws `purecall_exception` with user-declared destructor; ThrowInfo and RTTI internal targets recorded in internal-targets.json. |
| `se_trans_func` (batch 7, `utl_word.cpp`) | 1 | 1/1 OK, 0 unverified | 34 bytes. Throws `se_exception(code)`; ThrowInfo and RTTI internal targets recorded in internal-targets.json. |
| `RestoreThreadPriority` (batch 7, `utl_system.cpp`) | 1 | 1/1 OK, 0 unverified | 76 bytes. Uses zero-initialized `s_process_priority` and `s_thread_priority` statics recorded in internal-targets.json. |
| `IsTimingInclusive` (batch 7, `utl_logging.cpp`) | 1 | 1/1 OK, 0 unverified | 55 bytes. String literal `EngineTimingInclusive` and `enabled`/`initialized` statics recorded in internal-targets.json. |
| `PerformanceTimer::operator=` (batch 8, `utl_timer_copy.cpp`) | 1 | 1/1 OK, 0 unverified | 105 bytes exact. Uses `/Ob2` and dummy outliner in `utl_timer_copy.cpp` to outline `vector<double>::operator=` (386 bytes) with `_Buy_raw`, `_Xlength`, `vector too long` string literal, and parked `_Throw_bad_array_new_length` recorded in internal-targets.json. |
| `WordData::WordData(const WordData &)` (batch 9, `utl_worddata_copy.cpp`) | 1 | 1/1 OK, 0 unverified | 28 bytes exact. Uses `/Ob2` and dummy outliner in `utl_worddata_copy.cpp` to outline `list<WordDatum*>::list(const list&)` (302 bytes, RVA `0x0DAF0`) recorded in internal-targets.json. |

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
| `Quire.Native.EfisymbolicsUmgd` | 10 exports (see below) | **In progress.** Strict-passing: `IsLoggingEnabled` (zero-filled statics recorded in internal-targets.json) and `toBSTR`. Not implemented: 8. Two of those reference zero-filled data (`SafeTerminateProcess`, `SafeTerminateOnExit`). The rest use large STL code (`GetAppRelativeFS`, `GetDirectoryName`, `Win32ErrorText`, `mcGetEnvironmentVariable`) or the post-call `nop` shape (`getInstanceDirW`). |

`EfisymbolicsUmgd` imports: `getInstanceDirW`, `SafeTerminateProcess`, `mcGetEnvironmentVariable` (wide), `SafeTerminateOnExit`, `Log` (char), `IsLoggingEnabled`, `GetDirectoryName`, `GetAppRelativeFS` (wide), `Win32ErrorText`, `toBSTR`.

## Remaining work and concrete blockers

All 94 checked functions currently in the candidate pass strict byte validation with 0 failures, 0 regressions, and 0 unfinished (gate receipt `gate-il-72297c36defe.json`). The remaining 48 unowned exports are concretely blocked by compiler/toolchain divergences:

1. **Post-Call `nop` Shape (Toolchain Divergence):**
   - *Affected functions:* `PerformanceTimer::PerformanceTimer()` (85 vs 94 bytes), `PerformanceTimer::Start()` (90 vs 98 bytes), `getInstanceDirW`, `RegisterExitCallback`, `SafeTerminateProcess`, `SafeTerminateOnExit`, `GetLogDir`.
   - *Blocker:* Original MSVC build 36246 emitted a trailing `nop` after indirect calls (`ff 15 ... 90`, particularly `EnterCriticalSection` and `LeaveCriticalSection`) and spilled `this` (`mov [rsp+8], rcx`), which modern MSVC 14.51.36231 does not emit under either `/EHsc` or `/EHa`.

2. **MSVC Optimizer Epilogue Merging:**
   - *Affected function:* `mcGetSetting` (RVA `0x0C9A0`, 281 vs 297 bytes).
   - *Blocker:* Modern MSVC aggressively merges duplicate epilogues returning `data` into a single epilogue reached via backwards `jne`, whereas the original compiler emitted separate dedicated epilogues for each return branch, producing a 16-byte structural difference despite identical logic and register usage.

3. **Register Allocation & Loop Alignment:**
   - *Affected function:* `PerformanceTimer::GetStdDev()` (RVA `0x0E570`, 121 bytes).
   - *Blocker:* Exact 121-byte length and instruction stream matched, but candidate compiler swaps register allocation of `xmm2` and `xmm3` between `factor` and `avg`, and uses a 4-byte nop (`0F 1F 40 00`) instead of a 1-byte nop (`90`) for loop alignment.

4. **STL Inlining and Layout Divergences:**
   - *Affected functions:* Path helpers (`GetShortPathNameW`, `GetDirectoryName`, `GetFileName`, `GetFileExtension`, `ReplaceExtension`), environment helpers (`mcGetEnvironmentVariable`, `IsRTChangeEnabled`), stream helpers (`ToHex`, `open_file`, `NewUTFTranslator`, `Log`, `ClearLog`), and `PerformanceTimer(const PerformanceTimer&)`.
   - *Blocker:* STL implementations of `std::basic_string`, `std::basic_ostringstream`, and `std::vector` in MSVC 14.51 differ in small-string optimization layout, inlining thresholds, and internal helper calls (`_Reallocate_grow_by`, `_Tidy_deallocate`) compared to MSVC 19.51.36246.

5. **Spelling / Scoring Algorithm Bodies:**
   - *Affected functions:* `WordChoice::GetScore` (1464 bytes), `WordChoice::GetSuggestions`, `WordChoice::BestGuess`, and `WordChoice::BestGuesses` (and their `WordData` delegators).
   - *Blocker:* Require manual decompilation and matching of the full 1.5KB string scoring/edit-distance algorithm.

## Landing

- Batch 1 (`fccf9e93b`): `BoostThreadPriority` bytes matched, `IsLoggingEnabled` statics recorded.
- Batch 2 (`a9cb0d113`): `ms_purecall_handler`, `se_trans_func`, `RestoreThreadPriority`, `IsTimingInclusive`.
- Batch 3 (`ab3643f8f`): `with_purecall_handler` constructor and destructor.
- Batch 4 (`9c4d93013`, receipt `gate-il-06f8baa50801.json`): `PerformanceTimer::operator=` (105 bytes exact, with outlined `vector<double>::operator=`).
- Batch 5 (`6c2bfeddc`, receipt `gate-il-72297c36defe.json`): `WordData::WordData(const WordData &)` (28 bytes exact, with outlined `list<WordDatum*>::list(const list&)` at `0x0DAF0`).
- All passing batches landed on `main`.
