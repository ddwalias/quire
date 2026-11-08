#nullable disable
#pragma warning disable
using System;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

public partial interface IOleBaseComponentCalculatableWrapper
{
    string LhsExpression { get; }
    string RhsExpression { get; }
    bool IsInputEquation { get; }
    bool IsOutputEquation { get; }
    bool IsValidEquation { get; }
    bool IsInvalidEquation { get; }
    bool IsUnknownEquation { get; }
    OleWrapperCustomErrorType CustomErrorType { get; }
    event Action<IOleBaseComponentCalculatableWrapper, EngineError> ErrorChanged;
    void SetCustomError(string errorMessage, EngineError originalError = default);
}
