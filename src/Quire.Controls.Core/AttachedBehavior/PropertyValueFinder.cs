#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using Quire;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertyValueFinder : PropertyValueBase, IOneTimePropertyOwner
{
    private string _propertyName;

    private object _propertyProviderKey;

    private Type _propertyOwnerType;

    public string PropertyName
    {
        get => _propertyName;
        set => this.OneTimeSetExt(ref _propertyName, value, "PropertyName");
    }

    public object PropertyProviderKey
    {
        get => _propertyProviderKey;
        private set => this.OneTimeSetExt(ref _propertyProviderKey, value, "PropertyProviderKey");
    }

    public Type PropertyOwnerType
    {
        get => _propertyOwnerType;
        set => this.OneTimeSetExt(ref _propertyOwnerType, value, "PropertyOwnerType");
    }

    public void InitializeValue(FrameworkElement element, object propertyProviderKey)
    {
        try
        {
            if (PropertyProviderKey != propertyProviderKey)
            {
                PropertyProviderKey = propertyProviderKey;
            }
            DependencyPropertyDescriptor dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromName(PropertyName, PropertyOwnerType, element.GetType());
            DependencyProperty dependencyProperty = dependencyPropertyDescriptor.DependencyProperty;
            if (!element.HasLocalValue(dependencyProperty))
            {
                PropertyValueKeeper propertyValueKeeper = element.TryFindResource(PropertyProviderKey) as PropertyValueKeeper;
                dependencyPropertyDescriptor.SetValue(element, propertyValueKeeper.PropertyValue);
            }
        }
        catch (Exception ex)
        {
            DebugTools.DevAssert(false, ex.Message);
        }
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertyValueFinderMechanicalDifferentialTests")]
    public PropertyValueFinder() { }
}
