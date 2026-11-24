# Quire.Engine.Misc (mcmisc.dll) checkpoint

- Owner: task_004a68e456a8 (Orca worker for assembly `Quire.Engine.Misc`).
- Branch: `feature/haiku-engine-misc-high`, based on `main`.
- Members: all 8 mcmisc owners are implemented in hand-written partials (`src/Quire.Engine.Misc/Mirrored/`). No `MirrorStub` markers remain. Member names match the original: `FileUtils` 21, `StringUtils` 17, `LispUtils` 26, `LispUtils+LinePartition` 10, `XmlUtils` 41, `HashSetUtils` 2, `Pair`2` 5, `Triple`3` 7. The 2 `<PrivateImplementationDetails>` methods are compiler-generated and have no original.
- Release IL report: `PYTHONUTF8=1 python tools/validation/release_il.py --base main --project Quire.Engine.Misc --all` → PASS; 131 checked, 131 exact, 0 missing, 0 skipped, 0 unfinished.
- Batch: local names renamed from ILSpy-style (`num2`, `flag3`, `text2`, `obj0`, `xml1`, `startPos2`, snake_case locals) to meaningful names; `global::` qualifiers replaced by `using` directives. Release IL unchanged by the rename (gate re-run after each step).
- Remaining mismatches: none.
- Not covered by the gate: field layout (the gate compares methods only). Control-flow shapes that are ILSpy-derived (`ExpandUnicode` flag state machine, `SelfTest` goto) were kept because the IL shape was not re-derived; they remain candidates for later idiomatic rewrites under the same gate.
