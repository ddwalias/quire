using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Quire.Controls;

public partial class DocumentScrollViewer : ScrollViewer
{
    private const string PartVerticalScrollBar = "PART_VerticalScrollBar";
    private ScrollBar _verticalScrollBar;

    public ScrollBar VerticalScrollBar => _verticalScrollBar;

    public string TooltipText
    {
        get => ((ContentControl)_verticalScrollBar?.ToolTip)?.Content.ToString();
        set
        {
            if (_verticalScrollBar.ToolTip is ToolTip toolTip)
            {
                toolTip.Content = value;
            }
        }
    }

    public override void OnApplyTemplate()
    {
        _verticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
        base.OnApplyTemplate();
    }

    public DocumentScrollViewer()
    {
    }
}
