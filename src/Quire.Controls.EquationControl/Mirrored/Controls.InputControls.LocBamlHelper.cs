using System;

namespace Quire.Controls.InputControls;

public static partial class LocBamlHelper
{
    public static Array GetValues(Type enumType)
    {
        if (enumType == null || !enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum.");
        }

        return Enum.GetValues(enumType);
    }
}
