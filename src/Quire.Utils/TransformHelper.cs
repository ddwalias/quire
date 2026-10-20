#nullable disable
using System;
using System.IO;

namespace Quire.Utils;

public partial class TransformHelper
{
    public string Base64JPGToBase64PNG(string base64JPG)
    {
        byte[] buffer = Convert.FromBase64String(base64JPG);
        ImageConverter imageConverter = new ImageConverter();
        PNGFormatter formatter = new PNGFormatter();
        MemoryStream inStream = new MemoryStream(buffer, false);
        MemoryStream memoryStream = new MemoryStream();
        imageConverter.ImageStreamToImageStream(formatter, inStream, memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public TransformHelper()
    {
    }
}
