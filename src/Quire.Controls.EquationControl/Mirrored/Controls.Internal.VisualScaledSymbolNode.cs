using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualScaledSymbolNode
{
    public void ScaleToHeight(double height)
    {
        Size size = new Size(_inkBounds.Width, _inkBounds.Height);
        if (size.Height > height)
        {
            _scaleFactor.ScaleX = 1.0;
            _scaleFactor.ScaleY = 1.0;
            return;
        }

        _scaleFactor.ScaleY = height / size.Height;
        if (_scaleFactor.ScaleY > 4.0)
        {
            _scaleFactor.ScaleX = _scaleFactor.ScaleY / 3.0;
        }
    }
}
