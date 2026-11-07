#nullable disable
using System;

namespace Quire.Controls.Core;

public partial interface ICustomSplitItem
{
    event Action<ICustomSplitItem> SplitItemIfNeeded;
    double GetValidSplitOffset(double originalOffset, double pageHeight, ref int lineNumberHint);
    void ValidateLayout();
}
