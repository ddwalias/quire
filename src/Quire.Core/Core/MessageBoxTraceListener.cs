#nullable disable
using System;
using System.Collections.Concurrent;
using System.Windows;
using Quire.Mirror;

namespace Quire.Core;

public partial class MessageBoxTraceListener
{
    [MirrorVerified("Release IL")]
    private static ConcurrentQueue<string> _messages;

    [MirrorVerified("Release IL")]
    private static bool _dialogShown;

    [MirrorVerified("Release IL")]
    private static readonly object _padlock;

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Fail(string message) => ((global::System.Diagnostics.TraceListener)this).Fail(message, null);

    [MirrorVerified("Release IL")]
    public override void Fail(string message, string detailMessage)
    {
        string text = message + (string.IsNullOrEmpty(detailMessage) ? "" : (Environment.NewLine + detailMessage));
        Application current = Application.Current;
        if (current?.Dispatcher != null)
        {
            _messages.Enqueue(text);
            if (!_dialogShown)
            {
                lock (_padlock)
                {
                    if (!_dialogShown)
                    {
                        try
                        {
                            _dialogShown = true;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                string result = "";
                                while (_messages.TryDequeue(out result))
                                {
                                    // Release IL calls ShowDialog with callvirt on the created box; the named local gives that form.
                                    global::Quire.UI.ScrollableMessageBox dialog = new global::Quire.UI.ScrollableMessageBox(result, "Assertion Failed");
                                    dialog.ShowDialog();
                                }
                            });
                        }
                        catch
                        {
                            Console.Error.WriteLine("Failed to show assertion UI");
                        }
                        finally
                        {
                            _dialogShown = false;
                        }
                    }
                }
            }
        }
        else
        {
            Console.Error.WriteLine("ASSERT: " + text);
        }
    }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Write(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void WriteLine(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public MessageBoxTraceListener() { }

    [MirrorVerified("Release IL")]
    static MessageBoxTraceListener()
    {
        _messages = new ConcurrentQueue<string>();
        _dialogShown = false;
        _padlock = new object();
    }
}
