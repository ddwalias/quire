#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NodeMessageAdorner
{
    private void NodeMessageAdorner_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
        if (e.HeightChanged)
        {
            InvalidateArrangeForAllNodeMessageAdornersExcludingSelf();
        }
    }

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    protected override void AddToAdornerLayer() => FireAdornerAdded();

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private string _message;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public string Message => _message;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private readonly NodeMessageAdornerType _type;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public NodeMessageAdornerType Type => _type;

    public override void Remove()
    {
        base.Remove();
        AdornedElement.RemoveHandler(CommonRoutedEvents.WasMeasuredEvent, new System.Windows.RoutedEventHandler(HandleAdornedElementWasMeasured));
        InvalidateArrangeForAllNodeMessageAdornersExcludingSelf();
    }

    private System.Collections.Generic.IEnumerable<NodeMessageAdorner> GetAllNodeMessageAdorners()
    {
        System.Windows.Documents.Adorner[] adorners = (ParentAdornerLayer != null) ? ParentAdornerLayer.GetAdorners(AdornedElement) : null;
        if (adorners == null)
        {
            return null;
        }
        return System.Linq.Enumerable.OfType<NodeMessageAdorner>(adorners);
    }

    private void InvalidateArrangeForAllNodeMessageAdornersExcludingSelf()
    {
        System.Collections.Generic.IEnumerable<NodeMessageAdorner> allNodeMessageAdorners = GetAllNodeMessageAdorners();
        if (allNodeMessageAdorners != null)
        {
            foreach (NodeMessageAdorner adorner in allNodeMessageAdorners)
            {
                if (adorner != this)
                {
                    adorner.InvalidateArrange();
                }
            }
        }
    }

    private static double CalculateOffset(System.Collections.Generic.IList<NodeMessageAdorner> adorners, int idx)
    {
        double num = 0.0;
        for (int i = 0; i < idx; i++)
        {
            num += adorners[i].ActualHeight;
        }
        return num;
    }

    public static System.Windows.FrameworkElement GetAdornedElement(System.Windows.FrameworkElement control)
    {
        if (control is IIndirectMessageAdornerOwner indirectMessageAdornerOwner)
        {
            return indirectMessageAdornerOwner.GetAdornedElement();
        }
        return control;
    }
}
