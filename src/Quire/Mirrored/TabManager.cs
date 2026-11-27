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
}
