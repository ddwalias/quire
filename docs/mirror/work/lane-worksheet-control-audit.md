# WorksheetControl and WorksheetControlDebugger checkpoint

- Owner: current worker; branch `feature/worksheet-control-audit`, rebased on main `a0db404bb`.
- Claimed: `Quire.Controls.Worksheet.WorksheetControl` and `WorksheetControlDebugger`.
- Baseline Release IL: PASS, 1455 checked, 0 failures (`.mirror-cache/release-il-c21f14f35d22/release-il.json`).
- Batch 1 (`WorksheetControl`): exact straightforward accessors and forwarders; 64 method accessors changed from generated stub to exact. Release IL PASS, 1525 checked, 0 failures (`.mirror-cache/release-il-b3e96063c6d5/release-il.json`).
- Remaining `WorksheetControl`: 231 direct generated-stub methods, 121 nested generated stubs, and 22 unpaired Mathcad methods (6 direct, 16 nested). The original IL has 39 closure methods across 20 lambda owners and no iterator types; the four activation-property setters are parked on a branch-direction mismatch.
- Remaining `WorksheetControlDebugger`: 79 generated stubs (7 direct, 72 nested), 2 closure methods across 2 owners, no iterator types, and no unpaired Mathcad methods. Nested UI and visible-object types are the largest remaining group.
