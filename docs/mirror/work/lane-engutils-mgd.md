# Lane EngutilsMgd — Quire.EngutilsMgd

- Owner: `feature/engutils-mgd-remaining-agy`
- Branch: `feature/engutils-mgd-remaining-agy`
- Members: All managed members across `Engine.StringUtils`, `Quire.Engine.Tags`, `Quire.Engine.CollectionUtils`, `Quire.Engine.Reflection`, `Quire.Engine.CircTracker`, `Quire.Engine.UnwindProtect`, `Quire.Engine.NormalizeComponentExceptions`, `Quire.Engine.UnmanagedMemoryStream`, `Quire.Engine.CachingWrapper`, `Quire.Engine.ComponentSupport`, `Quire.Engine.Logging`, `Quire.Engine.Timing`, `Quire.Engine.Timing2`, `Quire.Native.EngutilsMgd.PathCache`, and `Quire.Native.EngutilsMgd.PushbackReader`, plus CRT implementation / module methods.
- Release IL report (Logging, Timing, Timing2 batch): PASS; 242 checked; 0 failures (`.mirror-cache/release-il-5d124c8ee600/release-il.json`).
- Remaining:
  - `MplUtils` (`GetMplFileDependencies`, `GetMplFileModuleName`): the source is kept uncommitted in `MixedMode/MplUtils.cpp` and is not in the vcxproj. Both mismatch at #16 (`stloc.s 21` vs Mathcad `stloc.s 22`, the uint64 length local). Tried a named `size_t length` local (before and after `pin_ptr`) and an explicit `Substring dll(L".DLL")`; the slot numbering did not change.
  - `Engine.Timing`/`Timing2` `.ctor` and `Logging` `.ctor` are not verified (`False` verdicts in the report).
  - Unported C++ native helpers in `engutils-mgd.dll`.
