#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class NodeMessageAdorner
{
    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private string _message;

    private readonly TextBox _messageTextBox = new TextBox();

    private readonly Border _messageBorder = new Border();

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    private readonly NodeMessageAdornerType _type;

    private void NodeMessageAdorner_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.HeightChanged)
        {
            InvalidateArrangeForAllNodeMessageAdornersExcludingSelf();
        }
    }

    public NodeMessageAdorner(FrameworkElement adornedElement, string error, NodeMessageAdornerType type)
        : base(adornedElement, false)
    {
        Binding binding = new Binding(string.Format("{0}.Inverse", FrameworkElement.LayoutTransformProperty.Name))
        {
            Source = adornedElement
        };
        binding.Converter = GeneralTransformToTransformConverter.Instance;
        SetBinding(FrameworkElement.LayoutTransformProperty, binding);
        _type = type;
        _message = error;
        IsHitTestVisible = true;
        Opacity = 1.0;
        _messageBorder.Child = _messageTextBox;
        _messageTextBox.Text = _message;
        _messageTextBox.FontFamily = new FontFamily("Verdana");
        _messageTextBox.IsReadOnly = true;
        _messageTextBox.FontSize = 7.0;
        _messageTextBox.Padding = new Thickness(2.0);
        _messageTextBox.TextWrapping = TextWrapping.Wrap;
        _messageTextBox.BorderThickness = new Thickness(0.0);
        _messageTextBox.Focusable = false;
        switch (type)
        {
            case NodeMessageAdornerType.Error:
                _messageTextBox.Background = new SolidColorBrush(Color.FromRgb(250, 210, 210));
                break;
            case NodeMessageAdornerType.Warning:
                _messageTextBox.Background = new SolidColorBrush(Color.FromRgb(210, 250, 210));
                break;
            case NodeMessageAdornerType.Notification:
                _messageTextBox.Background = new SolidColorBrush(Color.FromRgb(210, 210, 250));
                break;
        }
        AddVisualChild(_messageBorder);
        AddLogicalChild(_messageBorder);
        Setup();
    }

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public string Message => _message;

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    public NodeMessageAdornerType Type => _type;

    private void Setup()
    {
        if (AdornedElement != null)
        {
            AdornedElement.GotKeyboardFocus += delegate
            {
                Visibility = Visibility.Visible;
            };
            AdornedElement.LostKeyboardFocus += delegate
            {
                Visibility = Visibility.Hidden;
            };
            Visibility = ((AdornedElement.IsKeyboardFocusWithin || !AdornedElement.Focusable) ? Visibility.Visible : Visibility.Hidden);
            AdornedElement.AddHandler(CommonRoutedEvents.WasMeasuredEvent, new RoutedEventHandler(HandleAdornedElementWasMeasured));
        }
        SizeChanged += NodeMessageAdorner_SizeChanged;
    }

    private void HandleAdornedElementWasMeasured(object sender, RoutedEventArgs routedEventArgs)
    {
        if (_messageBorder.Width != CalculateDesiredMessageBorderWidth())
        {
            InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size constraint)
    {
        constraint.Width = CalculateDesiredMessageBorderWidth();
        _messageBorder.Width = constraint.Width;
        _messageBorder.Measure(constraint);
        return new Size(constraint.Width, _messageBorder.DesiredSize.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset = GetOffset(this);
        Point location = new Point(0.0, AdornedElementDesiredSize.Height + offset);
        Rect finalRect = new Rect(location, finalSize);
        _messageBorder.Arrange(finalRect);
        return finalSize;
    }

    public Point RelativeLocation
    {
        get
        {
            return _messageBorder.TranslatePoint(new Point(0.0, 0.0), this);
        }
    }

    public Size Size
    {
        get
        {
            return new Size(ActualWidth, ActualHeight);
        }
    }

    private Size AdornedElementDesiredSize
    {
        get
        {
            FrameworkElement frameworkElement = (FrameworkElement)AdornedElement;
            return (Size)frameworkElement.LayoutTransform.Transform((Point)frameworkElement.DesiredSize);
        }
    }

    private double CalculateDesiredMessageBorderWidth()
    {
        return System.Math.Max(100.0, AdornedElementDesiredSize.Width);
    }

    public override void Remove()
    {
        base.Remove();
        AdornedElement.RemoveHandler(CommonRoutedEvents.WasMeasuredEvent, new RoutedEventHandler(HandleAdornedElementWasMeasured));
        InvalidateArrangeForAllNodeMessageAdornersExcludingSelf();
    }

    private IEnumerable<NodeMessageAdorner> GetAllNodeMessageAdorners()
    {
        Adorner[] adorners = (ParentAdornerLayer != null) ? ParentAdornerLayer.GetAdorners(AdornedElement) : null;
        if (adorners == null)
        {
            return null;
        }
        return Enumerable.OfType<NodeMessageAdorner>(adorners);
    }

    private void InvalidateArrangeForAllNodeMessageAdornersExcludingSelf()
    {
        IEnumerable<NodeMessageAdorner> allNodeMessageAdorners = GetAllNodeMessageAdorners();
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
        IEnumerable<NodeMessageAdorner> allNodeMessageAdorners = GetAllNodeMessageAdorners();
        List<NodeMessageAdorner> list = new List<NodeMessageAdorner>();
        List<NodeMessageAdorner> list2 = new List<NodeMessageAdorner>();
        List<NodeMessageAdorner> list3 = new List<NodeMessageAdorner>();
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

    private static double CalculateOffset(IList<NodeMessageAdorner> adorners, int idx)
    {
        double num = 0.0;
        for (int i = 0; i < idx; i++)
        {
            num += adorners[i].ActualHeight;
        }
        return num;
    }

    [MirrorVerified("PtcControlsCoreNodeMessageAdornerMechanicalDifferentialTests")]
    protected override void AddToAdornerLayer() => FireAdornerAdded();

    public static FrameworkElement GetAdornedElement(FrameworkElement control)
    {
        if (control is IIndirectMessageAdornerOwner indirectMessageAdornerOwner)
        {
            return indirectMessageAdornerOwner.GetAdornedElement();
        }
        return control;
    }
}
