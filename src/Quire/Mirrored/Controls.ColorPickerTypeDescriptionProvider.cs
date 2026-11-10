using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace Quire.Controls;

internal sealed partial class ColorPickerTypeDescriptionProvider : TypeDescriptionProvider
{
    private static readonly TypeDescriptionProvider _defaultProvider = TypeDescriptor.GetProvider(typeof(UserControl));

    public ColorPickerTypeDescriptionProvider()
        : base(_defaultProvider)
    {
    }

    public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
    {
        return new ColorPickerTypeDescriptor(base.GetTypeDescriptor(objectType, instance));
    }
}
