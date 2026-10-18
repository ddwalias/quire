namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Xml.XmlWorksheetData</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Xml.XmlWorksheetData.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcXmlXmlWorksheetDataMechanicalDifferentialTests
{
    [SkippableFact]
    public void CreateMathcad14MLElement_060003da() => Mechanical.Compare(typeof(global::Quire.Xml.XmlWorksheetData), 0x060003da);

    [SkippableFact]
    public void CreateMLElement_060003db() => Mechanical.Compare(typeof(global::Quire.Xml.XmlWorksheetData), 0x060003db);

    [SkippableFact]
    public void CreateMcdxMLElement_060003dc() => Mechanical.Compare(typeof(global::Quire.Xml.XmlWorksheetData), 0x060003dc);
}
