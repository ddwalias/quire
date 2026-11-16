using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Controls.Plots.Presenter;
using Quire.Undo;

namespace Quire.Controls.Plots;

[TemplatePart(Name = "PART_SplitterTemplate", Type = typeof(ContentControl))]
internal partial class PlotGridSplitter : GridSplitter
{
    internal const string SplitterTemplate = "PART_SplitterTemplate";

    internal IPlotAxisView Axis { get; set; }

    public bool IsDraggingEnabled { get; set; } = true;

    public event MouseButtonEventHandler MouseDownAuxiliary;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        ContentControl contentControl = Template.FindName("PART_SplitterTemplate", this) as ContentControl;
        if (Axis == null)
        {
            return;
        }
        if (contentControl != null)
        {
            using (new CommandScope(new CompoundCommand()))
            {
                contentControl.Content = Axis;
                Axis.Splitter = this;
                return;
            }
        }
        DebugTools.DevAssert(false, null);
    }

    protected override void OnKeyDown(KeyEventArgs keyEventArgs)
    {
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (!IsDraggingEnabled)
        {
            MouseDownAuxiliary?.Invoke(this, e);
            e.Handled = true;
        }
        else
        {
            base.OnMouseDown(e);
            MouseDownAuxiliary?.Invoke(this, e);
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!IsDraggingEnabled)
        {
            e.Handled = true;
        }
        else
        {
            base.OnMouseMove(e);
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        if (!IsDraggingEnabled)
        {
            e.Handled = true;
        }
        else
        {
            base.OnMouseUp(e);
        }
    }

    public PlotGridSplitter()
    {
    }
}
