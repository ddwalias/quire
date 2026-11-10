using System;
using System.ComponentModel;
using Quire.Calculation.Core;

namespace Quire.Controls.ExcelComponent.Model;

internal abstract partial class OleBaseComponentCalculatableWrapper
{
    protected Action EquationChangedHandler { get; set; }

    public string LhsExpression { get; protected set; }

    public string RhsExpression { get; protected set; }

    public EngineError Error
    {
        get
        {
            return _error;
        }
        private set
        {
            WrappedCalculatable.SetError(value);
            if (_error == value)
            {
                return;
            }
            _error = value;
            if (ErrorChanged != null)
            {
                ErrorChanged(this, value);
            }
        }
    }

    public int ErrorCount
    {
        get
        {
            return Error != null ? 1 : 0;
        }
    }

    protected void ClearAllErrors()
    {
        Error = null;
    }

    public bool ContainsError(EngineError error)
    {
        return error.ErrorSource == this;
    }

    private void WrappedCalculatablePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        FirePropertyChanged(e);
    }
}
