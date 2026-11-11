using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Core;
using Quire.EquationEditor.View;
using Quire.Wpf;
using Point = System.Windows.Point;
using Size = System.Windows.Size;
using Color = System.Drawing.Color;

namespace Quire.Controls.Internal;

internal partial class VisualPairNaryNodeEditor
{
    public static readonly DependencyProperty OperatorPartIndexProperty;
    public static readonly DependencyProperty CaretOpacityProperty;

    private VisualSymbolNode[] _operatorParts;
    private readonly List<RubberBandHighlighter> _operandHighlighters = new List<RubberBandHighlighter>();

    public int? OperatorPartIndex
    {
        get => (int?)GetValue(OperatorPartIndexProperty);
        set => SetValue(OperatorPartIndexProperty, value);
    }

    private double CaretOpacity => (double)GetValue(CaretOpacityProperty);

    private static void OnCaretOpacityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualPairNaryNodeEditor)d).OnCaretOpacityChanged((double)e.NewValue);
    }

    private static void OnOperatorPartIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((VisualPairNaryNodeEditor)d).OnOperatorPartIndexChanged((int?)e.NewValue);
    }

    static VisualPairNaryNodeEditor()
    {
        FrameworkPropertyMetadata frameworkPropertyMetadata = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata.DefaultValue = null;
        frameworkPropertyMetadata.AffectsArrange = true;
        frameworkPropertyMetadata.PropertyChangedCallback += OnOperatorPartIndexPropertyChanged;
        OperatorPartIndexProperty = DependencyProperty.Register("OperatorPartIndex", typeof(int?), typeof(VisualPairNaryNodeEditor), frameworkPropertyMetadata);

        FrameworkPropertyMetadata frameworkPropertyMetadata2 = new FrameworkPropertyMetadata();
        frameworkPropertyMetadata2.AffectsRender = true;
        frameworkPropertyMetadata2.PropertyChangedCallback += OnCaretOpacityPropertyChanged;
        CaretOpacityProperty = DependencyProperty.Register("CaretOpacity", typeof(double), typeof(VisualPairNaryNodeEditor), frameworkPropertyMetadata2);
    }

    private void OnOperatorPartIndexChanged(int? operatorPartIndex)
    {
        UpdateOperatorSymbols();
    }

    protected override void OnCaretColorChanged(Color color)
    {
        UpdateOperatorSymbols();
    }

    private void OnCaretOpacityChanged(double d)
    {
        UpdateOperatorSymbols();
    }

    public VisualPairNaryNodeEditor(VisualPairNaryNode editNode, VisualMathRegionNode region)
        : base((VisualMathNode)editNode, region)
    {
        IVisualMathNode[] operatorParts = editNode.OperatorParts;
        int num = operatorParts.Length;
        _operatorParts = new VisualSymbolNode[num];
        for (int i = 0; i < num; i++)
        {
            VisualSymbolNode visualSymbolNode = ((VisualSymbolNode)operatorParts[i]).Clone();
            AddVisualChild(visualSymbolNode);
            _operatorParts[i] = visualSymbolNode;
        }
        BindToSettings();
    }

    public override void GetFocus()
    {
        base.GetFocus();
        SetVisibilityOfEditorSymbols(Visibility.Visible);
    }

    public override void Remove()
    {
        EditNode.SetVisibilityOfOperatorSymbol(Visibility.Visible);
        RemoveHighlighters();
        base.Remove();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        for (int i = 0; i < _operatorParts.Length; i++)
        {
            _operatorParts[i].Measure(WpfUtils.SizeInfinity);
        }
        return AdornedElement.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (!(AdornedElement is VisualPairNaryNode visualPairNaryNode))
        {
            return finalSize;
        }
        for (int i = 0; i < _operatorParts.Length; i++)
        {
            _operatorParts[i].Arrange(new Rect(visualPairNaryNode.OperatorPartstopLeft[i], _operatorParts[i].DesiredSize));
        }
        return finalSize;
    }

    private new void BindToSettings()
    {
        base.BindToSettings();
        WpfUtils.BindToSetting(MathcadSettings.Instance, this, CaretOpacityProperty, "CaretHighlightOpacity");
    }

    private void UpdateOperatorSymbols()
    {
        if (!OperatorPartIndex.HasValue)
        {
            RemoveHighlighters();
        }
        else if (_operandHighlighters.Count == 0)
        {
            IVisualMathNode[] nodeChildren = ((VisualPairNaryNode)AdornedElement).NodeChildren;
            for (int i = 0; i < nodeChildren.Length; i++)
            {
                VisualMathNode visualMathNode = (VisualMathNode)nodeChildren[i];
                _operandHighlighters.Add(CreateNodeHighlighter(visualMathNode, HighlightCornerStyle.Rounded, visualMathNode));
            }
        }
        SetVisibilityOfEditorSymbols(Visibility.Visible);
        if (OperatorPartIndex.HasValue)
        {
            System.Windows.Media.Color value = WpfUtils.GdiToWpf(GetCaretColor());
            for (int j = 0; j < _operatorParts.Length; j++)
            {
                _operatorParts[j].MathFormat.FontColor = value;
                if (j == OperatorPartIndex)
                {
                    _operatorParts[j].Opacity = 1.0;
                }
                else
                {
                    _operatorParts[j].Opacity = CaretOpacity;
                }
            }
        }
        else
        {
            SetVisibilityOfEditorSymbols(Visibility.Hidden);
        }
        if (IsCaretBlinkEnabled())
        {
            UpdateCaretBlinking();
        }
    }

    private void RemoveHighlighters()
    {
        foreach (RubberBandHighlighter operandHighlighter in _operandHighlighters)
        {
            operandHighlighter.Remove();
        }
        _operandHighlighters.Clear();
    }

    protected override void UpdateCaretBlinking()
    {
        if (OperatorPartIndex.HasValue)
        {
            int period = 0;
            if (Blink)
            {
                period = GetCaretBlinkPeriod();
            }
            WpfUtils.Blink(_operatorParts[OperatorPartIndex.Value], period);
        }
    }

    private void SetVisibilityOfEditorSymbols(Visibility visibility)
    {
        EditNode.SetVisibilityOfOperatorSymbol((visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible);
        for (int i = 0; i < _operatorParts.Length; i++)
        {
            _operatorParts[i].Visibility = visibility;
        }
    }
}
