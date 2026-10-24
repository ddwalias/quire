#nullable disable
#pragma warning disable

namespace Quire.Wpf.Printing;

static partial class XpsPrintInterop
{

    [System.Runtime.InteropServices.DllImport("XpsPrint.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern int StartXpsPrintJob(string printerName, string jobName, string outputFileName, nint progressEvent, Microsoft.Win32.SafeHandles.SafeWaitHandle completionEvent, [System.Runtime.InteropServices.In][System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] byte[] printablePagesOn, uint printablePagesOnCount, out IXpsPrintJob xpsPrintJob, out IXpsPrintJobStream documentStream, nint printTicketStream);

    public static void XpsFileToPrinterFile(string printerName, string inputXpsFile, string outputPrinterFile, out int jobId)
    {
        using (System.Threading.ManualResetEvent manualResetEvent = new System.Threading.ManualResetEvent(initialState: false))
        {
            System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(StartXpsPrintJob(printerName, null, outputPrinterFile, IntPtr.Zero, manualResetEvent.SafeWaitHandle, null, 0u, out IXpsPrintJob xpsPrintJob, out IXpsPrintJobStream documentStream, IntPtr.Zero));
            try
            {
                using (System.IO.FileStream fileStream = new System.IO.FileStream(inputXpsFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    int cb;
                    byte[] array = new byte[4096];
                    while ((cb = fileStream.Read(array, 0, array.Length)) > 0)
                    {
                        documentStream.Write(array, (uint)cb, out uint _);
                    }
                }
                documentStream.Close();
                manualResetEvent.WaitOne();
                XPS_JOB_STATUS jobStatus = xpsPrintJob.GetJobStatus();
                jobId = (int)jobStatus.jobId;
                switch (jobStatus.completion)
                {
                    case XPS_JOB_COMPLETION.XPS_JOB_FAILED:
                        throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(jobStatus.jobStatus) ?? new System.Printing.PrintJobException();
                    case XPS_JOB_COMPLETION.XPS_JOB_CANCELLED:
                        throw new System.Printing.PrintingCanceledException();
                    case XPS_JOB_COMPLETION.XPS_JOB_IN_PROGRESS:
                        throw new InvalidOperationException();
                    default:
                        DebugTools.FailAssertion($"{jobStatus.completion}");
                        break;
                    case XPS_JOB_COMPLETION.XPS_JOB_COMPLETED:
                        break;
                }
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xpsPrintJob);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(documentStream);
            }
        }
        if (!System.IO.File.Exists(outputPrinterFile))
        {
            throw new System.IO.FileNotFoundException(outputPrinterFile);
        }
    }

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
