#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

internal partial class MatrixLayoutManager
{
    public override object Clone()
    {
        return new MatrixLayoutManager(this);
    }

    protected override MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows)
    {
        // Release IL allocates these locals before maxWidths; declaring them first keeps the same slots.
        VisualMathNode node;
        LayoutMetrics metrics;
        VisualTextNode text;
        MaxMatrixWidth[] maxWidths = new MaxMatrixWidth[Columns];
        for (int column = 0; column < Columns; column++)
        {
            for (int row = 0; row < rows; row++)
            {
                int index = row * Columns + column;
                node = cellsContent[index] as VisualMathNode;
                if (node == null)
                {
                    continue;
                }

                double linearWidth;
                if (node is VisualBinaryOperatorNode)
                {
                    text = ExtractVisualTextWidth((node as VisualBinaryOperatorNode)[0] as VisualMathNode, out linearWidth);
                }
                else
                {
                    text = ExtractVisualTextWidth(node, out linearWidth);
                }

                metrics = cellsInfo[index];
                if (text != null)
                {
                    string integral = text.Text;
                    if (text.Text.Contains("."))
                    {
                        integral = text.Text.Substring(0, text.Text.IndexOf("."));
                    }

                    FormattedText formatted = new FormattedText(integral, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, node.Font.FontTypeface, node.Font.FontSize, Brushes.Black);
                    metrics.ItegralTextWidth = formatted.Width + linearWidth;
                    cellsInfo[index] = metrics;
                    maxWidths[column].MaxIntegral = Math.Max(maxWidths[column].MaxIntegral, metrics.ItegralTextWidth);
                    maxWidths[column].MaxFractional = Math.Max(maxWidths[column].MaxFractional, metrics.OverallSize.Width - metrics.ItegralTextWidth);
                }
                else
                {
                    maxWidths[column].MaxVisualNode = Math.Max(maxWidths[column].MaxVisualNode, metrics.OverallSize.Width);
                }
            }
        }

        return maxWidths;
    }
}
