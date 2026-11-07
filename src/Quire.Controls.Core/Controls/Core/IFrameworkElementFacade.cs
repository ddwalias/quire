#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;

namespace Quire.Controls.Core;

public partial interface IFrameworkElementFacade
{
    bool IsItemLoaded(object obj);
    void SetSize(object obj, Size size, DragMode dragMode = (DragMode)(0));
    Size GetSize(object obj);
    void SetHeight(object obj, double height);
    void SetWidth(object obj, double width);
    void SetMaxSize(object obj, Size maxSize);
    void ExecuteOnLoad(object control, Action action);
    void ExecuteOnUnload(object control, Action action);
    void ExecuteInBackground(Action action, bool asynch = false);
    object GetParent(object control);
    void SetHandleChangesEnabled(object item, bool value);
    void ClearHandleChangesEnabled(object item);
    void ClearMaxSize(object obj);
    void SetItemName(object item, string name);
    string GetItemName(object item);
    bool GetOverrideLabelColor(object equationVisualElement);
    IDictionary<IdLabels, ExtendedFont> GetLabeledIdFormatProperty(object equationVisualElement);
    void SetOverrideLabelColor(object item, bool overideLabelColor);
    void SetLabeledIdFormatProperty(object item, object labelIdFormatDictionary);
}
