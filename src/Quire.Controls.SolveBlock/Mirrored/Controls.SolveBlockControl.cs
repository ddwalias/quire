using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;

namespace Quire.Controls;

public partial class SolveBlockControl
{
    public static readonly DependencyProperty IsSolveBlockLabelVisibleProperty = DependencyProperty.Register(
        "IsSolveBlockLabelVisible", typeof(bool), typeof(SolveBlockControl), new PropertyMetadata(true));

    protected override ISerializationStrategy SerializationStrategy { get; set; }
    protected override IDeserializationStrategy DeserializationStrategy { get; set; }
    public ITextStyleChangedHandler TextStyleChangedHandler { get; set; }

    public global::Quire.Controls.SolveBlockEquationTypes GetSolveBlockEquationType(
        global::Quire.Controls.Core.IWhiteboardItem item) => _sbModel.GetSolveBlockEquationType(item);

    public override string GetBlockEquationTypeAttributeName(
        global::Quire.Controls.Core.IWhiteboardItem item) => _sbModel.GetBlockEquationTypeAttributeName(item);

    protected override string GetInternalHelpContext() => "SolveBlockControl";

    public EngineResult[] EngineResults
    {
        get
        {
            _engineResults = _sbModel.AllCalculatables.Select((ISolveBlockCalculatable wrapper) => wrapper.EngineResult).ToArray();
            return _engineResults;
        }
        set => _engineResults = value;
    }

}
