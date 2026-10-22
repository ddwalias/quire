#nullable disable
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Xps;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf.Printing;

public partial class PrintJob
{
    [MirrorVerified("PtcWpfPrintingPrintJobMechanicalDifferentialTests")]
    public PrintJob() { }

    [MirrorVerified("PtcWpfPrintingPrintJobMechanicalDifferentialTests")]
    public IDocumentPaginatorSource PrintableDocument { get; set; }

    [MirrorVerified("PtcWpfPrintingPrintJobMechanicalDifferentialTests")]
    public bool IsRunning { get; private set; }

    [MirrorVerified("PtcWpfPrintingPrintJobMechanicalDifferentialTests")]
    public event EventHandler<WritingProgressChangedEventArgs> JobStarted;

    [MirrorVerified("PtcWpfPrintingPrintJobMechanicalDifferentialTests")]
    public event EventHandler<WritingCompletedEventArgs> JobCompleted;

    [MirrorVerified("Release IL")]
    public void CancelAsyncJob()
    {
        if (DebugTools.AssertNotNull(_docWriter, null, Array.Empty<object>()).Passed)
        {
            _docWriter.CancelAsync();
        }
    }

    [MirrorVerified("Release IL")]
    private void HandleWritingCancelled(object sender, WritingCancelledEventArgs cancelledArgs) => HandleWritingCompleted(sender, new WritingCompletedEventArgs(true, null, cancelledArgs.Error));

    [MirrorVerified("Release IL")]
    private void HandleWritingCompleted(object sender, WritingCompletedEventArgs args) => JobFinalize((XpsDocumentWriter)sender, args);
}
