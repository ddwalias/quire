#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Wpf;

public partial class BuildArchitectureToBitnessConverter : global::System.Windows.Data.IValueConverter
{
    [MirrorVerified("Release IL")]
    public static readonly BuildArchitectureToBitnessConverter Instance = new BuildArchitectureToBitnessConverter();

    [MirrorVerified("PtcWpfBuildArchitectureToBitnessConverterMechanicalDifferentialTests")]
    private BuildArchitectureToBitnessConverter() { }

    [MirrorVerified("Release IL")]
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        switch ((propertiesTypeSerializationVersionArchitecture)value)
        {
            case propertiesTypeSerializationVersionArchitecture.x86:
                return "32-bit";
            case propertiesTypeSerializationVersionArchitecture.x64:
                return "64-bit";
            default:
                DebugTools.FailAssertion(value.ToString());
                return DependencyProperty.UnsetValue;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string text = value.ToString();
        switch (text)
        {
            case "32-bit":
                return propertiesTypeSerializationVersionArchitecture.x86;
            case "64-bit":
                return propertiesTypeSerializationVersionArchitecture.x64;
            default:
                DebugTools.FailAssertion(value.ToString());
                return DependencyProperty.UnsetValue;
        }
    }

}
