namespace Quire.Automation.Wrappers;

public partial class StringValue
{
    internal global::Quire.Automation.StringValue _innerObject;

    public string Type => _innerObject.Type;

    public string AsString => _innerObject.AsString;

    public string Value
    {
        get => _innerObject.Value;
        set => _innerObject.Value = value;
    }

    public StringValue()
    {
        _innerObject = new global::Quire.Automation.StringValue();
    }

    internal StringValue(global::Quire.Automation.StringValue inner)
    {
        _innerObject = inner;
    }

    internal StringValue(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        string s = info.GetValue("_core", typeof(string)) as string;
        _innerObject = new global::Quire.Automation.StringValue(s);
    }

    public void GetObjectData(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue("_core", _innerObject.Core);
    }
}
