# WorksheetViewModel checkpoint

- Owner: worksheet-vm-codex
- Branch: feature/worksheet-vm-codex
- Members: moved the leading private field declarations into the hand-written partial and removed their complete generated stubs.
- Release IL report: `.mirror-cache/release-il-dd6fb11940cb/release-il.json` (whole type FAIL; 1518 checked). The moved fields have no mismatch entries; existing property and method stubs account for the type failures.
- Remaining mismatches: unrelated existing stubs; four events and properties remain.
