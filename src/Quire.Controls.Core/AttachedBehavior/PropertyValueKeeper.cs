#nullable disable
using System.Windows;
using System.Windows.Data;
using Quire.Mirror;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueKeeper
{
    private string _propertyPath;

    public string PropertyPath
    {
        get => _propertyPath;
        set => this.OneTimeSetExt(ref _propertyPath, value, "PropertyPath");
    }

    public void SetBinding(DependencyObject sourceObj)
    {
        BindingOperations.SetBinding(this, PropertyValueBase.PropertyValueProperty, new Binding(PropertyPath)
        {
            Source = sourceObj,
            Mode = BindingMode.OneWay
        });
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueKeeperMechanicalDifferentialTests")]
    public void RemoveBinding(DependencyObject sourceObj) => BindingOperations.ClearBinding(this, PropertyValueBase.PropertyValueProperty);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueKeeperMechanicalDifferentialTests")]
    public PropertyValueKeeper() { }
}
