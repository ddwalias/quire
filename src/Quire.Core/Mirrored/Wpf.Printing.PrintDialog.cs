#nullable disable
#pragma warning disable

namespace Quire.Wpf.Printing;

partial class PrintDialog
{
    public global::Quire.Wpf.Printing.Win32PrintRange PrintRange
    {
        get
        {
            global::System.Windows.Controls.PageRangeSelection pageRangeSelection = base.PageRangeSelection;
            switch (pageRangeSelection)
            {
                case (global::System.Windows.Controls.PageRangeSelection)0:
                    return (global::Quire.Wpf.Printing.Win32PrintRange)0;
                case (global::System.Windows.Controls.PageRangeSelection)1:
                    return (global::Quire.Wpf.Printing.Win32PrintRange)2;
                case (global::System.Windows.Controls.PageRangeSelection)2:
                    return (global::Quire.Wpf.Printing.Win32PrintRange)4194304;
                case (global::System.Windows.Controls.PageRangeSelection)3:
                    return (global::Quire.Wpf.Printing.Win32PrintRange)1;
                default:
                    global::Quire.DebugTools.DevAssert(false, $"Unexpected PageRangeSelection value {base.PageRangeSelection}", global::System.Array.Empty<object>());
                    return (global::Quire.Wpf.Printing.Win32PrintRange)0;
            }
        }
        set
        {
            switch (value)
            {
                case (global::Quire.Wpf.Printing.Win32PrintRange)0:
                    base.PageRangeSelection = (global::System.Windows.Controls.PageRangeSelection)0;
                    return;
                case (global::Quire.Wpf.Printing.Win32PrintRange)2:
                    base.PageRangeSelection = (global::System.Windows.Controls.PageRangeSelection)1;
                    return;
                case (global::Quire.Wpf.Printing.Win32PrintRange)4194304:
                    base.PageRangeSelection = (global::System.Windows.Controls.PageRangeSelection)2;
                    return;
                case (global::Quire.Wpf.Printing.Win32PrintRange)1:
                    base.PageRangeSelection = (global::System.Windows.Controls.PageRangeSelection)3;
                    return;
                default:
                    global::Quire.DebugTools.DevAssert(false, $"{value} PrintRange is not supported", global::System.Array.Empty<object>());
                    return;
            }
        }
    }
}
