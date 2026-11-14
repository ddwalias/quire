# Quire.Utils checkpoint

- Owner: dispatched worker `task_91423dd05532`.
- Branch: `feature/utils-20261114`.
- Members: all 37 managed members across all 12 types of `utilsmgd` are implemented in hand-written sources (`BMPFormatter` 2, `COMStreamAdaptor` 17, `ImageConverter` 4, `ImageFormatter` 5, `JPEGFormatter` 5, `PNGFormatter` 2, `TransformHelper` 2, and 5 interfaces with 0 method bodies). No `MirrorStub` markers or unowned stubs remain.
- Release IL report: `PYTHONUTF8=1 python tools/validation/release_il.py --base main --project Quire.Utils` — PASS; 37 checked, 37 exact, 0 failures, 0 unfinished (report: `.mirror-cache/release-il-af534eff485f/release-il.json`).
- Remaining mismatches: none. Exact Release IL parity with Mathcad Prime 12.0.1.0 `utilsmgd.dll`.
