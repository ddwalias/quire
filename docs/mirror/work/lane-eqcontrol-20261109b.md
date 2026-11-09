# EquationControl lane checkpoint

- Owner: dispatched EquationControl worker
- Branch: `mirror/eqcontrol-20261109b`
- Batch: `Controls.InputControls.StringCompressor::{CompressString, DecompressString}`
- Shapes tried: direct `if` equality and inverted `if` branches differed in branch opcodes; enum `switch` matched both methods.
- Release IL: PASS, 614 checked (`.mirror-cache/release-il-2eedc1c08c83/release-il.json`). Earlier candidate reports: FAIL at `ae31463cd9e7` and `01f85df6906e` while trying the two `if` shapes.
- Remaining: all other stub-marked members in EquationControl; 5,379 markers across 316 types after this batch.
