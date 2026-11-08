#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.Controls;

internal partial class PlotBackgroundTypeToXmlValueConverter
{
    [MirrorVerified("PtcControlsPlotBackgroundTypeToXmlValueConverterMechanicalDifferentialTests")]
    private string AttributeName { get; }

    [MirrorVerified("PtcControlsPlotBackgroundTypeToXmlValueConverterMechanicalDifferentialTests")]
    private Dictionary<string, object> AttributeNameToDefaultValueDictionary { get; }

    public PlotBackgroundTypeToXmlValueConverter(string attrName, Dictionary<string, object> attributeNameToDefaultValueDictionary)
        : base(typeof(BackgroundTypeE))
    {
        AttributeName = attrName;
        AttributeNameToDefaultValueDictionary = attributeNameToDefaultValueDictionary;
    }

    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType != typeof(string))
        {
            return base.ConvertTo(context, culture, value, destinationType);
        }
        string text = ((value is BackgroundTypeE backgroundTypeE) ? backgroundTypeE : BackgroundTypeE.Undefined).ToString().ToLowerInvariant();
        Dictionary<string, object> attributeNameToDefaultValueDictionary = AttributeNameToDefaultValueDictionary;
        if (!attributeNameToDefaultValueDictionary.ContainsKey(AttributeName) || !object.Equals(text, attributeNameToDefaultValueDictionary[AttributeName]?.ToString()))
        {
            return text;
        }
        return null;
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        object obj = base.ConvertFrom(context, culture, value);
        if (!BackgroundTypeE.Undefined.Equals(obj))
        {
            return obj;
        }
        return BackgroundTypeE.Paper;
    }
}
