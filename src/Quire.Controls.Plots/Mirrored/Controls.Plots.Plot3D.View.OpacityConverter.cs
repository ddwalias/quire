#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Quire.Calculation.Core;
using Quire.Core;

namespace Quire.Controls.Plots.Plot3D.View;

public partial class OpacityConverter
{
    public static readonly OpacityConverter Instance = new OpacityConverter();

    private OpacityConverter()
    {
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3 || !(values[0] is SynchronizationStatus) || !(values[1] is bool) || !(values[2] is bool))
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return DependencyProperty.UnsetValue;
        }
        SynchronizationStatus synchronizationStatus = (SynchronizationStatus)values[0];
        bool flag = (bool)values[1];
        bool flag2 = (bool)values[2];
        return (flag || (flag2 && synchronizationStatus == SynchronizationStatus.Pending)) ? 0.5 : 1.0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
