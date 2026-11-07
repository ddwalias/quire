# Lane E-Misc — Quire.Engine.Misc (mcmisc.dll)

- Owner: E-Misc (dispatch task_68f784c59376). Branch `mirror/engine-misc-20261108` from `main`. Assembly assigned by the maintainer (not listed in WORK at creation).
- Batch 1: all 86 stub markers, as static field initializers that Release IL compiles into `.cctor`:
  - `FileUtils` (84 fields: `s_Char_*`, `s_Start/End_*script`, `s_CharMappings`, `s_AsciiToUnicodeTable`, `s_UnicodeToAsciiTable`, `s_NameMappings`, the name/shortcut tables, `s_bNamedCharMapInitialized`), in Mathcad declaration order, in `Mirrored/Engine.Misc.FileUtils.cs`.
  - `StringUtils` (`Whitespace`, `s_CommandSpecialChars`), in `Mirrored/Engine.Misc.StringUtils.cs`.
  - Stubs removed from the two `Generated/Mirror/*.g.cs` files, as MirrorGen would after the hand-written members exist.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Engine.Misc` PASS, 131 checked, 131 exact, 0 failures. Report `.mirror-cache/release-il-db4fdbc033a0/release-il.json`.
- Stubs: 86 before, 0 after.
- Remaining mismatches: none.
- Tooling note: `tools/reference/reference.py` fails on Windows (`text=True` decodes ilspycmd/MethodAudit output as cp1252, and the Greek literals break it). Reference IL was read through a local UTF-8 wrapper outside the repo; the shared tool itself is unchanged.
