using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Quire.Controls.InputControls;

internal static partial class StringCompressor
{
    public static string CompressString(string text, CompressStringAlgorithmTypes algorithm = (CompressStringAlgorithmTypes)0)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        MemoryStream stream = new MemoryStream();
        switch (algorithm)
        {
            case (CompressStringAlgorithmTypes)0:
                using (GZipStream gzip = new GZipStream(stream, CompressionLevel.Fastest))
                {
                    gzip.Write(bytes, 0, bytes.Length);
                }
                break;
            case (CompressStringAlgorithmTypes)2:
                return text;
        }

        return Convert.ToBase64String(stream.ToArray());
    }

    public static string DecompressString(string compressedText, CompressStringAlgorithmTypes algorithm = (CompressStringAlgorithmTypes)0)
    {
        if (compressedText == null)
        {
            return null;
        }

        MemoryStream input = new MemoryStream(Convert.FromBase64String(compressedText));
        MemoryStream output = new MemoryStream();
        switch (algorithm)
        {
            case (CompressStringAlgorithmTypes)0:
                using (GZipStream gzip = new GZipStream(input, CompressionMode.Decompress))
                {
                    gzip.CopyTo(output);
                }
                break;
            case (CompressStringAlgorithmTypes)2:
                return compressedText;
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
