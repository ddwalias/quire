# MspconsMgd lane checkpoint

- Owner: AGY, Orca task `task_c28a40ad7053`.
- Branch: `feature/mspcons-mgd-reader-agy`.
- Members: Complete C++/CLI port of `Quire.MspconsMgd` (`/clr:NetCore` targeting net10.0-windows) matching `mspcons-mgd.dll`. All 8 types (100% of managed members, 0 stubs):
  - `Engine.InternFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.InternFunctionLabeled` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.TagFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.ReaderException` (`.ctor` - 1/1 exact)
  - `Engine.PCons` (13/13 exact: constructors, `hd`, `tl`, `Car`, `Cdr`, `length`, `shallowCopy`, `reverse`, `nreverse`, `FromArray`, `ToArray`, `_ToString`, `Equals`)
  - `Engine.ReadFunction` (delegate: `.ctor`, `Invoke`, `BeginInvoke`, `EndInvoke` - 4/4 exact)
  - `Engine.ReaderMacro` (`.ctor` - 1/1 exact)
  - `Engine.Reader` (13/13 exact: `.ctor`, `~Reader`, `add_reader_macro(int, ReadFunction, bool)`, `add_reader_macro(int, ReaderMacro)`, `read_symbol_or_number`, `read_string`, `read_labels`, `read_tag`, `read`, `eat_white`, `read_list`, `Dispose(bool)`, `Dispose()`)
- Release IL report: `PASS; 109 checked; 0 failures; 0 skipped stubs; 0 missing` (`release-il-e33bf788180f/release-il.json`).
- Remaining mismatches: None. All managed members in `Quire.MspconsMgd` match Mathcad Prime 12.0.1.0 Release IL exactly.
