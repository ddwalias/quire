#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Quire.Controls.Core;

public abstract partial class ViewModelBaseWithDataErrors : ViewModelBase, INotifyDataErrorInfo
{
    protected readonly ResourceDictionary _errorsDictionary;

    private readonly Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

    private bool _hasErrors;

    public bool HasErrors
    {
        get
        {
            return _hasErrors;
        }
        private set
        {
            if (_hasErrors == value)
            {
                return;
            }
            _hasErrors = value;
            OnPropertyChanged("HasErrors");
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    protected ViewModelBaseWithDataErrors(ResourceDictionary errorsDictionary)
    {
        _errorsDictionary = errorsDictionary;
    }

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
        {
            return null;
        }
        return _errors[propertyName];
    }

    protected void SetError(string propertyName, object errorKey, params object[] formatMessageArguments)
    {
        if (DebugTools.AssertNotNull(errorKey, null).Failed)
        {
            return;
        }

        if (!_errors.ContainsKey(propertyName))
        {
            _errors[propertyName] = new List<string>();
        }
        string format = (DebugTools.DevAssert(_errorsDictionary.Contains(errorKey), null).Passed ? Convert.ToString(_errorsDictionary[errorKey]) : string.Empty);
        _errors[propertyName].Add(string.Format(format, formatMessageArguments));
        HasErrors = true;
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void RemoveAllErrorsForProperty(string propertyName)
    {
        if (_errors.ContainsKey(propertyName))
        {
            _errors.Remove(propertyName);
            HasErrors = _errors.Count > 0;
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
