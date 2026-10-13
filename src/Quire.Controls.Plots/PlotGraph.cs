using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots;

/// <summary>Draws XY or polar traces with axes, grid, legend and a hover read-out.</summary>
public sealed class PlotGraph : FrameworkElement
{
    public static readonly Color[] Palette =
    [
        Color.FromRgb(0x1F, 0x5F, 0xBF), Color.FromRgb(0xD9, 0x48, 0x1C), Color.FromRgb(0x2E, 0x8B, 0x3E),
        Color.FromRgb(0x8E, 0x3C, 0xB0), Color.FromRgb(0xC0, 0x8A, 0x00), Color.FromRgb(0x1A, 0x9A, 0xA0),
    ];

    private static readonly Typeface LabelFace = new("Segoe UI");
    private const double LeftMargin = 56, BottomMargin = 26, TopMargin = 10, RightMargin = 14;

    private IReadOnlyList<TraceData> _traces = [];
    private IReadOnlyList<string> _labels = [];
    private Point? _hover;

    public PlotKind Kind { get; set; } = PlotKind.XY;
    public double? XMin { get; set; }
    public double? XMax { get; set; }
    public double? YMin { get; set; }
    public double? YMax { get; set; }
    public ResultFormat Format { get; set; } = ResultFormat.Default;

    public PlotGraph()
    {
        MouseMove += (_, e) => { _hover = e.GetPosition(this); InvalidateVisual(); };
        MouseLeave += (_, _) => { _hover = null; InvalidateVisual(); };
        ClipToBounds = true;
    }

    public void SetTraces(IReadOnlyList<TraceData> traces, IReadOnlyList<string> labels)
    {
        _traces = traces;
        _labels = labels;
        InvalidateVisual();
    }

    private double Dpi => VisualTreeHelper.GetDpi(this).PixelsPerDip;

    private FormattedText Label(string s, double size = 11, Brush? brush = null) =>
        new(s, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, LabelFace, size, brush ?? Brushes.DimGray, Dpi);

    protected override void OnRender(DrawingContext dc)
    {
        var bounds = new Rect(RenderSize);
        dc.DrawRectangle(Brushes.White, null, bounds);
        if (Kind == PlotKind.Polar) RenderPolar(dc);
        else RenderXY(dc);
    }

    // ---------------------------------------------------------------- XY

    private void RenderXY(DrawingContext dc)
    {
        var plot = new Rect(LeftMargin, TopMargin, Math.Max(10, ActualWidth - LeftMargin - RightMargin),
            Math.Max(10, ActualHeight - TopMargin - BottomMargin));
        var (x0, x1) = Range(_traces.SelectMany(t => t.X), XMin, XMax);
        var (y0, y1) = Range(_traces.SelectMany(t => t.Y), YMin, YMax);

        double Sx(double x) => plot.Left + (x - x0) / (x1 - x0) * plot.Width;
        double Sy(double y) => plot.Bottom - (y - y0) / (y1 - y0) * plot.Height;

        var grid = new Pen(new SolidColorBrush(Color.FromRgb(0xE4, 0xE8, 0xEE)), 1);
        var axis = new Pen(Brushes.Gray, 1);
        foreach (var t in Ticks(x0, x1))
        {
            double sx = Sx(t);
            dc.DrawLine(grid, new Point(sx, plot.Top), new Point(sx, plot.Bottom));
            var l = Label(Tick(t));
            dc.DrawText(l, new Point(sx - l.Width / 2, plot.Bottom + 4));
        }
        foreach (var t in Ticks(y0, y1))
        {
            double sy = Sy(t);
            dc.DrawLine(grid, new Point(plot.Left, sy), new Point(plot.Right, sy));
            var l = Label(Tick(t));
            dc.DrawText(l, new Point(plot.Left - l.Width - 5, sy - l.Height / 2));
        }
        dc.DrawRectangle(null, axis, plot);
        if (y0 < 0 && y1 > 0) dc.DrawLine(axis, new Point(plot.Left, Sy(0)), new Point(plot.Right, Sy(0)));
        if (x0 < 0 && x1 > 0) dc.DrawLine(axis, new Point(Sx(0), plot.Top), new Point(Sx(0), plot.Bottom));

        dc.PushClip(new RectangleGeometry(plot));
        for (int i = 0; i < _traces.Count; i++)
            DrawPolyline(dc, _traces[i].X.Zip(_traces[i].Y, (x, y) => (Sx(x), Sy(y))), i);
        dc.Pop();

        DrawLegend(dc, plot);
        DrawHover(dc, plot, Sx, Sy, (x, y) => (x, y));
    }

