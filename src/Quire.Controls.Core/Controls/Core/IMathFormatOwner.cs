#nullable disable
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IMathFormatOwner
{
    void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e);
}
