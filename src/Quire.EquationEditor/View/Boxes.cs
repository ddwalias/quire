using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Quire.EquationEditor.View;

/// <summary>
/// Layout box for 2D math. Every box has a width and extends <see cref="Ascent"/> above and
/// <see cref="Descent"/> below its baseline; <see cref="Draw"/> receives the left baseline point.
/// </summary>
public abstract class Box
{
    public double Width { get; protected set; }
    public double Ascent { get; protected set; }
    public double Descent { get; protected set; }
    public double Height => Ascent + Descent;

    public abstract void Draw(DrawingContext dc, double x, double baseline);
}

public sealed class SpaceBox : Box
{
    public SpaceBox(double width) => Width = width;
    public override void Draw(DrawingContext dc, double x, double baseline) { }
}

public sealed class GlyphBox : Box
{
    private readonly FormattedText _text;
    private readonly Brush? _highlight;

    public GlyphBox(string text, Typeface face, double size, Brush brush, double pixelsPerDip, Brush? highlight = null)
    {
        _text = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, face, size, brush, pixelsPerDip);
        _highlight = highlight;
        Width = _text.WidthIncludingTrailingWhitespace;
        // a tighter box than the line height so fences and fractions hug the glyphs
        Ascent = Math.Min(_text.Baseline, size * 0.82);
        Descent = Math.Min(Math.Max(0, _text.Height - _text.Baseline), size * 0.26);
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        if (_highlight != null) dc.DrawRectangle(_highlight, null, new Rect(x - 1, baseline - Ascent, Width + 2, Height));
        dc.DrawText(_text, new Point(x, baseline - _text.Baseline));
    }
}

public sealed class HBox : Box
{
    private readonly Box[] _items;

    public HBox(IEnumerable<Box> items)
    {
        _items = items.ToArray();
        Width = _items.Sum(b => b.Width);
        Ascent = _items.Length == 0 ? 0 : _items.Max(b => b.Ascent);
        Descent = _items.Length == 0 ? 0 : _items.Max(b => b.Descent);
    }

    public HBox(params Box[] items) : this((IEnumerable<Box>)items) { }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        foreach (var b in _items)
        {
            b.Draw(dc, x, baseline);
            x += b.Width;
        }
    }
}

/// <summary>Stacked fraction with its bar on the math axis.</summary>
public sealed class FractionBox : Box
{
    private readonly Box _num, _den;
    private readonly double _axis, _gap, _pad, _thickness;
    private readonly Pen _pen;

    public FractionBox(Box num, Box den, double axis, double thickness, Brush brush)
    {
        _num = num; _den = den; _axis = axis; _thickness = thickness;
        _gap = thickness * 2.5;
        _pad = thickness * 2;
        _pen = new Pen(brush, thickness);
        Width = Math.Max(num.Width, den.Width) + 2 * _pad;
        Ascent = axis + _gap + num.Height;
        Descent = Math.Max(0, den.Height + _gap - axis);
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        double bar = baseline - _axis;
        dc.DrawLine(_pen, new Point(x + _pad / 2, bar), new Point(x + Width - _pad / 2, bar));
        _num.Draw(dc, x + (Width - _num.Width) / 2, bar - _gap - _num.Descent);
        _den.Draw(dc, x + (Width - _den.Width) / 2, bar + _gap + _den.Ascent);
    }
}

/// <summary>Base with optional superscript and subscript.</summary>
public sealed class ScriptBox : Box
{
    private readonly Box _base;
    private readonly Box? _sup, _sub;
    private readonly double _supShift, _subShift;

    public ScriptBox(Box @base, Box? sup, Box? sub, double emSize)
    {
        _base = @base; _sup = sup; _sub = sub;
        _supShift = sup == null ? 0 : Math.Max(@base.Ascent - sup.Ascent * 0.55, emSize * 0.38);
        _subShift = sub == null ? 0 : Math.Max(emSize * 0.22, sub.Ascent - emSize * 0.45);
        Width = @base.Width + Math.Max(sup?.Width ?? 0, sub?.Width ?? 0) + (sup != null || sub != null ? emSize * 0.05 : 0);
        Ascent = Math.Max(@base.Ascent, sup == null ? 0 : _supShift + sup.Ascent);
        Descent = Math.Max(@base.Descent, sub == null ? 0 : _subShift + sub.Descent);
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        _base.Draw(dc, x, baseline);
        double sx = x + _base.Width + 1;
        _sup?.Draw(dc, sx, baseline - _supShift);
        _sub?.Draw(dc, sx, baseline + _subShift);
    }
}

