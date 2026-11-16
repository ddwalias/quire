#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface Sheets
{
    [System.Runtime.CompilerServices.IndexerName("_Default")]
    object this[object Index] { get; }
    new System.Collections.IEnumerator GetEnumerator();
}
