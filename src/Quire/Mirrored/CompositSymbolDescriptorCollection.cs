using System;
using System.Globalization;

namespace Quire;

internal partial class CompositSymbolDescriptorCollection
{
    public global::System.Collections.ObjectModel.Collection<global::Quire.Controls.VisualDescriptorTableBase> Collections { get; set; }
    public CompositSymbolDescriptorCollection()
    {
        Collections = new global::System.Collections.ObjectModel.Collection<global::Quire.Controls.VisualDescriptorTableBase>();
    }
}
