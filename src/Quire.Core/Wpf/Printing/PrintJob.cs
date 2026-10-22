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
    private void FireJobStarted()
    {
        if (JobStarted != null)
        {
            JobStarted(this, new WritingProgressChangedEventArgs((WritingProgressChangeLevel)2, 0, 0, null));
        }
    }

    [MirrorVerified("Release IL")]
    private void FireJobCompleted(WritingCompletedEventArgs args)
    {
        if (JobCompleted != null)
        {
            JobCompleted(this, args);
        }
    }

    [MirrorVerified("Release IL")]
    private void AttachWriteEventHandlers(XpsDocumentWriter docWriter)
    {
        docWriter.WritingCompleted += HandleWritingCompleted;
        docWriter.WritingCancelled += HandleWritingCancelled;
    }

    [MirrorVerified("Release IL")]
    private void DetachWriteEventHandlers(XpsDocumentWriter docWriter)
    {
        docWriter.WritingCompleted -= HandleWritingCompleted;
        docWriter.WritingCancelled -= HandleWritingCancelled;
    }

    [MirrorVerified("Release IL")]
    private XpsDocumentWriter GetXpsDocumentWriter(string filePath)
    {
        if (System.IO.File.Exists(filePath))
        {
            try
            {
                using (System.IO.FileStream fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                {
                }
                System.IO.File.Delete(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
            }
            catch (System.IO.IOException ex)
            {
                throw new Quire.IO.FileIsUsedByAnotherProcessException(ex);
            }
        }
        _activeXpsDocument = new System.Windows.Xps.Packaging.XpsDocument(filePath, System.IO.FileAccess.ReadWrite);
        return System.Windows.Xps.Packaging.XpsDocument.CreateXpsDocumentWriter(_activeXpsDocument);
    }

    [MirrorVerified("Release IL")]
    private bool JobInitialize(XpsDocumentWriter docWriter)
    {
        if (DebugTools.AssertNotNull(docWriter, null, Array.Empty<object>()).Failed)
        {
            return false;
        }
        DebugTools.DevAssert(_docWriter == null, null, Array.Empty<object>());
        _docWriter = docWriter;
        IsRunning = true;
        FireJobStarted();
        AttachWriteEventHandlers(docWriter);
        return true;
    }

    [MirrorVerified("Release IL")]
    private void JobFinalize(XpsDocumentWriter docWriter, WritingCompletedEventArgs args)
    {
        DebugTools.DevAssert(docWriter == _docWriter, "This PrintJob was finished by a different document writer than was used to start it!", Array.Empty<object>());
        DetachWriteEventHandlers(docWriter);
        if (_activeXpsDocument != null)
        {
            _activeXpsDocument.Close();
            _activeXpsDocument = null;
        }
        _docWriter = null;
        IsRunning = false;
        FireJobCompleted(args);
    }

    [MirrorVerified("Release IL")]
    private void HandleWritingCancelled(object sender, WritingCancelledEventArgs cancelledArgs) => HandleWritingCompleted(sender, new WritingCompletedEventArgs(true, null, cancelledArgs.Error));

    [MirrorVerified("Release IL")]
    private void HandleWritingCompleted(object sender, WritingCompletedEventArgs args) => JobFinalize((XpsDocumentWriter)sender, args);
}
