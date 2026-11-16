# Lane Quire.Efi checkpoint

- Owner: Quire.Efi lane (Claude worker); branch `mirror/efi-lane-20261117`, worktree `claude-efi-lane`.
- Scope: the 538 generated files in `src/Quire.Efi/Generated/Mirror` (7 316 stub lines). Their original assemblies split into three groups under D20:
  - C# originals (Roslyn can match): `efiplot_` (32 files, 498 stubs), `efisixsigma_` (18 files, 334 stubs), `efiio-csharp` (5 files, 141 stubs). These are the work of this lane.
  - C++/CLI originals (`efi*_` plus `efiutl`, `efiio_` and others): their IL calls `<Module>` global functions and template instances, which a Roslyn project cannot emit. They need their own C++/CLI targets (`Quire.Efi*Impl`, D20) and the pinned MSVC build. Blocked until those targets exist.
  - Mpl originals (`efi*` without `_`, e.g. `EfiSymbolicsLexer`, 325 files): compiled by Mpl; they need the Quire Mpl toolchain (`Quire.Mpl`, `Quire.Mpl.Emit`). Blocked until that chain exists.
- Batch 1: `DOE.BoxBehnken` (`.ctor`, `getDesignMatrix`, `init`, field `m_designs`); Release IL PASS (`24 checked`; `.mirror-cache/release-il-22ec13e7002d/release-il.json`).
- Batch 2: `DOE.DoeBase` (fields `m_nfacts`..`m_utils`, `.ctor`, abstract `getDesignMatrix`, `getDesignMatrix(out MatVal)`); Release IL PASS (`27 checked`; `.mirror-cache/release-il-3edaefa0421f/release-il.json`).
- Batch 3: `DOE.DoeLabel`, `DOE.Plots`, `DOE.Taguchi`, `DOE.PlackettBurman`; the 31 Taguchi tables and the PlackettBurman tables are kept verbatim (data); Release IL PASS (`41 checked`; `.mirror-cache/release-il-5b0cdca9eea3/release-il.json`).
