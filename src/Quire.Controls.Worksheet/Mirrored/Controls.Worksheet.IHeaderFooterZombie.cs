using System;

namespace Quire.Controls.Worksheet;

public partial interface IHeaderFooterZombie
{
    void ResetVisualBrush();

    void RestoreVisualBrush();

    void Show();

    void Collapse();

    void SubscribeToZombieIsInitialized(Action handler);

    void UnsubscribeFromZombieIsInitialized(Action handler);
}
