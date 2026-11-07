#nullable disable
using System;
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class MatrixFormatSerializer
{
    private const string NestedMatrixFormatsElemName = "nestedMatrixFormats";
    private const string NestedMatrixFormatElemName = "nestedMatrixFormat";
    private const string SizeAttrName = "size";
    private const string OffsetAttrName = "offset";
    private const string ShowIndicesAttrName = "show-indices";
    private const string ExpandNestedArraysAttrName = "expand-nested-arrays";
    private const string PositionAttrName = "position";

    private readonly MatrixFormat matrixFormat;
    private readonly string nameSpace;

    public MatrixFormatSerializer(MatrixFormat matrixFormat, string nameSpace)
    {
        this.matrixFormat = matrixFormat;
        this.nameSpace = nameSpace;
    }

    [MirrorVerified("PtcControlsCoreSerializationMatrixFormatSerializerMechanicalDifferentialTests")]
    public XmlSchema GetSchema() => throw new NotImplementedException();

    [MirrorVerified("PtcControlsCoreSerializationMatrixFormatSerializerMechanicalDifferentialTests")]
    public bool MatrixFormatWasRead { get; private set; }

    public void ReadXml(XmlReader reader)
    {
        // Release IL returns early on a null format, then compares the literal on the left.
        if (matrixFormat == null)
        {
            return;
        }
        if ("matrix" == reader.LocalName)
        {
            MatrixFormatWasRead = true;
            ReadMatrixFormatAttrs(reader, matrixFormat);
            ReadNestedMatrixFormats(reader, matrixFormat);
        }
    }

    private void ReadMatrixFormatAttrs(XmlReader reader, MatrixFormat format)
    {
        if (reader.MoveToAttribute(SizeAttrName))
        {
            format.Size = (TableSize)new TableSizeConverter().ConvertFromString(reader.GetAttribute(SizeAttrName));
        }
        if (reader.MoveToAttribute(OffsetAttrName))
        {
            format.Offset = (TableCoordinate)new TableCoordinateConverter().ConvertFromString(reader.GetAttribute(OffsetAttrName));
        }
        if (reader.MoveToAttribute(ShowIndicesAttrName))
        {
            format.ShowIndices = bool.Parse(reader.GetAttribute(ShowIndicesAttrName));
        }
        if (reader.MoveToAttribute(ExpandNestedArraysAttrName))
        {
            format.CollapseNestedMatrices = !bool.Parse(reader.GetAttribute(ExpandNestedArraysAttrName));
        }
    }

    private void ReadNestedMatrixFormats(XmlReader reader, MatrixFormat format)
    {
        reader.MoveToElement();
        if (reader.ReadToDescendant(NestedMatrixFormatsElemName))
        {
            reader.ReadToDescendant(NestedMatrixFormatElemName);
            do
            {
                ReadNestedMatrixFormat(reader, format);
            }
            while (reader.ReadToNextSibling(NestedMatrixFormatElemName));
            reader.ReadEndElement();
        }
    }

    private void ReadNestedMatrixFormat(XmlReader reader, MatrixFormat format)
    {
        MatrixFormat nested = new MatrixFormat();
        ReadMatrixFormatAttrs(reader, nested);
        if (reader.MoveToAttribute(PositionAttrName))
        {
            format.NestedMatrixFormats[(TableCoordinate)new TableCoordinateConverter().ConvertFromString(reader.GetAttribute(PositionAttrName))] = nested;
        }
        ReadNestedMatrixFormats(reader, nested);
    }

    public void WriteXml(XmlWriter writer)
    {
        // Release IL returns early on a null format, as in ReadXml.
        if (matrixFormat == null)
        {
            return;
        }
        writer.WriteStartElement("matrix", nameSpace);
        WriteMatrixFormatAttrs(writer, matrixFormat);
        WriteNestedMatrixFormats(writer, matrixFormat);
        writer.WriteEndElement();
    }

    private void WriteMatrixFormatAttrs(XmlWriter writer, MatrixFormat format)
    {
        writer.WriteAttributeString(SizeAttrName, format.Size.ToString());
        writer.WriteAttributeString(OffsetAttrName, format.Offset.ToString());
        writer.WriteStartAttribute(ShowIndicesAttrName);
        writer.WriteValue(format.ShowIndices);
        writer.WriteEndAttribute();
        writer.WriteStartAttribute(ExpandNestedArraysAttrName);
        writer.WriteValue(!format.CollapseNestedMatrices);
        writer.WriteEndAttribute();
    }

    private void WriteNestedMatrixFormats(XmlWriter writer, MatrixFormat format)
    {
        if (format.NestedMatrixFormats.PopulatedCount == 0)
        {
            return;
        }
        writer.WriteStartElement(NestedMatrixFormatsElemName, nameSpace);
        foreach (TableCoordinate populatedPosition in format.NestedMatrixFormats.PopulatedPositions)
        {
            WriteNestedMatrixFormat(writer, format.NestedMatrixFormats[populatedPosition], populatedPosition);
        }
        writer.WriteEndElement();
    }

    private void WriteNestedMatrixFormat(XmlWriter writer, MatrixFormat format, TableCoordinate position)
    {
        writer.WriteStartElement(NestedMatrixFormatElemName, nameSpace);
        WriteMatrixFormatAttrs(writer, format);
        writer.WriteAttributeString(PositionAttrName, position.ToString());
        WriteNestedMatrixFormats(writer, format);
        writer.WriteEndElement();
    }
}
