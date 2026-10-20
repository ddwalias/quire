#nullable disable
using System;
using System.Globalization;
using System.Windows;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Wpf;

public partial class BuildArchitectureToBitnessConverter
{
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

}
