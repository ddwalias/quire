namespace Quire.EquationEditor.Tree
{
    public partial interface IMathNodeList
    {
        MathNode this[int index] { get; set; }

        int Count { get; }

        int IndexOf(MathNode value);

        MathNode[] ToArray();

        void InsertRange(int index, MathNode[] nodes);

        void RemoveRange(int index, int count);
    }
}
