#nullable disable
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Core.Serialization;

public partial interface IHasRegionsWithConverterAnnotations
{
    bool HasRegionsWithConverterAnnotations { get; }

    IEnumerable<UIElement> RegionsWithConverterAnnotations { get; }
}
