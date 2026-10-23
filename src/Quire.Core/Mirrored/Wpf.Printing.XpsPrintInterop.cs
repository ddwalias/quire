#nullable disable
#pragma warning disable

namespace Quire.Wpf.Printing;

static partial class XpsPrintInterop
{

    [System.Runtime.InteropServices.DllImport("XpsPrint.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int StartXpsPrintJob(string printerName, string jobName, string outputFileName, nint progressEvent, Microsoft.Win32.SafeHandles.SafeWaitHandle completionEvent, [System.Runtime.InteropServices.In][System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] byte[] printablePagesOn, uint printablePagesOnCount, out IXpsPrintJob xpsPrintJob, out IXpsPrintJobStream documentStream, nint printTicketStream);

    partial interface IXpsPrintJob
    {
        void Cancel();
        global::Quire.Wpf.Printing.XpsPrintInterop.XPS_JOB_STATUS GetJobStatus();
    }

    partial interface IXpsPrintJobStream
    {
        void Read(byte[] pv, uint cb, out uint pcbRead);
        void Write(byte[] pv, uint cb, out uint pcbWritten);
        void Close();
    }
}
