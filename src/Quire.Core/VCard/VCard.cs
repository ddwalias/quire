#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCard
{
    public VCard(string vCardString)
    {
        foreach (string vCardStringLine in GetVCardStringLines(vCardString))
        {
            VCardContentLine vCardContentLine = new VCardContentLine(vCardStringLine);
            bool flag;
            Telephone telephone;
            switch (vCardContentLine.Name)
            {
                case "VERSION":
                    Version = vCardContentLine.Value;
                    break;
                case "FN":
                    FormattedName = vCardContentLine.Value;
                    break;
                case "N":
                    Name = new Name
                    {
                        FamilyName = vCardContentLine.GetValue(0),
                        GivenName = vCardContentLine.GetValue(1),
                        AdditionalNames = vCardContentLine.GetValue(2),
                        HonorificPrefixes = vCardContentLine.GetValue(3),
                        HonorificSuffixes = vCardContentLine.GetValue(4)
                    };
                    break;
                case "TITLE":
                    Title = vCardContentLine.Value;
                    break;
                case "ORG":
                    Organization = vCardContentLine.Value;
                    break;
                case "ADR":
                    DeliveryAddress = new Address
                    {
                        POBox = vCardContentLine.GetValue(0),
                        ExtendedAddress = vCardContentLine.GetValue(1),
                        StreetAddress = vCardContentLine.GetValue(2),
                        City = vCardContentLine.GetValue(3),
                        StateOrProvince = vCardContentLine.GetValue(4),
                        PostalCode = vCardContentLine.GetValue(5),
                        CountryName = vCardContentLine.GetValue(6)
                    };
                    break;
                case "TEL":
                case "X-MS-TEL":
                {
                    flag = vCardContentLine.GetParam("FAX") != null;
                    Telephone telephoneValue = default;
                    telephoneValue.IsFax = flag;
                    telephoneValue.IsVoice = !flag;
                    telephoneValue.Number = vCardContentLine.Value;
                    telephoneValue.Type = Telephone.Types.Work;
                    telephone = telephoneValue;
                    if (TelephoneList == null)
                    {
                        TelephoneList = new Telephone[1];
                        TelephoneList[0] = telephone;
                        break;
                    }
                    Telephone[] array = new Telephone[TelephoneList.Length + 1];
                    for (int i = 0; i < TelephoneList.Length; i++)
                    {
                        array[i] = TelephoneList[i];
                    }
                    array[TelephoneList.Length] = telephone;
                    TelephoneList = array;
                    break;
                }
                case "EMAIL":
                    Email = vCardContentLine.Value;
                    break;
                case "URL":
                    Url = vCardContentLine.Value;
                    break;
                case "LABEL":
                    AddressLabel = vCardContentLine.Value;
                    break;
                case "PHOTO":
                    Photo = Convert.FromBase64String(vCardContentLine.Value);
                    break;
            }
        }
    }

    private static IEnumerable<string> GetVCardStringLines(string vCardString)
    {
        string[] lines = vCardString.Replace("\r\n ", "").Replace("\r\n\t", "").Split(Newline, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == null)
            {
                break;
            }

            if (lines[i].Contains("ENCODING=QUOTED-PRINTABLE") && lines[i].EndsWith("=") && i + 1 < lines.Length)
            {
                lines[i] = lines[i].Substring(0, lines[i].Length - 1) + lines[i + 1];
                for (int j = i + 1; j < lines.Length - 1; j++)
                {
                    lines[j] = lines[j + 1];
                }
                i--;
            }
        }
        return lines;
    }

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
