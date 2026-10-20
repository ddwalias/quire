# Ptc.Calculation.Core.Trace2DResults.ListResultNode`1

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** ILSpy C# and IL from `python tools\reference\reference.py --assembly Ptc.Calculation.Core 'Ptc.Calculation.Core.Trace2DResults.ListResultNode`1' --il`
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsListResultNode1Lifecycle_MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.ctor(IList<T>, int)` | 0x0600024f | Initializes `_meaningfulValuesCount` to -1 before `object::.ctor()`, then stores `data` and the requested meaningful count | Release IL shows the field initializer before the base call. Direct Mathcad concrete `DoubleListResultNode(IList<double>)` comparisons verify field state because the declaring generic base type is abstract. | Verified |
| `.ctor(IList<T>, T, int)` | 0x06000250 | Initializes the count field to -1, then stores `ExtendList(data, minLength, paddingValue)` in `_data` | Direct Mathcad comparisons cover padding, no-padding and null data through concrete derived constructors. | Verified |
| `ExtendList(IList<T>, int, T)` | 0x06000251 | Reads `data.Count()`; returns `data` when it already meets the minimum; otherwise copies it to a `List<T>` with `minLength` capacity and appends padding values until the minimum | Direct Mathcad cases cover passthrough, empty/short padding, zero minimum and null input. | Verified |
| `MeaningfulValuesCount get` | 0x06000253 | If the cached count is -1, counts items for which the virtual `IsMeaningfulValue` is true, stores the count, and returns it | Direct Mathcad comparison verifies lazy computation and cached read for an empty double list. Nonempty double counting calls `Quire.DoubleExtension.IsValid`, Tier C in `Ptc.Core.dll`, and is blocked outside Lane C. | Blocked |
| `Count get` | 0x06000254 | Returns `_data.Count` | Direct Release comparison on `ListResultNode<double>` | Verified |
| `Item get(int)` | 0x06000255 | Returns `_data[index]` | Direct Release comparison covers indexed read | Verified |
| `Item set(int, T)` | 0x06000256 | Assigns `_data[index] = value` | Direct Release comparison covers indexed write and backing list state | Verified |
| `List get` | 0x06000257 | Returns `_data` | Direct Release comparison checks the backing list | Verified |

The list accessors are already implemented and are directly compared by `PtcCalculationCoreTrace2DResultsListResultNode1MechanicalDifferentialTests` on closed `double` methods and concrete `DoubleListResultNode` targets. The abstract `NeutralValue` and `IsMeaningfulValue` members are declarations, not bodies.

The two protected base constructors cannot be instantiated with the current direct-method helper because it rejects a constructor declared on an abstract type. Their field effects and null/padding behavior are observed through direct Mathcad comparisons of concrete `DoubleListResultNode` constructors. The derived constructor failures in the Phase report exposed the missing base behavior: `_meaningfulValuesCount` remained 0 instead of -1, and the padded constructor failed to throw on null data. The focused Release lifecycle and accessor run passes 7/7.

The lazy getter's nonempty predicate branch remains blocked because `DoubleListResultNode.IsMeaningfulValue` calls `Quire.DoubleExtension.IsValid`, a Tier C member in `Ptc.Core.dll` outside Lane C's assignment. The empty-list direct case verifies cache initialization without crossing that boundary.

The existing generic `Mechanical.Compare(typeof(ListResultNode<>), token)` tests failed because that helper cannot close a method whose declaring type is generic. Those four tests have been replaced with artifact-bound direct comparisons on `ListResultNode<double>` methods and concrete derived targets; the cases cover count, indexed read/write and the backing list.
