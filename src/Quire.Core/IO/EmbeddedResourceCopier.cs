#nullable disable
using Quire.Mirror;
using System.IO;
using System.Reflection;

namespace Quire.IO;

public static partial class EmbeddedResourceCopier
{
    [MirrorVerified("Release IL")]
    public static void Copy(Assembly assemblyContainingResource, string resourceName, string destinationFilePath)
    {
        if (File.Exists(destinationFilePath))
        {
            File.SetAttributes(destinationFilePath, FileAttributes.Normal);
        }
        using Stream stream = assemblyContainingResource.GetManifestResourceStream(resourceName);
        if (stream != null)
        {
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, (int)stream.Length);
            char[] array2 = new char[stream.Length];
            int i;
            for (i = 0; i < stream.Length; i++)
            {
                array2[i] = (char)array[i];
            }
            using StreamWriter streamWriter = new StreamWriter(destinationFilePath);
            streamWriter.Write(array2);
            streamWriter.Flush();
            return;
        }
    }
}
