#nullable disable
using System;

namespace Quire.Controls.Core;

public partial interface INotifyMeasureOverride
{
    event Action MeasureOverideCompleted;
}
