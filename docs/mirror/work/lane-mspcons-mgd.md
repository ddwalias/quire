# MspconsMgd lane checkpoint

- Owner: AGY, Orca task `task_ffc4273193a5`.
- Branch: `feature/mspcons-mgd-agy`.
- Members: Established original-language C++/CLI project split for `Quire.MspconsMgd` (`/clr:NetCore` targeting net10.0-windows) matching `mspcons-mgd.dll`. Implemented all 8 types:
  - `Engine.InternFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.InternFunctionLabeled` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.TagFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.ReaderException` (`.ctor` - 1/1 exact)
  - `Engine.PCons` (13 methods: 12/13 exact: constructors, `hd`, `tl`, `Car`, `Cdr`, `length`, `shallowCopy`, `reverse`, `nreverse`, `FromArray`, `ToArray`, `_ToString`; `Equals` has complete body, marked `[MirrorStub]` due to MSVC 19.51 SSA loop-rotation difference)
  - `Engine.ReadFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.ReaderMacro` (`.ctor` - 1/1 exact)
  - `Engine.Reader` (13 methods: 12/13 exact: `.ctor`, `~Reader`, `add_reader_macro(int, ReadFunction, bool)`, `add_reader_macro(int, ReaderMacro)`, `read_string`, `read_labels`, `read_tag`, `read`, `eat_white`, `read_list`, `Dispose(bool)`, `Dispose()`; `read_symbol_or_number` marked `[MirrorStub]`)
- Candidate Release IL: 123 checked methods PASS matching Mathcad Release IL; 2 skipped stubs (`Engine.Reader::read_symbol_or_number`, `Engine.PCons::Equals`).
- Concrete Blockers:
  1. `Engine.Reader::read_symbol_or_number`: Calls `Engine.StringUtils::ParseInt` in `Quire.EngutilsMgd`. `Quire.EngutilsMgd` is concurrently assigned to another agent thread and has not yet implemented `StringUtils::ParseInt` (currently a `MirrorStub.NotImplemented` placeholder). `read_symbol_or_number` carries `[Quire::Mirror::MirrorStub]` as an unfinished placeholder stub until `Quire.EngutilsMgd` completes that dependency.
  2. `Engine.PCons::Equals`: Structure-matching MSVC 19.51 compiler loop rotation / jump-threading on loop entry (instruction #12 `ldloc.1` vs Mathcad's MSVC 19.29 `ldloc.0`, 65 entries vs 65 entries). Retains full implementation body with `[Quire::Mirror::MirrorStub]` placeholder attribute.
