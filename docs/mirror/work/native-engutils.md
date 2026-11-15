# Quire.Native.Engutils (engutils.dll) checkpoint

- **Owner:** native-engutils worker (Orca task `task_671ffcd32e87`).
- **Branch:** `feature/native-engutils-20261114`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/native-engutils-20261114`.
- **Base commit:** `25e8d9d2023a44b516f131357a4b36d6c0da79ce` (landed on `main`).
- **Gate:** `PYTHONUTF8=1 python tools/validation/native_gate.py --base main --project Quire.Native.Engutils`.
- **Status:** PASS; 8 exports/targets fully matched and landed on `main`. 10 remaining exports parked (see section 3): none passed in the 2026-11-15 retry, and the lane is parked pending two build decisions (section 4).

---

## 1. Landed Exports (Commit `25e8d9d2023a44b516f131357a4b36d6c0da79ce`)

All 8 checked symbols achieve exact Release byte parity (0 mismatches, 0 unverified relocation sites):

1. `?parse_int@@YA_NPEBDIAEAH@Z` (185 bytes, 1 relocation site)
2. `?s_log@Output@Engine@@0_NB` (1 byte data, 0 relocation sites)
3. `?GetStream@Output@Engine@@QEAAAEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@XZ` (23 bytes, 1 relocation site)
4. `?IndentDown@Output@Engine@@QEAAXXZ` (25 bytes, 1 relocation site)
5. `?Linesep@Output@Engine@@QEAAX_N@Z` (166 bytes, 3 relocation sites)
6. `?Put@Output@Engine@@QEAAXV?$Substring@$$CBD@@@Z` (80 bytes, 4 relocation sites)
7. `?indent@Output@Engine@@AEAAXXZ` (208 bytes, 4 relocation sites)
8. Manifest target `??_C@_07GHIAJMBC@?5?5?5?5?5?5?5@` (RVA `0x7120`, 8 bytes, 7-space literal)

---

## 2. Parked Exports and Blockers

The remaining 10 exports of `engutils.dll` are parked rather than changing the build settings. Below are the names, root causes, and three source forms tried for each.

### 1. `?GetEnv@@YA?AV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@PEB_W@Z`
- **Original Extent:** 221 bytes (RVA `0x2C60`).
- **Root Cause:** Unexported internal helper / CRT relocation cascade.
  The candidate code matches the original machine instructions exactly (221 bytes). However, constructing the return `std::wstring` calls `basic_string::_Construct` (RVA `0x2D40`), which in the original Mathcad binary was inlined via LTCG and invokes unexported internal helpers (`0x4A80`, `0x2A50`, `0x2850`). In per-object compilation, these downstream internal targets fail strict verification under `native_bytes.py`.
- **Forms Tried:**
  1. *Form 1:* Two-stage `_wgetenv_s` without checking the return value of the second allocation call (`_wgetenv_s(&requiredSize, buffer, requiredSize, name); return std::wstring(buffer);`). Produced exact 221-byte machine code parity, but blocked on unverified downstream CRT internal helper targets.
  2. *Form 2:* Two-stage `_wgetenv_s` with standard error checking on both calls (`if (_wgetenv_s(...) == 0)`). Emitted 225 bytes (4 extra bytes due to `test eax, eax; jne`).
  3. *Form 3:* Single-stage `_wdupenv_s` / `_wgetenv` wrappers. Emitted 80–120 bytes with different register allocation and CRT import patterns.

### 2. `?parse_number@@YA_NPEBD@Z`
- **Original Extent:** 938 bytes (RVA `0x3F80`–`0x432A`).
- **Root Cause:** COMDAT switch jump table placement.
  The original binary embeds an inlined 5-way switch jump table directly into `.text` (`0x41E4`–`0x432A`) within the `.pdata` function extent. In per-object compilation, MSVC emits switch jump tables into separate COMDAT sections (`$LN...`), terminating the function code at the return instruction (~144–249 bytes) and leaving the remaining jump table structure unreachable without link-time whole-program layout.
- **Forms Tried:**
  1. *Form 1:* Switch-based state machine covering digits, signs, decimal points, exponents (`e`/`E`), and float suffixes (`f`/`F`). Emitted 144 bytes vs 938 bytes due to out-of-line jump table emission.
  2. *Form 2:* Iterative character validation FSM using nested `if`/`else` ladders to prevent jump table generation. Emitted 249 bytes vs 938 bytes.
  3. *Form 3:* Standard library delegator using `strtod` / `strtod_l` and suffix validation. Emitted 86 bytes vs 938 bytes.

### 3. `?Throw@Engine@@YAXPEBDZZ`
- **Original Extent:** 80 bytes (RVA `0x18B0`).
- **Root Cause:** Unexported internal helper.
  The original function delegates formatted buffer construction to an unexported internal helper at RVA `0x4840` (`format_helper(format, args, 1024)`) and constructs `std::runtime_error` using inlined CRT helper `0x2B90` and ThrowInfo `0x88D8`. Under per-object compilation, the exact 80-byte instruction sequence is reproduced, but cannot be verified without manifest entries for `0x4840`, `0x2B90`, and `0x88D8`.
- **Forms Tried:**
  1. *Form 1:* `throw std::runtime_error(format_helper(format, args, 1024));` with `__declspec(safebuffers)`. Reached exact 80 bytes, but had 3 unverified internal targets (`0x4840`, `0x2B90`, `0x88D8`).
  2. *Form 2:* Local stack buffer `char buf[1024]` with `vsnprintf` and `throw std::runtime_error(buf)`. Emitted 81 bytes vs 80 bytes.
  3. *Form 3:* Dynamic string formatting using `std::string` resizing and `vsnprintf`. Emitted 143 bytes vs 80 bytes.

### 4. `?ErrorToString@Engine@@YA?AV?$basic_string@DU?$char_traits@D@std@@V?$allocator@D@2@@std@@AEBVError@Hex@@@Z`
- **Original Extent:** 1761 bytes (RVA `0x1140`).
- **Root Cause:** LTCG inlining & unexported cross-module helpers.
  The original binary inlines the complete `std::basic_ostringstream<char>` constructor, buffer allocation, formatting, and destructor machinery via LTCG (extent: 1761 bytes). Furthermore, it invokes `Hex::operator<<` (`0xA2D0` in `mshex.dll`) and `Hex::Error::GetDetails` (`0x8780` in `mshex.dll`), which are explicitly parked under Decision D25 ("Parked: mshex `operator<<(ostream&, Error const&)` (0xa2d0) and `Error::GetDetails` (0x8780); Engutils stays blocked on them.").
- **Forms Tried:**
  1. *Form 1:* Idiomatic streaming `std::ostringstream ss; Hex::operator<<(ss, err); return ss.str();`. Emitted 463 bytes vs 1761 bytes due to non-inlined stringstream internals.
  2. *Form 2:* Direct delegation `return err.GetDetails();`. Emitted 54 bytes vs 1761 bytes.
  3. *Form 3:* Explicit manual error code and message string formatting. Emitted 491 bytes vs 1761 bytes.

### 5. `?insert_html_entity_refs@@YA?AV?$basic_string@_WU?$char_traits@_W@std@@V?$allocator@_W@2@@std@@AEBV12@@Z`
- **Original Extent:** 1218 bytes (RVA `0x39F0`–`0x3EB0`).
- **Root Cause:** LTCG inlining.
  The original binary inlines internal `std::basic_string<wchar_t>` buffer reallocation and exponential growth routines via `/GL /LTCG`, referencing multiple internal helpers (`0x4400`, `0x4330`, `0x4470`, `0x4A40`, `0x4A80`, `0x56FC`). Per-object compilation emits out-of-line calls to standard STL methods or alternative inline structures.
- **Forms Tried:**
  1. *Form 1:* Range-for loop over characters with numeric entity fallback formatting for non-ASCII characters (`&#...;`). Emitted 1140 bytes vs 1218 bytes.
  2. *Form 2:* Index-based loop with direct `std::wstring::append` and `push_back`. Emitted 498 bytes vs 1218 bytes.
  3. *Form 3:* Sized `std::wstring::append` with pre-allocated buffer reserve. Emitted 722 bytes vs 1218 bytes.

