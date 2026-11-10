using System.Collections.Generic;
using System.Windows.Media;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal abstract partial class VisualTextNodeForEditor
{
    public abstract string Text { get; set; }

    public abstract FormattedText FormattedNodeText { get; }

    public abstract IList<double> CharacterWidths { get; }

    public abstract IList<double> ScreenCharacterWidths { get; }

    public abstract event Delegate1<string> TextChanged;

    protected VisualTextNodeForEditor()
    {
    }
}
