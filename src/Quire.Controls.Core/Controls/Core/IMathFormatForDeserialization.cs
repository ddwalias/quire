#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface IMathFormatForDeserialization
{
    IMathFormat DeserializeMathFormat { set; }
    IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat { set; }
}
