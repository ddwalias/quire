# Ptc.Calculation.Core.ResultNodeToXmcdTransform

- **Original:** `Ptc.Calculation.Core.dll` 12.0.1.0, SHA-256 `f5a48d158835f91485948d42fc85e4fd27ddc5e2a17b4d886eb6722a753d4a09`, MVID `402d6e7b-941e-48f3-b17b-e1cd1a92b615`
- **Reference:** MemberGen (tools/Quire.Tools.MemberGen): straight-line bodies rebuilt from the IL; kept only where the Release IL equals Mathcad's
- **Quire:** `src/Quire.Calculation.Core`; tests: `Quire.Differential.Tests.PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests`

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| `Visit(IEmptyResultNode)` | 0x06000173 | MemberGen: `WritePlaceholderXml()` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IDisplayMatrixResultNode)` | 0x0600017b | MemberGen: `WriteDisplayMatrixXml(displayMatrix)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IEagerMatrixResultNode)` | 0x0600017c | MemberGen: `WriteMatrixXml(eagerMatrix)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Writer get` | 0x0600016a | MemberGen: `public XmlWriter Writer { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `Writer set` | 0x0600016b | MemberGen: `public XmlWriter Writer { get; private set; }` | Mechanical; Release IL equals Mathcad's | Verified |
| `WritePlaceholderXml()` | 0x0600016e | MemberGen: `Writer.WriteElementString("ml", "placeholder", "http://schemas.mathsoft.com/math50", string.Empty)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IRealResultNode)` | 0x06000174 | MemberGen: `WriteRealXml(real.Value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IImaginaryResultNode)` | 0x06000175 | MemberGen: `WriteImaginaryXml(imag.Value)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IComplexResultNode)` | 0x06000176 | MemberGen: `Writer.WriteStartElement("ml", "complex", "http://schemas.mathsoft.com/math50"); WriteRealXml(complex.RValue); WriteImaginaryXml(complex....` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IStringResultNode)` | 0x06000177 | MemberGen: `Writer.WriteStartElement("ml", "str", "http://schemas.mathsoft.com/math50"); Writer.WriteString(str.Value); Writer.WriteEndElement();` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(IFunctionResultNode)` | 0x06000178 | MemberGen: `Writer.WriteStartElement("ml", "functionValue", "http://schemas.mathsoft.com/math50"); Writer.WriteStartAttribute("description"); Writer....` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(ILazyMatrixResultNode)` | 0x0600017d | MemberGen: `lazyMatrix.MatrixProxy.WriteXml(Writer)` | Mechanical; Release IL equals Mathcad's | Verified |
| `Visit(Unit)` | 0x06000185 | MemberGen: `Writer.WriteStartElement("u", "unitReference", "http://schemas.mathsoft.com/units10"); Writer.WriteAttributeString("unit", unit.Canonical...` | Mechanical; Release IL equals Mathcad's | Verified |

## Test cases

**Observed** (`PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests`): each member on generated inputs (Mechanical: zero, null and distinct values per
parameter and field), comparing the result, the arguments and the target's fields with Mathcad's.

MemberGen members are compared by `Quire.Differential.Tests.PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests`.
