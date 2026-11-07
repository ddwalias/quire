#nullable disable
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IZombie
{
    GeneralTransform ItemToZombieTransform { get; }
    Quire.Core.IMaybeHasPointThatSnapsToPixel SnapsToPixelItem { get; }
}
