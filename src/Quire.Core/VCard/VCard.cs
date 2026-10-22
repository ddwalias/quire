#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCard
{
    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Version { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string FormattedName { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Name Name { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Title { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Organization { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Address DeliveryAddress { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string AddressLabel { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public Telephone[] TelephoneList { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Email { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public string Url { get; private set; }

    [MirrorVerified("PtcVCardVCardMechanicalDifferentialTests")]
    public byte[] Photo { get; private set; }

    [MirrorVerified("Release IL")]
    public string PrimaryPhone
    {
        get
        {
            if (TelephoneList != null)
            {
                Telephone[] telephoneList = TelephoneList;
                for (int i = 0; i < telephoneList.Length; i++)
                {
                    Telephone telephone = telephoneList[i];
                    if (telephone.IsVoice)
                    {
                        return telephone.Number;
                    }
                }
            }
            return null;
        }
    }

    [MirrorVerified("Release IL")]
    public string PrimaryFax
    {
        get
        {
            if (TelephoneList != null)
            {
                Telephone[] telephoneList = TelephoneList;
                for (int i = 0; i < telephoneList.Length; i++)
                {
                    Telephone telephone = telephoneList[i];
                    if (telephone.IsFax)
                    {
                        return telephone.Number;
                    }
                }
            }
            return null;
        }
    }

    [MirrorVerified("Release IL")]
    public string SafeUrl
    {
        get
        {
            if (Url == null)
            {
                return null;
            }
            string safeUrl = null;
            try
            {
                System.Uri uri;
                if (Url.ToLowerInvariant().IndexOf("://") <= 0)
                {
                    uri = new System.Uri("http://" + Url);
                }
                else
                {
                    uri = new System.Uri(Url);
                }
                string scheme = uri.Scheme;
                if (scheme == "http" || scheme == "https")
                {
                    safeUrl = System.Uri.EscapeUriString(uri.ToString());
                }
            }
            catch (Exception)
            {
            }
            return safeUrl;
        }
    }
}