### 6. `?IndentToCol@Output@Engine@@QEAAX_K@Z`
- **Original Extent:** 206 bytes (RVA `0x3480`).
- **Root Cause:** LTCG inlining / STL implementation difference.
  In modern MSVC STL (v14.51), `std::deque<size_t>::_Growmap(size_t _Newsize)` accepts a size argument emitted as `mov edx, 1` (5 bytes: `BA 01 00 00 00`). In Mathcad's older toolset build, `_Growmap()` took no size parameter, producing an exact 5-byte difference (211 bytes candidate vs 206 bytes original).
- **Forms Tried:**
  1. *Form 1:* `m_indents.push_back(m_col + col);`. Emitted 41 bytes (out-of-line call in per-object compilation).
  2. *Form 2:* `m_indents.emplace_back(m_col + col);`. Inlines fast path and growth check, emitting 211 bytes (differing solely by the 5-byte `mov edx, 1`).
  3. *Form 3:* Manual capacity verification with inline element insertion. Emitted 160–190 bytes with differing register allocation.

### 7. `?IndentUp@Output@Engine@@QEAAX_K@Z`
- **Original Extent:** 246 bytes (RVA `0x32B0`).
- **Root Cause:** LTCG inlining / STL implementation difference.
  Shares the identical `std::deque::_Growmap` signature difference as `IndentToCol`, emitting 251 bytes vs 246 bytes (exact 5-byte difference due to `mov edx, 1`).
