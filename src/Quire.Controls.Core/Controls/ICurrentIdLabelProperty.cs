#nullable disable
using System;
using Quire.Controls.Core;

namespace Quire.Controls;

public partial interface ICurrentIdLabelProperty
{
    IdLabels? CurrentIdLabel { get; set; }
    IdLabels? CurrentDisplayIdLabel { get; set; }
}
