namespace Quire.EquationEditor.Tree
{
    public partial interface IReadOnly
    {
        bool IsEditable { get; }
    }

    public partial interface IReadOnlyAbstract
    {
        void MakeReadOnly();

        void MakeEditable();
    }
}
