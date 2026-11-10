#nullable disable

using System.Windows;

namespace Quire;

public static partial class IOWindowConstants
{
    public const string Alias = "Alias";

    public const string IdName = "IdName";

    public const string Region = "Region";

    public static readonly PropertyPath AliasPath = new PropertyPath("Alias");

    public static readonly PropertyPath IdNamePath = new PropertyPath("IdName");
}