/// <summary>Radical sign over content, with an optional root index.</summary>
public sealed class RadicalBox : Box
{
    private readonly Box _content;
    private readonly Box? _index;
    private readonly Pen _pen;
    private readonly double _signWidth, _gap, _indexWidth;

    public RadicalBox(Box content, Box? index, double emSize, Brush brush)
    {
        _content = content; _index = index;
        _pen = new Pen(brush, Math.Max(1, emSize / 16)) { LineJoin = PenLineJoin.Round };
        _gap = emSize * 0.15;
        _signWidth = emSize * 0.55;
        _indexWidth = index == null ? 0 : Math.Max(0, index.Width - _signWidth * 0.4);
        Width = _indexWidth + _signWidth + content.Width + emSize * 0.15;
        Ascent = content.Ascent + _gap + _pen.Thickness;
        Descent = content.Descent;
        if (index != null) Ascent = Math.Max(Ascent, Height * 0.55 - Descent + index.Height);
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        double top = baseline - _content.Ascent - _gap, bottom = baseline + _content.Descent;
        double x0 = x + _indexWidth;
        double h = bottom - top;
        var g = new StreamGeometry();
        using (var c = g.Open())
        {
            c.BeginFigure(new Point(x0, top + h * 0.6), false, false);
            c.LineTo(new Point(x0 + _signWidth * 0.25, top + h * 0.5), true, true);
            c.LineTo(new Point(x0 + _signWidth * 0.55, bottom), true, true);
            c.LineTo(new Point(x0 + _signWidth, top), true, true);
            c.LineTo(new Point(x + Width, top), true, true);
        }
        g.Freeze();
        dc.DrawGeometry(null, _pen, g);
        _index?.Draw(dc, x, top + h * 0.45 - _index.Descent);
        _content.Draw(dc, x0 + _signWidth + 2, baseline);
    }
}

/// <summary>Content between scalable delimiters: ( ), [ ], | | or none.</summary>
public sealed class FenceBox : Box
{
    private readonly Box _content;
    private readonly char _open, _close;
    private readonly Pen _pen;
    private readonly double _fence;

    public FenceBox(Box content, char open, char close, double emSize, Brush brush)
    {
        _content = content; _open = open; _close = close;
        _pen = new Pen(brush, Math.Max(1, emSize / 15)) { StartLineCap = PenLineCap.Round, EndLineCap = PenLineCap.Round };
        _fence = emSize * (content.Height > emSize * 1.6 ? 0.4 : 0.32);
        double extra = emSize * 0.08;
        Width = content.Width + 2 * _fence;
        Ascent = content.Ascent + extra;
        Descent = content.Descent + extra;
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        double top = baseline - Ascent, bottom = baseline + Descent;
        DrawDelimiter(dc, _open, x + _fence * 0.2, x + _fence * 0.8, top, bottom, left: true);
        _content.Draw(dc, x + _fence, baseline);
        double rx = x + _fence + _content.Width;
        DrawDelimiter(dc, _close, rx + _fence * 0.2, rx + _fence * 0.8, top, bottom, left: false);
    }

    private void DrawDelimiter(DrawingContext dc, char kind, double x0, double x1, double top, double bottom, bool left)
    {
        double outer = left ? x0 : x1, inner = left ? x1 : x0;
        switch (kind)
        {
            case '(' or ')':
            {
                var g = new StreamGeometry();
                using (var c = g.Open())
                {
                    c.BeginFigure(new Point(inner, top), false, false);
                    c.BezierTo(new Point(outer - (inner - outer) * 0.15, top + (bottom - top) * 0.25),
                        new Point(outer - (inner - outer) * 0.15, top + (bottom - top) * 0.75), new Point(inner, bottom), true, true);
                }
                g.Freeze();
                dc.DrawGeometry(null, _pen, g);
                break;
            }
            case '[' or ']':
                dc.DrawLine(_pen, new Point(inner, top), new Point(outer, top));
                dc.DrawLine(_pen, new Point(outer, top), new Point(outer, bottom));
                dc.DrawLine(_pen, new Point(outer, bottom), new Point(inner, bottom));
                break;
            case '|':
                dc.DrawLine(_pen, new Point((x0 + x1) / 2, top), new Point((x0 + x1) / 2, bottom));
                break;
        }
    }
}

/// <summary>Grid of cells, vertically centred on the math axis. Optionally framed as a table.</summary>
public sealed class GridBox : Box
{
    private readonly Box[,] _cells;
    private readonly double[] _colWidths, _rowAscents, _rowDescents;
    private readonly double _colGap, _rowGap;
    private readonly Pen? _tablePen;

