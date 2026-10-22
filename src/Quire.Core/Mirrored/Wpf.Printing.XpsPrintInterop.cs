#nullable disable
#pragma warning disable

namespace Quire.Wpf.Printing;

static partial class XpsPrintInterop
{

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
