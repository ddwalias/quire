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

    public void ImageStreamToImageStream(ImageFormatter formatter, object _inStream, object _outStream)
    {
        Stream stream;
        Stream stream2;
        using (stream = GetStream(_inStream, true))
        {
            using (stream2 = GetStream(_outStream, false))
            {
                using (Image img = Image.FromStream(stream))
                {
                    SaveImageToStream(img, formatter, stream2);
                }
            }
        }
    }

    public ImageConverter()
    {
    }
}
