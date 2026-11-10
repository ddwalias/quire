using System;
using Quire.Controls.Core.Serialization;
using Quire.Controls.ExcelComponent.View;

namespace Quire.Controls.ExcelComponent;

public partial class ExcelComponentControl
{
    protected override ISerializationStrategy SerializationStrategy { get; set; }

    protected override IDeserializationStrategy DeserializationStrategy { get; set; }

    private void ExcelInitialize()
    {
        BindCommands();
    }

    protected override IOleContainerItem CreateOleContainerItem()
    {
        return new ExcelContainer();
    }

    protected override string GetInternalHelpContext()
    {
        return "ExcelComponentControl";
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public ExcelComponentControl Clone()
    {
        return new ExcelComponentControl(this);
    }
}
