#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Quire.Controls.Core.IORegions;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Core.Serialization;

internal partial class IntegrationRegionSerializer : IXElementSerializable
{
    private readonly IIORegionContainer _documentArea;

    private ISerializationHelper _serializationHelper;

    public IntegrationRegionSerializer(IIORegionContainer area, ISerializationHelper serializationHelper)
    {
        _documentArea = area;
        _serializationHelper = serializationHelper;
    }

    public void ReadXml(XElement xElement)
    {
        List<IInputRegion> list = new List<IInputRegion>();
        List<IOutputRegion> list2 = new List<IOutputRegion>();
        IEnumerable<XElement> enumerable = xElement.Elements(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace("region"));
        List<UIElement> regions = _documentArea.IORegions.OfType<UIElement>().ToList();
        foreach (XElement item in enumerable)
        {
            bool condition = false;
            XAttribute xAttribute = item.Attribute("region-id");
            string s = (xAttribute != null) ? xAttribute.Value : null;
            long result;
            UIElement uIElement = null;
            if (long.TryParse(s, out result))
            {
                uIElement = _serializationHelper.GetRegionById(regions, result);
            }
            xAttribute = item.Attribute("ioTagType");
            IOTagType enumValue;
            if (uIElement != null && xAttribute != null && EnumHelper.TryParse<IOTagType>(xAttribute.Value, out enumValue, true))
            {
                IInputRegion inputRegion = uIElement as IInputRegion;
                IOutputRegion outputRegion = uIElement as IOutputRegion;
                if (inputRegion != null && enumValue.HasFlag(IOTagType.Input))
                {
                    if (inputRegion.IsSuitableForInputTagging())
                    {
                        ReadIOAliases(uIElement, item, true);
                        list.Add(inputRegion);
                    }
                    else
                    {
                        MC14ConversionAnnotation.AddConversionAnnotation(uIElement, MC14ConversionAnnotationType.InputRegionAnnotation);
                    }
                    condition = true;
                }
                if (outputRegion != null && enumValue.HasFlag(IOTagType.Output))
                {
                    if (outputRegion.IsSuitableForOutputTagging())
                    {
                        ReadIOAliases(uIElement, item, false);
                        list2.Add(outputRegion);
                    }
                    else
                    {
                        MC14ConversionAnnotation.AddConversionAnnotation(uIElement, MC14ConversionAnnotationType.OutputRegionAnnotation);
                    }
                    condition = true;
                }
            }
            DebugTools.DevAssert(condition, null);
        }
        _documentArea.InputOutputRegionManager.ToggleInputRegions(list);
        _documentArea.InputOutputRegionManager.ToggleOutputRegions(list2);
    }

    private void ReadIOAliases(DependencyObject ioRegion, XElement xElement, bool isInp)
    {
        string text = (isInp ? "inputAliases" : "outputAliases");
        XElement xElement2 = xElement.Element(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace(text));
        if (xElement2 != null)
        {
            List<string> list = new List<string>();
            foreach (XElement item in xElement2.Elements(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace("alias")))
            {
                list.Add(item.Value);
            }
            if (isInp)
            {
                IOItemsAliasManager.SetInputAliases(ioRegion, list);
            }
            else
            {
                IOItemsAliasManager.SetOutputAliases(ioRegion, list);
            }
        }
    }

    public void WriteXml(XElement xElement)
    {
        WriteIORegionsFromContainer(xElement, _documentArea);
    }

    private void WriteIORegionsFromContainer(XElement xElement, IIORegionContainer ioContainer)
    {
        foreach (IIORegionBase item in ioContainer.IORegions)
        {
            if (item is IIORegion && IOTag.IsIOTagged(item as DependencyObject))
            {
                xElement.Add(WriteRegion(item as IIORegion));
            }
            if (item is IIORegionContainer)
            {
                WriteIORegionsFromContainer(xElement, item as IIORegionContainer);
            }
        }
    }

    private XElement WriteRegion(IIORegion element)
    {
        XElement xElement = new XElement(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace("region"));
        XElement xElement2 = WriteIOAliases(element, true);
        if (!xElement2.IsEmpty)
        {
            xElement.Add(xElement2);
        }
        xElement2 = WriteIOAliases(element, false);
        if (!xElement2.IsEmpty)
        {
            xElement.Add(xElement2);
        }
        UIElement uIElement = element as UIElement;
        xElement.SetAttributeValue("region-id", _serializationHelper.GetRegionId(uIElement));
        xElement.SetAttributeValue("ioTagType", IOTag.GetIOTag(uIElement));
        return xElement;
    }

    private XElement WriteIOAliases(IIORegion element, bool isInp)
    {
        XElement xElement = new XElement(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace(isInp ? "inputAliases" : "outputAliases"));
        List<string> aliases = IOItemsAliasManager.GetAliases(element as DependencyObject, isInp);
        if (aliases != null && aliases.Count > 0)
        {
            foreach (string item in aliases)
            {
                XElement xElement2 = new XElement(IntegrationDataSerializer.GetXNameWithIntegrationNameSpace("alias"));
                xElement2.SetValue(item);
                xElement.Add(xElement2);
            }
        }
        return xElement;
    }
}
