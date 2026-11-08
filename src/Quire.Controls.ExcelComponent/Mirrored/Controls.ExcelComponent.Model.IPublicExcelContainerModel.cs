#nullable disable
#pragma warning disable
using System;
using Quire.Microsoft.Office.Interop.Excel;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IPublicExcelContainerModel
{
    QueryAttachedInputsOrOutputsHandler QueryAttachedInputsOrOutputs { get; set; }
    bool NeedRecalculateOnClose { get; }
    event Action RequestEvaluation;
    void SetValue(string place, object value, bool forceRun);
    global::Quire.Microsoft.Office.Interop.Excel.Range GetRange(string rangeAddress, bool forceRun);
    bool HasAttachedInputsOrOutputs(bool checkOutputOnly);
}
