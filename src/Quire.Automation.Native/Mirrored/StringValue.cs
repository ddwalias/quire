namespace Quire.Automation;

public partial class StringValue
{
    public StringValue()
        : base("")
    {
    }

    public StringValue(string s)
        : base(s, global::Quire.Mcd.RuntimeOptions.SaveThreadOptions())
    {
    }

    public StringValue(string s, global::Quire.Mcd.RuntimeOptions o)
        : base(s, o)
    {
    }

    public string String => core as string;

    internal override void WriteValueXml(global::System.Xml.XmlWriter xml, global::Quire.Automation.UnitAction action, double scaleFactor)
    {
        WriteMathElementStart(xml, "str");
        WriteXmlAttribute(xml, "space", "preserve");
        xml.WriteString(String);
        xml.WriteEndElement();
    }

    internal override void WriteValueRL(global::System.IO.TextWriter rl, global::Quire.Automation.UnitAction action)
    {
        rl.Write("(string \"{0}\")", Escaped(Value));
    }

    private static string Escaped(string str)
    {
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(str.Length);
        foreach (char c in str)
        {
            if (c == '"' || c == '\\')
            {
                stringBuilder.Append('\\');
            }
            stringBuilder.Append(c);
        }
        return stringBuilder.ToString();
    }

    public string Value
    {
        get => String;
        set => core = value;
    }

    public override string Type => "String";

    public override string AsString => core.ToString();
}
