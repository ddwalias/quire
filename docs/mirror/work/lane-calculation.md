# Lane Calculation — Quire.Calculation

- Owner: Lane Calculation (dispatch task_8628e3ed614d). Branch `mirror/calculation-20261108` from `main`.
- Exact batch 1: `ProductInfo.Constants` (ConverterProductName, ConverterExe, .ctor), `EngineConstants` (ResultKey, ErrorKey, IdBoundLabelsKey, WarningRedefinitionKey, Algorithms), `ThreadExtensions` (appCulture, appCultureUI static fields), `IDataExchangeIValueModel` (SetValue, GetValue), `IValueToIResultNodeTransform` (SimplifyUnits, Transform), `AddressErrorException` (4 constructors). Landed in commit `7b695a96`.
- Exact batch 2: `CollapsibleDispatchQueue` (.ctor, Enqueue, Run, Status enum, private fields, and compiler-generated closures `<>c` with 4 lambdas and `<>c__DisplayClass3_0`). Reordered member declarations to align Roslyn method ordinals with Mathcad's closure naming, and used `goto` to match exact branch leave shape.
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation` PASS, 34 checked, 0 failures. Report `.mirror-cache/release-il-6e545048a905/release-il.json`.
- Remaining Mathcad-paired mismatches from this lane: none of the members listed in batches 1–2.
