#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.IO;

public partial interface IInputOutputItemsConflicts
{
    IEnumerable<GeneralIssue> GeneralWarnings { get; }
    IEnumerable<GeneralIssue> GeneralErrors { get; }
    IEnumerable<ItemIssue> ItemWarnings { get; }
    IEnumerable<ItemIssue> ItemErrors { get; }
    void AddGeneralWarning(string warning);
    void AddGeneralError(string error);
    void AddItemWarning(string alias, string warning);
    void AddItemError(string alias, string error);
    void AppendLists(IInputOutputItemsConflicts source);
}
