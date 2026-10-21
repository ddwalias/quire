#nullable disable
using System.IO;
using System.Reflection;
using Quire.IO;
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCardFactory
{
    [MirrorVerified("Release IL")]
    public static VCard GetInstance()
    {
        VCard vCard = null;
        string text = Path.Combine(FileUtils.GetAssemblyFolder(Assembly.GetEntryAssembly()), "Misc", "contact.vcf");
        Uri uri = new Uri(text);
        if (uri.IsFile)
        {
            text = uri.LocalPath;
        }
        using (StreamReader streamReader = new StreamReader(text))
        {
            vCard = new VCard(streamReader.ReadToEnd());
        }
        return vCard;
    }

    [MirrorVerified("PtcVCardVCardFactoryMechanicalDifferentialTests")]
    public VCardFactory() { }
}
