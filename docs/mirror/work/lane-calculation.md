# Lane Calculation — Quire.Calculation

- Owner: Lane Calculation (dispatch task_8628e3ed614d). Branch `mirror/calculation-20261108` from `main`.
- Exact batch 1: `ProductInfo.Constants` (ConverterProductName, ConverterExe, .ctor), `EngineConstants` (ResultKey, ErrorKey, IdBoundLabelsKey, WarningRedefinitionKey, Algorithms), `ThreadExtensions` (appCulture, appCultureUI static fields), `IDataExchangeIValueModel` (SetValue, GetValue), `IValueToIResultNodeTransform` (SimplifyUnits, Transform), `AddressErrorException` (4 constructors).
- Release IL: `python tools/validation/release_il.py --base main --project Quire.Calculation` PASS, 27 checked, 0 failures. Report `.mirror-cache/release-il-d12f906b3f6f/release-il.json`.
- Remaining Mathcad-paired mismatches from this lane: none of the members listed in batch 1.
