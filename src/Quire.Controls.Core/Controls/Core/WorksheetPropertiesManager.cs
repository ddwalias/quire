#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire;
using Quire.Core;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class WorksheetPropertiesManager
{
    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static readonly Type _typeOfThis = typeof(WorksheetPropertiesManager);

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static readonly DependencyPropertyKey WorksheetPropertiesSourcePropertyKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "WorksheetPropertiesSource",
            typeof(IWorksheetPropertiesSource),
            _typeOfThis,
            new FrameworkPropertyMetadata(DummyWorksheetPropertiesSource.GetInstance()));

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void SetWorksheetPropertiesSource(DependencyObject dObj, IWorksheetPropertiesSource propertiesSource) => dObj.SetValue(WorksheetPropertiesSourcePropertyKey, propertiesSource);

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static IWorksheetPropertiesSource GetWorksheetPropertiesSource(DependencyObject dObj)
    {
        return dObj.GetValue(WorksheetPropertiesSourcePropertyKey.DependencyProperty) as IWorksheetPropertiesSource;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static readonly DependencyPropertyKey WorksheetPropertiesChangeInfoKey =
        DependencyProperty.RegisterAttachedReadOnly(
            "WorksheetPropertiesChangeInfo",
            typeof(WorksheetPropertiesChangeInfo),
            _typeOfThis,
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void SetWorksheetPropertiesChangeInfo(DependencyObject dObj, WorksheetPropertiesChangeInfo newInfo) => dObj.SetValue(WorksheetPropertiesChangeInfoKey, newInfo);

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static WorksheetPropertiesChangeInfo GetWorksheetPropertiesChangeInfo(DependencyObject dObj)
    {
        return dObj.GetValue(WorksheetPropertiesChangeInfoKey.DependencyProperty) as WorksheetPropertiesChangeInfo;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static void SetOrRemoveWorksheetPropertiesSource<TSource>(TSource propertiesSourceElement, bool doSet)
        where TSource : FrameworkElement, IWorksheetPropertiesSource
    {
        SetOrRemoveWorksheetPropertiesSource(propertiesSourceElement, propertiesSourceElement, doSet);
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static void SetOrRemoveWorksheetPropertiesSource(FrameworkElement worksheetElement, IWorksheetPropertiesSource propertiesSource, bool doSet)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(worksheetElement != null && propertiesSource != null, null, Array.Empty<object>()))
        {
            return;
        }
        WorksheetPropertiesChangeInfo worksheetPropertiesChangeInfo;
        WorksheetPropertiesChangeInfo worksheetPropertyChangeInfo;
        if (doSet)
        {
            SetWorksheetPropertiesSource(worksheetElement, propertiesSource);
            worksheetPropertiesChangeInfo = new WorksheetPropertiesChangeInfo(WorksheetPropertyNames.GetNames(), propertiesSource, worksheetElement);
            SetWorksheetPropertiesChangeInfo(worksheetElement, worksheetPropertiesChangeInfo);
            worksheetPropertiesChangeInfo.RegisterToWorksheetPropertyChanges();
            return;
        }
        worksheetPropertyChangeInfo = GetWorksheetPropertyChangeInfo(worksheetElement);
        if (worksheetPropertyChangeInfo != null)
        {
            worksheetPropertyChangeInfo.UnregisterFromWorksheetPropertyChanges();
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
        worksheetElement.ClearValue(WorksheetPropertiesChangeInfoKey);
        DebugTools.DevAssert(GetWorksheetPropertiesSource(worksheetElement) == propertiesSource, null, Array.Empty<object>());
        worksheetElement.ClearValue(WorksheetPropertiesSourcePropertyKey);
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static void RegisterMockWorksheetPropertiesSourcePropertyForType(Type type, IWorksheetPropertiesSource mockSource)
    {
        WorksheetPropertiesSourcePropertyKey.OverrideMetadata(type, new FrameworkPropertyMetadata(mockSource));
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static bool HasWorksheetProperties<TTargetVisual>(TTargetVisual visual)
        where TTargetVisual : Visual
    {
        return WorksheetPropertiesSourcePropertyKey.DependencyProperty.DefaultMetadata.DefaultValue != GetWorksheetProperties(visual);
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static void RegisterToWorksheetPropertyNotifications(Type type)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(type.IsSubclassOf(typeof(Visual)) && type.GetInterface(typeof(IWorksheetPropertiesTarget).Name) != null, null, Array.Empty<object>()))
        {
            return;
        }
        WorksheetPropertiesChangeInfoKey.OverrideMetadata(type, new FrameworkPropertyMetadata(OnWorksheetPropertyChangeInfoChanged));
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static FrameworkElement GetWorksheetFrameworkElement<TTargetVisual>(TTargetVisual visual)
        where TTargetVisual : Visual
    {
        WorksheetPropertiesChangeInfo worksheetPropertyChangeInfo = GetWorksheetPropertyChangeInfo(visual);
        if (worksheetPropertyChangeInfo == null)
        {
            return null;
        }
        return worksheetPropertyChangeInfo.WorksheetFrameworkElement;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static ContentControl GetWorksheetContentControl<TTargetVisual>(TTargetVisual visual)
        where TTargetVisual : Visual
    {
        WorksheetPropertiesChangeInfo worksheetPropertyChangeInfo = GetWorksheetPropertyChangeInfo(visual);
        if (worksheetPropertyChangeInfo == null)
        {
            return null;
        }
        return worksheetPropertyChangeInfo.WorksheetFrameworkElement as ContentControl;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void OnWorksheetPropertyChanged(WorksheetPropertiesChangeInfo oldInfo, PropertyChangedEventArgs eArgs)
    {
        if (!WorksheetPropertyNames.Contains(eArgs.PropertyName))
        {
            return;
        }
        IWorksheetPropertiesSource propertiesSource = oldInfo.PropertiesSource;
        FrameworkElement worksheetFrameworkElement = oldInfo.WorksheetFrameworkElement;
        WorksheetPropertiesChangeInfo worksheetPropertiesChangeInfo = new WorksheetPropertiesChangeInfo(eArgs.PropertyName, propertiesSource, worksheetFrameworkElement);
        if (worksheetPropertiesChangeInfo.IsValid())
        {
            oldInfo.UnregisterFromWorksheetPropertyChanges();
            if (worksheetFrameworkElement != null)
            {
                SetWorksheetPropertiesChangeInfo(worksheetFrameworkElement, worksheetPropertiesChangeInfo);
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
            worksheetPropertiesChangeInfo.RegisterToWorksheetPropertyChanges();
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static WorksheetPropertiesChangeInfo GetWorksheetPropertyChangeInfo<TTargetVisual>(TTargetVisual visual)
        where TTargetVisual : Visual
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(visual != null, null))
        {
            return null;
        }
        return GetWorksheetPropertiesChangeInfo(visual);
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void OnWorksheetPropertyChangeInfoChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs eArgs)
    {
        WorksheetPropertiesChangeInfo worksheetPropertiesChangeInfo = eArgs.NewValue as WorksheetPropertiesChangeInfo;
        WorksheetPropertiesChangeInfo previousChangeInfo = eArgs.OldValue as WorksheetPropertiesChangeInfo;
        if (worksheetPropertiesChangeInfo != null)
        {
            string[] propertyNames;
            IWorksheetProperties oldProperties;
            if (previousChangeInfo == null)
            {
                propertyNames = WorksheetPropertyNames.GetNames();
                oldProperties = GetWorksheetPropertiesSource(dObj);
            }
            else
            {
                propertyNames = worksheetPropertiesChangeInfo.PropertyNames;
                oldProperties = null;
            }
            UpdateTarget(dObj as IWorksheetPropertiesTarget, propertyNames, oldProperties, worksheetPropertiesChangeInfo.PropertiesSource);
        }
        else if (previousChangeInfo != null)
        {
            if (!WpfUtils.HasLocalValue(dObj, eArgs.Property) && previousChangeInfo.PropertiesSource != dObj)
            {
                SetWorksheetPropertiesSource(dObj, previousChangeInfo.CachedWorksheetProperties);
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }
        else
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
        }
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    public static IWorksheetProperties GetWorksheetProperties<TTargetVisual>(TTargetVisual visual)
        where TTargetVisual : Visual
    {
        WorksheetPropertiesChangeInfo worksheetPropertyChangeInfo = GetWorksheetPropertyChangeInfo(visual);
        IWorksheetPropertiesSource worksheetPropertiesSource = (worksheetPropertyChangeInfo != null) ? worksheetPropertyChangeInfo.PropertiesSource : null;
        if (worksheetPropertiesSource == null && visual != null)
        {
            DebugTools.DevAssert(worksheetPropertyChangeInfo == null, null);
            worksheetPropertiesSource = GetWorksheetPropertiesSource(visual);
        }
        return worksheetPropertiesSource;
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
    private static void UpdateTarget(IWorksheetPropertiesTarget propertyTarget, string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(propertyTarget != null && propertyNames != null && newProperties != null, null, Array.Empty<object>()))
        {
            return;
        }
        propertyTarget.OnWorksheetPropertiesChanged(propertyNames, oldProperties, newProperties);
    }

    [MirrorOf("Ptc.Controls.Core.WorksheetPropertiesManager+WorksheetPropertiesChangeInfo, Ptc.Controls.Core")]
    private partial class WorksheetPropertiesChangeInfo : PropertiesChangeInfo<IWorksheetPropertiesSource>
    {
        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        private IWorksheetPropertiesSource _cachedWorksheetProperties;

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public IWorksheetPropertiesSource CachedWorksheetProperties =>
            _cachedWorksheetProperties ?? (_cachedWorksheetProperties = new CachedWorksheetPropertiesSource(PropertiesSource));

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public FrameworkElement WorksheetFrameworkElement { get; private set; }

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public WorksheetPropertiesChangeInfo(string propertyName, IWorksheetPropertiesSource propertySource, FrameworkElement worksheetElement)
            : this(new string[1] { propertyName }, propertySource, worksheetElement)
        {
        }

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public WorksheetPropertiesChangeInfo(string[] propertyNames, IWorksheetPropertiesSource propertySource, FrameworkElement worksheetElement)
            : base(propertyNames, propertySource)
        {
            WorksheetFrameworkElement = worksheetElement;
        }

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public void RegisterToWorksheetPropertyChanges()
        {
            if (PropertiesSource != null)
            {
                PropertiesSource.PropertyChanged += OnWorksheetPropertyChanged;
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        public void UnregisterFromWorksheetPropertyChanges()
        {
            if (PropertiesSource != null)
            {
                PropertiesSource.PropertyChanged -= OnWorksheetPropertyChanged;
            }
            else
            {
                DebugTools.FailAssertion(null, Array.Empty<object>());
            }
        }

        [MirrorVerified("PtcControlsCoreWorksheetPropertiesManagerMechanicalDifferentialTests")]
        private void OnWorksheetPropertyChanged(object sender, PropertyChangedEventArgs eArgs)
        {
            DebugTools.DevAssert(sender == PropertiesSource, null);
            WorksheetPropertiesManager.OnWorksheetPropertyChanged(this, eArgs);
        }
    }
}
