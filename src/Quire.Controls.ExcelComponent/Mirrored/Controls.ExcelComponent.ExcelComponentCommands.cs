#nullable disable
#pragma warning disable

using System;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls.ExcelComponent;

public static partial class ExcelComponentCommands
{
    private static readonly Type _typeOfExcelComponentControl = typeof(ExcelComponentControl);
    public static readonly RoutedUICommand InsertExcelInputExpression = new RoutedUICommandWithFunctionalityKey("Insert Excel Input Expression", "InsertExcelInputExpression", _typeOfExcelComponentControl, EnumFunctionalityKey.IntegratedExcelTableInsertExcelInputExpressionCommand);
    public static readonly RoutedUICommand InsertExcelOutputExpression = new RoutedUICommandWithFunctionalityKey("Insert Excel Output Expression", "InsertExcelOutputExpression", _typeOfExcelComponentControl, EnumFunctionalityKey.IntegratedExcelTableInsertExcelOutputExpressionCommand);
}
