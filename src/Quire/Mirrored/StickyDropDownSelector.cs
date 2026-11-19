#nullable disable

using System.Windows;
using System.Windows.Data;

namespace Quire;

internal partial class StickyDropDownSelector
{
    public object StickyValue
    {
        get
        {
            return GetValue(StickyValueProperty);
        }
        set
        {
            SetValue(StickyValueProperty, value);
        }
    }

    protected override void UpdateHeaderFromChildItem(IDropDownSelectorItem item)
    {
    }

    protected override void SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is IDropDownSelectorItem dropDownSelectorItem)
        {
            CheckedValue = dropDownSelectorItem.Value;
            Binding binding = BindingOperations.GetBinding(this, CurrentValueProperty);
            if (binding != null && binding.Mode == BindingMode.TwoWay)
            {
                CurrentValue = CheckedValue;
            }

            StickyValue = dropDownSelectorItem.Value;
            base.UpdateHeaderFromChildItem(dropDownSelectorItem);
            UpdateChildrenToValue(CurrentValue);
        }
    }

    public StickyDropDownSelector() { }
}
