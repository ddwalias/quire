#nullable disable
using System.Windows;
using System.Windows.Data;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class PropertyBinding
{
    [MirrorVerified("PtcWpfPropertyBindingMechanicalDifferentialTests")]
    public DependencyProperty TargetProperty;

    [MirrorVerified("PtcWpfPropertyBindingMechanicalDifferentialTests")]
    public Binding Binding;

    [MirrorVerified("PtcWpfPropertyBindingMechanicalDifferentialTests")]
    public BindingExpressionBase Set(DependencyObject targetObject) => BindingOperations.SetBinding(targetObject, TargetProperty, Binding);

    [MirrorVerified("PtcWpfPropertyBindingMechanicalDifferentialTests")]
    public void Clear(DependencyObject targetObject) => BindingOperations.ClearBinding(targetObject, TargetProperty);

    [MirrorVerified("PtcWpfPropertyBindingMechanicalDifferentialTests")]
    public PropertyBinding() { }
}
