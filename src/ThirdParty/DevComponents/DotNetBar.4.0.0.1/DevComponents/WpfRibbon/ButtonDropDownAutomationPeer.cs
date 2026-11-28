#nullable disable
using System.Windows;
using System.Windows.Automation.Peers;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

public partial class ButtonDropDownAutomationPeer
{
    [MirrorVerified("Release IL")]
    public ButtonDropDownAutomationPeer(ButtonDropDown owner) : base(owner) { }

    [MirrorVerified("Release IL")]
    protected override AutomationControlType GetAutomationControlTypeCore() => (AutomationControlType)0;
}
