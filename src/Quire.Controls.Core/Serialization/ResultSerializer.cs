#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.Serialization;

public partial class ResultSerializer
{
    private const string ErrorId = "error-id";

    private const string NodeHandleAttr = "error-node-handle";

    private const string Argument = "errorArgument";

    private const string ResourceStringElement = "resource-string";

    private const string OriginalEngineErrorIdAttr = "previous-error-id";

    private const string ErrorCodeElem = "errorCode";

    private const string ResultData = "resultData";

    public const string Result = "result";

    public const string Trace2dResult = "Trace2dResult";

    private const string SymbolicResult = "symResult";

    private const string CalculationStatusAttr = "calculation-status";

    private const string ResultIdAttr = "result-id";

    private const string ResultTagAttr = "tag";

    private const string EngineErrorElem = "engineError";

    private const string EngineErrorsElem = "engineErrors";

    private const string WarningsElem = "Warnings";

    private const string WarningElem = "Warning";

    private const string NodeId = "nodeId";

    private const string OldType = "oldType";

    private const string OldLocation = "oldLocation";

    private static readonly XmcdToIResultNodeTransform _xmcdToResultNodeTransform = new XmcdToIResultNodeTransform(new SimpleResultNodeFactory());

    [MirrorVerified("PtcControlsCoreSerializationResultSerializerMechanicalDifferentialTests")]
    private readonly Dictionary<int, EngineError> _errorMap = new Dictionary<int, EngineError>();

    private int _nextKey = -1;

    [MirrorVerified("PtcControlsCoreSerializationResultSerializerMechanicalDifferentialTests")]
    private void AddError(EngineError error, int id) => _errorMap.Add(id, error);

    private int AddError(EngineError error)
    {
        _nextKey++;
        AddError(error, _nextKey);
        return _nextKey;
    }

    private EngineError FindError(int id)
    {
        if (_errorMap.ContainsKey(id))
        {
            return _errorMap[id];
        }

        return null;
    }

    private int FindError(EngineError error)
    {
        foreach (KeyValuePair<int, EngineError> item in _errorMap)
        {
            if (item.Value.IsEquivalentTo(error, checkNodeHandle: false))
            {
                return item.Key;
            }
        }

        return -1;
    }

    public void WriteXml(XElement xElement)
    {
        WriteXml(xElement, ResultManager.Instance);
    }

    public void WriteXml(XElement xElement, IResultManager resultManager)
    {
        XAttribute[] namespaceAttributes =
        {
            new XAttribute(XNamespace.Xmlns + "ml", "http://schemas.mathsoft.com/math50"),
            new XAttribute(XNamespace.Xmlns + "u", "http://schemas.mathsoft.com/units10")
        };
        foreach (XAttribute attribute in namespaceAttributes)
        {
            xElement.SetAttributeValue(attribute.Name, attribute.Value);
        }

        using (XmlWriter writer = xElement.CreateWriter())
        {
            foreach (KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> result in resultManager.Results)
            {
                EngineResult engineResult = result.Value.First;
                writer.WriteStartElement(ResultData, ResultDataSerializer.Result10);
                foreach (XAttribute attribute in namespaceAttributes)
                {
                    writer.WriteAttributeString("xmlns", attribute.Name.LocalName, null, attribute.Value);
                }

                writer.WriteAttributeString(ResultIdAttr, result.Key.ToString());
                if (engineResult.SerializationTag.IsNotEmpty())
                {
                    writer.WriteAttributeString(ResultTagAttr, engineResult.SerializationTag);
                }

                writer.WriteAttributeString(CalculationStatusAttr, engineResult.SynchronizationStatus.ToString());
                AddResultToElement(engineResult.ResultNode, writer);
                if (engineResult.Errors != null)
                {
                    WriteEngineErrors(engineResult.Errors, writer);
                }

                if (engineResult.Warnings.IsNotEmpty())
                {
                    WriteEngineWarnings(engineResult.Warnings, writer);
                }

                writer.WriteEndElement();
            }
        }

        RemoveNsFromChildren(xElement);
    }

    private static void AddResultToElement(IResultNode node, XmlWriter writer)
    {
        if (node is EmptySimpleResultNode)
        {
            return;
        }

        bool isWrapped = node is ISymbolicResultNode || node is ITrace2DResultNode;
        if (!isWrapped)
        {
            writer.WriteStartElement("ml", Result, "http://schemas.mathsoft.com/math50");
        }

        ResultNodeToXmcdTransform.Transform(node, writer);
        if (!isWrapped)
        {
            writer.WriteEndElement();
        }
    }

    private void WriteEngineErrors(IEnumerable<EngineError> errors, XmlWriter writer)
    {
        bool wroteContainer = false;
        foreach (EngineError error in errors)
        {
            if (error != null)
            {
                if (!wroteContainer)
                {
                    writer.WriteStartElement(EngineErrorsElem, ResultDataSerializer.Result10);
                    wroteContainer = true;
                }

                WriteEngineError(error, writer);
            }
        }

        if (wroteContainer)
        {
            writer.WriteEndElement();
        }
    }

    private void WriteEngineWarnings(EngineRedefInfo[] resultWarnings, XmlWriter writer)
    {
        List<EngineRedefInfo> warnings = resultWarnings?.ToList();
        if (warnings.IsNullOrEmpty())
        {
            return;
        }

        writer.WriteStartElement(WarningsElem, ResultDataSerializer.Result10);
        warnings.Each(warning => WriteWarning(warning, writer));
        writer.WriteEndElement();
    }

