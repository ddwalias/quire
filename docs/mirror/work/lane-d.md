# Lane D — Quire.Engine.Utl

- Owner: Lane D.
- Branch: `mirror/d-engine-utl-20261021-d1`.
- Worktree: `C:/Projects/Quire-wt/lane-d-engine-utl`.
- Original assembly: utl-mgd (and mcmisc).
- Selected members:
  - `Quire.utl_mgd.Function` (9 matching interface declarations: `Invoke`, `Invoke0`–`Invoke7`)
  - `Quire.Engine.Misc.HashSetUtils::HashSetToArray`
  - `Quire.Engine.Misc.Pair`2`: `.ctor`, `get_First`, `set_First`, `get_Second`, `set_Second`
  - `Quire.Engine.Misc.Triple`3`: `.ctor`, `get_First`, `set_First`, `get_Second`, `set_Second`, `get_Third`, `set_Third`
  - `Quire.Engine.Misc.StringUtils`: `Capitalize`, `CharName`, `PrefixLines`, `ContainsString`, `CharNames`, `StringSubstitute`, `SameLines` (both overloads), `IsWhitespace`, `StringQuote` (all 3 overloads), `StringUnquote`, `ExpandStringEscapes`, `.ctor`
- Release IL report: PASS; 41 checked; 32 exact bodies; 9 exact declarations; 252 unfinished remaining (.mirror-cache/release-il-cfe111fa21f7/release-il.json).
- Remaining mismatches: 252 unfinished members in full `Quire.Engine.Utl` inventory. `utl-mgd` compiled methods with bodies have `.maxstack <= 7` (Roslyn C# compiler emits `.maxstack >= 8`) and native internal bodies; `StringUtils.ToCommandString` and `.cctor` reference unreadable `<PrivateImplementationDetails>` field tokens.
