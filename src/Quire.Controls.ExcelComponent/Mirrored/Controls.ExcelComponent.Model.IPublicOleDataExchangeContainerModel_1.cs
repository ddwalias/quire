#nullable disable
#pragma warning disable
using System;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IPublicOleDataExchangeContainerModel<TPersistentData>
{
    bool DoActionWithLock(Action action);
}
