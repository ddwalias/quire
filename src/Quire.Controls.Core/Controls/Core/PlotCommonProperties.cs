#nullable disable
using System;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class PlotCommonProperties : DependencyObject
{
    public static class PropertyNames
    {
        public const string ShowTicks = "ShowTicks";

        public const string ShowNumbers = "ShowNumbers";

        public const string ShowAxisExpressions = "ShowAxisExpressions";
    }

    private static readonly Type _typeOfThis = typeof(PlotCommonProperties);

    public static readonly DependencyProperty ShowAxisExpressionsProperty = DependencyProperty.Register("ShowAxisExpressions", typeof(bool), _typeOfThis, new PropertyMetadata((object)true));

    public static readonly DependencyProperty ShowTicksProperty = DependencyProperty.Register("ShowTicks", typeof(bool), _typeOfThis, CreateShowTicksMetadata());

    public static readonly DependencyProperty ShowNumbersProperty = DependencyProperty.Register("ShowNumbers", typeof(bool), _typeOfThis, CreateShowNumbersMetadata());

    public const bool ShowTicksDefaultValue = true;

    public const bool ShowNumbersDefaultValue = true;

    public static FrameworkPropertyMetadata CreateShowTicksMetadata(PropertyChangedCallback propertyChangedCallback)
    {
        return new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback);
    }

    public static FrameworkPropertyMetadata CreateShowTicksMetadata()
    {
        return new FrameworkPropertyMetadata((object)true, FrameworkPropertyMetadataOptions.AffectsRender);
    }

    public static FrameworkPropertyMetadata CreateShowNumbersMetadata(PropertyChangedCallback propertyChangedCallback)
    {
        return new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender, propertyChangedCallback);
    }

    public static FrameworkPropertyMetadata CreateShowNumbersMetadata()
    {
        return new FrameworkPropertyMetadata((object)true, FrameworkPropertyMetadataOptions.AffectsRender);
    }

    [MirrorVerified("PtcControlsCorePlotCommonPropertiesMechanicalDifferentialTests")]
    public PlotCommonProperties()
    {
    }
}
