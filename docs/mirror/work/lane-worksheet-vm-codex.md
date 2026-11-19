# WorksheetViewModel checkpoint

- Owner: worksheet-vm-codex
- Branch: feature/worksheet-vm-codex
- Members: WorksheetCommand, AddWordToDictionaryCommand, AddItemAtInsertionPointAndActivateCommand, RemoveActiveItemCommand, CreateTextRegionFromEquationControlCommand, CreateTextRegionFromWhiteboardControlCommand.
- Release IL report: `.mirror-cache/release-il-002b9c68640f/release-il.json`; whole type fails on existing members; changed nested command members have no mismatch entries.
- Parked: RemoveItemCommand Undo contains an ExecuteOnLoad lambda that affects closure ordinals; one focused attempt not yet completed.
