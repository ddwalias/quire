# Ptc.Calculation.Core.CalculatableAggregateExtensions

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** IL inspected through `python tools\reference\reference.py Ptc.Calculation.Core.CalculatableAggregateExtensions --assembly Ptc.Calculation.Core --il`
- **Quire:** `src/Quire.Calculation.Core`; test: `Quire.Differential.Tests.PtcCalculationCoreCalculatableAggregateExtensionsMechanicalDifferentialTests`

| Member | Token | Contract | Evidence / limits | Status |
|---|---|---|---|---|
| `GetAggregateRl(ICalculatableAggregate)` | 0x0600007f | Allocate a string list with capacity one plus child count; append the aggregate Rl, then each child Rl in order | Direct Release Mathcad comparison with empty, ordered children, and a changing enumerable returned across the count and traversal reads | Verified |

The method directly iterates the second `CalculatableChildren` result to preserve the Mathcad behavior without calling the out-of-Lane `Ptc.EnumerableExtensions.Each` stub. Tier B interface properties are exercised through dispatch proxies in the direct comparison; this batch does not implement those interfaces.