    public GridBox(Box[,] cells, double emSize, double axis, Brush? tableBrush = null)
    {
        _cells = cells;
        int rows = cells.GetLength(0), cols = cells.GetLength(1);
        _colGap = emSize * 0.8;
        _rowGap = emSize * 0.25;
        _colWidths = Enumerable.Range(0, cols).Select(c => Enumerable.Range(0, rows).Max(r => cells[r, c].Width)).ToArray();
        _rowAscents = Enumerable.Range(0, rows).Select(r => Enumerable.Range(0, cols).Max(c => cells[r, c].Ascent)).ToArray();
        _rowDescents = Enumerable.Range(0, rows).Select(r => Enumerable.Range(0, cols).Max(c => cells[r, c].Descent)).ToArray();
        if (tableBrush != null)
        {
            _tablePen = new Pen(tableBrush, 1);
            _colGap = emSize * 1.2;
        }
        Width = _colWidths.Sum() + _colGap * Math.Max(0, cols - 1) + (_tablePen != null ? _colGap : 0);
        double h = _rowAscents.Sum() + _rowDescents.Sum() + _rowGap * Math.Max(0, rows - 1);
        Ascent = h / 2 + axis;
        Descent = h / 2 - axis;
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        double top = baseline - Ascent;
        double inset = _tablePen != null ? _colGap / 2 : 0;
        if (_tablePen != null)
        {
            dc.DrawRectangle(null, _tablePen, new Rect(x, top - 1, Width, Height + 2));
        }
        double y = top;
        for (int r = 0; r < _rowAscents.Length; r++)
        {
            double cx = x + inset;
            double rowBaseline = y + _rowAscents[r];
            for (int c = 0; c < _colWidths.Length; c++)
            {
                var cell = _cells[r, c];
                // right-align numbers in tables, centre in matrices
                double offset = _tablePen != null ? _colWidths[c] - cell.Width : (_colWidths[c] - cell.Width) / 2;
                cell.Draw(dc, cx + offset, rowBaseline);
                cx += _colWidths[c] + _colGap;
            }
            y += _rowAscents[r] + _rowDescents[r] + _rowGap;
            if (_tablePen != null && r < _rowAscents.Length - 1)
                dc.DrawLine(_tablePen, new Point(x, y - _rowGap / 2), new Point(x + Width, y - _rowGap / 2));
        }
    }
}

/// <summary>Large operator (Σ, ∏, ∫) with limits below/above (or to the right for ∫).</summary>
public sealed class BigOpBox : Box
{
    private readonly Box _symbol;
    private readonly Box? _lower, _upper;
    private readonly bool _sideLimits;
    private readonly double _gap;

    public BigOpBox(Box symbol, Box? lower, Box? upper, bool sideLimits, double emSize)
    {
        _symbol = symbol; _lower = lower; _upper = upper; _sideLimits = sideLimits;
        _gap = emSize * 0.1;
        if (sideLimits)
        {
            Width = symbol.Width + Math.Max(lower?.Width ?? 0, upper?.Width ?? 0) + _gap;
            Ascent = Math.Max(symbol.Ascent, upper == null ? 0 : symbol.Ascent * 0.8 + upper.Height);
            Descent = Math.Max(symbol.Descent, lower == null ? 0 : symbol.Descent * 0.6 + lower.Height);
        }
        else
        {
            Width = Math.Max(symbol.Width, Math.Max(lower?.Width ?? 0, upper?.Width ?? 0));
            Ascent = symbol.Ascent + (upper == null ? 0 : upper.Height + _gap);
            Descent = symbol.Descent + (lower == null ? 0 : lower.Height + _gap);
        }
    }

    public override void Draw(DrawingContext dc, double x, double baseline)
    {
        if (_sideLimits)
        {
            _symbol.Draw(dc, x, baseline);
            double lx = x + _symbol.Width + _gap;
            _upper?.Draw(dc, lx, baseline - Ascent + _upper.Ascent);
            _lower?.Draw(dc, lx, baseline + _symbol.Descent * 0.6 + _lower.Ascent);
            return;
        }
        _symbol.Draw(dc, x + (Width - _symbol.Width) / 2, baseline);
        _upper?.Draw(dc, x + (Width - _upper.Width) / 2, baseline - _symbol.Ascent - _gap - _upper.Descent);
        _lower?.Draw(dc, x + (Width - _lower.Width) / 2, baseline + _symbol.Descent + _gap + _lower.Ascent);
    }
}
