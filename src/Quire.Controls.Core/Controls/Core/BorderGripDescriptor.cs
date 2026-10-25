#nullable disable
using System.Windows;
using System.Windows.Controls;

namespace Quire.Controls.Core;

public partial class BorderGripDescriptor : DependencyObject
{
    public static readonly DependencyProperty DockSideProperty = DependencyProperty.Register("DockSide", typeof(Dock), typeof(BorderGripDescriptor), new PropertyMetadata((object)Dock.Right));

    public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register("CanResize", typeof(bool), typeof(BorderGripDescriptor), new PropertyMetadata((object)true));

    public static readonly DependencyProperty PreserveDimensionsProperty = DependencyProperty.Register("PreserveDimensions", typeof(bool), typeof(BorderGripDescriptor), new PropertyMetadata((object)false));

    public Dock DockSide
    {
        get
        {
            return (Dock)GetValue(DockSideProperty);
        }
        set
        {
            SetValue(DockSideProperty, (object)value);
        }
    }

    public bool CanResize
    {
        get
        {
            return (bool)GetValue(CanResizeProperty);
        }
        set
        {
            SetValue(CanResizeProperty, (object)value);
        }
    }

    public bool PreserveDimensions
    {
        get
        {
            return (bool)GetValue(PreserveDimensionsProperty);
        }
        set
        {
            SetValue(PreserveDimensionsProperty, (object)value);
        }
    }

    public BorderGripDescriptor() { }
}