- **Forms Tried:**
  1. *Form 1:* `m_indents.push_back(m_col);` after updating column. Emitted 81 bytes (out-of-line call).
  2. *Form 2:* `m_indents.emplace_back(m_col);`. Emitted 251 bytes (5-byte difference from modern `_Growmap` signature).
  3. *Form 3:* Manual buffer bounds check before insertion. Emitted 195 bytes.

### 8. `??0Output@Engine@@QEAA@AEAV?$basic_ostream@DU?$char_traits@D@std@@@std@@@Z`
- **Original Extent:** 249 bytes (RVA `0x3090`).
- **Root Cause:** LTCG inlining.
  Original constructor inlines the initial `std::deque` block map allocation and proxy initialization via LTCG. In per-object compilation, modern MSVC STL emits out-of-line initialization calls.
- **Forms Tried:**
  1. *Form 1:* Member initializer list with `m_indents()` and conditional `m_indents.push_back(0)`. Emitted 133 bytes vs 249 bytes.
  2. *Form 2:* Inlined `emplace_back(0)` with explicit field zeroing. Emitted 145 bytes vs 249 bytes.
  3. *Form 3:* Explicit allocation of initial deque map. Emitted 180 bytes vs 249 bytes.

### 9. `??0Output@Engine@@QEAA@$$QEAV01@@Z`
- **Original Extent:** 208 bytes (RVA `0x2F90`).
- **Root Cause:** LTCG inlining / STL implementation difference.
  Move constructor inlines `std::deque` proxy unlinking and buffer transfer under LTCG (208 bytes). Candidate per-object compilation emits modern STL move semantics (203 bytes).
- **Forms Tried:**
  1. *Form 1:* Defaulted move constructor `Output(Output&&) = default;`. Emitted out-of-line runtime calls.
  2. *Form 2:* Memberwise move with `std::move(other.m_indents)`. Emitted 203 bytes vs 208 bytes.
  3. *Form 3:* Custom pointer transfer with manual zeroing of source instance. Emitted 165 bytes vs 208 bytes.

