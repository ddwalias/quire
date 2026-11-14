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

    public System.Windows.Size Size
    {
        get
        {
            return new System.Windows.Size(ActualWidth, ActualHeight);
        }
    }

    private System.Windows.Size AdornedElementDesiredSize
    {
        get
        {
            System.Windows.FrameworkElement frameworkElement = (System.Windows.FrameworkElement)AdornedElement;
            return (System.Windows.Size)frameworkElement.LayoutTransform.Transform((System.Windows.Point)frameworkElement.DesiredSize);
        }
    }

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

    private double GetOffset(NodeMessageAdorner current)
    {
        System.Collections.Generic.IEnumerable<NodeMessageAdorner> allNodeMessageAdorners = GetAllNodeMessageAdorners();
        System.Collections.Generic.List<NodeMessageAdorner> list = new System.Collections.Generic.List<NodeMessageAdorner>();
        System.Collections.Generic.List<NodeMessageAdorner> list2 = new System.Collections.Generic.List<NodeMessageAdorner>();
        System.Collections.Generic.List<NodeMessageAdorner> list3 = new System.Collections.Generic.List<NodeMessageAdorner>();
        if (allNodeMessageAdorners != null)
        {
            foreach (NodeMessageAdorner item in allNodeMessageAdorners)
            {
                if (item._type == NodeMessageAdornerType.Error)
                {
                    list.Add(item);
                }
                if (item._type == NodeMessageAdornerType.Warning)
                {
                    list2.Add(item);
                }
                if (item._type == NodeMessageAdornerType.Notification)
                {
                    list3.Add(item);
                }
            }
            if (current._type == NodeMessageAdornerType.Error)
            {
                int idx = list.IndexOf(current);
                return CalculateOffset(list, idx);
            }
            if (current._type == NodeMessageAdornerType.Warning)
            {
                int idx2 = list2.IndexOf(current);
                return CalculateOffset(list, list.Count) + CalculateOffset(list2, idx2);
            }
            if (current._type == NodeMessageAdornerType.Notification)
            {
                int idx3 = list3.IndexOf(current);
                return CalculateOffset(list, list.Count) + CalculateOffset(list2, list2.Count) + CalculateOffset(list3, idx3);
            }
        }
        return 0.0;
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
