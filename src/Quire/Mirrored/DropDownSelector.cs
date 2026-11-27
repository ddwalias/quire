using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Quire.Xml;

namespace Quire;

public partial class DropDownSelector
{
    protected Dictionary<IDropDownSelectorItem, object> _headerDict;

    public EmptyDropDownSelectorItem[] EmptyItems { get; set; } = new EmptyDropDownSelectorItem[1]
    {
        new EmptyDropDownSelectorItem()
    };

    public object CurrentValue
    {
        get
        {
            return GetValue(CurrentValueProperty);
        }
        set
        {
            SetValue(CurrentValueProperty, value);
        }
    }

    public object CheckedValue
    {
        get
        {
            return GetValue(CheckedValueProperty);
        }
        set
        {
            SetValue(CheckedValueProperty, value);
        }
    }

    public bool UpdateHeader { get; set; }

    protected IEnumerable<IDropDownSelectorItem> DropDownItems => Items.OfType<IDropDownSelectorItem>();

    private void OnItemContainerGeneratorOnStatusChanged(object sender, EventArgs e)
    {
        if (ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
        {
            return;
        }

        DropDownItems.Select(GetElement).Each(SubscribeElementToClickEvent);
    }

    public override void EndInit()
    {
        SaveHeaders();
        foreach (IDropDownSelectorItem dropDownItem in DropDownItems)
        {
            SubscribeElementToClickEvent(GetElement(dropDownItem));
            if ((dropDownItem.Value == null && CurrentValue == null) || (dropDownItem.Value != null && dropDownItem.Value.Equals(CurrentValue)))
            {
                dropDownItem.IsChecked = true;
            }

            if (dropDownItem.IsChecked)
            {
                UpdateHeaderFromChildItem(dropDownItem);
            }
        }

        base.EndInit();
    }

    private DropDownSelectorItem GetElement(object item)
    {
        if (IsItemItsOwnContainer(item))
        {
            return item as DropDownSelectorItem;
        }

        if (ItemContainerGenerator.ContainerFromItem(item) is UIElement uIElement)
        {
            if (VisualTreeHelper.GetChildrenCount(uIElement) == 0)
            {
                uIElement.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }

            if (DebugTools.DevAssert(VisualTreeHelper.GetChildrenCount(uIElement) > 0, null).Passed)
            {
                return VisualTreeHelper.GetChild(uIElement, 0) as DropDownSelectorItem;
            }
        }

        return null;
    }

    private void SaveHeaders()
    {
        if (!UpdateHeader)
        {
            _headerDict = null;
        }
        else
        {
            if (Items.Count == 0)
            {
                return;
            }

            _headerDict = new Dictionary<IDropDownSelectorItem, object>(Items.Count + EmptyItems.Length);
            double maxWidth = 0.0;
            foreach (IDropDownSelectorItem item in DropDownItems.Concat(EmptyItems))
            {
                AddMeasuredDuplicateElemToHeaderDict(item, ref maxWidth);
            }

            SetHeadersMinimumWidth(maxWidth);
        }
    }

    protected void SetHeadersMinimumWidth(double maxWidth)
    {
        if (DebugTools.DevAssert(maxWidth > 0.0, null).Passed && DebugTools.AssertNotNull(_headerDict, null).Passed)
        {
            foreach (FrameworkElement item in _headerDict.Values.OfType<FrameworkElement>())
            {
                item.MinWidth = maxWidth;
            }
        }
    }

    protected static void HandleCurrentValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DropDownSelector obj = d as DropDownSelector;
        DebugTools.DevAssert(obj != null, null);
        obj.UpdateChildrenToValue(e.NewValue);
    }

    protected void UpdateChildrenToValue(object newValue)
    {
        EnsureItemHeader(newValue);
        IDropDownSelectorItem dropDownSelectorItem = null;
        foreach (IDropDownSelectorItem dropDownItem in DropDownItems)
        {
            if (MatchValueToItem(dropDownItem, newValue))
            {
                dropDownItem.IsChecked = true;
                dropDownSelectorItem = dropDownItem;
            }
            else
            {
                dropDownItem.IsChecked = false;
            }
        }

        if (dropDownSelectorItem == null)
        {
            EmptyDropDownSelectorItem[] emptyItems = EmptyItems;
            foreach (EmptyDropDownSelectorItem emptyDropDownSelectorItem in emptyItems)
            {
                if (MatchValueToItem(emptyDropDownSelectorItem, newValue))
                {
                    dropDownSelectorItem = emptyDropDownSelectorItem;
                    break;
                }
            }
        }

        UpdateHeaderFromChildItem(dropDownSelectorItem ?? EmptyItems[0]);
    }

    protected virtual void EnsureItemHeader(object newValue)
    {
    }

    protected static bool MatchValueToItem(IDropDownSelectorItem childItem, object value)
    {
        object childValue = childItem.Value;
        if (value != null && childValue != null)
        {
            return value.ToString().Equals(childValue.ToString());
        }

        return value == childValue;
    }

    private static object Duplicate(object obj)
    {
        if (obj != null)
        {
            if (!(obj is string text))
            {
                return XmlHelper.CreateXamlCopy(obj);
            }

            return new TextBlock
            {
                Text = text
            };
        }

        return null;
    }
}
