namespace Quire.EquationEditor.View
{
    public partial interface IVisualMathNode
    {
        global::System.Windows.Rect ScreenBounds { get; }

        global::System.Windows.Point ScreenCenterPoint { get; }

        global::System.Windows.Rect ScreenInkBounds { get; }

        IVisualMathNode this[int index] { get; set; }

        global::Quire.EquationEditor.Tree.INodeInfo NodeInfo { get; set; }

        bool ShowOperatorOnlyWhenActive { get; set; }

        bool HideSubTreeFromClicks { get; set; }

        bool HideFromClicks { get; set; }

        global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElements { get; }

        global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> CoreElementsTopLine { get; }

        bool IsValid { get; set; }

        IVisualMathNode[] NavigatableParts { get; }

        global::Quire.Controls.Core.IMathFormatDobjController MathFormat { get; }

        bool IsMonoLine { get; set; }

        void BuildChildren(IVisualMathNode[] children);

        void ModifyChildMathFormat(int childIndex, global::Quire.Controls.Core.MathFormatValues childMathFormat);
    }

    public partial interface IVisualMathNodeEditor
    {
        bool Blink { get; set; }

        void GetFocus();

        void Remove();

        void ShowDefaultCaret(global::Quire.Direction caretSide);

        void SetCaretType(bool isReadWrite);
    }

    public partial interface IVisualMathNodeWithMathChildren
    {
        IVisualMathNode[] MathChildren { get; }
    }

    public partial interface IVisualLeafOperatorEditor
    {
        void SelectOperator(bool selected);
    }
}
