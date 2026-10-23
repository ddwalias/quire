#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    private static void SafeClipboardAction(System.Action clipboardAction)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            try
            {
                clipboardAction();
                break;
            }
            catch
            {
            }
            System.Threading.Thread.Sleep(10);
        }
    }
}
