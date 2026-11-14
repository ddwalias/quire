# Quire.Native.Engutils (engutils.dll) checkpoint

- **Owner:** native-engutils worker (Orca task `task_671ffcd32e87`).
- **Branch:** `feature/native-engutils-20261114`, worktree `C:/Users/DuyDang/orca/workspaces/Quire/native-engutils-20261114`.
- **Base commit:** `25e8d9d2023a44b516f131357a4b36d6c0da79ce` (landed on `main`).
- **Gate:** `PYTHONUTF8=1 python tools/validation/native_gate.py --base main --project Quire.Native.Engutils`.
- **Status:** PASS; 8 exports/targets fully matched and landed on `main`. 10 remaining exports parked per coordinator decision due to build-level differences (LTCG inlining, COMDAT switch jump table placement, and unexported internal helpers).

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
