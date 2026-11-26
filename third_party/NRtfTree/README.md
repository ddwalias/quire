# NRtfTree compatibility fork

This is a source vendoring of [sgolivernet/nrtftree](https://github.com/sgolivernet/nrtftree),
upstream commit `334a4c5` (the `0.4.0` release), under LGPL-3.0-or-later. The upstream
`LICENSE` and `LICENSE-LESSER` files are included.

The project preserves the assembly identity `nrtftree-library, Version=0.4.0.0` used by
Mathcad Prime 12.0.1.0. Mathcad's copy is unsigned. Compatibility patches:

- `RtfTree.LoadRtfFile(string)` delegates through `Encoding.Default`; the fork adds the
  encoding overload and `LoadRtfStream(StreamReader)` used by Mathcad.
- `RtfTree.GetColorID(RtfColorTable, Color)` is public and owns color-table insertion;
  `RtfMerger` calls it instead of defining its own private helper.
- `GetColorTable`, `GetFontTable`, font merging, keyword parsing and `NextNode` follow
  Mathcad's Release behavior where upstream 0.4.0 differs.

`tools/validation/compare_nrtftree_il.py` compares the complete candidate assembly with
Mathcad's Release IL. The project is part of the Quire solution and its consumers reference
this project directly.
