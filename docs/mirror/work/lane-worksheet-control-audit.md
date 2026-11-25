# WorksheetControl and WorksheetControlDebugger checkpoint

- Owner: current worker; branch `feature/worksheet-control-audit`, based on main `0a3572649`.
- Claimed: `Quire.Controls.Worksheet.WorksheetControl` and `WorksheetControlDebugger`.
- Baseline Release IL: PASS, 1455 checked, 0 failures (`.mirror-cache/release-il-c21f14f35d22/release-il.json`).
- Batch 1 (`WorksheetControl`): exact straightforward accessors and forwarders; 64 method accessors changed from generated stub to exact. Release IL PASS, 1525 checked, 0 failures (`.mirror-cache/release-il-b3e96063c6d5/release-il.json`).
- Remaining `WorksheetControl`: 231 direct generated-stub methods, 121 nested generated stubs, and 22 unpaired Mathcad methods (6 direct, 16 nested). The original IL has 39 closure methods across 20 lambda owners and no iterator types; the four activation-property setters are parked on a branch-direction mismatch.
- Remaining `WorksheetControlDebugger`: 79 generated stubs (7 direct, 72 nested), 2 closure methods across 2 owners, no iterator types, and no unpaired Mathcad methods. Nested UI and visible-object types are the largest remaining group.
- Batch 2 (`WorksheetControlDebugger`): exact `Instance`, `DestroyInstance`, both close-instance handlers, and `OnSpiritMainWindowClosing`. `OnModeChanged` remains a generated stub: trial control-flow layouts emitted the right instruction count but instantiated `_worksheetControlUI` before `_visibleObjects`, opposite Mathcad. The private constructor also remains a stub because Mathcad reads `DebugTools.IsDebug` before the base constructor call.
- Batch 2 Release IL: PASS, 1535 checked, 0 failures (`.mirror-cache/release-il-21d838e09c79/release-il.json`).
- Regeneration: full `dotnet build Quire.slnx -c Release`, explicit `Quire.Componentsmgd` Release build, then MirrorGen `emit`; target `WorksheetControl` and `WorksheetControlDebugger` generated files reflect the emitter output. All unrelated generated-file output was restored.
