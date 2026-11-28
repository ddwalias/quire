#nullable disable
using System.Windows;
using Quire.Mirror;

namespace Quire.DevComponents.WpfRibbon;

internal partial class ElementAutoSizeBag
{
    [MirrorVerified("Release IL")]
    public UIElement Element;

    [MirrorVerified("Release IL")]
    private bool m_SettingsRecorded;

    [MirrorVerified("Release IL")]
    public virtual void RecordSetting(UIElement item)
    {
        Element = item;
        m_SettingsRecorded = true;
    }

    [MirrorVerified("Release IL")]
    public virtual void RestoreSettings() => m_SettingsRecorded = false;

    [MirrorVerified("Release IL")]
    public ElementAutoSizeBag() { }

    [MirrorVerified("Release IL")]
    protected bool SettingsRecorded { get => m_SettingsRecorded; set => m_SettingsRecorded = value; }
}
