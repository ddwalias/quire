#nullable disable
#pragma warning disable

namespace Quire.Controls.Mathboard;

public partial interface ISectionIndicatorInfo
{
    string Text { get; }

    double Top { get; }

    double Height { get; }
}
