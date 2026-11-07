namespace Quire.EquationEditor.View
{
    public partial interface IVisualNode
    {
        bool IsLoaded { get; }

        event global::System.Windows.RoutedEventHandler Loaded;
    }

    public partial interface IVisualOutfixNode
    {
        IVisualMathNode LeftFence { get; }

        IVisualMathNode RightFence { get; }
    }

    public partial interface IVisualPairNaryNode
    {
        IVisualMathNode[] NodeChildren { get; }

        IVisualMathNode[] OperatorParts { get; }
    }

    public partial interface IVisualUnaryOperatorNode
    {
        IVisualMathNode Child { get; set; }

        IVisualMathNode Operator { get; }
    }

    public partial interface IVisualParensHighlighter
    {
        double Opacity { get; set; }

        void Remove();
    }

    public partial interface IVisualMathNodeHighlighter
    {
        double Opacity { set; }

        void Remove();
    }
}