    // ---------------------------------------------------------------- polar

    private void RenderPolar(DrawingContext dc)
    {
        double size = Math.Min(ActualWidth - 20, ActualHeight - 20);
        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        double rMax = YMax ?? _traces.SelectMany(t => t.Y).Where(double.IsFinite).Select(Math.Abs).DefaultIfEmpty(1).Max();
        if (rMax <= 0) rMax = 1;
        double scale = size / 2 / rMax;
        var grid = new Pen(new SolidColorBrush(Color.FromRgb(0xE0, 0xE4, 0xEA)), 1);
        foreach (var r in Ticks(0, rMax).Where(r => r > 0))
        {
            dc.DrawEllipse(null, grid, center, r * scale, r * scale);
            var l = Label(Tick(r));
            dc.DrawText(l, new Point(center.X + r * scale + 2, center.Y - l.Height));
        }
        for (int deg = 0; deg < 360; deg += 30)
        {
            double a = deg * Math.PI / 180;
            var end = new Point(center.X + Math.Cos(a) * rMax * scale, center.Y - Math.Sin(a) * rMax * scale);
            dc.DrawLine(grid, center, end);
            var l = Label(deg + "°");
            dc.DrawText(l, new Point(end.X + Math.Cos(a) * 10 - l.Width / 2, end.Y - Math.Sin(a) * 10 - l.Height / 2));
        }
        for (int i = 0; i < _traces.Count; i++)
            DrawPolyline(dc, _traces[i].X.Zip(_traces[i].Y, (th, r) => (center.X + r * Math.Cos(th) * scale, center.Y - r * Math.Sin(th) * scale)), i);
        DrawLegend(dc, new Rect(8, 8, ActualWidth - 16, ActualHeight - 16));
    }

    // ---------------------------------------------------------------- helpers

    private static void DrawPolyline(DrawingContext dc, IEnumerable<(double X, double Y)> points, int index)
    {
        var pen = new Pen(new SolidColorBrush(Palette[index % Palette.Length]), 1.6) { LineJoin = PenLineJoin.Round };
        var g = new StreamGeometry();
        int count = 0;
        using (var c = g.Open())
        {
            bool open = false;
            foreach (var (x, y) in points)
            {
                if (!double.IsFinite(x) || !double.IsFinite(y) || Math.Abs(x) > 1e7 || Math.Abs(y) > 1e7)
                {
                    open = false;
                    continue;
                }
                var p = new Point(x, y);
                if (!open) { c.BeginFigure(p, false, false); open = true; }
                else c.LineTo(p, true, true);
                count++;
            }
        }
        g.Freeze();
        dc.DrawGeometry(null, pen, g);
        if (count <= 40)
            foreach (var (x, y) in points.Where(p => double.IsFinite(p.X) && double.IsFinite(p.Y)))
                dc.DrawEllipse(pen.Brush, null, new Point(x, y), 2.2, 2.2);
    }

