#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface ICustomFontSizeOwner
{
    double FontSizeForReport { get; }
    global::System.Nullable<double> FontSizeToRestore { set; }
}
