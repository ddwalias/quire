using System.Windows;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Axes : WireBase
{
    private TextGenerator txtgen = new TextGenerator();

    public static readonly DependencyProperty ExtentProperty =
        DependencyProperty.Register("Extent", typeof(double), typeof(Axes),
            new UIPropertyMetadata(3.0, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty ShowNumbersProperty =
        DependencyProperty.Register("ShowNumbers", typeof(bool), typeof(Axes),
            new UIPropertyMetadata(true, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty LargeTickProperty =
        DependencyProperty.Register("LargeTick", typeof(double), typeof(Axes),
            new UIPropertyMetadata(0.05, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty SmallTickProperty =
        DependencyProperty.Register("SmallTick", typeof(double), typeof(Axes),
            new UIPropertyMetadata(0.025, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty FontProperty =
        DependencyProperty.Register("Font", typeof(Font), typeof(Axes),
            new UIPropertyMetadata(Font.Modern, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register("FontSize", typeof(double), typeof(Axes),
            new UIPropertyMetadata(0.1, WireBase.PropertyChanged, null, isAnimationProhibited: true));

    public static readonly DependencyProperty LabelsProperty =
        DependencyProperty.Register("Labels", typeof(string), typeof(Axes),
            new PropertyMetadata("XYZ", WireBase.PropertyChanged), ValidateLabels);

    public double Extent
    {
        get => (double)GetValue(ExtentProperty);
        set => SetValue(ExtentProperty, value);
    }

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => SetValue(ShowNumbersProperty, value);
    }

    public double LargeTick
    {
        get => (double)GetValue(LargeTickProperty);
        set => SetValue(LargeTickProperty, value);
    }

    public double SmallTick
    {
        get => (double)GetValue(SmallTickProperty);
        set => SetValue(SmallTickProperty, value);
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

    public string Labels
    {
        get => (string)GetValue(LabelsProperty);
        set => SetValue(LabelsProperty, value);
    }

    public Axes()
    {
        PropertyChanged(default);
    }

    private static bool ValidateLabels(object obj)
    {
        if (obj is not string text)
        {
            return true;
        }
        return text.Length % 3 == 0;
    }

    protected override void Generate(DependencyPropertyChangedEventArgs args, Point3DCollection lines)
    {
        lines.Clear();
        lines.Add(new Point3D(-Extent, 0.0, 0.0));
        lines.Add(new Point3D(Extent, 0.0, 0.0));
        lines.Add(new Point3D(0.0, -Extent, 0.0));
        lines.Add(new Point3D(0.0, Extent, 0.0));
        lines.Add(new Point3D(0.0, 0.0, -Extent));
        lines.Add(new Point3D(0.0, 0.0, Extent));
        for (int i = (int)(-10.0 * Extent); i <= (int)(10.0 * Extent); i++)
        {
            double num = (i % 10 == 0) ? LargeTick : SmallTick;
            double num2 = (double)i / 10.0;
            lines.Add(new Point3D(num2, -num, 0.0));
            lines.Add(new Point3D(num2, num, 0.0));
            lines.Add(new Point3D(-num, num2, 0.0));
            lines.Add(new Point3D(num, num2, 0.0));
            lines.Add(new Point3D(0.0, -num, num2));
            lines.Add(new Point3D(0.0, num, num2));
            txtgen.Font = Font;
            txtgen.FontSize = FontSize;
            txtgen.Thickness = Thickness;
            txtgen.Rounding = Rounding;
            txtgen.BaselineDirection = new Vector3D(1.0, 0.0, 0.0);
            txtgen.UpDirection = new Vector3D(0.0, 1.0, 0.0);
            if (i != 0 && i % 10 == 0)
            {
                string text = ((int)num2).ToString();
                bool flag = i == (int)(-10.0 * Extent) || i == (int)(10.0 * Extent);
                string text2 = (i == (int)(-10.0 * Extent)) ? "-" : "+";
                if (flag && Labels != null)
                {
                    text = text2 + Labels.Substring(0, Labels.Length / 3);
                }
                if (flag || ShowNumbers)
                {
                    txtgen.Origin = new Point3D(num2, -num * 1.25, 0.0);
                    txtgen.HorizontalAlignment = HorizontalAlignment.Center;
                    txtgen.VerticalAlignment = VerticalAlignment.Top;
                    txtgen.Generate(lines, text);
                }
                if (flag)
                {
                    text = text2 + Labels.Substring(Labels.Length / 3, Labels.Length / 3);
                }
                if (flag || ShowNumbers)
                {
                    txtgen.Origin = new Point3D(num * 1.25, num2, 0.0);
                    txtgen.HorizontalAlignment = HorizontalAlignment.Left;
                    txtgen.VerticalAlignment = VerticalAlignment.Center;
                    txtgen.Generate(lines, text);
                }
                if (flag)
                {
                    text = text2 + Labels.Substring(2 * Labels.Length / 3);
                }
                if (flag || ShowNumbers)
                {
                    txtgen.Origin = new Point3D(0.0, -num * 1.25, num2);
                    txtgen.BaselineDirection = new Vector3D(0.0, 0.0, 1.0);
                    txtgen.HorizontalAlignment = HorizontalAlignment.Center;
                    txtgen.VerticalAlignment = VerticalAlignment.Top;
                    txtgen.Generate(lines, text);
                }
            }
        }
    }
}
