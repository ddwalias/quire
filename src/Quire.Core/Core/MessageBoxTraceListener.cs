#nullable disable
using System.Collections.Concurrent;
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

    [MirrorVerified("Release IL")]
    static MessageBoxTraceListener()
    {
        _messages = new ConcurrentQueue<string>();
        _dialogShown = false;
        _padlock = new object();
    }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Fail(string message) => Fail(message, null);

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Write(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void WriteLine(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public MessageBoxTraceListener() { }
}
