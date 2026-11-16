using Quire.Controls.Core;
using Quire.Controls.Text;

namespace Quire.Controls.Worksheet;

public partial class RichTextBaseRegionPersistentData : RegionPersistentData
{
    private readonly IRichTextWithEmbeddedItem _text;

    public RichTextBaseRegionPersistentData(RichTextWithEmbeddedItem text)
        : base(text)
    {
        _text = text;
    }

    protected override void Deserialize()
    {
        base.Deserialize();
        if (!(_text is Paragraph))
        {
            DeserializeWidth();
        }
    }
}
