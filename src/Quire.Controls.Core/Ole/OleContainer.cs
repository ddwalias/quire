#nullable disable
using System;
using System.Windows;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.Controls.Core.Ole;

public abstract partial class OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>
{
    private static readonly Type _typeOfThis;

    private const DispatcherPriority OleForceClosePriority = (DispatcherPriority)4;

    public static readonly RoutedEvent ContentChangedEvent;

    public static readonly DependencyProperty IsInPrintingProperty;

    public static readonly DependencyProperty IsInCopyProperty;

    static OleContainer()
    {
        _typeOfThis = typeof(OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        WorksheetPropertiesManager.RegisterToWorksheetPropertyNotifications(_typeOfThis);
        IsInPrintingProperty = DocumentAttachedProperties.IsInPrintingProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnIsInPrintingPropertyChanged));
        IsInCopyProperty = DocumentAttachedProperties.IsInCopyProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(OnIsInCopyPropertyChanged));
        ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(_typeOfThis);
    }

    private static void OnIsInPrintingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>)d).OnIsInPrintingPropertyChanged(e);
    }

    private static void OnIsInCopyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((OleContainer<TViewModel, TModel, TIPublicModel, TPersistentData>)d).OnIsInCopyPropertyChanged(e);
    }
}