### 10. `??0Output@Engine@@QEAA@AEBV01@@Z` & `??1Output@Engine@@QEAA@XZ`
- **Original Extents:** Copy constructor 75 bytes (RVA `0x2F40`); Destructor 52 bytes (RVA `0x2F00`).
- **Root Cause:** LTCG inlining / STL implementation difference.
  Copy constructor and destructor inline the `std::deque` element copy and deallocation tidy loop in the original binary. Candidate per-object compilation emits modern STL helper routines (70 bytes vs 75 bytes for copy constructor; 47 bytes vs 52 bytes for destructor).
- **Forms Tried:**
  1. *Form 1:* Defaulted operations (`= default;`). Emitted out-of-line library calls.
  2. *Form 2:* Memberwise copy constructor and empty destructor body. Emitted 70 bytes (copy ctor) and 47 bytes (dtor).
  3. *Form 3:* Explicit iteration over `m_indents` elements. Emitted 95 bytes (copy ctor) and 60 bytes (dtor).

---

## 3. Retry on 2026-11-15 (task `task_8a362502b87d`, then `task_975c6d38c09f`)

Each export was gated under `native_gate.py --base main --project Quire.Native.Engutils` (Release, per-object). Three forms per export; `parse_number` and `insert_html_entity_refs` got extra forms. Lengths are candidate bytes against the original extent. "offset not recorded" means the gate reported a length mismatch, which stops the byte comparison before an offset is reached. No form passed, so nothing landed.

| Export | Form 1 | Form 2 | Form 3 | First differing offset |
| --- | --- | --- | --- | --- |
| `Output(ostream&)` `??0...AEAV?$basic_ostream...` (249) | member init + `if (s_log)` push 125 | member init + `emplace_back`, body fills 241 | zero-init members + `if (s_log)` push: differs at `+0x2` (original `5c`, candidate `4c`), then `s_log` target at `+0x50` | F1/F2 offset not recorded; F3 `+0x2` |
| `Output(const Output&)` (75) | member init copy 70 | body assignment 155 | `= default` 70 | not recorded (length) |
| `Output(Output&&)` (208) | member init + `std::move` 203 | `= default` 203 | `= default` 203 | not recorded (length) |
| `~Output()` (52) | `= default` 47 | `{}` 22 | `= default` 47 | not recorded (length) |
| `IndentUp(size_t)` (246) | `push_back` 81 | `emplace_back`, `m_col` set first 296 | local `top`, `push_back` 81 | not recorded (length) |
| `IndentToCol(size_t)` (206) | `push_back` 41 | `emplace_back` 41 | local `value`, `push_back` 41 | not recorded (length) |
| `GetEnv(const wchar_t*)` (221) | `_alloca`, unchecked 2nd call, `wstring(buf, wcslen)` 502 | checked 2nd call, `wstring(buf)` 273 | `wstring value(size)` + `resize` 789 | not recorded (length) |
| `Throw(const char*, ...)` (80) | `format_message` helper (unexported 0x4840 not in manifest) 97 | `char buf[1024]`, `vsnprintf` 99 | `std::string(1024)` + `strlen` 182 | not recorded (length) |
| `insert_html_entity_refs` (1218) | recursive `substr` + `+` 1194 | iterative `switch`, `append` 494 | `find_first_of`, recursive 1230 | not recorded (length) |
| `parse_number` (938) | hand-written DFA over `isdigit` 187 | `isdigit` if-ladder 280 | `strtod` + `f`/`F` suffix 81 | not recorded (length) |
| `ErrorToString` (1761) | not gated | not gated | not gated | not gated: the candidate needs a link input for `mshex.dll` (`Hex::operator<<`, `Hex::Error::GetDetails`), and the original inlines `std::basic_ostringstream` |

Forms are saved in the scratchpad (`engutils-forms/`), not committed.

### Why none passed

