using System;

namespace Quire.Controls.InputControls;

public abstract partial class InputControlClearScriptEngine
{
    internal partial class ClearScriptEngineWindow : global::Microsoft.ClearScript.Windows.IHostWindow
    {
        public nint OwnerHandle => IntPtr.Zero;

        public void EnableModeless(bool enable)
        {
        }

        public ClearScriptEngineWindow()
        {
        }
    }
}
