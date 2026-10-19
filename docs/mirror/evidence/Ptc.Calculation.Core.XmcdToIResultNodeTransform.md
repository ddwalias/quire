# Ptc.Calculation.Core.XmcdToIResultNodeTransform

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen for the existing methods; `tools/reference/reference.py Ptc.Calculation.Core.XmcdToIResultNodeTransform --assembly Ptc.Calculation.Core --il` for `.cctor()` (IL output remains in ignored `.mirror-cache`)
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests`, `Quire.Differential.Tests.PtcCalculationCoreXmcdToIResultNodeTransformCctorMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `.cctor()` | 0x06000224 | Loads the `DataVectors` XSLT into `_resultVectorUnescapingXslt` | Directly compares output for text and nested vector nodes; Release IL equals Mathcad's | Verified |
| `.ctor(IResultNodeFactory)` | 0x06000225 | MemberGen: `_resultNodeFactory = resultNodeFactory;` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetUnitPowerDenominator(XmlNode)` | 0x06000237 | MemberGen: `GetIntegerFromAttribute(node, "power-denominator", 1)` | Mechanical; Release IL equals Mathcad's | Verified |
| `GetUnitPowerNumerator(XmlNode)` | 0x06000238 | MemberGen: `GetIntegerFromAttribute(node, "power-numerator", 1)` | Mechanical; Release IL equals Mathcad's | Verified |
| `BuildSymbolic(XmlNode)` | 0x0600022c | MemberGen: `_resultNodeFactory.CreateSymbolicResultNode(node.InnerXml)` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

**Observed** (`PtcCalculationCoreXmcdToIResultNodeTransformCctorMechanicalDifferentialTests`): invokes `.cctor()` on both artifacts and compares the XSLT output for a text node and nested vector nodes.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreXmcdToIResultNodeTransformMechanicalDifferentialTests`.
