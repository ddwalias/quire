#nullable disable

namespace Quire.License;

public sealed partial class LicenseManager
{
    // Compile-order device: this closure follows the file-reader and message handler to keep Mathcad ordinal 47.
    public static global::System.Collections.Generic.IList<string> GetLicenseHashCodesFromText(string licenseText)
    {
        if (global::Quire.EnumerableExtensions.IsNullOrEmpty<char>(licenseText))
        {
            return null;
        }

        global::System.Text.RegularExpressions.MatchCollection matchCollection =
            global::System.Text.RegularExpressions.Regex.Matches(
                licenseText,
                "\\bSIGN2=\"([\\w\\s\\\\]+)\"",
                global::System.Text.RegularExpressions.RegexOptions.IgnoreCase
                    | global::System.Text.RegularExpressions.RegexOptions.Multiline);
        int count = matchCollection.Count;
        global::System.Collections.Generic.List<string> list;
        if (count == 0)
        {
            list = null;
        }
        else
        {
            list = new global::System.Collections.Generic.List<string>(count);
            for (int index = 0; index < count; index++)
            {
                global::System.Text.RegularExpressions.Match match = matchCollection[index];
                if (global::Quire.DebugTools.DevAssert(match.Success, null).Passed)
                {
                    string flatHash = global::System.Text.RegularExpressions.Regex.Replace(
                        match.Groups[1].Value.Replace("\\", string.Empty),
                        "\\s+",
                        " ",
                        global::System.Text.RegularExpressions.RegexOptions.Multiline);
                    if (!global::System.String.IsNullOrWhiteSpace(flatHash)
                        && !list.Any(code => global::Quire.License.LicenseConfigProvider.HashCodesEqual(code, flatHash)))
                    {
                        list.Add(flatHash);
                    }
                }
            }
        }

        return list;
    }
}
