#nullable disable
#pragma warning disable

namespace Quire.Microsoft.Office.Interop.Excel;

public partial interface _Worksheet
{
    object Parent { get; }
    void Activate();
    string Name { get; set; }
    bool EnableCalculation { get; set; }
    [System.Runtime.CompilerServices.IndexerName("Range")]
    Range this[object Cell1, object Cell2] { get; }
}
