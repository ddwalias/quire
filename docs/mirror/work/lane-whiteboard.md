# Lane Whiteboard — Quire.Controls.Whiteboard

- Owner: Orca task `task_5da5143048d1` (maintainer-assigned; no WORK.md lane row).
- Branch: `mirror/whiteboard-20261108`; worktree `C:/Users/DuyDang/orca/workspaces/Quire/whiteboard-20261108-b`.
- Baseline (main `5c997717`): not measured before batch 1; the batch 1 report counts 926 unfinished after the change.

## Batch 1 — small leaf types

- Members: `WhiteboardItemPositionComparer` (`.ctor`, `Compare`), `WhiteboardElementXamlHelper` (`.ctor`, `GetPrintView`), `ItemsControlCanvas` (`.ctor`, `ArrangeOverride`), `ProductInfo.Constants` (`.ctor`; `ConverterProductName`, `ConverterExe` as `const`).
- Hand-written: `src/Quire.Controls.Whiteboard/Mirrored/*.cs`; stubs removed from the matching `Generated/Mirror/*.g.cs`.
- Shapes: `Compare` as the decompiled `if (a == b) return 0;` form (matched first time). `GetPrintView` as `if (element is not IWhiteboardControl) return null;` then `as` + null test (matched first time). `ArrangeOverride` as `as` + `!= null` (matched first time).
- Release IL: PASS — 82 checked, 82 exact, 0 failures, 926 unfinished after this batch (`release-il-59487b3819e9`).
- Remaining mismatches in this batch: none.
