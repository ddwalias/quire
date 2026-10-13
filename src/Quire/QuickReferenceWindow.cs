using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.View;

namespace Quire;

/// <summary>Keyboard syntax reference: what to type and how it renders.</summary>
internal sealed class QuickReferenceWindow : Window
{
    private static readonly (string Type, string Meaning)[] Rows =
    [
        ("x := 5 m", "Define x (type : for :=)"),
        ("k ≡ 9.81", "Global definition, visible everywhere (type ~)"),
        ("x =", "Evaluate"),
        ("v = km/hr", "Evaluate and show in other units"),
        ("y := 2 x =", "Define and show the value"),
        ("f(x, y) := x^2 + y", "Function"),
        ("x := 0, 0.5 .. 10", "Range variable (first, second .. last)"),
        ("v[i] := i^2", "Vector element; i may be a range"),
        ("M := [1, 2; 3, 4]", "Matrix; ; separates rows"),
        ("M[1, 0] =", "Element (ORIGIN is 0 unless redefined)"),
        ("Mᵀ  M^-1  |M|", "Transpose, inverse, determinant"),
        ("sum(k^2, k, 1, 10) =", "Summation; prod(…) for products"),
        ("integral(x^2, x, 0, 1) =", "Definite integral (∞ allowed)"),
        ("deriv(sin(x), x) =", "Derivative at the current x"),
        ("root(x^2 - 2, x) =", "Root from the current x, or root(f, x, a, b)"),
        ("x^2 + y^2 == 25", "Comparison / constraint (Ctrl+= types ==)"),
        ("if(x > 0, x, 0)", "Conditional"),
        ("20 °C", "Absolute temperature"),
        ("x_max", "Literal subscript"),
        ("`ft·lbf`", "Names with special characters"),
    ];

    public QuickReferenceWindow()
    {
        Title = "Quire — Quick reference";
        Width = 720;
        Height = 760;
        Background = Brushes.White;
        var grid = new Grid { Margin = new Thickness(16) };
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(220) });
        grid.ColumnDefinitions.Add(new ColumnDefinition());
        int r = 0;
        void Cell(UIElement e, int c)
        {
            Grid.SetRow(e, r);
            Grid.SetColumn(e, c);
            grid.Children.Add(e);
        }
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Cell(new TextBlock { Text = "Type", FontWeight = FontWeights.Bold }, 0);
        Cell(new TextBlock { Text = "Shows as", FontWeight = FontWeights.Bold }, 1);
        Cell(new TextBlock { Text = "Meaning", FontWeight = FontWeights.Bold }, 2);
        foreach (var (type, meaning) in Rows)
        {
            r++;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Cell(new TextBlock { Text = type, FontFamily = new FontFamily("Cascadia Mono, Consolas"), VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 4, 8, 4) }, 0);
            var parsed = LinearParser.TryParseStatement(type.Split("  ")[0], out _);
            if (parsed != null) Cell(new MathView { Statement = parsed, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left }, 1);
            Cell(new TextBlock { Text = meaning, TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, Foreground = Brushes.DimGray }, 2);
        }
        Content = new ScrollViewer { Content = grid, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
    }
}
