#nullable disable
using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class AppVersionProvider
{
    private static AppVersionData LoadVersionData()
    {
        try
        {
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Ptc.Core.AppVersion.json");
            if (stream == null)
            {
                throw new InvalidOperationException("Could not find embedded resource: Ptc.Core.AppVersion.json");
            }
            using StreamReader streamReader = new StreamReader(stream);
            AppVersionData versionData = JsonSerializer.Deserialize<AppVersionData>(streamReader.ReadToEnd(), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (versionData == null)
            {
                throw new InvalidOperationException();
            }
            return versionData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to load application version data from embedded JSON: " + ex.Message, ex);
        }
    }

    [MirrorVerified("Release IL")]
    public static Version CurrentAppVersion => new Version(VersionData.CurrentAppVersion);

    [MirrorVerified("Release IL")]
    public static Version CurrentFormatVersion => new Version(VersionData.CurrentFormatVersion);

    [MirrorVerified("Release IL")]
    public static Version CurrentEngNbVersion => new Version(VersionData.CurrentEngNbVersion);
}
