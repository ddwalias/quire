#nullable disable
using System.Globalization;
using System.Reflection;
using Quire.Mirror;

namespace Quire.XamlGeneratedNamespace;

public partial class GeneratedInternalTypeHelper
{
    [MirrorVerified("Release IL")]
    protected override object CreateInstance(Type type, CultureInfo culture) => Activator.CreateInstance(type, (BindingFlags)564, null, null, culture);

    [MirrorVerified("Release IL")]
    protected override object GetPropertyValue(PropertyInfo propertyInfo, object target, CultureInfo culture) => propertyInfo.GetValue(target, (BindingFlags)0, null, null, culture);

    [MirrorVerified("Release IL")]
    protected override void SetPropertyValue(PropertyInfo propertyInfo, object target, object value, CultureInfo culture) => propertyInfo.SetValue(target, value, (BindingFlags)0, null, null, culture);

    [MirrorVerified("Release IL")]
    protected override void AddEventHandler(EventInfo eventInfo, object target, Delegate handler) => eventInfo.AddEventHandler(target, handler);

    [MirrorVerified("Release IL")]
    public GeneratedInternalTypeHelper() { }
}
