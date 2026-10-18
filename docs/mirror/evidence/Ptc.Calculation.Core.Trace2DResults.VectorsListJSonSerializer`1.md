# Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen for `Write`; `tools/reference/reference.py 'Ptc.Calculation.Core.Trace2DResults.VectorsListJSonSerializer`1' --assembly Ptc.Calculation.Core --il` for `.cctor()` (IL output remains in ignored `.mirror-cache`)
- **Quire:** `src/Quire.Calculation.Core`; test: `Quire.Differential.Tests.PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Write(Stream, !0[])` | 0x0600023d | MemberGen: `_jSonVectorsListSerializer.WriteObject(jsonStream, flatList)` | Mechanical; Release IL equals Mathcad's | Verified |
| `.cctor()` | 0x0600023f | Creates a `DataContractJsonSerializer` for `T[]` and stores it in `_jSonVectorsListSerializer` | Directly compared; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests`): compares `Write` for empty, one-element and mixed-value `double[]` inputs by the bytes written to separate streams; directly invokes `.cctor()` on closed `double` serializer types and compares the initialized serializer type.

**Observed** (`PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests.cctor_0600023f`): directly invokes the closed `double[]` serializer's `.cctor()` on both artifacts and compares the initialized serializer type; the existing `Write` comparison verifies serialization behavior.
