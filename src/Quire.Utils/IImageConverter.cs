#nullable disable

namespace Quire.Utils;

public partial interface IImageConverter
{
    void ImageStreamToImageStream(ImageFormatter formatter, object inStream, object outStream);
}
