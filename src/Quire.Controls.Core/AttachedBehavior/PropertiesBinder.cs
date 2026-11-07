#nullable disable
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Quire;
using Quire.Core;
using Quire.Mirror;
using Quire.Undo;

namespace Quire.Controls.Core.AttachedBehavior;

public partial class PropertiesBinder : DependencyObject, IAttachedBehavior, IOneTimePropertyOwner
{
    private static readonly Type _typeOfThis = typeof(PropertiesBinder);

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public static readonly DependencyProperty SourceValueConnectorProperty = DependencyProperty.Register("SourceValueConnector", typeof(object), _typeOfThis, new PropertyMetadata(null, (sender, args) => sender.SetValue(TargetValueConnectorProperty, args.NewValue)));

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public static readonly DependencyProperty TargetValueConnectorProperty = DependencyProperty.Register("TargetValueConnector", typeof(object), _typeOfThis, new PropertyMetadata(null, OnValueChanged));

    private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        object value = sender.GetValue(SourceValueConnectorProperty);
        if (object.Equals(value, args.OldValue) && !object.Equals(value, args.NewValue))
        {
            sender.SetValue(TargetValueConnectorProperty, value);
            if (BindingOperations.GetBindingExpression(sender, TargetValueConnectorProperty) != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() =>
                {
                    BindingExpression bindingExpression = BindingOperations.GetBindingExpression(sender, TargetValueConnectorProperty);
                    if (bindingExpression != null)
                    {
                        bindingExpression.UpdateSource();
                    }
                    else
                    {
                        DebugTools.DevAssert(false, "BindingExpression is null for " + TargetValueConnectorProperty.Name);
                    }
                }), DispatcherPriority.DataBind, Array.Empty<object>());
            }
        }
    }

    private string _pathToSource;

    private string _pathToTarget;

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public object SourceValueConnector { get => base.GetValue(SourceValueConnectorProperty); set => base.SetValue(SourceValueConnectorProperty, value); }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public object TargetValueConnector { get => base.GetValue(TargetValueConnectorProperty); set => base.SetValue(TargetValueConnectorProperty, value); }

    public string PathToSource
    {
        get => _pathToSource;
        set => this.OneTimeSetExt(ref _pathToSource, value, "PathToSource");
    }

    public string PathToTarget
    {
        get => _pathToTarget;
        set => this.OneTimeSetExt(ref _pathToTarget, value, "PathToTarget");
    }

    [MirrorVerified("PtcControlsCoreAttachedBehaviorPropertiesBinderMechanicalDifferentialTests")]
    public bool BlockUndoWhenBindingToTarget { get; set; }

    public PropertiesBinder()
    {
        BlockUndoWhenBindingToTarget = true;
    }

    public void Attach(FrameworkElement sourceElem)
    {
        if (!string.IsNullOrEmpty(PathToSource))
        {
            BindingOperations.SetBinding(this, SourceValueConnectorProperty, new Binding(PathToSource)
            {
                Source = sourceElem,
                Mode = BindingMode.OneWay
            });
        }
        if (!string.IsNullOrEmpty(PathToTarget))
        {
            if (BlockUndoWhenBindingToTarget)
            {
                using (new CommandScope(new CompoundCommand()))
                {
                    CreateTargetBinding(sourceElem);
                }
            }
            else
            {
                CreateTargetBinding(sourceElem);
            }
        }
    }

    private void CreateTargetBinding(FrameworkElement sourceElem)
    {
        bool flag = object.Equals(TargetValueConnector, SourceValueConnector);
        BindingOperations.SetBinding(this, TargetValueConnectorProperty, new Binding(PathToTarget)
        {
            Source = sourceElem,
            Mode = BindingMode.TwoWay
        });
        if (flag)
        {
            DebugTools.DevAssert(object.Equals(TargetValueConnector, SourceValueConnector), "Current value was overwritten for " + TargetValueConnectorProperty.Name);
        }
    }

    public void Detach(FrameworkElement element)
    {
        BindingOperations.ClearBinding(this, SourceValueConnectorProperty);
        BindingOperations.ClearBinding(this, TargetValueConnectorProperty);
    }
}
