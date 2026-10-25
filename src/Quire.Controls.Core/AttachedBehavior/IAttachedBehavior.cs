#nullable disable
using System.Windows;

namespace Quire.Controls.Core.AttachedBehavior;

public partial interface IAttachedBehavior
{
    void Attach(FrameworkElement element);

    void Detach(FrameworkElement element);
}
