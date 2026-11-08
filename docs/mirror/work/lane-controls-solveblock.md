# Lane Controls SolveBlock

- Owner: SolveBlock lane; branch: `mirror/solveblock-20261108`.
- Batches 1–5 passed/landed; batch 5 `9481c27a`.
- Batch 6 result/error passthroughs: PASS (`36 checked`); initial MSB4166, retry PASS; landed `e5f6a6e4`.
- Batch 7 `WrappedCalculatable`: PASS (`37 checked`); landed `64b35a02`.
- Batch 8 constructor: PASS (`38 checked`); landed `f1756f18`.
- Batch 9 simple equation type update: PASS (`39 checked`); landed `1a947618`.
- Batch 10 `UpdateEquationType`: PASS (`40 checked`); landed `adcea50f`.
- Batch 11 `_equationType`: PASS (`40 checked`); landed `47c1dc7e`.
- Batch 12 notifier: PASS (`41 checked`); landed `91575596`.
- Batch 13 evaluation request: PASS (`42 checked`); landed `1ab5bfb`.
- Batch 14 error ownership: PASS (`44 checked`); landed `c9460106`.
- Batch 15 restore result: PASS (`45 checked`); landed `bfc84597`.
- Batch 16 `EquationType`: PASS (`47 checked`); landed `9e93b697`.
- Batch 17 aggregate equation updater: PASS (`49 checked`); landed `aa6074cc`.
- Batch 18 event hookup/unhook and disposal: PASS (`52 checked`; `.mirror-cache/release-il-2d197fca6c9a`).
- `GetEquationTypeOfRl` remains stubbed due enum/closure mismatch.
- Remaining: 369 stub markers in 18 generated types; no known mismatches in landed batches.
