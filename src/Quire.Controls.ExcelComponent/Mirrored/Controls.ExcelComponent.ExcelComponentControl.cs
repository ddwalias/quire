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

    public global::Quire.Controls.ExcelComponent.Model.ExcelComponentEquationType GetEquationType(global::Quire.Controls.Core.IEquationControl equation)
    {
        return _exModel.GetEquationType(equation);
    }

    public global::Quire.Calculation.Core.EngineError GetError(global::Quire.Controls.EquationControl item)
    {
        return _exModel.GetError(item);
    }
}
