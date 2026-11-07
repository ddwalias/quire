#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Quire;
using Quire.Controls.Core;
using Quire.Core;
using Quire.PersistentDataObjects;
using Quire.Wpf;

namespace Quire.Controls.Core.Serialization;

public static partial class MC14ConversionAnnotation
{
    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty ConversionAnnotationsProperty;

    private static readonly DependencyPropertyKey TopLevelAnnotationAdornersPropertyKey;

    public const string AnnotatedTooltipResourceKey = "AnnotatedTooltip";

    private static readonly ResourceDictionary _resourceDictionary;

    public static AnnotationTypeList GetConversionAnnotations(UIElement element)
    {
        return (AnnotationTypeList)element.GetValue(ConversionAnnotationsProperty);
    }

    public static void SetConversionAnnotations(UIElement element, AnnotationTypeList value)
    {
        if (value != null)
        {
            element.SetValue(ConversionAnnotationsProperty, value);
        }
    }

    public static void ClearConversionAnnotations(UIElement element)
    {
        if (element != null)
        {
            element.ClearValue(ConversionAnnotationsProperty);
        }
    }

    public static bool HasConversionAnnotations(UIElement element)
    {
        return GetConversionAnnotations(element) != null;
    }

    public static void AddConversionAnnotation(UIElement element, MC14ConversionAnnotationType type)
    {
        AnnotationTypeList conversionAnnotations = GetConversionAnnotations(element);
        if (conversionAnnotations == null)
        {
            conversionAnnotations = new AnnotationTypeList();
            conversionAnnotations.AddOnce(type);
            SetConversionAnnotations(element, conversionAnnotations);
        }
        else
        {
            DebugTools.DevAssert(conversionAnnotations.IsFrozen, null);
            AnnotationTypeList annotationTypeList = new AnnotationTypeList(conversionAnnotations);
            annotationTypeList.AddOnce(type);
            SetConversionAnnotations(element, annotationTypeList);
        }
    }

    public static void SetOldConversionAnnotation(UIElement element)
    {
        DebugTools.DevAssert(GetConversionAnnotations(element) == null, null);
        AnnotationTypeList annotationTypeList = new AnnotationTypeList();
        annotationTypeList.AddOnce(MC14ConversionAnnotationType.ChangedRegion);
        SetConversionAnnotations(element, annotationTypeList);
    }

