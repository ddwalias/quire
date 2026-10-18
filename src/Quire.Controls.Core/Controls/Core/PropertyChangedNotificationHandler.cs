#nullable disable
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class PropertyChangedNotificationHandler
{
    [MirrorVerified("PtcControlsCorePropertyChangedNotificationHandlerMechanicalDifferentialTests")]
    private readonly Dictionary<string, Action> _propertyChangedNotificationHandlers;

    [MirrorVerified("PtcControlsCorePropertyChangedNotificationHandlerMechanicalDifferentialTests")]
    public void AddPropertyChangedNotificationHandler(string propertyName, Action propertyChangedHandler) => _propertyChangedNotificationHandlers.Add(propertyName, propertyChangedHandler);
}
