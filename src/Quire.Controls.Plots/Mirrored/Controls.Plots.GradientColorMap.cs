#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots;

public partial class GradientColorMap
{
    private static readonly Type TypeOfThis = typeof(GradientColorMap);

    public static readonly DependencyProperty SpecialProperty = DependencyProperty.Register("Special", typeof(Color), TypeOfThis, new PropertyMetadata((object)Colors.White));

    public static DependencyProperty GradientStopsProperty = DependencyProperty.Register("GradientStops", typeof(GradientStopCollection), TypeOfThis, new PropertyMetadata((PropertyChangedCallback)null));

    public Color Special
    {
        get
        {
            return (Color)GetValue(SpecialProperty);
        }
        set
        {
            SetValue(SpecialProperty, (object)value);
        }
    }

    public GradientStopCollection GradientStops
    {
        get
        {
            return (GradientStopCollection)GetValue(GradientStopsProperty);
        }
        set
        {
            SetValue(GradientStopsProperty, (object)value);
        }
    }

    public Brush GetBrush(double value)
    {
        IEnumerator<GradientStop> enumerator = GradientStops.GetEnumerator();
        GradientStop gradientStop = null;
        GradientStop gradientStop2 = null;
        while (enumerator.MoveNext())
        {
            gradientStop = gradientStop2;
            gradientStop2 = enumerator.Current;
            if (gradientStop2.Offset >= value)
            {
                break;
            }
        }
        Color color;
        if (gradientStop == null)
        {
            if (gradientStop2 != null && gradientStop2.Offset >= value)
            {
                color = gradientStop2.Color;
            }
            else
            {
                color = Special;
            }
        }
        else
        {
            Interval interval = new Interval
            {
                Lower = gradientStop.Offset,
                Upper = gradientStop2.Offset,
            };
            if (interval.Norm < double.Epsilon)
            {
                color = gradientStop.Color;
            }
            else if (interval.Contains(value))
            {
                double num = interval.Normalize(value);
                double num2 = 1.0 - num;
                color = gradientStop.Color * (float)num2 + gradientStop2.Color * (float)num;
            }
            else
            {
                color = Special;
            }
        }
        return new SolidColorBrush(color);
    }
}
