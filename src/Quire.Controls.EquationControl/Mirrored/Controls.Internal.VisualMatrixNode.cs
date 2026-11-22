using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.ControlAdorners;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;
using Quire.Matrices;
using Quire.Wpf;

namespace Quire.Controls.Internal;

internal partial class VisualMatrixNode
{
    internal MatrixResizeAdorner ResizeAdorner { get; set; }

    public System.Nullable<TableCoordinate> ErrorCell { get; set; }

    public TableSize OriginalSize { get; set; }

    private Visibility _headingsVisibility;
    private double _effectiveBaselineY;
    private MatrixNodeInfo _matrixInfo;
    private StackPanel _rowHeading;
    private StackPanel _columnHeading;
    private string[] _rowIndices;
    private string[] _columnIndices;
    private VerticalFence _leftBracket;
    private VerticalFence _rightBracket;
    private double _opacity = 1.0;
    private readonly VisualMatrixContents _matrixGrid;
    private double _rowHeadingTop;
    private double _columnHeadingLeft;
    private double _matrixTop;
    private double _matrixLeft;
    private double _matrixContentTop;
    private double _matrixContentLeft;
    private IVisualMathNode[] _children;

    public override double EffectiveBaselineY
    {
        get
        {
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override double EffectiveScriptBaselineY => 0.0;

    public override double TextBaselineY => _effectiveBaselineY;

    public int RowCount => _matrixGrid.Rows;

    public int ColumnCount
    {
        get
        {
            return _matrixGrid.Columns;
        }
        set
        {
            _matrixGrid.Columns = value;
        }
    }

    public UIElementCollection Cells => _matrixGrid.Children;

    public StackPanel RowHeading => _rowHeading;

    public StackPanel ColumnHeading => _columnHeading;

    public override INodeInfo NodeInfo
    {
        get
        {
            return base.NodeInfo;
        }
        set
        {
            base.NodeInfo = value;
            _matrixInfo = (MatrixNodeInfo)base.NodeInfo;
            _children = new IVisualMathNode[_matrixInfo.ViewportSize.Rows * _matrixInfo.ViewportSize.Columns];
            ColumnCount = _matrixInfo.ViewportSize.Columns;
            PopulateMatrixHeadings();
        }
    }

    public string[] MatrixHeadingRowIndices => _rowIndices;

    public string[] MatrixHeadingColumnIndices => _columnIndices;

    public IVisualMathNode ErrorNode
    {
        get
        {
            if (ErrorCell.HasValue)
            {
                if (ErrorCell.Value.Row >= _matrixInfo.ViewportOffset.Row + _matrixInfo.ViewportSize.Rows || ErrorCell.Value.Column >= _matrixInfo.ViewportOffset.Column + _matrixInfo.ViewportSize.Columns)
                {
                    return Cells[_matrixGrid.CellCount - 1] as IVisualMathNode;
                }
                if (ErrorCell.Value.Row < _matrixInfo.ViewportOffset.Row || ErrorCell.Value.Column < _matrixInfo.ViewportOffset.Column)
                {
                    return Cells[0] as IVisualMathNode;
                }
            }
            return null;
        }
    }

    public Visibility HeadingsVisibility
    {
        get
        {
            return _headingsVisibility;
        }
        set
        {
            _headingsVisibility = value;
            SetHeadingsCellsVisibility();
        }
    }

    public override IVisualMathNode this[int index]
    {
        get
        {
            return _children[index];
        }
        set
        {
            _children[index] = value;
        }
    }

    public override IVisualMathNode LeftFence => _leftBracket;

    public override IVisualMathNode RightFence => _rightBracket;

    public override Size DesiredSizeWithoutFences
    {
        get
        {
            return _matrixGrid.DesiredSize;
        }
    }

    public override bool Horizontal => true;

    public override void SetParensVisibility(Visibility visibility)
    {
        _opacity = ((visibility == Visibility.Visible) ? 1 : 0);
        _leftBracket.Opacity = _opacity;
        _rightBracket.Opacity = _opacity;
    }

    private void CreateBrackets()
    {
        if (_leftBracket != null)
        {
            Children.Remove(_leftBracket);
        }
        if (_rightBracket != null)
        {
            Children.Remove(_rightBracket);
        }
        _leftBracket = VerticalFence.CreateLeftBracket();
        _rightBracket = VerticalFence.CreateRightBracket();
        _leftBracket.Opacity = _opacity;
        _rightBracket.Opacity = _opacity;
        Children.Add(_leftBracket);
        Children.Add(_rightBracket);
    }

    private StackPanel CreateHeading(string panelName, ExtendedFont font)
    {
        StackPanel stackPanel = new StackPanel
        {
            Name = panelName,
            Orientation = Orientation.Vertical,
            Opacity = 0.6
        };
        TextElement.SetFontSize(stackPanel, font.FontSize - 2.0);
        TextElement.SetFontFamily(stackPanel, font.FontTypeface.FontFamily);
        return stackPanel;
    }

    protected override void OnMouseMove(MouseEventArgs args)
    {
        base.OnMouseMove(args);
        if (ResizeAdorner != null && DebugTools.DevAssert(ResizeAdorner.VisualMatrix == this, null).Passed)
        {
            return;
        }
        if (Mouse.LeftButton == MouseButtonState.Pressed)
        {
            return;
        }
        EquationControl equationControl = this.TryFindVisualAncestor<EquationControl>();
        if (equationControl != null)
        {
            equationControl.SetMatrixForResizeAdorner(this);
        }
    }

    private void LoadedHandler(object sender, RoutedEventArgs e)
    {
        Loaded -= LoadedHandler;
        EquationControl equationControl = this.TryFindVisualAncestor<EquationControl>();
        if (equationControl != null)
        {
            equationControl.UpdateMatrixForResizeAdorner(this);
        }
    }

    private string[] GetMatrixRowOrColumnIndices(StackPanel heading)
    {
        ArrayList arrayList = new ArrayList();
        if (heading.Children.Count > 0)
        {
            Regex regex = new Regex("\\d");
            foreach (UIElement child in heading.Children)
            {
                if (!(child is Label label))
                {
                    continue;
                }
                string[] array = label.Content.ToString().Split('\n');
                foreach (string text in array)
                {
                    if (regex.IsMatch(text))
                    {
                        arrayList.Add(text);
                    }
                    else
                    {
                        arrayList.Add('…'.ToString());
                    }
                }
            }
        }
        else
        {
            int num2 = ((heading.Name == "RowHeading") ? _matrixInfo.ViewportSize.Rows : _matrixInfo.ViewportSize.Columns);
            int origin = _matrixInfo.Origin;
            for (int num3 = 0; num3 < num2; num3++)
            {
                arrayList.Add((num3 + origin).ToString());
            }
        }
        return (string[])arrayList.ToArray(typeof(string));
    }

    public void SetHeadingsCellsVisibility()
    {
        foreach (UIElement child in RowHeading.Children)
        {
            child.Visibility = HeadingsVisibility;
        }
        foreach (UIElement child2 in ColumnHeading.Children)
        {
            child2.Visibility = HeadingsVisibility;
        }
    }

    public override void BuildChildren(IVisualMathNode[] children)
    {
        base.BuildChildren(children);
        ConstructMatrixGrid();
    }

    private IVisualMathNode CheckIfSpecialCellNeeded(TableCoordinate cellPosition, MatrixDisplayHelper displayHelper)
    {
        if (displayHelper.ShowDownRightEllipsis(cellPosition))
        {
            return CreateTextNode('⋱'.ToString());
        }
        if (displayHelper.ShowHorizontalEllipsis(cellPosition))
        {
            return CreateTextNode('…'.ToString());
        }
        if (displayHelper.ShowVerticalEllipsis(cellPosition))
        {
            return CreateTextNode('⋮'.ToString());
        }
        if (displayHelper.ShowEmptyCell(cellPosition))
        {
            return CreateTextNode(" ");
        }
        return null;
    }

    private int GetIndexOfNode(IVisualMathNode childNode)
    {
        if (!(childNode is VisualMathNode element))
        {
            return -1;
        }
        return _matrixGrid.Children.IndexOf(element);
    }

    private static bool IsResizeIndicator(IVisualTextNode text)
    {
        if (text != null)
        {
            if (!(text.Text == '…'.ToString()) && !(text.Text == '⋮'.ToString()) && !(text.Text == '⋰'.ToString()))
            {
                return text.Text == '⋱'.ToString();
            }
            return true;
        }
        return false;
    }

}
