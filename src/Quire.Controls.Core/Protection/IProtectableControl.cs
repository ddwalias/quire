#nullable disable
using System;

namespace Quire.Controls.Core.Protection;

public partial interface IProtectableControl
{
    Quire.ProtectionLevel ProtectionLevel { get; }
    Quire.ProtectionData ProtectionData { get; }
    event Action<IProtectableControl> ProtectedStateChanged;
}
