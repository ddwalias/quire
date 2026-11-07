namespace Quire.EquationEditor.Tree.Parsing;

public partial interface IChildHandlerPanacea
{
    int ChildIndex { get; set; }

    IChildHandlerPanacea Clone();
}
