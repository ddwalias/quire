#nullable disable

using System;
using System.Globalization;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

public partial class ReadonlyOptionConverter
{
    public const string SaveAndSync = "SaveAndSync";

    private static ReadonlyOptionConverter _instance;

    public static ReadonlyOptionConverter Instance => _instance ?? (_instance = new ReadonlyOptionConverter());

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        DocumentReadonlyOptions documentReadonlyOptions = value as DocumentReadonlyOptions;
        if (value != null && DebugTools.AssertNotNull(documentReadonlyOptions, null).Passed)
        {
            if (parameter as string == "SaveAndSync")
            {
                return documentReadonlyOptions.GenerateInputsOutputsStatesOnSave;
            }

            DebugTools.FailAssertion("ReadonlyOptionConverter unsupported parameter {0}", parameter);
            return DependencyProperty.UnsetValue;
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public ReadonlyOptionConverter()
    {
    }
}
