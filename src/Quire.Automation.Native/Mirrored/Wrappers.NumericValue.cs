namespace Quire.Automation.Wrappers;

public partial class NumericValue
{
    internal global::Quire.Automation.NumericValue _innerObject;

    public string Type => _innerObject.Type;

    public string AsString => _innerObject.AsString;

    public double Real
    {
        get => _innerObject.Real;
        set => _innerObject.Real = value;
    }

    public double Imag
    {
        get => _innerObject.Imag;
        set => _innerObject.Imag = value;
    }

    public int Integer
    {
        get => _innerObject.Integer;
        set => _innerObject.Integer = value;
    }

    public NumericValue()
    {
        _innerObject = new global::Quire.Automation.NumericValue();
    }

    internal NumericValue(global::Quire.Automation.NumericValue inner)
    {
        _innerObject = inner;
    }

    internal NumericValue(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        global::Quire.Mcd.NumVal num = info.GetValue("_core", typeof(global::Quire.Mcd.NumVal)) as global::Quire.Mcd.NumVal;
        _innerObject = new global::Quire.Automation.NumericValue(num);
    }

    public void GetObjectData(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue("_core", _innerObject.Core);
    }
}
