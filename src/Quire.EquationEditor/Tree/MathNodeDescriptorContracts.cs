namespace Quire.EquationEditor.Tree
{
    public partial interface IMathNodeDescriptor
    {
        string DescriptorName { get; }

        Parsing.Fixity Fixity { get; }

        bool IsInfix { get; }

        bool IsNary { get; }

        bool IsLinearNary { get; }

        bool IsUnary { get; }

        int Precedence { get; }

        NodeValueType ValueType { get; }

        bool CarriesValue { get; }

        bool HasCaretPositions { get; }

        global::System.Type InfoType { get; }

        global::System.Nullable<char> OperatorSymbol { get; }

        SymbolMargin OperatorSymbolMargin { get; }

        string OperatorSExpName { get; }

        string OperatorVisualRepresentation { get; }

        string RLFormat { get; }

        string XMLFormat { get; }

        string SplitOposite { get; }

        string[] Alternates { get; }

        int ChildCount { get; }

        int[] ChildrenToRemoveOnDelete { get; }

        int[] ContainerSlots { get; }

        bool HasContainerSlot { get; }

        bool IsFullContainer { get; }

        bool IsSkipGenerateVisual { get; }

        bool AllChildSlotsAreContainerSlots { get; }

        bool IsPlaceholder { get; }

        bool IsEvaluatable { get; }

        bool IsEvaluation { get; }

        bool IsScopable { get; }

        bool IsScopingBoundary { get; }

        bool MustScopeParent { get; }

        bool JumpThrough { get; }

        global::System.Text.RegularExpressions.Regex ValidValueRegex { get; }

        global::System.Text.RegularExpressions.Regex MatchRegex { get; }

        Parsing.IValueCleaner ValueFixer { get; }

        bool HasVisualRepresentation { get; }

        bool AllowCaretOnLeft { get; }

        bool AllowCaretOnRight { get; }

        bool RestrictFromNavigation { get; }

        bool RestrictsDescendants { get; }

        bool ProhibitsDescendants { get; }

        bool RestrictsAncestors { get; }

        bool AlsoRemoveParent { get; }

        bool DeleteParentInsteadOfNode { get; }

        bool Temporal { get; }

        bool ShowOnlyWhenFocused { get; }

        global::System.Nullable<int> ReplaceWithChildOnDelete { get; }

        global::System.Collections.Generic.IDictionary<int, string> DefaultNodeOverrides { get; }

        bool IsImplicit { get; }

        string SExpFormat { get; }

        bool DistinctNodeEdges { get; }

        bool ExplicitValue { get; }

        global::System.Nullable<Parsing.OperatorFamily> Family { get; }

        global::System.Nullable<MathNodeGroups> Group { get; }

        bool IsLiteral { get; }

        bool IsIdentifier { get; }

        bool NoLogicOnlyVisual { get; }

        bool PreFenceNavigatable { get; }

        bool PostFenceNavigatable { get; }

        int NavigablePartCount { get; }

        int[] NavigablePartPositions { get; }

        global::System.Nullable<int> PrimalChildPosition { get; }

        bool AllowVerticalNavigation { get; }

        global::Quire.Direction[] PrimalChildTypeoverExtendDirections { get; }

        bool IsLeafOperator { get; }

        int[] InherentChildren { get; }

        bool JumpIntoChildren { get; }

        int[] NavigatablePartRows { get; }

        int[] ChildrenRows { get; }

        int[] ProhibitedAutoLabelIdDescendants { get; }

        int RowCount { get; }

        bool IsPairNary { get; }

        string FirstElementDescriptorName { get; }

        string MiddleElementDescriptorName { get; }

        string LastElementDescriptorName { get; }

        string AlternateMiddleElementDescriptorName { get; }

        string AlternateOperatorSExpName { get; }

        bool IsPairNarySubElement { get; }

        string ParentPairNaryDescriptorName { get; }

        string OperatorText { get; }

        bool IsBinOctHexModeOn { get; }

        bool IsContainerSlot(int slotIndex);

        bool IsChildAllowed(string childName, int childIndex);

        bool AllowCaretOn(global::Quire.Direction direction);

        bool IsParentAllowed(string parentName, int indexInParent);

        Parsing.Panacea MediateChildInsertionDispute(string childName, int childIndex);

        Parsing.Panacea MediateParentRestriction(string parentName, int indexInParent);

        bool IsDescendentAllowed(string descendantAllowed, int childIndex);

        bool IsAncestorTreeAllowed(global::System.Collections.Generic.IEnumerable<global::Quire.Pair<string, int>> ancestors);

        Parsing.Panacea MediateAncestorRestriction(global::System.Collections.Generic.IEnumerable<global::Quire.Pair<string, int>> ancestors);

        Parsing.Panacea MitigateChildPatternProhibition(global::System.Collections.Generic.IList<string> children);
    }
}
