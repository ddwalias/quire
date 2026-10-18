namespace Quire.Differential.Tests;

/// <summary>
/// Direct comparisons of the <c>Ptc.Xml.XmlNodeWriter</c> members that MemberGen reconstructed mechanically
/// (docs/mirror/evidence/Ptc.Xml.XmlNodeWriter.md): each compares one member on generated inputs (Mechanical).
/// </summary>
public class PtcXmlXmlNodeWriterMechanicalDifferentialTests
{
    [SkippableFact]
    public void WriteStartDocument_060003bc() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003bc);

    [SkippableFact]
    public void WriteStartDocument_060003bd() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003bd);

    [SkippableFact]
    public void WriteEndDocument_060003be() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003be);

    [SkippableFact]
    public void WriteDocType_060003bf() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003bf);

    [SkippableFact]
    public void WriteCData_060003c5() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003c5);

    [SkippableFact]
    public void WriteComment_060003c6() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003c6);

    [SkippableFact]
    public void WriteProcessingInstruction_060003c7() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003c7);

    [SkippableFact]
    public void WriteEntityRef_060003c8() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003c8);

    [SkippableFact]
    public void WriteCharEntity_060003c9() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003c9);

    [SkippableFact]
    public void WriteWhitespace_060003ca() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003ca);

    [SkippableFact]
    public void WriteSurrogateCharEntity_060003cc() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003cc);

    [SkippableFact]
    public void WriteChars_060003cd() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003cd);

    [SkippableFact]
    public void WriteRaw_060003ce() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003ce);

    [SkippableFact]
    public void Flush_060003d2() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003d2);

    [SkippableFact]
    public void LookupPrefix_060003d3() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003d3);

    [SkippableFact]
    public void WriteState_get_060003d4() => Mechanical.Compare(typeof(global::Quire.Xml.XmlNodeWriter), 0x060003d4);
}
