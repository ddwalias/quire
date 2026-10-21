using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class WireText : WireBase
{
    private TextGenerator txtgen = new TextGenerator();

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(WireText),
            new UIPropertyMetadata("", WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty FontProperty =
        DependencyProperty.Register("Font", typeof(Font), typeof(WireText),
            new UIPropertyMetadata(Font.Modern, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register("FontSize", typeof(double), typeof(WireText),
            new UIPropertyMetadata(0.1, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty OriginProperty =
        DependencyProperty.Register("Origin", typeof(Point3D), typeof(WireText),
            new UIPropertyMetadata(default(Point3D), WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty BaselineDirectionProperty =
        DependencyProperty.Register("BaselineDirection", typeof(Vector3D), typeof(WireText),
            new UIPropertyMetadata(new Vector3D(1.0, 0.0, 0.0), WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty UpDirectionProperty =
        DependencyProperty.Register("UpDirection", typeof(Vector3D), typeof(WireText),
            new UIPropertyMetadata(new Vector3D(0.0, 1.0, 0.0), WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty HorizontalAlignmentProperty =
        DependencyProperty.Register("HorizontalAlignment", typeof(HorizontalAlignment), typeof(WireText),
            new UIPropertyMetadata(HorizontalAlignment.Left, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty VerticalAlignmentProperty =
        DependencyProperty.Register("VerticalAlignment", typeof(VerticalAlignment), typeof(WireText),
            new UIPropertyMetadata(VerticalAlignment.Top, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public Font Font
    {
        get => (Font)GetValue(FontProperty);
        set => SetValue(FontProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Point3D Origin
    {
        get => (Point3D)GetValue(OriginProperty);
        set => SetValue(OriginProperty, value);
    }

    public Vector3D BaselineDirection
    {
        get => (Vector3D)GetValue(BaselineDirectionProperty);
        set => SetValue(BaselineDirectionProperty, value);
    }

    public Vector3D UpDirection
    {
        get => (Vector3D)GetValue(UpDirectionProperty);
        set => SetValue(UpDirectionProperty, value);
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => (HorizontalAlignment)GetValue(HorizontalAlignmentProperty);
        set => SetValue(HorizontalAlignmentProperty, value);
    }

    public VerticalAlignment VerticalAlignment
    {
        get => (VerticalAlignment)GetValue(VerticalAlignmentProperty);
        set => SetValue(VerticalAlignmentProperty, value);
    }

    public WireText()
    {
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        txtgen.Font = Font;
        txtgen.FontSize = FontSize;
        txtgen.Rounding = Rounding;
        txtgen.Thickness = Thickness;
        txtgen.BaselineDirection = BaselineDirection;
        txtgen.Origin = Origin;
        txtgen.UpDirection = UpDirection;
        txtgen.VerticalAlignment = VerticalAlignment;
        txtgen.HorizontalAlignment = HorizontalAlignment;
        txtgen.Generate(lines, Text);
    }
}
