#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface Windows
{
    int Count { get; }

    [System.Runtime.CompilerServices.IndexerName("_Default")]
    Window this[object Index] { get; }
}
