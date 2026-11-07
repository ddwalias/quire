#nullable disable
using System;

namespace Quire.Controls.Core.Protection;

public partial interface IProtectableModel
{
    ProtectionData ProtectionData { get; }
    bool HasOpenDescendantOleObjects { get; }
    void SetProtection(ProtectionData protectionData, Version formatVersion = default);
    void RemoveProtection();
}
