namespace Quire.Controls.Core;

/// <summary>
/// Number display settings (worksheet-wide). Mathcad's own ResultFormat, a DependencyObject, is mirrored separately.
/// </summary>
public sealed record ResultFormatOptions(int Decimals = 3, int ExponentialThreshold = 3, bool ShowTrailingZeros = false)
{
    public static readonly ResultFormatOptions Default = new();
}
