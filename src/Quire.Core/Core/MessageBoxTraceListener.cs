#nullable disable
using Quire.Mirror;

namespace Quire.Core;

public partial class MessageBoxTraceListener
{
    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Fail(string message) => Fail(message, null);

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void Write(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public override void WriteLine(string message) { }

    [MirrorVerified("PtcCoreMessageBoxTraceListenerMechanicalDifferentialTests")]
    public MessageBoxTraceListener() { }
}
