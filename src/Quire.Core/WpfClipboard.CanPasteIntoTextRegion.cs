#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public static bool CanPasteIntoTextRegion()
    {
        try
        {
            System.Windows.IDataObject dataObject = System.Windows.Clipboard.GetDataObject();
            if (dataObject != null && dataObject.GetDataPresent(_canPasteIntoTextRegion))
            {
                return (bool)dataObject.GetData(_canPasteIntoTextRegion);
            }
        }
        catch
        {
        }
        return true;
    }
}
