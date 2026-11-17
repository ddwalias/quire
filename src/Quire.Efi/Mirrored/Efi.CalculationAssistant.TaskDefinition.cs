#nullable disable
#pragma warning disable

namespace Quire.Efi.CalculationAssistant;

public partial class TaskDefinition
{
    public TaskAction taskAction { get; set; }

    public object inData { get; set; }

    public object outData { get; set; }
}
