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

## Survey follow-up — Engine.Scan

- Owner: luna-rtf; branch `feature/luna-equationcontrol-round4`.
- Read original Release IL with `python tools/reference/reference.py Engine.Scan --assembly mcfront12 --il`. The implementation attempt was reverted after `.mirror-cache/release-il-d6802128af6b/release-il.json` reported 33 mismatching methods.
- Blocker: the C# `Quire.Engine.Native` target does not reproduce the original C++/CLI `call`/`callvirt`, constructor field-store order before `base()`, `.maxstack`, delegate comparison, and `initlocals` shapes. Per maintainer direction, do not claim other `Quire.Engine.Native` types.
- Kept local for the combined landing: exact `EquationControl.GetImeWindowPosition` and `RtfHelper.ApplyBackgroundColor`; their passing reports are recorded above and in `lane-quire-controls-equationcontrol.md`.
