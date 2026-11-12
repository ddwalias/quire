# Lane utl-unported: remaining utl.dll exports (checkpoint)

Owner: utl-unported worker. Branch: `mirror/utl-unported-20261113`. Project: `Quire.Native.Utl`. Not landed.

## Passing (added to the .def)

- `FromHex(const char *)` (0x8830)
- `toBSTR(unsigned int, const char *)` (0x10480)
- `die(const wchar_t *)` (0x103E0)
- `IsTimingEnabled()` (0xEFB0)

Manifest additions in `docs/mirror/native/internal-targets.json` (utl.dll): `??_U@YAPEAX_K@Z` (toolchain, 0x12180),
literals `"EngineTiming"` (0x19150), `"Fatal Error"` (0x19288), and the two statics of `IsTimingEnabled` (0x229D1, 0x229D2).

## Left out

Not reproduced yet (the port returns std::string/wstring, or builds on STL internals):
ToHex, GetShortPathNameW (4), GetAppRelativeFS (2), RelativePath, ReplaceExtension, GetDirectoryName, GetFileName,
GetFileExtension, getInstanceDir/W, mcGetEnvironmentVariable (2), Win32ErrorText.

Not reproduced yet (other code paths; not analysed beyond the first look):
GetCachedFile, UpdatePathForPlatformAndDestdir, open_file, NewUTFTranslator, GetLogDir, IsRTChangeEnabled.

ostream logging: Log (2), ClearLog (2).

Container code in the timers and exit handling: ResetTimers, WriteTimers, RegisterExitCallback, DieOnExit,
SafeTerminateOnExit, SafeTerminateProcess.

WordChoice/WordData list code (see lane-utl-next): GetScore, GetSuggestions (2), BestGuess (2), BestGuesses (2),
WordData copy constructor (the list copy stays inline; needs an out-of-line list copy).

Layout or SEH not inferred: TimeIt (copy ctor, Substring ctor, dtor, `??_7TimeIt` vtable), PerformanceTimer copy
constructor and copy assignment, with_purecall_handler constructor and destructor (needs ms_purecall_handler),
ms_purecall_handler, se_trans_func, s_tls_init.

Tried, not matching:
- GetStdDev (0xE570): the bytes match except the load order: the original computes the mean before the loop test, the
  candidate sinks it into the non-empty branch.
- IsTimingInclusive (0xEFF0): the original's `enabled` static holds 01 in the file; the candidate's is zero-initialised.
- mcGetSetting (0xC9A0): the HKLM fallback keeps the HKCU result live (`sete dil`); the candidate's shape is 251 bytes against 297.
- RestoreThreadPriority (0xF550): needs BoostThreadPriority's saved priorities, which is FAIL on main too.

## Gate

`python tools/validation/native_gate.py --base main --project Quire.Native.Utl`: PASS; 58 checked, 58 ok, 2 unfinished
(IsLoggingEnabled and BoostThreadPriority, both unchanged from main). The DLL exports 44 names, the same as the .def.
