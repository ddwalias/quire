#nullable disable
#pragma warning disable

namespace Quire.Controls.ExcelComponent;

public partial class OleObjectControlCreationOptions
{
    public OleObjectControlCreationOptions(OleObjectControlCreationMethod creationMethod, bool autoResize)
    {
        OleObjectControlCreationMethod = creationMethod;
        OleObjectAutoResize = autoResize;
    }

    public bool OleObjectAutoResize { get; private set; }

    public OleObjectControlCreationMethod OleObjectControlCreationMethod { get; private set; }
}
