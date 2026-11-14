#nullable disable
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Quire.Serialization;

namespace Quire.Controls.Core.Ole;

public partial interface IPublicOleContainerModel<TPersistentData>
{
    bool IsStorageEmpty { get; }
    string ProgId { get; }
    Guid ClassId { get; }
    string Save(Action onServerBusy);
    bool Load(string fileName);
    void CleanStorage(bool deleteFile);
    string SaveBitmap(Func<BitmapSource> getImageSource);
    BitmapSource LoadBitmap(string filename);
    IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, TPersistentData data);
    TPersistentData GetRegionPersistentData(IS11NProvider s11NProvider, string uniqueId, Action onServerBusy);
}
