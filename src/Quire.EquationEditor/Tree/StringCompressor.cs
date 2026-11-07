using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Quire.EquationEditor.Tree;

public static partial class StringCompressor
{
    public static string CompressString(string text, CompressStringAlgorithmTypes algorithm = CompressStringAlgorithmTypes.GZip)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        MemoryStream memoryStream = new MemoryStream();
        switch (algorithm)
        {
            case CompressStringAlgorithmTypes.GZip:
            {
                using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionLevel.Fastest))
                {
                    gZipStream.Write(bytes, 0, bytes.Length);
                }

                break;
            }
            case CompressStringAlgorithmTypes.Brotli:
                // Mathcad's Release IL retains the empty Brotli switch arm.
                break;
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static string DecompressString(string compressedText, CompressStringAlgorithmTypes algorithm = CompressStringAlgorithmTypes.GZip)
    {
        if (compressedText == null)
        {
            return null;
        }

        MemoryStream stream = new MemoryStream(Convert.FromBase64String(compressedText));
        MemoryStream memoryStream = new MemoryStream();
        switch (algorithm)
        {
            case CompressStringAlgorithmTypes.GZip:
            {
                using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress))
                {
                    gZipStream.CopyTo(memoryStream);
                }

                break;
            }
            case CompressStringAlgorithmTypes.Brotli:
                // Mathcad's Release IL retains the empty Brotli switch arm.
                break;
        }

        return Encoding.UTF8.GetString(memoryStream.ToArray());
    }
}
