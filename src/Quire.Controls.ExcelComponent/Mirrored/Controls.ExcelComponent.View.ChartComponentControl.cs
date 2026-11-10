using System;
using System.Windows.Threading;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls.ExcelComponent.View;

public partial class ChartComponentControl
{
    protected override ISerializationStrategy SerializationStrategy { get; set; }

    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    private DispatcherOperation UpdateIdentifierLabelsOperation { get; set; }

    protected override IOleContainerItem CreateOleContainerItem()
    {
        return new ChartContainer();
    }

    protected override string GetInternalHelpContext()
    {
        return "ChartComponentControl";
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public ChartComponentControl Clone()
    {
        return new ChartComponentControl(this);
    }
}
