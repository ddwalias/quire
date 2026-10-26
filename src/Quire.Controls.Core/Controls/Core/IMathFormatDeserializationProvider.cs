#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface IMathFormatDeserializationProvider
{
    StaticMathFormat TargetMathFormat { get; }

    IDictionary<IdLabels, ExtendedFont> TargetLabeledIdFormat { get; }
}
