using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Quire.Controls.Worksheet;

internal partial class WorksheetItemsControl : ItemsControl
{
    protected override DependencyObject GetContainerForItemOverride()
    {
        return new WorksheetItemPresenter();
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return false;
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        WorksheetItemPresenter worksheetItemPresenter = (WorksheetItemPresenter)element;
        FrameworkElement frameworkElement = (FrameworkElement)item;
        worksheetItemPresenter.Child = frameworkElement;
        BindingOperations.SetBinding(worksheetItemPresenter, Panel.ZIndexProperty, new Binding
        {
            Source = frameworkElement,
            Path = new PropertyPath(Panel.ZIndexProperty)
        });
    }

    protected override void ClearContainerForItemOverride(DependencyObject element, object item)
    {
        WorksheetItemPresenter worksheetItemPresenter = (WorksheetItemPresenter)element;
        worksheetItemPresenter.Child = null;
        BindingOperations.ClearBinding(worksheetItemPresenter, Panel.ZIndexProperty);
    }
}
