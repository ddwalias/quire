#nullable disable
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.ComTypes;

namespace Quire.Utils;

public partial class ImageConverter
{
    protected void SaveImageToStream(Image img, ImageFormatter formatter, Stream stream)
    {
        if (formatter.EncoderParams == null)
        {
            img.Save(stream, formatter.Format);
        }
        else
        {
            img.Save(stream, formatter.Encoder, formatter.EncoderParams);
        }
        stream.Flush();
    }

    protected Stream GetStream(object obj, bool bRead)
    {
        if (obj is string)
        {
            if (bRead)
            {
                return new WebClient().OpenRead(obj.ToString());
            }
            return new WebClient().OpenWrite(obj.ToString());
        }
        if (obj is Stream)
        {
            return (Stream)obj;
        }
        if (!(obj is IStream))
        {
            throw new ArgumentException();
        }
        return new COMStreamAdaptor((IStream)obj);
    }

    public void ImageStreamToImageStream(ImageFormatter formatter, object inputSource, object outputTarget)
    {
        // Assign pre-declared locals: the Release IL stores each stream before its try block.
        Stream inputStream;
        Stream outputStream;
        using (inputStream = GetStream(inputSource, true))
        {
            using (outputStream = GetStream(outputTarget, false))
            {
                using (Image img = Image.FromStream(inputStream))
                {
                    SaveImageToStream(img, formatter, outputStream);
                }
            }
        }
    }

    public ImageConverter()
    {
    }
}
