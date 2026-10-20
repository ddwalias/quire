#nullable disable
using System.Windows.Input;
using System.Windows.Input;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Services;

public partial class MouseFacade
{
    [MirrorVerified("PtcServicesMouseFacadeMechanicalDifferentialTests")]
    public void ClearOverrideCursor() => SetOverrideCursor(null);

    [MirrorVerified("PtcServicesMouseFacadeMechanicalDifferentialTests")]
    public Cursor GetOverrideCursor() => Mouse.OverrideCursor;

    [MirrorVerified("PtcServicesMouseFacadeMechanicalDifferentialTests")]
    public bool SetCursor(Cursor cursor) => Mouse.SetCursor(cursor);

    [MirrorVerified("PtcServicesMouseFacadeMechanicalDifferentialTests")]
    public MouseFacade() { }

    [MirrorVerified("PtcServicesMouseFacadeMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    [MirrorVerified("Release IL")]
    public Cursor ShowWaitCursor() => SetOverrideCursor(Cursors.Wait);
}
