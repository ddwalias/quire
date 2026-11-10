namespace Quire.Controls;

public partial class InsertIdCommandParameterComparer
{
    public bool Equals(object x, object y)
    {
        return Equals(x as InsertIdCommandParameter, y as InsertIdCommandParameter);
    }

    public int GetHashCode(object obj)
    {
        return GetHashCode(obj as InsertIdCommandParameter);
    }

    public bool Equals(InsertIdCommandParameter x, InsertIdCommandParameter y)
    {
        if (object.Equals(x.Name, y.Name))
        {
            var xLabel = x.Label;
            var yLabel = y.Label;
            return xLabel.GetValueOrDefault() == yLabel.GetValueOrDefault() & xLabel.HasValue == yLabel.HasValue;
        }

        return false;
    }

    public int GetHashCode(InsertIdCommandParameter obj)
    {
        return (obj.Name.ToString() + obj.Label.ToString()).GetHashCode();
    }
}
