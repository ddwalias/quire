#nullable disable
using System.Windows.Input;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public static partial class RibbonCommandManager
{
    [MirrorVerified("Release IL")]
    private static Dictionary<ICommand, IButtonDropDownCommandExtender> m_Commands;

    [MirrorVerified("Release IL")]
    public static void Connect(ICommand command, IButtonDropDownCommandExtender extender) => m_Commands[command] = extender;

    [MirrorVerified("Release IL")]
    public static void Disconnect(ICommand command, IButtonDropDownCommandExtender extender) => m_Commands.Remove(command);
}
