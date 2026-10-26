#nullable disable
using System;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Quire.Controls.Core.Serialization;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class ResultFormat : DependencyObject, IXmlSerializable
{
    private readonly string ns;

    private static readonly DependencyProperty NumberFormatProperty = DependencyProperty.Register("NumberFormat", typeof(NumberFormat), typeof(ResultFormat));

    private static readonly DependencyProperty MatrixFormatProperty = DependencyProperty.Register("MatrixFormat", typeof(MatrixFormat), typeof(ResultFormat));

    public NumberFormat NumberFormat
    {
        get => GetValue(NumberFormatProperty) as NumberFormat;
        set => SetValue(NumberFormatProperty, value);
    }

    private MatrixFormat MatrixFormat
    {
        get => GetValue(MatrixFormatProperty) as MatrixFormat;
        set => SetValue(MatrixFormatProperty, value);
    }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public bool NumberFormatWasRead { get; private set; }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public bool MatrixFormatWasRead { get; private set; }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public string RootNodeFormatName { get; set; }

    public ResultFormat(string ns)
        : this(new NumberFormat(), new MatrixFormat(), ns)
    {
    }

    public ResultFormat(NumberFormat numberFormat, MatrixFormat matrixFormat, string ns)
    {
        this.ns = ns;
        NumberFormat = numberFormat;
        MatrixFormat = matrixFormat;
    }

    [MirrorVerified("PtcControlsCoreResultFormatMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    public void ReadXml(XmlReader reader)
    {
        string text = "resultFormat";
        if (!string.IsNullOrEmpty(RootNodeFormatName))
        {
            text = RootNodeFormatName;
        }
        reader.ReadStartElement(text, ns);
        NumberFormatSerializer numberFormatSerializer = new NumberFormatSerializer(NumberFormat, ns);
        numberFormatSerializer.ReadXml(reader);
        NumberFormatWasRead = numberFormatSerializer.NumberFormatWasRead;
        MatrixFormatSerializer matrixFormatSerializer = new MatrixFormatSerializer(MatrixFormat, ns);
        matrixFormatSerializer.ReadXml(reader);
        MatrixFormatWasRead = matrixFormatSerializer.MatrixFormatWasRead;
    }

    public void WriteXml(XmlWriter writer)
    {
        string text = "resultFormat";
        if (!string.IsNullOrEmpty(RootNodeFormatName))
        {
            text = RootNodeFormatName;
        }
        writer.WriteStartElement(text, ns);
        new NumberFormatSerializer(NumberFormat, ns).WriteXml(writer);
        new MatrixFormatSerializer(MatrixFormat, ns).WriteXml(writer);
        writer.WriteEndElement();
    }
}
