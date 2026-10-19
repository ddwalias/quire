#nullable disable
using System;
using System.IO;
using Quire.Mirror;

namespace Quire.IO;

public partial class FileOperationsProvider
{
    [MirrorVerified("PtcIOFileOperationsProviderTierAMechanicalDifferentialTests")]
    public Exception CreateDirectory(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
        }
        catch (Exception result)
        {
            return result;
        }

        return null;
    }
}
