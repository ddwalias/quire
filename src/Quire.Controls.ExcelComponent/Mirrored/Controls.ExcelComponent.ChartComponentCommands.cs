#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using Quire.Controls.ExcelComponent.View;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent;

public static partial class ChartComponentCommands
{
    private static readonly Type _typeOfChartComponentControl = typeof(ChartComponentControl);
    public static readonly RoutedUICommand InsertAxisExpression = new RoutedUICommandWithFunctionalityKey("Insert Axis Expression", "InsertAxisExpression", _typeOfChartComponentControl, EnumFunctionalityKey.ChartComponentInsertAxisExpressionCommand);
}
