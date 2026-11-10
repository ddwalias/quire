using System.Windows;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualEvalNode
{
    public static readonly DependencyProperty EvaluationPlaceholderProperty;

    static VisualEvalNode()
    {
        FrameworkPropertyMetadata metadata = new FrameworkPropertyMetadata();
        metadata.DefaultValue = string.Empty;
        metadata.AffectsMeasure = true;
        metadata.AffectsRender = true;
        metadata.PropertyChangedCallback = new PropertyChangedCallback(EvaluationPlaceholderChanged);
        EvaluationPlaceholderProperty = DependencyProperty.Register("EvaluationPlaceholder", typeof(string), typeof(VisualEvalNode), metadata);
    }

    public VisualEvalNode()
        : base(new VisualSymbolNode('=', 2))
    {
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, EvaluationPlaceholderProperty, "EvalPlaceholder");
    }

    protected static void EvaluationPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualEvalNode)d).HandleEvaluationPlaceholderChanged(e);
    }

    private void HandleEvaluationPlaceholderChanged(DependencyPropertyChangedEventArgs e)
    {
    }
}
