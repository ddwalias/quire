#nullable disable
using System.Xml;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ResultNodeToXmcdTransform
{
    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IEmptyResultNode empty) => WritePlaceholderXml();

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IDisplayMatrixResultNode displayMatrix) => WriteDisplayMatrixXml(displayMatrix);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IEagerMatrixResultNode eagerMatrix) => WriteMatrixXml(eagerMatrix);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public XmlWriter Writer { get; private set; }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    private void WritePlaceholderXml() => Writer.WriteElementString("ml", "placeholder", "http://schemas.mathsoft.com/math50", string.Empty);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMatrixWriter_MechanicalDifferentialTests")]
    private void WriteMatrixXml(IMatrixResultNode matrix)
    {
        Writer.WriteStartElement("ml", "matrix", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("rows");
        Writer.WriteValue(matrix.Rows);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("cols");
        Writer.WriteValue(matrix.Columns);
        Writer.WriteEndAttribute();
        for (int i = 0; i < matrix.Columns; i++)
        {
            for (int j = 0; j < matrix.Rows; j++)
            {
                matrix[j, i].Accept(this);
            }
        }

        Writer.WriteEndElement();
    }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMatrixWriter_MechanicalDifferentialTests")]
    private void WriteDisplayMatrixXml(IDisplayMatrixResultNode displayMatrix)
    {
        if (displayMatrix.Viewport.Size.Equals(displayMatrix.Size))
        {
            WriteMatrixXml(displayMatrix);
            return;
        }

        Writer.WriteStartElement("ml", "matrix", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("rows");
        Writer.WriteValue(displayMatrix.Rows);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("cols");
        Writer.WriteValue(displayMatrix.Columns);
        Writer.WriteEndAttribute();
        Writer.WriteStartElement("ml", "display", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("offset");
        Writer.WriteValue(displayMatrix.ViewportOffset.ToString());
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("size");
        Writer.WriteValue(displayMatrix.Viewport.Size.ToString());
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
        for (int i = 0; i < displayMatrix.Viewport.Columns; i++)
        {
            for (int j = 0; j < displayMatrix.Viewport.Rows; j++)
            {
                displayMatrix.Viewport[j, i].Accept(this);
            }
        }

        Writer.WriteEndElement();
    }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IRealResultNode real) => WriteRealXml(real.Value);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IImaginaryResultNode imag) => WriteImaginaryXml(imag.Value);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IComplexResultNode complex)
    {
        Writer.WriteStartElement("ml", "complex", "http://schemas.mathsoft.com/math50");
        WriteRealXml(complex.RValue);
        WriteImaginaryXml(complex.IValue);
        Writer.WriteEndElement();
    }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IStringResultNode str)
    {
        Writer.WriteStartElement("ml", "str", "http://schemas.mathsoft.com/math50");
        Writer.WriteString(str.Value);
        Writer.WriteEndElement();
    }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(IFunctionResultNode function)
    {
        Writer.WriteStartElement("ml", "functionValue", "http://schemas.mathsoft.com/math50");
        Writer.WriteStartAttribute("description");
        Writer.WriteString(function.Description);
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
    }

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    public void Visit(ILazyMatrixResultNode lazyMatrix) => lazyMatrix.MatrixProxy.WriteXml(Writer);

    [MirrorVerified("PtcCalculationCoreResultNodeToXmcdTransformMechanicalDifferentialTests")]
    private void Visit(Unit unit)
    {
        Writer.WriteStartElement("u", "unitReference", "http://schemas.mathsoft.com/units10");
        Writer.WriteAttributeString("unit", unit.CanonicalName);
        Writer.WriteStartAttribute("power-numerator");
        Writer.WriteValue(unit.PowerNumerator);
        Writer.WriteEndAttribute();
        Writer.WriteStartAttribute("power-denominator");
        Writer.WriteValue(unit.PowerDenominator);
        Writer.WriteEndAttribute();
        Writer.WriteEndElement();
    }
}
