namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableColumnToRlTransform
{
    private int a(global::System.Collections.Generic.IList<global::Quire.EquationEditor.IEquationModel> models)
    {
        int count = models.Count;
        for (int index = models.Count - 1; index > 0; index--)
        {
            if (!models[index].EquationRoot.IsPlaceholder)
            {
                break;
            }

            count--;
        }

        return count;
    }
}
