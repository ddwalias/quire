# Quire.Engine.Misc (mcmisc.dll) checkpoint

- Owner: task_9170d3bdbec8 (Orca worker for assembly `Quire.Engine.Misc`).
- Branch: `feature/engine-misc-20261113-r3`, based on `main` at `1d868dc06`.
- Members: all 8 mcmisc owners are implemented in hand-written partials (`src/Quire.Engine.Misc/Mirrored/`). No `MirrorStub` markers remain in `src/Quire.Engine.Misc`. Member names match the original for every owner: `FileUtils` 21, `StringUtils` 17, `LispUtils` 26, `LispUtils+LinePartition` 10, `XmlUtils` 41, `HashSetUtils` 2, `Pair\`2` 5, `Triple\`3` 7. The 2 `<PrivateImplementationDetails>` methods are compiler-generated and have no original.
- Release IL report: `python tools/validation/release_il.py --base main --project Quire.Engine.Misc` → `Release IL: PASS; 131 checked`, 131 exact, 0 failures, 0 unfinished. Report at `.mirror-cache/release-il-d3000d5e7640/release-il.json`.
- Remaining mismatches: none.
- Not covered by the gate: field layout (the gate compares methods only).
