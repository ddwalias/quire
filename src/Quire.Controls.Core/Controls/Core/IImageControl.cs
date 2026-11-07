#nullable disable
using System;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IImageControl
{
    ImageSource Source { get; set; }
    Uri OriginalImageUri { get; set; }
}
