#nullable disable
using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Core;

public partial interface IUIElementFacade
{
    bool SetFocus(object obj);
    void Measure(object obj, Size size);
    void Arrange(object obj, Rect finalRect);
    Size GetDesiredSize(object obj);
    Rect GetDescendantBounds(object obj);
    bool IsItemMeasureValid(object obj);
    bool IsItemArrangeValid(object obj);
    void SetRenderTransform(object obj, Transform transform);
    void BlockUntilLoaded();
    void BlockUntilInput();
    void ExecuteOnLayoutUpdated(object control, Action action);
}
