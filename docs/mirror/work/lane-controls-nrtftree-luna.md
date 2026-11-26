# Lane Controls NRtfTree — checkpoint

- Owner: `task_c53d1b661ac1`; branch `feature/luna-rtf`.
- Members implemented: landed `RtfItemModel::.ctor`; Core RTF item/document helpers, tree construction and rich-text methods; Worksheet RTF model, wrapper and visitor members.
- Release IL: Core PASS, 3412 checked (`.mirror-cache/release-il-5281d1d30aa5/release-il.json`); Worksheet PASS, 1889 checked (`.mirror-cache/release-il-bda9e2a0b4d0/release-il.json`).
- Remaining: `PrepareRtf` has a private-call/callvirt compiler mismatch; color-table members need `GetColorID`; `TextToRtfTree` and the `ToEnumerable`/hyperlink closure chain await `LoadRtfStream` and the patched fork on main.
