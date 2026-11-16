using System;

namespace Quire.Controls.Plots;

public partial interface IEquationProxy
{
    string Rl { get; }
    bool IsCommitted { get; set; }
    event Action<IEquationProxy> RequestEvaluation;
}
