namespace Quire.Automation.Wrappers;

public partial class MatrixValue
{
    internal global::Quire.Automation.MatrixValue _innerObject;

    public string Type => _innerObject.Type;

    public string AsString => _innerObject.AsString;

    public int Rows => _innerObject.Rows;

    public int Cols => _innerObject.Cols;

    public MatrixValue()
    {
        _innerObject = new global::Quire.Automation.MatrixValue();
    }

    internal MatrixValue(global::Quire.Automation.MatrixValue inner)
    {
        _innerObject = inner;
    }

    internal MatrixValue(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        global::Quire.Mcd.MatVal mat = info.GetValue("_core", typeof(global::Quire.Mcd.MatVal)) as global::Quire.Mcd.MatVal;
        _innerObject = new global::Quire.Automation.MatrixValue(mat);
    }

    public void GetObjectData(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        info.AddValue("_core", _innerObject.Core);
    }

    public object GetElement(int row, int col)
    {
        return WrapperFactory.Instance.AutomationToWrapper(_innerObject.GetElement(row, col));
    }

    public void SetElement(int row, int col, object obj)
    {
        object automationValue = WrapperFactory.Instance.WrapperToAutomation(obj);
        _innerObject.SetElement(row, col, (automationValue != null) ? automationValue : obj);
    }
}
