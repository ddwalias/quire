#nullable disable

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core;

namespace Quire;

public partial class IdLabelToItemIndexInFormatsListConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        int num = -1;
        if (!values.Any((object value) => value == DependencyProperty.UnsetValue))
        {
            IdLabels? currentDisplayLabel = values[0] as IdLabels?;
            ItemCollection labelIDsCollection = values[1] as ItemCollection;
            num = FindItemIndexToSelect(currentDisplayLabel, labelIDsCollection);
        }

        return num;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static int FindItemIndexToSelect(IdLabels? currentDisplayLabel, IList labelIDsCollection)
    {
        if (!currentDisplayLabel.HasValue || labelIDsCollection == null)
        {
            return -1;
        }

        for (int num = 0; num < labelIDsCollection.Count; num++)
        {
            if (((KeyValuePair<IdLabels, ExtendedFont>)labelIDsCollection[num]).Key == currentDisplayLabel.Value)
            {
                return num;
            }
        }

        return -1;
    }

    public IdLabelToItemIndexInFormatsListConverter()
    {
    }
}
