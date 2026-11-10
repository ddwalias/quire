using System;
using System.ComponentModel;

namespace Quire.Controls;

internal sealed partial class ColorPickerTypeDescriptor : CustomTypeDescriptor
{
    public ColorPickerTypeDescriptor(ICustomTypeDescriptor parent)
        : base(parent)
    {
    }

    public override PropertyDescriptorCollection GetProperties()
    {
        return PropertyDescriptorCollection.Empty;
    }

    public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
    {
        return PropertyDescriptorCollection.Empty;
    }
}
