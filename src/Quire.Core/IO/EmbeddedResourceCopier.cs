using System.IO;
using System.Reflection;
using Quire.Mirror;

namespace Quire.IO;

public static partial class EmbeddedResourceCopier
{
    [MirrorVerified("EmbeddedResourceCopierMechanicalDifferentialTests")]
    public static void Copy(Assembly assemblyContainingResource, string resourceName, string destinationFilePath)
    {
        if (File.Exists(destinationFilePath))
        {
            File.SetAttributes(destinationFilePath, FileAttributes.Normal);
        }

        using Stream stream = assemblyContainingResource.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            char[] characters = new char[stream.Length];
            for (int i = 0; i < stream.Length; i++)
            {
                characters[i] = (char)bytes[i];
            }

            using StreamWriter writer = new(destinationFilePath);
            writer.Write(characters);
            writer.Flush();
            return;
        }
    }
}
