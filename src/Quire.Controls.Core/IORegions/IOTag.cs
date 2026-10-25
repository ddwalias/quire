#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core.IORegions;

public static partial class IOTag
{
    public static readonly DependencyProperty IOTagProperty = DependencyProperty.RegisterAttached("IOTag", typeof(IOTagType), typeof(IOTag), new PropertyMetadata((object)IOTagType.None, new PropertyChangedCallback(HandleIOTagPropertyChanged)));

    public static event Action<DependencyObject, bool> IOTagSet;

    public static event Action<DependencyObject, bool> IOTagUnSet;

    private static void HandleIOTagPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        bool flag = IOTagType.Input == ((IOTagType)e.NewValue & IOTagType.Input) && IOTagType.Input != ((IOTagType)e.OldValue & IOTagType.Input);
        bool flag2 = IOTagType.Input != ((IOTagType)e.NewValue & IOTagType.Input) && IOTagType.Input == ((IOTagType)e.OldValue & IOTagType.Input);
        bool flag3 = IOTagType.Output == ((IOTagType)e.NewValue & IOTagType.Output) && IOTagType.Output != ((IOTagType)e.OldValue & IOTagType.Output);
        bool flag4 = IOTagType.Output != ((IOTagType)e.NewValue & IOTagType.Output) && IOTagType.Output == ((IOTagType)e.OldValue & IOTagType.Output);
        if (flag && IOTagSet != null)
        {
            IOTagSet(d, true);
        }
        if (flag3 && IOTagSet != null)
        {
            IOTagSet(d, false);
        }
        if (flag2 && IOTagUnSet != null)
        {
            IOTagUnSet(d, true);
        }
        if (flag4 && IOTagUnSet != null)
        {
            IOTagUnSet(d, false);
        }
    }

    public static void SetIOTag(DependencyObject element, IOTagType tagType)
    {
        element.SetValue(IOTagProperty, tagType);
    }

    public static void ClearIOTag(DependencyObject element)
    {
        element.ClearValue(IOTagProperty);
    }

    public static IOTagType GetIOTag(DependencyObject element)
    {
        return (IOTagType)element.GetValue(IOTagProperty);
    }

    public static bool IsIOTagged(DependencyObject element)
    {
        return GetIOTag(element) != IOTagType.None;
    }

    private static bool IsRegionOfType(DependencyObject element, IOTagType type)
    {
        return type == (GetIOTag(element) & type);
    }

    public static bool HasInputTag(DependencyObject element)
    {
        return IsRegionOfType(element, IOTagType.Input);
    }

    public static bool HasOutputTag(DependencyObject element)
    {
        return IsRegionOfType(element, IOTagType.Output);
    }
}