    private void DrawLegend(DrawingContext dc, Rect plot)
    {
        if (_traces.Count < 2) return;
        double y = plot.Top + 4;
        for (int i = 0; i < _traces.Count; i++)
        {
            var text = Label(i < _labels.Count ? _labels[i] : _traces[i].Label, 11, Brushes.Black);
            double x = plot.Right - text.Width - 26;
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(0xD0, 0xFF, 0xFF, 0xFF)), null, new Rect(x - 4, y - 1, text.Width + 30, text.Height + 2));
            dc.DrawLine(new Pen(new SolidColorBrush(Palette[i % Palette.Length]), 2), new Point(x, y + text.Height / 2), new Point(x + 16, y + text.Height / 2));
            dc.DrawText(text, new Point(x + 20, y));
            y += text.Height + 2;
        }
    }

    private void DrawHover(DrawingContext dc, Rect plot, Func<double, double> sx, Func<double, double> sy, Func<double, double, (double, double)> _)
    {
        if (_hover is not { } h || !plot.Contains(h) || _traces.Count == 0) return;
        double best = double.MaxValue;
        (double X, double Y, int T) hit = default;
        for (int t = 0; t < _traces.Count; t++)
            for (int i = 0; i < _traces[t].X.Length; i++)
            {
                double px = sx(_traces[t].X[i]), py = sy(_traces[t].Y[i]);
                if (!double.IsFinite(px) || !double.IsFinite(py)) continue;
                double d = (px - h.X) * (px - h.X) + (py - h.Y) * (py - h.Y);
                if (d < best) { best = d; hit = (_traces[t].X[i], _traces[t].Y[i], t); }
            }
        if (best > 400) return;
        var p = new Point(sx(hit.X), sy(hit.Y));
        dc.DrawEllipse(null, new Pen(new SolidColorBrush(Palette[hit.T % Palette.Length]), 1.5), p, 4, 4);
        var label = Label($"({Tick(hit.X, precise: true)}, {Tick(hit.Y, precise: true)})", 11, Brushes.Black);
        var box = new Rect(Math.Min(p.X + 8, plot.Right - label.Width - 6), Math.Max(p.Y - label.Height - 8, plot.Top), label.Width + 6, label.Height + 2);
        dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(0xEE, 0xFF, 0xFF, 0xE8)), new Pen(Brushes.Gray, 0.5), box);
        dc.DrawText(label, new Point(box.X + 3, box.Y + 1));
    }

    private string Tick(double v, bool precise = false)
    {
        var f = precise ? Format : Format with { Decimals = Math.Min(Format.Decimals, 3) };
        return ResultFormatter.NumberText(Math.Abs(v) < 1e-12 ? 0 : v, f);
    }

    public static (double Lo, double Hi) Range(IEnumerable<double> values, double? min, double? max)
    {
        var finite = values.Where(v => double.IsFinite(v)).ToArray();
        double lo = min ?? (finite.Length > 0 ? finite.Min() : 0);
        double hi = max ?? (finite.Length > 0 ? finite.Max() : 1);
        if (hi <= lo)
        {
            double pad = Math.Abs(lo) > 0 ? Math.Abs(lo) * 0.1 : 1;
            lo -= pad; hi += pad;
        }
        else if (min == null || max == null)
        {
            // expand to nice tick boundaries
            double step = NiceStep(hi - lo);
            if (min == null) lo = Math.Floor(lo / step) * step;
            if (max == null) hi = Math.Ceiling(hi / step) * step;
        }
        return (lo, hi);
    }

    public static double NiceStep(double range, int target = 6)
    {
        double raw = range / target;
        double mag = Math.Pow(10, Math.Floor(Math.Log10(raw)));
        double n = raw / mag;
        return (n < 1.5 ? 1 : n < 3 ? 2 : n < 7 ? 5 : 10) * mag;
    }

    public static IEnumerable<double> Ticks(double lo, double hi)
    {
        if (!(hi > lo) || !double.IsFinite(lo) || !double.IsFinite(hi)) yield break;
        double step = NiceStep(hi - lo);
        for (double t = Math.Ceiling(lo / step - 1e-9) * step; t <= hi + step * 1e-9; t += step) yield return t;
    }
}
