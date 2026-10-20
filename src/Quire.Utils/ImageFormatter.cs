#nullable disable
using System;
using System.Drawing.Imaging;

namespace Quire.Utils;

public abstract partial class ImageFormatter
{
    protected ImageCodecInfo m_encoder;

    internal abstract string MimeType { get; }

    internal ImageCodecInfo Encoder
    {
        get
        {
            if (m_encoder == null)
            {
                m_encoder = GetEncoderInfo(MimeType);
            }
            return m_encoder;
        }
    }

    internal ImageFormat Format => new ImageFormat(Encoder.FormatID);

    internal virtual EncoderParameters EncoderParams => null;

    internal static ImageCodecInfo GetEncoderInfo(string mimeType)
    {
        int i;
        ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
        for (i = 0; i < imageEncoders.Length; i++)
        {
            if (imageEncoders[i].MimeType == mimeType)
            {
                return imageEncoders[i];
            }
        }
        throw new ArgumentException();
    }

    protected ImageFormatter()
    {
    }
}
