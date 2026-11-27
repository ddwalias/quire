# Lane Controls NRtfTree — checkpoint

- Owner: `task_c53d1b661ac1`; branch `feature/luna-rtf`.
- Members implemented: landed `RtfItemModel::.ctor`; Core RTF item/document helpers, tree construction and rich-text methods; Worksheet RTF model, wrapper and visitor members.
- Release IL: Core PASS, 3412 checked (`.mirror-cache/release-il-5281d1d30aa5/release-il.json`); Worksheet PASS, 1889 checked (`.mirror-cache/release-il-bda9e2a0b4d0/release-il.json`).
- Rebased onto main `6527a5efb`, which includes the patched NRtfTree fork.
- Implemented: `TextToRtfTree`, `NormalizeHighlightColor`, the hyperlink serialization/key-value repair chain, and `ToEnumerable`; the whole-type declaration order now produces the Mathcad closure ordinals.
- Release IL: Core PASS, 3,431 checked (`.mirror-cache/release-il-2a9c7f267d18/release-il.json`); Worksheet PASS, 1,976 checked (`.mirror-cache/release-il-0717a47d3baf/release-il.json`).
- Parked: `ApplyBackgroundColor` because the current Roslyn compiler folds repeated `SolidColorBrush.Color` reads and changes the Release IL; `PrepareRtf` retains its private-call/callvirt mismatch.

## Follow-up — ApplyBackgroundColor

- Owner: luna-rtf; branch `feature/luna-equationcontrol-round4`.
- Recovered the original Release IL from Mathcad and implemented `ApplyBackgroundColor`, reusing the first `SolidColorBrush` local for the optional border brush and preserving repeated `Color` property reads.
- Release IL: Core PASS, 3,432 checked (`.mirror-cache/release-il-8c3c07ed114e/release-il.json`). `PrepareRtf` remains parked.
