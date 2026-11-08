namespace Quire.EquationEditor.Tree;

public partial class ComboBoxMathCreator
{
    public static readonly ComboBoxMathCreator Default = new ComboBoxMathCreator(new StringToComboBoxMathTransformer());

    private readonly IStringToComboBoxMathTransformer a;

    public ComboBoxMathCreator(IStringToComboBoxMathTransformer transformer)
    {
        a = transformer;
    }

    public MathNode CreateStringMathNode(string value) => a.TransformStringMathNode(value);

    public MathNode CreateValueMath(string value) => a.TransformStringToNumericMathTree(value, true) ?? CreateStringMathNode(value);
}
