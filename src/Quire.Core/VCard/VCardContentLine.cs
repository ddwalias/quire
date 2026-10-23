#nullable disable
using Quire.Mirror;

namespace Quire.VCard;

public partial class VCardContentLine
{
    public VCardContentLine(string line)
    {
        int separator = line.IndexOf(":");
        string nameAndParameters = line.Substring(0, separator);
        string valueText = line.Substring(separator + 1);
        string[] nameAndParametersParts = nameAndParameters.Split(semicolon, StringSplitOptions.None);
        string[] values = valueText.Split(semicolon, StringSplitOptions.None);
        Name = nameAndParametersParts[0];
        Values = values;
        Params = new Dictionary<string, string>();
        for (int i = 1; i < nameAndParametersParts.Length; i++)
        {
            string[] parameterParts = nameAndParametersParts[i].Split(equals, StringSplitOptions.None);
            string key = parameterParts[0];
            string value = parameterParts.Length > 1 ? parameterParts[1] : "";
            Params.Add(key, value);
        }
        if (GetParam("ENCODING") == "QUOTED-PRINTABLE")
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i] = DecodePrinatbleQuoted(Values[i]);
            }
        }
    }

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

    [MirrorVerified("Release IL")]
    private static string DecodePrinatbleQuoted(string encodedString)
    {
        System.Text.StringBuilder builder = new System.Text.StringBuilder(encodedString.Length);
        for (int i = 0; i < encodedString.Length; i++)
        {
            char character = encodedString[i];
            if (character == '=')
            {
                character = (char)int.Parse(encodedString.Substring(i + 1, 2), System.Globalization.NumberStyles.HexNumber);
                i += 2;
            }
            builder.Append(character);
        }
        return builder.ToString();
    }
}
