#nullable disable
#pragma warning disable
using System;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IExcelComponentCalculatableWrapper
{
    ExcelComponentEquationType ExcelComponentEquationType { get; }
    event Action<ICalculatable, ExcelComponentEquationType, ExcelComponentEquationType> ExcelComponentEquationTypeChanged;
}
