namespace Quire.EquationEditor.SpecTable.Model;

public partial class SpecTableColumnToRlTransform
{
    internal static string a(
        string text,
        global::Quire.EquationEditor.IEquationModel model,
        global::System.Collections.Generic.List<global::Quire.Pair<global::Quire.EquationEditor.IEquationModel, uint>> map)
    {
        var builder = new global::System.Text.StringBuilder(text);
        var regex = new global::System.Text.RegularExpressions.Regex(":0x[0-9a-fA-F]{8}");
        foreach (global::System.Text.RegularExpressions.Match match in regex.Matches(text))
        {
            uint tag = uint.Parse(match.Value.Substring(3), global::System.Globalization.NumberStyles.AllowHexSpecifier);
            map.Add(new global::Quire.Pair<global::Quire.EquationEditor.IEquationModel, uint>(model, tag));
            builder.Remove(match.Index + 3, 8);
            builder.Insert(match.Index + 3, string.Format("{0:x8}", map.Count));
        }

        return builder.ToString();
    }

    internal string a(
        ISpecTableColumn column,
        global::Quire.EquationEditor.Tree.Transform.MathNodeToRlTransformer transformer,
        global::System.Collections.Generic.List<global::Quire.Pair<global::Quire.EquationEditor.IEquationModel, uint>> map)
    {
        var models = new global::System.Collections.Generic.List<global::Quire.EquationEditor.IEquationModel>(column.Data);
        int count = a(models);
        if (count == 1)
        {
            return a(transformer.SetRlidAndTransform(models[0].EquationRoot), models[0], map);
        }

        var builder = new global::System.Text.StringBuilder();
        for (int index = 0; index < count - 1; index++)
        {
            string transformed = transformer.SetRlidAndTransform(models[index].EquationRoot);
            builder.Append(a(transformed, models[index], map)).Append(" ");
        }

        string last = transformer.SetRlidAndTransform(models[count - 1].EquationRoot);
        builder.Append(a(last, models[count - 1], map));
        // Mathcad calls String.Format and boxes the count here.
        return string.Format("(op_matrix (unboxed {0}) (unboxed 1) (list {1}))", count, builder);
    }

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
