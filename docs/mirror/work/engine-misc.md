# Quire.Engine.Misc (mcmisc.dll) checkpoint

- Owner: task_35bfb3564a80 (Orca worker for assembly `Quire.Engine.Misc`).
- Branch: `feature/engine-misc-20261114`, based on current `main`.
- Members: all 8 mcmisc owners are implemented in hand-written partials (`src/Quire.Engine.Misc/Mirrored/`). No `MirrorStub` markers remain. Member names match the original: `FileUtils` 21, `StringUtils` 17, `LispUtils` 26, `LispUtils+LinePartition` 10, `XmlUtils` 41, `HashSetUtils` 2, `Pair`2` 5, `Triple`3` 7. The 2 `<PrivateImplementationDetails>` methods are compiler-generated and have no original.
- Release IL report: `PYTHONUTF8=1 python tools/validation/release_il.py --base main --project Quire.Engine.Misc` → PASS; 131 checked. Report `.mirror-cache/release-il-3f07282f69c0/release-il.json`.
- Remaining mismatches: none.
- Not covered by the gate: field layout (the gate compares methods only).
