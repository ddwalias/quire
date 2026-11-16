using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class FileNameControl
{
    public override object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return new fieldType
        {
            Item = new object(),
            ItemElementName = ItemChoiceType1.fileName,
            text = GetRegionTextPersistentData(s11NProvider)
        };
    }
}
