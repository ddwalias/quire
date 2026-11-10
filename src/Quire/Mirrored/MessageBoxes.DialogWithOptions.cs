#nullable disable

using System.Windows.Controls;

namespace Quire.MessageBoxes;

public partial class DialogWithOptions
{
    protected override TextBlock Message => null;

    protected override Image Image => null;

    protected override ItemsControl Buttons => _buttons;
}
