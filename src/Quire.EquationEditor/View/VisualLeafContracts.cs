namespace Quire.EquationEditor.View
{
    public partial interface IVisualMatrixNode
    {
        int RowCount { get; }

        int ColumnCount { get; set; }

        global::Quire.TableSize OriginalSize { set; }

        global::System.Windows.Controls.UIElementCollection Cells { get; }

        global::System.Windows.Visibility HeadingsVisibility { get; set; }

        string[] MatrixHeadingRowIndices { get; }

        string[] MatrixHeadingColumnIndices { get; }

        global::System.Nullable<global::Quire.TableCoordinate> ErrorCell { get; set; }

        IVisualMathNode ErrorNode { get; }

        void TryDragIntoHiddenPart(IVisualMathNode nodeHit, out int rowShift, out int columnShift);
    }

    public partial interface IVisualIdNode
    {
        global::Quire.EquationEditor.Tree.FormattedString Identifier { get; }

        global::System.Nullable<global::Quire.Controls.Core.IdLabels> IdLabel { get; }

        global::System.Collections.Generic.IList<double> CharacterWidths { get; }

        double CaretNormalHeight { get; }

        double CaretNormalOffset { get; }

        double CaretSubscriptHeight { get; }

        double CaretSubscriptOffset { get; }
    }

    public partial interface IVisualTextNode
    {
        global::System.Collections.Generic.IList<double> CharacterWidths { get; }

        global::System.Collections.Generic.IList<double> ScreenCharacterWidths { get; }

        string Text { get; set; }
    }

    public partial interface IVisualTextNodeEditor
    {
        int CaretPosition { get; set; }

        global::System.Collections.Generic.IList<double> CharacterWidths { get; }

        string Text { get; set; }

        void SetSelection(int start, int end);

        void UnsetSelection();
    }

    public partial interface IVisualIdNodeEditor
    {
        int CaretPosition { get; set; }

        global::Quire.EquationEditor.Tree.Navigation.CaretInsertionTypes CaretInsertionType { get; set; }

        void SetSelection(int start, int end);

        void UnsetSelection();
    }

    public partial interface IVisualComboBoxControlNode
    {
        global::Quire.EquationEditor.Tree.ComboBoxInputControlData ComboBoxData { get; }

        string GetSelectedDisplayedText { get; }

        bool IsEditModeActive { get; }

        bool IsDropDownOpen { get; set; }

        IEditableComboBoxVisualTable EditableComboBoxTable { get; }

        void SetSelectedIndex(int index);

        void OpenEditingOnComboBoxIsVisible();
    }

    public partial interface IVisualInputControlNode
    {
        bool IsEditorFrameVisible { get; }

        global::System.Windows.Controls.Control InputControl { get; }

        global::Quire.Calculation.Core.IDataExchangeModel DataExchangeModel { get; }

        bool NavigateInto(global::Quire.Direction direction);
    }

    public partial interface IVisualScoper
    {
        global::Quire.Direction CaretSide { set; }

        global::Quire.EquationEditor.EquationEditModeEnum EditMode { set; }

        event global::Quire.Delegate0 LostFocus;

        void Highlight(global::System.Collections.Generic.List<global::System.Collections.Generic.List<IVisualMathNode>> scopedNodes, global::System.Collections.Generic.IList<IVisualMathNode> excludedNodes, global::Quire.Direction[] primalChildTypeoverExtendDirections, bool singleBlockHighlight = false);

        void Remove();

        void SetCaret(bool isReadWrite);
    }

    public partial interface IVisualVariantLineBinaryOperatorValues
    {
        double InBottomLineVOffset { get; set; }

        double InTopLineVOffset { get; set; }

        double InHOffset { get; set; }

        double TopLineBaseLine { get; }

        double BottomLineBaseLine { get; }

        double TopLineHeight { get; }

        double BottomLineHeight { get; }

        double TopLineWidth { get; }

        double BottomLineWidth { get; }

        double TotalHeight { get; }

        double TotalLinesHeightAboveBottom { get; }

        bool IsMultiline { get; }

        bool UseZeroCache { get; set; }

        int MeasureCallFromRoot { get; }

        void SetMeasureCallFromRoot(int callNumber);
    }
}