- **`Output` special members, `IndentUp`, `IndentToCol`:** The original inlines the `std::deque<size_t>` operations (proxy setup, `push_back` fast path, `_Tidy`) into each caller. The pinned toolset is the same MSVC 19.51.36246 as Mathcad's, so the difference is `/GL` inlining, not STL version. A per-object build calls these out of line, which shows as the length gaps above.
- **`GetEnv`:** The original's string construction calls the `basic_string` `_Construct` helper at `0x2D40` and inlines the `_alloca` path. None of the three forms reproduced the stack-allocation or construction shape.
- **`Throw`:** It needs the unexported format helper at `0x4840` (144 bytes) as its own ported function, plus the `std::runtime_error` construction thunks. Form 1 is the only form that calls the helper, and the helper is not yet ported or recorded.
- **`insert_html_entity_refs`:** The original is a loop over four special characters that recurses on the remainder: `prefix + "&" + name + ";" + recurse(rest)`. The name strings are `quot`, `amp`, `lt` and `gt`, read from `.rdata` at `0x7148`, `0x7158`, `0x7160` and `0x7168`. The recursive form is close (1194 and 1230 against 1218), but the string-growth codegen differs.
- **`parse_number`:** A table-driven DFA over the sign, digits, `.`, exponent and suffix states. Its jump tables sit inline in `.text` after the function body (`0x41E4`, `0x4204`, `0x42B0`, `0x4318`), and per-object compilation moves switch tables out of line. The three forms are approximations of the accepted syntax, not recovered from the DFA. None reproduces the table placement.
- **`ErrorToString`:** Needs a link input that the project does not have, which is a build-configuration change.

## 4. Next port and open decisions

- **Next port: the `0x4840` helper.** It is the format helper called by `Throw` (`format(out, format, va_list, 1024)`, 144 bytes, prologue `mov [rsp+10h],rbx`). It has its own body, so it is ported as file-local C++ and recorded in `docs/mirror/native/internal-targets.json` with `native_targets.py` once its bytes match. Porting it unblocks `Throw` and probably the `basic_string` helper calls in `GetEnv`. It is not yet attempted.
- **Open decision 1: WholeProgramOptimization.** Tested on 2026-11-15 and not applied; see section 5. The original was built with `/GL`. Setting `<WholeProgramOptimization>true</WholeProgramOptimization>` in `Quire.Native.Engutils.vcxproj` switches the gate to linked mode (D30). In that test six of the eight exports that matched per object failed. Do not retry WPO for this project.
- **Open decision 2: mshex link input.** `ErrorToString` needs `mshex.dll`'s import library. No other native project links mshex, so this is a new link dependency in `Quire.Native.Engutils.vcxproj`. It also depends on D25, which parks `Hex::operator<<` and `Hex::Error::GetDetails`. It is not applied here.

## 5. WholeProgramOptimization test (2026-11-15, not applied)

- **Test:** `<WholeProgramOptimization>` set to `true` in the Release configuration of `Quire.Native.Engutils.vcxproj` only (line 22, previously `false`). Gate run: `PYTHONUTF8=1 python tools/validation/native_gate.py --base main --project Quire.Native.Engutils`, linked mode (D30). Result: `Native bytes: FAIL; 7 checked`, exit 1. Reverted; nothing landed.
- **Checked exports (7):**
  - `parse_int`: passed (not in the failure list).
  - `GetStream`: length 4, original 23. Regressed.
  - `IndentDown`: length 3, original 25. Regressed.
  - `Linesep`: length 12, original 166. Regressed.
  - `Put`: length 48, original 80. Regressed.
  - `indent`: length 3, original 208. Regressed.
  - `s_log`: `16 bytes at original RVA 0xa404 are not in a section`. Regressed.
- **Result:** six of the eight exports that matched per object (section 1) regressed. Only `parse_int` still passes among the checked exports. The eighth, the manifest target `0x7120`, was not in the gate's checked list.
- **Parked exports (section 2) were not checked.** None of the 10 appears in the gate's checked list or failures, so the test gives no match result for them. WPO is therefore not shown to be the cause of their failures, and it is not a fix for them.
- **Gate defect for the maintainer:** the gate's JSON report had `"regressions": []` while six previously passing exports failed. Those failures were reported under `failures` only. Linked mode is meant to report such changes under `regressions`, so the gate's regression reporting does not cover this case.