    private void WriteWarning(EngineRedefInfo warningRedefInfo, XmlWriter writer)
    {
        writer.WriteStartElement(WarningElem, ResultDataSerializer.Result10);
        writer.WriteAttributeString(NodeId, warningRedefInfo.Name);
        if (warningRedefInfo.OldLocation != null)
        {
            writer.WriteAttributeString(OldLocation, warningRedefInfo.OldLocation);
        }

        writer.WriteAttributeString(OldType, warningRedefInfo.OldType.ToString());
        writer.WriteEndElement();
    }

    private void WriteEngineError(EngineError error, XmlWriter writer)
    {
        writer.WriteStartElement(EngineErrorElem, ResultDataSerializer.Result10);
        writer.WriteAttributeString(ErrorId, AddError(error).ToString());
        writer.WriteAttributeString(NodeHandleAttr, error.NodeHandle.ToString());
        if (error.OriginalError != null)
        {
            writer.WriteAttributeString(OriginalEngineErrorIdAttr, FindError(error.OriginalError).ToString());
        }

        if (error.ErrorCode != null)
        {
            string message = error.Message;
            if (!message.IsValidXmlString())
            {
                message = message.StripInvalidXmlChars();
            }

            WriteElement(ErrorCodeElem, message, writer);
        }

        WriteElement(Argument, error.Argument.ToString(), writer);
        if (error.ResourceString != null)
        {
            WriteElement(ResourceStringElement, error.ResourceString, writer);
        }

        writer.WriteEndElement();
    }

    private static void WriteElement(string name, string valueString, XmlWriter writer)
    {
        writer.WriteStartElement(name, ResultDataSerializer.Result10);
        writer.WriteString(valueString);
        writer.WriteEndElement();
    }

    private static void RemoveNsFromChildren(XElement element)
    {
        foreach (XElement child in element.Nodes())
        {
            XAttribute[] namespaceAttributes =
            {
                child.Attribute(XName.Get("ml", "http://www.w3.org/2000/xmlns/")),
                child.Attribute(XName.Get("u", "http://www.w3.org/2000/xmlns/")),
                child.Attribute(XName.Get("xmlns"))
            };
            for (int i = 0; i < namespaceAttributes.Length; i++)
            {
                XAttribute attribute = namespaceAttributes[i];
                if (attribute != null)
                {
                    attribute.Remove();
                }
            }
        }
    }

    public void ReadXml(XElement xElement)
    {
        ReadXml(xElement, ResultManager.Instance);
    }

    public void ReadXml(XElement xElement, IResultManager resultManager) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private List<EngineRedefInfo> ReadEngineWarnings(XContainer element, int resultId)
    {
        return element?.Elements(ResultDataSerializer.GetXNameWithResultNameSpace(WarningElem))
            .Select(warningElement => ReadWarning(warningElement, resultId))
            .ToList();
    }

    private EngineRedefInfo ReadWarning(XElement warningXElement, int resultId)
    {
        EnumHelper.TryParse<EngineDefType>(ReadXAttribute(warningXElement, OldType), out var oldType);
        return new EngineRedefInfo
        {
            Name = ReadXAttribute(warningXElement, NodeId),
            OldLocation = ReadXAttribute(warningXElement, OldLocation),
            OldType = oldType,
        };
    }

    private EngineErrorCollection ReadEngineErrors(XContainer element, int resultId)
    {
        EngineErrorCollection errors = new EngineErrorCollection();
        if (element != null)
        {
            foreach (XElement errorElement in element.Elements(ResultDataSerializer.GetXNameWithResultNameSpace(EngineErrorElem)))
            {
                EngineError error = ReadEngineError(errorElement, resultId);
                errors.Add(error);
            }
        }

        return errors;
    }

    private EngineError ReadEngineError(XElement element, int resultId)
    {
        EngineError engineError = new EngineError();
        int id = int.Parse(ReadXAttribute(element, ErrorId));
        engineError.Message = ReadElement(element, ErrorCodeElem);
        engineError.NodeHandle = uint.Parse(ReadXAttribute(element, NodeHandleAttr));
        engineError.Argument = uint.Parse(ReadElement(element, Argument));
        engineError.ResourceString = ReadElement(element, ResourceStringElement);
        if (element.Attribute(OriginalEngineErrorIdAttr) != null)
        {
            int originalId = int.Parse(ReadXAttribute(element, OriginalEngineErrorIdAttr));
            engineError.OriginalError = FindError(originalId);
        }

        engineError.ErrorSource = ResultManager.Instance.FindCalculatable(resultId);
        engineError.ResourceString = EngineErrorMessages.GetStringByKey(engineError.ErrorCode);
        AddError(engineError, id);
        return engineError;
    }

    private static string ReadElement(XContainer errorElement, string name)
    {
        XElement element = errorElement.Element(ResultDataSerializer.GetXNameWithResultNameSpace(name));
        if (element != null)
        {
            return element.Value;
        }

        return null;
    }

    private static string ReadXAttribute(XElement errorElement, string attributeName)
    {
        XAttribute attribute = errorElement.Attribute(attributeName);
        if (attribute != null)
        {
            return attribute.Value;
        }

        return null;
    }
}
