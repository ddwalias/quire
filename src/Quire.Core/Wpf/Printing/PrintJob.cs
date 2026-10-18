#nullable disable
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
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
}
