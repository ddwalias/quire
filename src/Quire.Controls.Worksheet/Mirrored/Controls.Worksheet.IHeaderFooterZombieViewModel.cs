using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;

namespace Quire.Controls.Worksheet;

public partial interface IHeaderFooterZombieViewModel
{
    VisualBrush ZombieBackground { get; }

    ObservableCollection<IWhiteboardItem> ZombieItems { get; }

    IMaybeHasPointThatSnapsToPixel SnapsToPixelItem { get; }

    TranslateTransform ItemToZombieTransform { get; }

    event Action zombieIsInitialized;

    void ResetVisualBrush();

    void RestoreVisualBrush();
}
