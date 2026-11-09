#nullable disable
#pragma warning disable

using System;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

public partial class EngineErrorEventArgs
{
    private readonly EngineError _engineError;

    public EngineErrorEventArgs(EngineError engineError)
    {
        _engineError = engineError;
    }

    public EngineError EngineError => _engineError;
}
