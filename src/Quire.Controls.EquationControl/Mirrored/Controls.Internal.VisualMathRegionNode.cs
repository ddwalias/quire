using System;
using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.Internal;

internal partial class VisualMathRegionNode
{
    public Visibility LhsOfDefinitionVisibility
    {
        get => _lhsOfDefinitionVisibility;
        set
        {
            _lhsOfDefinitionVisibility = value;
            if (TryFindDefinitionAndUpdateIsLhsVisible())
            {
                InvalidateVisual();
            }
        }
    }

    public bool SymbolicEvalKeywordsVisibilityState
    {
        get => _isSymbolicEvalKeywordsShown;
        set => _isSymbolicEvalKeywordsShown = value;
    }

    private bool TryFindDefinitionAndUpdateIsLhsVisible()
    {
        VisualDefinitionNode definitionNode = GetDefinitionNode();
        if (definitionNode == null)
        {
            return false;
        }

        definitionNode.UpdateVisibility(_lhsOfDefinitionVisibility);
        return true;
    }

    public IVisualFactory VisualFactory
    {
        get => _visualFactory;
        set => _visualFactory = value;
    }

    private Thickness RootPadding => (Thickness)GetValue(RootPaddingProperty);

    public IMathFormat RootMathFormat => new StaticMathFormat(MathFormat);

    public IVisualMathNode RootNode
    {
        get => (IVisualMathNode)GetValue(RootNodeProperty);
        set => SetValue(RootNodeProperty, value);
    }

    public override double EffectiveBaselineY => 0.0;

    public override double EffectiveScriptBaselineY => 0.0;

    public bool IsLogicalFocusWithin { get; set; }

    public bool IsLogicalFocusWithinScriptableControl => _isLogicalFocusWithinScriptableControl;

    public NodeMessageAdorner VisibleWarningMessage
    {
        get => _visibleWarningMessage;
        private set
        {
            _visibleWarningMessage = value;
            VisibleWarningMessageChanged?.Invoke();
        }
    }

    public Point Location
    {
        get => (Point)GetValue(LocationProperty);
        set => SetValue(LocationProperty, value);
    }

    internal RubberBandHighlighter ErrorHighlighter
    {
        get => _highlight;
        set
        {
            if (_highlight != null)
            {
                _highlight.Remove();
            }

            _highlight = value;
        }
    }

    internal NodeMessageAdorner ErrorMessage
    {
        get => _message;
        set
        {
            if (_message != null)
            {
                _message.Remove();
            }

            _message = value;
        }
    }

    public WarningsTypesToShow WarningDisplayFilter => AttachedProperties.GetWarningDisplayFilter(this);

    internal IList<ValueTuple<RubberBandHighlighter, NodeMessageAdorner>> WarningTuples { get; set; }

    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners
    {
        get
        {
            List<NodeMessageAdorner> adorners = new List<NodeMessageAdorner>();
            if (ErrorMessage != null)
            {
                adorners.Add(ErrorMessage);
            }

            return adorners;
        }
    }

    public bool Activating { get; set; }
}
