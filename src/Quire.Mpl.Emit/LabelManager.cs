using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace System.Reflection.Emit.Quire.Mpl;

internal partial class LabelManager
{
    private Dictionary<Label, LabelHandle> _labels = new Dictionary<Label, LabelHandle>();
    private int _id;

    public LabelManager()
    {
        _id = 0;
    }

    public void Clear()
    {
        _id = 0;
        _labels.Clear();
    }

    public Label DefineLabel()
    {
        Label result = default;
        typeof(Label).GetField("m_label", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValueDirect(__makeref(result), _id++);
        return result;
    }

    public void AddLabelHandle(Label label, LabelHandle labelHandle)
    {
        _labels.Add(label, labelHandle);
    }

    public LabelHandle GetLabelHandle(Label label)
    {
        return _labels[label];
    }

    public static int GetLabelId(Label label)
    {
        object obj = typeof(Label).GetField("m_label", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValueDirect(__makeref(label));
        if (obj != null)
        {
            return (int)obj;
        }
        throw new ArgumentNullException("label");
    }
}
