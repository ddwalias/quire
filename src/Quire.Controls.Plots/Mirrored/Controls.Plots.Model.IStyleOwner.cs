using System;
using System.Windows.Media;

namespace Quire.Controls.Plots.Model;

public partial interface IStyleOwner
{
    StyleType StyleType { get; set; }

    uint Alpha { get; set; }

    Color Color { get; set; }

    bool UseTraceInvalidStyle { get; set; }

    event Action<string> StyleChanged;
}
