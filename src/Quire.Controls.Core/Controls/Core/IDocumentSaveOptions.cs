#nullable disable
namespace Quire.Controls.Core;

public partial interface IDocumentSaveOptions
{
    bool TryGeneratingInputOutputStates { get; }
    bool IsDetachedCopy { get; }
    bool IsHandlingException { get; }
    bool WaitForCalculationForSaveAsApi { get; }
}
