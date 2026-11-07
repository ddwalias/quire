#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;

namespace Quire.Controls.Text;

[ContentProperty("WrappedItem")]
public partial class WhiteboardItemInTextWrapper
{
    private IWhiteboardItemInText _wrappedItem;

    static WhiteboardItemInTextWrapper()
    {
        FrameworkElement.MarginProperty.OverrideMetadata(typeof(WhiteboardItemInTextWrapper),
            new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0.01)));
    }

    public WhiteboardItemInTextWrapper()
    {
        RequestBringIntoView += OnRequestBringIntoView;
    }

    public WhiteboardItemInTextWrapper(IWhiteboardItemInText wrappedItem)
    {
        WrappedItem = wrappedItem;
        RequestBringIntoView += OnRequestBringIntoView;
    }

    public IWhiteboardItemInText WrappedItemForXamlSerialization
    {
        get
        {
            if (!DocumentAttachedProperties.GetIsInCopyNonSerializable(this))
            {
                return null;
            }
            return _wrappedItem;
        }
        set => WrappedItem = value;
    }

    public IWhiteboardItemInText WrappedItem
    {
        get => _wrappedItem;
        set
        {
            if (_wrappedItem == value)
            {
                return;
            }
            IContainerItemManager itemManager = WhiteboardItemContainerExtensions.ItemManager;
            if (_wrappedItem != null)
            {
                itemManager.UnhookSizeChanged(_wrappedItem, new WhiteboardItemSizeChangedEventHandler(HandleWrappedItemSizeChanged));
            }
            _wrappedItem = value;
            if (_wrappedItem != null)
            {
                InvalidateMeasure();
                itemManager.HookupSizeChanged(_wrappedItem, new WhiteboardItemSizeChangedEventHandler(HandleWrappedItemSizeChanged));
                itemManager.ExecuteOnLoad(_wrappedItem, CompareBaselineOffset);
                _wrappedItem.IsEmbedded = true;
            }
            Content = _wrappedItem;
        }
    }

    private void OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs args)
    {
        if (!WhiteboardItemContainerExtensions.ItemManager.GetIsBringToView(WrappedItem))
        {
            args.Handled = true;
        }
    }

    private void HandleWrappedItemSizeChanged(IWhiteboardItem item, IList<WhiteboardItemSizeChangedEventArgs> argsList)
    {
        if (item is IEquationControl equationControl)
        {
            WhiteboardDependencyProperties.SetShowResizeIndicator(equationControl as DependencyObject, equationControl.IsExceedsWidthConstrain);
        }
        CompareBaselineOffset();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        Size size = base.MeasureOverride(constraint);
        if (WrappedItem != null)
        {
            SetBaselineOffset(-WrappedItem.VerticalAlignmentOffset);
        }
        return size;
    }

    private void SetBaselineOffset(double value)
    {
        TextBlock.SetBaselineOffset(this, value);
        Grid grid = Parent as Grid;
        if (grid != null && grid.Parent is InlineUIContainer)
        {
            TextBlock.SetBaselineOffset(grid, value);
        }
        DependencyObject item = WrappedItem as DependencyObject;
        if (item != null)
        {
            TextBlock.SetBaselineOffset(item, value);
        }
    }

    public override bool ShouldSerializeContent() => false;
}
