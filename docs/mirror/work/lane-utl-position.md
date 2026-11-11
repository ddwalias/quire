# Lane utl-position: utl.dll PerformanceTimer constructor, Stop and destructor (QUI-15)

Owner: utl-position worker. Branch: `feature/utl-position-binary`. Project: `Quire.Native.Utl`.
Members: `PerformanceTimer::PerformanceTimer` (RVA 0xE240), `::Stop` (0xE390), `::~PerformanceTimer` (0xE2A0).
Native gate: PASS (38 checked). The constructor, Stop and destructor are byte-exact; `Start` now matches too.

## How the bytes were reached

The first candidate was 89/191/114 bytes against 94/197/119. Laying the original's blocks beside the candidate's gave
four causes, each fixed in `utl_timer.cpp` or the project file:

- **Exception model.** The original has a `nop` after `InitializeCriticalSection` and `DeleteCriticalSection`, and homes
  `rcx` to `[rsp+8]` in the destructor. Those are `/EHa` signatures: under `/EHs` the nop appears but `rcx` is not homed,
  and under `/EHsc` neither appears. `utl_timer.cpp` is compiled with `ExceptionHandling=Async` (per file, as Mshex does
  for its own exception model); the rest of the project stays `/EHsc`.
- **Vector layout.** `samples` is a `std::vector<double>` (not three raw pointers). The destructor is
  `DeleteCriticalSection(&lock)` followed by the member's own destructor; `/Ob3` is needed for MSVC to inline the
  vector tidy into it as the original does (`/Ob2` tail-jumps to `~vector`).
- **Constructor.** `frequency(0)` is in the initializer list (the original stores zero to +0x30 before the
  `QueryPerformanceFrequency` call); the critical section is initialised last, in the body.
- **Stop.** The lock is held by a scope guard (`CriticalSectionLock`: `Enter` in the constructor, `Leave` in the
  destructor), which gives the original's `[rsp+38h]` slot, the `nop` after `Enter` and the tail `jmp Leave`. The
  branch is `if (current > limit) { --count; total -= current; } else samples.push_back(current);`, which gives the
  original's `comisd xmm2,xmm0; jbe` layout and the dec-first fall through. `push_back` produces the original's
  `add [rbx+8],8` only with `/std:c++20` (the C++17 library caches `_Mylast + 1` in a register), so the file is
  compiled with `LanguageStandard=stdcpp20`.

`Start` uses the same guard and now matches the original at 0xE320 as well.

## Internal targets (docs/mirror/native/internal-targets.json, `utl.dll`)

- `__real@416312d000000000` at 0x194B0 (the 1e7 constant), size 8; `__real@408f400000000000` (1e3) was already listed.
- `??3@YAXPEAX_K@Z` at 0x11C00: sized `operator delete`, supplied by the static CRT, so `"toolchain": true`.
- `??$_Emplace_reallocate@AEBN@...` at 0xBE10: **parked** (D25/D29). The pinned STL's instance is 453 bytes against the
  original's 454 and differs in register choice (the original keeps `where` in r15 and the old size in bytes, comparing
  it with `0FFFFFFFFFFFFFFF8h`; ours compares the element count with `1FFFFFFFFFFFFFFFh`) and in the `memmove` target.
  Tried without effect on that function: `/EHsc`, `/EHs`, `/EHa`, `/Ob1`, `/Ob3`, `/Os`, `/Ox`, `/GS-`, `/fp:fast`,
  `/std:c++14/17/20/latest`, push_back vs emplace_back vs insert(end()), and a `/GL /LTCG` DLL link. Its callers verify
  by RVA.

## Remaining mismatches

None in the three members. `_Emplace_reallocate` (parked) is the only function in the call graph not byte-compared.
