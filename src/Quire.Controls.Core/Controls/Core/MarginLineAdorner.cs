#nullable disable
using System;
using System.Windows;
using System.Windows.Media;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class MarginLineAdorner : VisualAdornerBase
{
    private static readonly Type _typeOfThis;

    [MirrorVerified("PtcControlsCoreMarginLineAdornerMechanicalDifferentialTests")]
    public bool IsDisplayed { get; private set; }

    static MarginLineAdorner()
    {
        _typeOfThis = typeof(MarginLineAdorner);
        UIElement.VisibilityProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnVisibilityChanged));
    }

    private static void OnVisibilityChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((MarginLineAdorner)obj).IsDisplayed = (Visibility)args.NewValue != Visibility.Collapsed;
    }

    public static VisualAdornerBase Create(FrameworkElement element)
    {
        MarginLineAdorner marginLineAdorner = new MarginLineAdorner(element);
        marginLineAdorner.Add();
        return marginLineAdorner;
    }

    private MarginLineAdorner(FrameworkElement element)
        : base(element, trackVisibility: true)
    {
        ToolTip = element.FindResource("AnnotatedTooltip");
    }

    [MirrorVerified("PtcControlsCoreMarginLineAdornerMechanicalDifferentialTests")]
    protected override void AddToAdornerLayer() => FireAdornerAdded();

    public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
    {
        if (Visibility == Visibility.Collapsed)
        {
            return transform;
        }
        Thickness pageMargin = WorksheetPropertiesManager.GetWorksheetProperties(AdornedElement).PageMargin;
        double left = pageMargin.Left;
        Point val = transform.Transform(new Point(0.0, 0.0));
        return new TranslateTransform(left, val.Y);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        IsDisplayed = WorksheetPropertiesManager.GetWorksheetProperties(AdornedElement).WorksheetPageLayoutMode == PageLayoutMode.PageMode;
        if (!IsDisplayed)
        {
            return;
        }
        try
        {
            drawingContext.DrawGeometry(AnnotatedAdornerDefinitions.AdornerBrush, AnnotatedAdornerDefinitions.AdornerPen, AnnotatedAdornerDefinitions.MarginLine);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex.Message);
        }
    }
}
