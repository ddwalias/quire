namespace Quire.EquationEditor.Tree.Transform;

public partial class WorksheetXmlTransformArgs
{
    public System.Xml.XmlNamespaceManager NameSpaceManager => Item1;

    public Quire.PersistentDataObjects.IWorksheetMathPresentation MathPresentation => Item2;

    public WorksheetXmlTransformArgs(System.Xml.XmlNamespaceManager item1, Quire.PersistentDataObjects.IWorksheetMathPresentation item2)
        : base(item1, item2)
    {
    }
}
