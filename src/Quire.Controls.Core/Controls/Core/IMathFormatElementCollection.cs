#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface IMathFormatElementCollection
{
    IEnumerable<ICurrentMathFormatProperty> MathFormatElements { get; }
}
