#nullable disable
using System;

namespace Quire.Controls.Core;

public static partial class IdLabelsExtensions
{
    public static readonly string EmptyLabelRl = ((IdLabels?)null).GetRl(null);

    public static bool IsNullOrNone(this IdLabels? label)
    {
        if (label.HasValue)
        {
            return label.Value == IdLabels.None;
        }
        return true;
    }

    public static string GetRl(this IdLabels? label, string labelForNoLabel)
    {
        string text;
        if (label.IsNullOrNone())
        {
            text = labelForNoLabel;
        }
        else
        {
            text = (string)new IdLabelToStringConverter().Convert(label, typeof(string), null, null);
        }
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }
        return "{" + text + "}";
    }
}
