#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IMouseFacade
{
    global::System.Windows.Input.Cursor ShowWaitCursor() ;
    void ClearOverrideCursor() ;
    global::System.Windows.Input.Cursor GetOverrideCursor() ;
    global::System.Windows.Input.Cursor SetOverrideCursor(global::System.Windows.Input.Cursor cursor) ;
    bool SetCursor(global::System.Windows.Input.Cursor cursor) ;
}
