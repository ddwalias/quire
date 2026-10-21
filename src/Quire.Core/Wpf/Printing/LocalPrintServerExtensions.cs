#nullable disable
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using Quire.Mirror;

namespace Quire.Wpf.Printing;

public static partial class LocalPrintServerExtensions
{
    [MirrorVerified("Release IL")]
    public static IEnumerable<PrintQueue> GetLocalNonRedirectedPrintQueues(this LocalPrintServer server)
    {
        server = server ?? new LocalPrintServer();
        List<string> redirectedNames = (from queue in server.GetPrintQueues(new EnumeratedPrintQueueTypes[1] { EnumeratedPrintQueueTypes.TerminalServer })
            select queue.FullName).ToList();
        return from queue in server.GetPrintQueues(new EnumeratedPrintQueueTypes[1] { EnumeratedPrintQueueTypes.Local })
            where !redirectedNames.Contains(queue.FullName)
            select queue;
    }
}
