using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Quire;

public partial class TabManager
{
    private bool IsIndexInCollectionRange(int targetId)
    {
        return 0 <= targetId && targetId < ItemsCollection.Count;
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _mTabPanel = GetTemplateChild(PartTabPanel) as StackPanel;
        _scrollViewer = GetTemplateChild(PartTabScroll) as ScrollViewer;
        DebugTools.DevAssert(_scrollViewer?.CanContentScroll == false);
    }

    private ObservableCollection<WindowTabTupleData> ItemsCollection
    {
        get
        {
            return (ObservableCollection<WindowTabTupleData>)ItemsSource;
        }
    }

    private ContentPresenter TabItemContainerAtIndex(int index)
    {
        if (!IsIndexInCollectionRange(index))
        {
            return null;
        }

        return (ContentPresenter)ItemContainerGenerator.ContainerFromIndex(index);
    }
}
