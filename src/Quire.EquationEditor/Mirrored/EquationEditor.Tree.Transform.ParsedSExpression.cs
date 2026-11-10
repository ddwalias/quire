using System.Collections.Generic;

namespace Quire.EquationEditor.Tree.Transform;

public partial class ParsedSExpression
{
    private string a;

    private string b;

    private List<ParsedSExpression> c;

    private bool d;

    public string Label
    {
        get => a;
        set => a = value;
    }

    public string Value
    {
        get => b;
        set => b = value;
    }

    public List<ParsedSExpression> SExpList
    {
        get => c;
        set => c = value;
    }

    public bool IsString
    {
        get => d;
        set => d = value;
    }

    public bool HasList => SExpList != null && SExpList.Count != 0;

    public bool HasValue
    {
        get
        {
            if (IsString)
            {
                return Value != null;
            }

            return !string.IsNullOrEmpty(Value);
        }
    }

    public bool IsValid => HasList ^ HasValue;

    public ParsedSExpression()
    {
    }
}
