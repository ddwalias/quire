namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.WpfClipboard</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.WpfClipboard.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcWpfClipboardMechanicalDifferentialTests
{
    [SkippableFact]
    public void Clear_06000381() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000381);

    [SkippableFact]
    public void GetText_06000382() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000382);

    [SkippableFact]
    public void CopySExpressionToDataObject_06000386() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000386);

    [SkippableFact]
    public void CopyRtfStringToDataObject_06000388() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000388);

    [SkippableFact]
    public void HasText_0600038a() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x0600038a);

    [SkippableFact]
    public void HasImage_06000391() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000391);

    [SkippableFact]
    public void HasPrimeObject_06000392() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000392);

    [SkippableFact]
    public void HasXMLSpreadsheet_06000393() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000393);

    [SkippableFact]
    public void HasSExpression_06000397() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000397);

    [SkippableFact]
    public void HasTextWithEmbeddedItemsInfo_06000399() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06000399);

    [SkippableFact]
    public void ctor_0600039b() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x0600039b);

    [SkippableFact]
    public void ctor_06001160() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06001160);

    [SkippableFact]
    public void Dispose_06001161() => Mechanical.Compare(typeof(global::Quire.WpfClipboard), 0x06001161);
}
