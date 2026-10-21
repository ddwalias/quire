using System.Windows.Controls;

namespace Quire.Media3D;

public partial interface IViewportsListManager
{
    void AddToList(Viewport3D viewport);

    void RemoveFromList(Viewport3D viewport);
}
