namespace Quire.Automation;

public partial class FunctionValue
{
    public FunctionValue()
        : base()
    {
    }

    public FunctionValue(global::Quire.Mpl.Run.Function f, global::Quire.Mcd.RuntimeOptions o)
        : base(f, o)
    {
    }

    public global::Quire.Mpl.Run.Function Function => core as global::Quire.Mpl.Run.Function;

    internal override void WriteValueXml(global::System.Xml.XmlWriter xml, global::Quire.Automation.UnitAction action, double scaleFactor)
    {
        if (MathNamespaceUriVersion == s_MathNamespaceUriVersionForClassic)
        {
            WriteMathElementStart(xml, "str");
            WriteXmlAttribute(xml, "space", "preserve");
            xml.WriteString(ToString());
            xml.WriteEndElement();
        }
        else
        {
            WriteMathElementStart(xml, "functionValue");
            xml.WriteAttributeString("description", ToString());
            xml.WriteEndElement();
        }
    }

    internal override void WriteValueRL(global::System.IO.TextWriter rl, global::Quire.Automation.UnitAction action)
    {
    }

    public override string Type => "Function";

    public override string AsString => string.Format("Function {0}", ToString());

    public int Arity => Function.Purity;
}
