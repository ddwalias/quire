#nullable disable
#pragma warning disable

using System.Windows.Input;
using Quire.Controls.Whiteboard;

namespace Quire.Controls.Mathboard;

public partial class MathboardViewModel
{
    CommandBindingCollection IWhiteboardViewModel.CommandBindings
    {
        get
        {
            return base.CommandBindings;
        }
    }
}
