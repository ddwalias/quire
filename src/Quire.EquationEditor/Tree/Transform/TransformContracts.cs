namespace Quire.EquationEditor.Tree.Transform
{
    public partial interface IStringToMathNodeTransform
    {
        global::Quire.EquationEditor.Tree.MathNode Transform(string s);
    }

    public partial interface IXmlToMathNodeTransform
    {
        global::Quire.EquationEditor.Tree.MathNode Transform(global::System.Xml.XmlNode xmlMathRegionNode);

        global::Quire.EquationEditor.Tree.MathNode Transform(string sExpression);
    }
}
