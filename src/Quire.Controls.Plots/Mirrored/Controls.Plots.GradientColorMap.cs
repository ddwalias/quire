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
        GradientStop nextGradientStop = null;
        while (enumerator.MoveNext())
        {
            gradientStop = nextGradientStop;
            nextGradientStop = enumerator.Current;
            if (nextGradientStop.Offset >= value)
            {
                break;
            }
        }
        Color color;
        if (gradientStop == null)
        {
            if (nextGradientStop != null && nextGradientStop.Offset >= value)
            {
                color = nextGradientStop.Color;
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
                Upper = nextGradientStop.Offset,
            };
            if (interval.Norm < double.Epsilon)
            {
                color = gradientStop.Color;
            }
            else if (interval.Contains(value))
            {
                double num = interval.Normalize(value);
                double inverseRatio = 1.0 - num;
                color = gradientStop.Color * (float)inverseRatio + nextGradientStop.Color * (float)num;
            }
            else
            {
                color = Special;
            }
        }
        return new SolidColorBrush(color);
    }
}
