#nullable disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Quire;

public partial class Zoom
{
    private const double DefaultZoomValue = 1.3d;

    internal TextBlock displayedValue;

    internal RepeatButton decreaseZoomButton;

    internal Slider slider;

    internal RepeatButton increaseZoomButton;

    public double ZoomValue
    {
        get
        {
            return (double)GetValue(ZoomValueProperty);
        }
        set
        {
            SetValue(ZoomValueProperty, Math.Max(MinZoomValue, Math.Min(MaxZoomValue, value)));
        }
    }

    private double MinZoomValue
    {
        get
        {
            if (slider == null)
            {
                return 0.125;
            }

            return SliderValueToZoom(slider.Minimum, slider.Minimum);
        }
    }

    private double MaxZoomValue
    {
        get
        {
            if (slider == null)
            {
                return 8.0;
            }

            return SliderValueToZoom(slider.Maximum, slider.Minimum);
        }
    }

    private void BindCommands()
    {
        CommandBindings.Add(new CommandBinding(ApplyZoomCommand, ChangeZoom));
    }

    public void ChangeZoomBySteps(int steps)
    {
        ZoomValue += (double)steps * slider.SmallChange;
    }

    private void ChangeZoom(object sender, ExecutedRoutedEventArgs e)
    {
        int num = (int)Math.Round(ZoomValue * 100.0);
        int stepPercent = (int)Math.Round(slider.SmallChange * 100.0);
        if (e.Parameter as ZoomType? == ZoomType.ZoomIn)
        {
            ZoomValue = num % stepPercent == 0 ? ZoomValue + slider.SmallChange : ZoomValue + slider.SmallChange - ZoomValue % slider.SmallChange;
        }
        else if (e.Parameter as ZoomType? == ZoomType.ZoomOut)
        {
            ZoomValue = num % stepPercent == 0 ? ZoomValue - slider.SmallChange : ZoomValue - ZoomValue % slider.SmallChange;
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        double num = SliderValueToZoom(e.NewValue, ((Slider)sender).Minimum);
        if (ZoomValue != num)
        {
            ZoomValue = num;
        }
    }

    private void slider_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        ZoomValue = DefaultZoomValue;
    }

    private static double SliderValueToZoom(double sliderValue, double minSliderValue)
    {
        return Math.Pow(-minSliderValue, -sliderValue / minSliderValue);
    }

    private static double ZoomToSliderValue(double zoom, double minSliderValue)
    {
        return -Math.Log(zoom, -minSliderValue) * minSliderValue;
    }

    private void DecreaseZoomLarge(object sender, RoutedEventArgs e)
    {
        ZoomValue -= slider.LargeChange;
    }

    private void IncreaseZoomLarge(object sender, RoutedEventArgs e)
    {
        ZoomValue += slider.LargeChange;
    }
}
