#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCardContentLine
{
    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public string Name { get; private set; }

    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public Dictionary<string, string> Params { get; private set; }

    [MirrorVerified("PtcVCardVCardContentLineMechanicalDifferentialTests")]
    public string[] Values { get; private set; }

    [MirrorVerified("Release IL")]
    public string Value
    {
        get
        {
            if (Values != null)
            {
                if (Values.Length != 0)
                {
                    return Values[0];
                }
            }
            return null;
        }
    }

    [MirrorVerified("Release IL")]
    public string GetParam(string paramName)
    {
        if (!Params.ContainsKey(paramName))
        {
            return null;
        }
        return Params[paramName];
    }

    [MirrorVerified("Release IL")]
    public string GetValue(int index)
    {
        if (Values != null)
        {
            if (index < Values.Length)
            {
                goto ReturnValue;
            }
        }
        return null;

    ReturnValue:
        return Values[index];
    }
}
