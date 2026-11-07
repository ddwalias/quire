#nullable disable
using System;

namespace Quire.Controls.Core;

public partial interface IResizeToFitWhiteboardItem
{
    ResizeToFitState ResizeState { get; }
    event Action<IResizeToFitWhiteboardItem> RequestResizeToFitOrClearRestriction;
}
