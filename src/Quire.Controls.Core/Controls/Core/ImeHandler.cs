#nullable disable
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class ImeHandler
{
    public static void SetIsInputMethodSuspended(DependencyObject target)
    {
        if (WpfUtils.ShouldSuspendInputMethod())
        {
            InputMethod.SetIsInputMethodSuspended(target, true);
        }
    }
}
