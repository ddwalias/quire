#nullable disable
using System;
using System.Drawing.Imaging;

namespace Quire.Utils;

public partial class JPEGFormatter
{
    protected long m_quality = 50L;

    internal override string MimeType => "image/jpeg";

    internal override EncoderParameters EncoderParams
    {
        get
        {
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, Quality);
            return encoderParameters;
        }
    }

    public long Quality
    {
        get => m_quality;
        set
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentOutOfRangeException();
            }
            m_quality = value;
        }
    }

    public JPEGFormatter()
    {
    }
}
