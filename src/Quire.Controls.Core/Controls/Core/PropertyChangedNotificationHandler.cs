#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Quire.Controls.Core;

public partial class PropertyChangedNotificationHandler
{
    private readonly Dictionary<string, Action> _propertyChangedNotificationHandlers = new Dictionary<string, Action>();

    public void AddPropertyChangedNotificationHandler(string propertyName, Action propertyChangedHandler)
    {
        _propertyChangedNotificationHandlers.Add(propertyName, propertyChangedHandler);
    }

    public void AttachPropertyChangedHandler(INotifyPropertyChanged notifyPropertyChanged)
    {
        notifyPropertyChanged.PropertyChanged += PropertyChanged;
    }

    public void DetachPropertyChangedHandler(INotifyPropertyChanged notifyPropertyChanged)
    {
        notifyPropertyChanged.PropertyChanged -= PropertyChanged;
    }

    private void PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        string propertyName = e.PropertyName;
        UpdateProperty(propertyName);
    }

    private void UpdateProperty(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            UpdateAllProperties();
        }
        else if (_propertyChangedNotificationHandlers.ContainsKey(propertyName))
        {
            _propertyChangedNotificationHandlers[propertyName]();
        }
    }

    public void UpdateAllProperties()
    {
        foreach (Action value in _propertyChangedNotificationHandlers.Values)
        {
            value();
        }
    }

    public PropertyChangedNotificationHandler()
    {
    }
}
