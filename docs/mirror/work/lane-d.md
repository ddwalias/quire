# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-20261021-d1`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl`.
- Original assembly: utl-mgd (and mcmisc).
- Selected members:
  - `Quire.utl_mgd.Function` (9 matching interface declarations: `Invoke`, `Invoke0`–`Invoke7`)
  - `Quire.utl_mgd.FunctionBase`: `Invoke` (matching abstract method declaration)
  - `Quire.Engine.Misc.HashSetUtils::HashSetToArray`
  - `Quire.Engine.Misc.Pair`2`: `.ctor`, `get_First`, `set_First`, `get_Second`, `set_Second`
  - `Quire.Engine.Misc.Triple`3`: `.ctor`, `get_First`, `set_First`, `get_Second`, `set_Second`, `get_Third`, `set_Third`
  - `Quire.Engine.Misc.StringUtils`: `Capitalize`, `CharName`, `PrefixLines`, `ContainsString`, `CharNames`, `StringSubstitute`, `SameLines` (both overloads), `IsWhitespace`, `StringQuote` (all 3 overloads), `StringUnquote`, `ExpandStringEscapes`, `.ctor`
  - `Quire.Engine.Misc.FileUtils`: `StreamToPath`, `ReadExpanded` (both overloads), `ProcessInclude`, `SkipPast`, `ComputeLineNumber` (all 3 overloads), `.ctor`
  - `Quire.Engine.Misc.LispUtils`: `.ctor`
  - `Quire.Engine.Misc.LispUtils+LinePartition`: `.ctor`, `get_Line`, `get_Count`, `PutSegment`, `PeekSegment`, `TakeSegment`, `Segments`, `SegmentsFrom`, `JoinedText`, `JoinedTextFrom`
- Release IL report: PASS; 50 checked; 50 exact bodies in check; 10 exact declarations; 234 unfinished remaining (.mirror-cache/release-il-542a69053632/release-il.json).
- Remaining mismatches: 234 unfinished members in full `Quire.Engine.Utl` inventory. `utl-mgd` compiled methods with bodies have `.maxstack <= 7` (Roslyn C# compiler emits `.maxstack >= 8`) and native internal bodies; `StringUtils.ToCommandString`, `StringUtils..cctor`, and `FileUtils..cctor` reference unreadable `<PrivateImplementationDetails>` field tokens.
