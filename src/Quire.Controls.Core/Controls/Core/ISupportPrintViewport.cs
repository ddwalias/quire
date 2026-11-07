#nullable disable
using System;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface ISupportPrintViewport
{
    Nullable<Rect> PrintViewport { get; set; }
}
