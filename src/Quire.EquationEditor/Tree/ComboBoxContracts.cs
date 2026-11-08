namespace Quire.EquationEditor.Tree
{
    public partial interface IComboBoxInputControlDataAdapter
    {
        global::System.Collections.Generic.IList<string> Units { get; }

        global::System.Collections.Generic.IList<global::System.ValueTuple<string, global::System.Collections.Generic.IList<string>>> GroupByRows { get; }

        string TableTitle { get; }

        int SelectedValueRow { get; }
    }

    public partial interface IStringToComboBoxMathTransformer
    {
        MathNode TransformStringToNumericMathTree(string value, bool parseNaNs);

        MathNode TransformStringMathNode(string value);
    }

    public partial interface IInputScriptableWithInputNodeInfo
    {
        bool IsInputsExists { get; set; }

        bool IsInputsAreaShown { get; set; }

        object[] InputsToRestore { get; set; }
    }
}
