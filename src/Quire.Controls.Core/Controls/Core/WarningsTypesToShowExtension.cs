#nullable disable
namespace Quire.Controls.Core;

public static partial class WarningsTypesToShowExtension
{
    public static bool ShouldDisplayAllWarnings(this WarningsTypesToShow warningsToShow)
    {
        return warningsToShow == WarningsTypesToShow.All;
    }

    public static bool IsAnyWarningActive(this WarningsTypesToShow warningsToShow)
    {
        return warningsToShow != WarningsTypesToShow.None;
    }
}
