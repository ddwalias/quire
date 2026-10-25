#nullable disable
using System;

namespace Quire.Controls.Core.AttachedBehavior;

public partial interface IActivator
{
    bool IsActive { get; set; }

    event EventHandler<EventArgs> Activated;
}
