#nullable disable

namespace Quire;

public static partial class TextElementPropertiesBlocker
{
    // Preserve Mathcad's method ordinal for the LINQ callbacks below.
    static TextElementPropertiesBlocker()
    {
        _dependencyPropertiesToFixate = ReadInheritedDependencyProperties(new global::System.Type[2]
        {
            typeof(global::System.Windows.Documents.TextElement),
            typeof(global::System.Windows.Documents.Typography)
        });
    }

    public static void FixatePropertiesValues(global::System.Windows.DependencyObject objToFixate)
    {
        if (DebugTools.DevAssert(objToFixate != null, null).Failed)
        {
            return;
        }
        foreach (global::System.Windows.DependencyProperty property in _dependencyPropertiesToFixate)
        {
            try
            {
                objToFixate.SetValue(property, objToFixate.GetValue(property));
            }
            catch (global::System.Exception exception)
            {
                DebugTools.FailAssertion(exception.Message);
            }
        }
    }

    private static global::System.Windows.DependencyProperty[] ReadInheritedDependencyProperties(global::System.Collections.Generic.IEnumerable<global::System.Type> typesListToScan) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static global::System.Collections.Generic.List<global::System.Windows.DependencyProperty> ReadInheritedDependencyProperties(global::System.Type typeToScan)
    {
        global::System.Collections.Generic.List<global::System.Windows.DependencyProperty> properties = new global::System.Collections.Generic.List<global::System.Windows.DependencyProperty>();
        foreach (string propertyName in from field in typeToScan.GetFields(global::System.Reflection.BindingFlags.Static | global::System.Reflection.BindingFlags.Public)
                                        where field.FieldType == typeof(global::System.Windows.DependencyProperty)
                                        select field.Name.Substring(0, field.Name.LastIndexOf("Property")))
        {
            global::System.ComponentModel.DependencyPropertyDescriptor descriptor = global::System.ComponentModel.DependencyPropertyDescriptor.FromName(propertyName, typeToScan, typeof(global::System.Windows.DependencyObject));
            if (descriptor != null)
            {
                global::System.Windows.DependencyProperty dependencyProperty = descriptor.DependencyProperty;
                if (dependencyProperty.DefaultMetadata is global::System.Windows.FrameworkPropertyMetadata { AffectsMeasure: not false } metadata &&
                    global::Quire.DebugTools.DevAssert(metadata.Inherits && descriptor.IsAttached && !((global::System.ComponentModel.PropertyDescriptor)descriptor).IsReadOnly, null).Passed)
                {
                    properties.Add(dependencyProperty);
                }
            }
        }
        return properties;
    }
}
