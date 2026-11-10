#nullable disable

using System;
using System.Globalization;
using Quire.EquationEditor.Tree.Navigation;

namespace Quire;

public partial class CaretInsertionTypeToBooleanConverter
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
        {
            return null;
        }

        bool isSubscript = (bool)value;
        string caretType = parameter.ToString();
        if (isSubscript)
        {
            switch (caretType)
            {
                case "Normal":
                    return CaretInsertionTypes.Normal;
                case "Subscript":
                    return CaretInsertionTypes.Subscript;
            }
        }

        return CaretInsertionTypes.Normal;
    }
}
