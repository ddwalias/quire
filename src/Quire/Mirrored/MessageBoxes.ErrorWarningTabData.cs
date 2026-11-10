#nullable disable

using System.Collections.Generic;
using Quire.Controls.IO;

namespace Quire.MessageBoxes;

public partial class ErrorWarningTabData
{
    public IEnumerable<GeneralIssue> GeneralIssues { get; private set; }

    public IEnumerable<ItemIssue> ItemIssues { get; private set; }

    public bool IsError { get; private set; }

    public ErrorWarningTabData(bool isError, IEnumerable<GeneralIssue> generalIssues, IEnumerable<ItemIssue> itemIssues)
    {
        IsError = isError;
        GeneralIssues = generalIssues;
        ItemIssues = itemIssues;
    }
}
