#nullable disable
using System;
using System.Text.RegularExpressions;
using Quire.Mirror;

namespace Quire.Controls.Help;

public static partial class EngNoteContextData
{
    [MirrorVerified("PtcControlsHelpEngNoteContextDataMechanicalDifferentialTests")]
    public static string HelpEnvVarName { get; private set; }

    private const string HelpEnvVarNameKey = "HelpEnvVarName";

    public static void Parse(string[] contextData)
    {
        if (contextData == null || contextData.Length == 0)
        {
            return;
        }
        foreach (string text in contextData)
        {
            if (!string.IsNullOrEmpty(text))
            {
                foreach (Match match in KeyValuePairRegex().Matches(text))
                {
                    string key = match.Groups["key"].Value;
                    string value = match.Groups["value"].Value;
                    if (string.Equals(key, HelpEnvVarNameKey, StringComparison.OrdinalIgnoreCase))
                    {
                        HelpEnvVarName = value;
                    }
                }
            }
        }
    }
}
