#nullable disable
using System;

namespace Quire.Controls.Core;

public static partial class FindReplaceStatusExtensions
{
    public static bool IsEmpty(this FindReplaceStatus? findReplaceStatus)
    {
        if (findReplaceStatus is { } status)
        {
            return status.IsEmpty();
        }
        return false;
    }

    public static bool IsEmpty(this FindReplaceStatus findReplaceStatus)
    {
        return findReplaceStatus == FindReplaceStatus.Empty;
    }

    public static bool IsAtDocumentBoundary(this FindReplaceStatus findReplaceStatus)
    {
        return findReplaceStatus.HasFlag(FindReplaceStatus.EndOfDocument) || findReplaceStatus.HasFlag(FindReplaceStatus.BeginningOfDocument);
    }
}
