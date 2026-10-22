#nullable disable
#pragma warning disable

namespace Quire.Wpf.Printing;

partial interface IPrintDialogOptions
{
    global::Quire.Wpf.Printing.Win32PrintRange PrintRange { get; }
    global::Quire.Wpf.Printing.PageIndexRange PageRange { get; }
    bool PrintToFile { get; }
    string FileName { get; }
    global::System.Printing.PrintQueue PrintQueue { get; }
    global::System.Printing.PrintTicket PrintTicket { get; }
}
