#nullable disable
using System;

namespace Quire.Controls.Core.Serialization;

public partial class MC14ConversionLossException : Exception
{
    public string[] Messages { get; private set; }

    public MC14ConversionLossException(string message, Exception innerEx)
        : base(message, innerEx)
    {
        Messages = new string[1] { message };
    }

    public MC14ConversionLossException(string[] messages)
    {
        Messages = messages ?? new string[0];
    }
}
