using System.Collections.Generic;

namespace Quire.Controls.Internal;

internal partial interface IVisualText
{
    double Baseline { get; }

    double SuperscriptBaseline { get; }

    IList<double> CharacterWidths { get; }
}
