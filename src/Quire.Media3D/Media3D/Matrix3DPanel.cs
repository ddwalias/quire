using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class Matrix3DPanel : FrameworkElement
{
    private UniformGrid unigrid;
    private static readonly DependencyPropertyKey MatrixKey;
    public static readonly DependencyProperty MatrixProperty;
    public static readonly DependencyProperty MinimumProperty;
    public static readonly DependencyProperty MaximumProperty;

    public Matrix3D Matrix
    {
        get => (Matrix3D)GetValue(MatrixProperty);
        private set => SetValue(MatrixKey, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    protected override int VisualChildrenCount => 1;

    static Matrix3DPanel()
    {
        MatrixKey = DependencyProperty.RegisterReadOnly("Matrix", typeof(Matrix3D), typeof(Matrix3DPanel), new PropertyMetadata(default(Matrix3D)));
        MatrixProperty = MatrixKey.DependencyProperty;
        MinimumProperty = RangeBase.MinimumProperty.AddOwner(typeof(Matrix3DPanel), new PropertyMetadata(-3.0));
        MaximumProperty = RangeBase.MaximumProperty.AddOwner(typeof(Matrix3DPanel), new PropertyMetadata(3.0));
    }

    public Matrix3DPanel()
    {
        string[] array = new string[16]
        {
            "M11", "M12", "M13", "M14", "M21", "M22", "M23", "M24", "M31", "M32",
            "M33", "M34", "OffsetX", "OffsetY", "OffsetZ", "M44"
        };
        NameScope.SetNameScope(this, new NameScope());
        unigrid = new UniformGrid();
        unigrid.Rows = 4;
        unigrid.Columns = 4;
        AddVisualChild(unigrid);
        AddLogicalChild(unigrid);
        for (int i = 0; i < array.Length; i++)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Margin = new Thickness(12.0);
            unigrid.Children.Add(stackPanel);
            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;
            stackPanel2.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(stackPanel2);
            ScrollBar scrollBar = new ScrollBar();
            scrollBar.Orientation = Orientation.Horizontal;
            scrollBar.Value = (i % 5 == 0) ? 1 : 0;
            scrollBar.SmallChange = 0.01;
            scrollBar.LargeChange = 0.1;
            scrollBar.Focusable = true;
            stackPanel.Children.Add(scrollBar);
            RegisterName(array[i], scrollBar);
            Binding binding = new Binding("Minimum");
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            scrollBar.SetBinding(RangeBase.MinimumProperty, binding);
            binding = new Binding("Maximum");
            binding.Source = this;
            binding.Mode = BindingMode.OneWay;
            scrollBar.SetBinding(RangeBase.MaximumProperty, binding);
            TextBlock element = new TextBlock();
            element.Text = array[i] + " = ";
            stackPanel2.Children.Add(element);
            binding = new Binding("Value");
            binding.Source = scrollBar;
            binding.Mode = BindingMode.OneWay;
            element = new TextBlock();
            element.SetBinding(TextBlock.TextProperty, binding);
            stackPanel2.Children.Add(element);
        }
        AddHandler(RangeBase.ValueChangedEvent, new RoutedEventHandler(ScrollBarOnValueChanged));
    }

    private void ScrollBarOnValueChanged(object sender, RoutedEventArgs args)
    {
        Matrix3D matrix = default(Matrix3D);
        matrix.M11 = (FindName("M11") as ScrollBar).Value;
        matrix.M12 = (FindName("M12") as ScrollBar).Value;
        matrix.M13 = (FindName("M13") as ScrollBar).Value;
        matrix.M14 = (FindName("M14") as ScrollBar).Value;
        matrix.M21 = (FindName("M21") as ScrollBar).Value;
        matrix.M22 = (FindName("M22") as ScrollBar).Value;
        matrix.M23 = (FindName("M23") as ScrollBar).Value;
        matrix.M24 = (FindName("M24") as ScrollBar).Value;
        matrix.M31 = (FindName("M31") as ScrollBar).Value;
        matrix.M32 = (FindName("M32") as ScrollBar).Value;
        matrix.M33 = (FindName("M33") as ScrollBar).Value;
        matrix.M34 = (FindName("M34") as ScrollBar).Value;
        matrix.OffsetX = (FindName("OffsetX") as ScrollBar).Value;
        matrix.OffsetY = (FindName("OffsetY") as ScrollBar).Value;
        matrix.OffsetZ = (FindName("OffsetZ") as ScrollBar).Value;
        matrix.M44 = (FindName("M44") as ScrollBar).Value;
        Matrix = matrix;
    }

    protected override Visual GetVisualChild(int index)
    {
        if (index > 0)
        {
            throw new ArgumentException("index");
        }
        return unigrid;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        unigrid.Measure(availableSize);
        return unigrid.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        unigrid.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
        return finalSize;
    }
}
