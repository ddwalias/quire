# DevComponents registration checkpoint

- Owner: luna-devcomponents; branch `feature/luna-devcomponents`.
- Registration: `Quire.DevComponents.WpfDock` (80 metadata types, 802 methods) and `Quire.DevComponents.WpfRibbon` (177 types, 1,761 methods); both are registered as C# projects. Ribbon depends on `Quire.Core`; both assemblies reference absent `Xenocode.Client.dll`, used only for `Xenocode.Client.Attributes.RenameSymbol` metadata. Consumer projects now reference the Ribbon project, and `Quire` references both.
- Registration keeps the existing Fluent.Ribbon/AvalonDock package references in `Quire`, `Quire.Controls`, and `Quire.Controls.Plots`, plus temporary install DLL references in `Quire` for its hand-written consumers. The DLL references and old packages will be removed together during the separate consumer switch.
- Release IL: Dock report `.mirror-cache/release-il-fd7c256e3df2/release-il.json` (PASS, 14 checked; 731 generated stubs and 29 unmatched rows remain); Ribbon report `.mirror-cache/release-il-f6310be4c09c/release-il.json` (PASS, 12 checked; 1,607 generated stubs and 83 unmatched rows remain).
- Builds: both new projects and `Quire.Controls.Plots` build Release; `python tools/lint/lint.py --base origin/main` passes.
- Next: implement Dock members in batches, then Ribbon. Registration is not a claim of member completion.
