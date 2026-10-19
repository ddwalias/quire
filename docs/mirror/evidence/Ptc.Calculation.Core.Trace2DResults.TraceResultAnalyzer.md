# Ptc.Calculation.Core.Trace2DResults.TraceResultAnalyzer

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** `python tools\reference\reference.py --assembly Ptc.Calculation.Core Ptc.Calculation.Core.Trace2DResults.TraceResultAnalyzer --il`; ILSpy 11.1.0.9782
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `GetLabels(IList<IStringListResultNode>)` | 0x060002b7 | Sum meaningful counts, assert nonzero, then return meaningful strings | Calls `Ptc.DebugTools.DevAssert`; Quire.Core has a mirror stub outside Lane C ownership | Blocked |
| `AddOneDataSegment(IList<Point>, IList<double>, IList<double>, bool)` | 0x060002b8 | Pair x/y samples; preserve NaN separators and append the configured trailing separator | Calls `Ptc.Wpf.PointExtension.IsNaN`; Quire.Core has a mirror stub outside Lane C ownership | Blocked |
| `AddOneDataSegment(IList<Point>, double, IList<double>, bool)` | 0x060002b9 | Add non-NaN y values at one x; collapse repeated NaN separators and append the configured trailing separator | Release direct comparison includes valid values and repeated/trailing NaNs; Release IL exactly matches | Verified |
| `StringVectorToIndices(IStringListResultNode, int)` | 0x060002ba | Convert meaningful strings to increasing offset indices and other entries to NaN | Proxy provides `List`, `Count`, and `IsMeaningfulValue`; Release IL exactly matches | Verified |
| `ToDoubleDataList(IVectorListResultNode)` | 0x060002bb | Pass through double vectors; convert string vectors to continuing indices | Direct cases cover double/string vectors; unsupported vector types call out-of-scope `DebugTools.FailAssertion` and remain untested; Release IL exactly matches | Verified, cases limited |
| `BuildPointsForWaterfall(IList<Point>, IList<IList<double>>, IList<IList<double>>)` | 0x060002bc | Build paired x/y point segments across columns | Delegates to the public `AddOneDataSegment` overload, which depends on out-of-scope `PointExtension.IsNaN` | Blocked |
| `BuildPointsForBoxes(IList<Point>, IList<double>, IList<IList<double>>)` | 0x060002bd | Build box points, skipping NaN x values and null y rows | Direct comparison covers NaN, null rows, and separator insertion; Release IL exactly matches | Verified |
| `BuildTuplesOfPoints(IList<Point>, IList<double>, IList<double>[], IList<double>)` | 0x060002be | Build x/y points with per-vector x offsets and NaN row separators | Direct comparison covers NaN coordinates and offsets; Release IL exactly matches | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTraceResultAnalyzerMechanicalDifferentialTests`): five direct Release comparisons—private segment handling, string indexing, mixed double/string conversion, box points, and tuple points. Inputs include repeated and trailing NaNs, empty string values, continuing offsets, null rows, and per-vector x offsets. The proxy input adapters expose only the interface properties used by these methods.

The three blocked methods remain generated stubs. No Quire.Core members or Tier B Calculation members were changed.
