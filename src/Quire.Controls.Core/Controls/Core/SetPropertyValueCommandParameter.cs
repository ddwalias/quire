#nullable disable
using System.Windows;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class SetPropertyValueCommandParameter : Freezable
{
    public static DependencyProperty TargetObjectProperty = DependencyProperty.Register("TargetObject", typeof(IPropertyAggregate), typeof(SetPropertyValueCommandParameter));

    public static DependencyProperty PropertyNameProperty = DependencyProperty.Register("PropertyName", typeof(string), typeof(SetPropertyValueCommandParameter), new PropertyMetadata(string.Empty, null, CoercePropertyName));

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public static DependencyProperty DependencyPropValProperty = DependencyProperty.Register("DependencyPropVal", typeof(object), typeof(SetPropertyValueCommandParameter));

    public IPropertyAggregate TargetObject
    {
        get => (IPropertyAggregate)GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    public string PropertyName
    {
        get => (string)GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public object DependencyPropVal
    {
        get => GetValue(DependencyPropValProperty);
        set => SetValue(DependencyPropValProperty, value);
    }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public string UserData { get; set; }

    public SetPropertyValueCommandParameter(IPropertyAggregate obj, string propertyName, object val, string userData = null)
    {
        TargetObject = obj;
        PropertyName = propertyName;
        DependencyPropVal = val;
        UserData = userData;
    }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    public SetPropertyValueCommandParameter() { }

    private static object CoercePropertyName(DependencyObject d, object value)
    {
        string obj = value as string;
        DebugTools.DevAssert(obj != null, "Property name, provided as parameter in CoercePropertyName has to be non empty string.");
        return obj;
    }

    [MirrorVerified("PtcControlsCoreSetPropertyValueCommandParameterMechanicalDifferentialTests")]
    protected override Freezable CreateInstanceCore() => new SetPropertyValueCommandParameter();
}
