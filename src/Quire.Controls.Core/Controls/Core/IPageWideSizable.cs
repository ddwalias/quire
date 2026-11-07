#nullable disable
using System;

namespace Quire.Controls.Core;

public partial interface IPageWideSizable
{
    bool NotHookupSizeChangedHandlerUntilLoaded { get; set; }
    event Action HookupSizeChangedHandler;
    void FireHookupSizeChangedHandler();
}