    public static void SetConversionFailedAnnotation(UIElement element)
    {
        DebugTools.DevAssert(GetConversionAnnotations(element) == null, null);
        AnnotationTypeList annotationTypeList = new AnnotationTypeList();
        annotationTypeList.AddOnce(MC14ConversionAnnotationType.FailedToConvert);
        SetConversionAnnotations(element, annotationTypeList);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public static IList<VisualAdornerBase> GetTopLevelAnnotationAdorners(UIElement element)
    {
        return (IList<VisualAdornerBase>)element.GetValue(TopLevelAnnotationAdornersPropertyKey.DependencyProperty);
    }

    private static void SetTopLevelAnnotationAdorners(UIElement element, IList<VisualAdornerBase> value)
    {
        element.SetValue(TopLevelAnnotationAdornersPropertyKey, value);
    }

    private static void ClearTopLevelAnnotationAdorners(UIElement element)
    {
        element.ClearValue(TopLevelAnnotationAdornersPropertyKey);
    }

    private static void HandleChangedByConversionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement frameworkElement)
        {
            DebugTools.DevAssert(false, null);
        }
        else if (e.NewValue == null)
        {
            RemoveAnnotations(frameworkElement);
        }
        else
        {
            SetAnnotations(frameworkElement);
        }
    }

    private static void SetAnnotations(FrameworkElement element)
    {
        GetConversionAnnotations(element).Freeze();
        if (element is IIndirectMessageAdornerOwner)
        {
            IIndirectMessageAdornerOwner indirect = element as IIndirectMessageAdornerOwner;
            if (indirect.IsAdornedElementReady)
            {
                ElementIsLoadedSetAnnotations(element, null);
            }
            indirect.OnAdornedElementReady += ElementIsLoadedSetAnnotations;
        }
        else
        {
            if (element.IsLoaded && AdornerLayer.GetAdornerLayer(element) != null)
            {
                ElementIsLoadedSetAnnotations(element, null);
            }
            element.Loaded += ElementIsLoadedSetAnnotations;
        }
    }

    private static void RemoveAnnotations(FrameworkElement element)
    {
        FrameworkElement element2 = element;
        if (element is IIndirectMessageAdornerOwner)
        {
            (element as IIndirectMessageAdornerOwner).OnAdornedElementReady -= ElementIsLoadedSetAnnotations;
        }
        else
        {
            element.Loaded -= ElementIsLoadedSetAnnotations;
        }
        if (!element.IsLoaded)
        {
            return;
        }
        element = NodeMessageAdorner.GetAdornedElement(element);
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer != null)
        {
            Adorner[] adorners = adornerLayer.GetAdorners(element);
            if (adorners != null)
            {
                Adorner adorner = adorners.FirstOrDefault(adorner2 => adorner2 is AnnotationArrowAdorner);
                if (adorner != null)
                {
                    adornerLayer.Remove(adorner);
                }
            }
        }
        IList<VisualAdornerBase> topLevelAnnotationAdorners = GetTopLevelAnnotationAdorners(element2);
        if (topLevelAnnotationAdorners != null)
        {
            topLevelAnnotationAdorners.Each(item =>
            {
                item.Remove();
            });
            ClearTopLevelAnnotationAdorners(element2);
        }
    }

    private static void ElementIsLoadedSetAnnotations(object sender, RoutedEventArgs e)
    {
        FrameworkElement origElement = sender as FrameworkElement;
        if (origElement == null)
        {
            return;
        }
        ElementTrySetAnnotations(origElement);
    }

    private static void ElementTrySetAnnotations(FrameworkElement origElement)
    {
        FrameworkElement element = NodeMessageAdorner.GetAdornedElement(origElement);
        if (!element.IsLoaded)
        {
            WpfUtils.ExecuteOnLoad(element, () =>
            {
                ElementTrySetAnnotations(origElement);
            });
            return;
        }
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(element);
        if (adornerLayer == null)
        {
            if (!element.IsArrangeValid)
            {
                EventHandler layoutUpdatedDelegate = null;
                layoutUpdatedDelegate = delegate
                {
                    ElementTrySetAnnotations(origElement);
                    element.LayoutUpdated -= layoutUpdatedDelegate;
                };
                element.LayoutUpdated += layoutUpdatedDelegate;
                return;
            }
            if (!element.IsVisible)
            {
                DependencyPropertyChangedEventHandler isVisibleChangedDelegate = null;
                isVisibleChangedDelegate = delegate
                {
                    ElementTrySetAnnotations(origElement);
                    element.IsVisibleChanged -= isVisibleChangedDelegate;
                };
                element.IsVisibleChanged += isVisibleChangedDelegate;
                return;
            }
        }
        if (DebugTools.AssertNotNull(adornerLayer, "No adorner layer found on annotated element").Failed)
        {
            return;
        }
        Adorner[] adorners = adornerLayer.GetAdorners(element);
        Adorner adorner = null;
        if (adorners != null)
        {
            adorner = adorners.FirstOrDefault(adorner2 => adorner2 is AnnotationArrowAdorner);
        }
        IList<VisualAdornerBase> list = GetTopLevelAnnotationAdorners(origElement);
        if (list != null)
        {
            list.Each(visualAdornerBase =>
            {
                visualAdornerBase.Remove();
            });
            list.Clear();
        }
        else
        {
            list = new List<VisualAdornerBase>();
        }
        if (HasConversionAnnotations(origElement))
        {
            foreach (MC14ConversionAnnotationType conversionAnnotation in GetConversionAnnotations(origElement))
            {
                NodeMessageAdorner item = new NodeMessageAdorner(element, GetMC14ConversionAnnotationByType(conversionAnnotation), NodeMessageAdornerType.Notification);
                list.Add(item);
            }
            list.Add(MarginLineAdorner.Create(element));
        }
        if (list.Count > 0)
        {
            if (adorner == null)
            {
                adornerLayer.Add(new AnnotationArrowAdorner(element));
            }
            SetTopLevelAnnotationAdorners(origElement, list);
        }
        else
        {
            if (adorner != null)
            {
                adornerLayer.Remove(adorner);
            }
            ClearTopLevelAnnotationAdorners(origElement);
        }
    }

    static MC14ConversionAnnotation()
    {
        _typeOfThis = typeof(MC14ConversionAnnotation);
        ConversionAnnotationsProperty = DependencyProperty.RegisterAttached("ConversionAnnotations", typeof(AnnotationTypeList), _typeOfThis);
        TopLevelAnnotationAdornersPropertyKey = DependencyProperty.RegisterAttachedReadOnly("TopLevelAnnotationAdorners", typeof(IList<VisualAdornerBase>), _typeOfThis, new FrameworkPropertyMetadata(null));
        _resourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Quire.Controls.Core;component/Serialization/MC14ConversionDictionary.xaml")
        };
        ConversionAnnotationsProperty.OverrideMetadata(typeof(FrameworkElement), new PropertyMetadata(null, HandleChangedByConversionChanged));
    }

    public static string GetMC14ConversionAnnotationByType(MC14ConversionAnnotationType type)
    {
        return _resourceDictionary[type].ToString();
    }

    public static ConversionAnnotationMessage GetConversionAnnotationDescriptor(MC14ConversionAnnotationType type)
    {
        return (ConversionAnnotationMessage)_resourceDictionary[type];
    }

    public static ConversionAnnotationMessage GetConversionAnnotationDescriptor(string typeName)
    {
        return GetConversionAnnotationDescriptor((MC14ConversionAnnotationType)Enum.Parse(typeof(MC14ConversionAnnotationType), typeName));
    }

    public static string GetCategoryDesription(string categoryKey)
    {
        return (string)_resourceDictionary[categoryKey];
    }
}
