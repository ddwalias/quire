#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class ViewportsListManager : IViewportsListManager
{
    public static ViewportsListManager Instance = new ViewportsListManager();

    private static List<WeakReference> _loadedViewportsNeverUseDirectly = new List<WeakReference>();

    private static List<WeakReference> _allViewportsNeverUseDirectly = new List<WeakReference>();

    public void AddToList(Viewport3D viewport)
    {
        List<WeakReference> list = new List<WeakReference>();
        WeakReference weakReference = null;
        foreach (WeakReference item in AllViewportsAlive)
        {
            if (item.Target == viewport)
            {
                DebugTools.DevAssert(false, null);
                weakReference = item;
            }
            list.Add(item);
        }
        if (weakReference == null)
        {
            weakReference = new WeakReference(viewport);
            list.Add(weakReference);
        }
        ReplaceAllViewports(list);
        if (DebugTools.DevAssert(!viewport.IsLoaded, null).Failed)
        {
            AddToLoadedViewports(weakReference);
        }
        viewport.Loaded += OnViewportLoaded;
        viewport.Unloaded += OnViewportUnloaded;
    }

    public void RemoveFromList(Viewport3D viewport)
    {
        List<WeakReference> list = new List<WeakReference>();
        foreach (WeakReference item in AllViewportsAlive)
        {
            if (item.Target == viewport)
            {
                RemoveFromLoadedViewports(item);
                viewport.Loaded -= OnViewportLoaded;
                viewport.Unloaded -= OnViewportUnloaded;
            }
            else
            {
                list.Add(item);
            }
        }
        ReplaceAllViewports(list);
    }

    private ViewportsListManager()
    {
    }

    private static IEnumerable<WeakReference> LoadedViewports => _loadedViewportsNeverUseDirectly;

    private static void ReplaceLoadedViewports(List<WeakReference> loadedViewports)
    {
        if (_loadedViewportsNeverUseDirectly.Count != 0)
        {
            CompositionTarget.Rendering -= OnRendering;
        }
        _loadedViewportsNeverUseDirectly.Clear();
        _loadedViewportsNeverUseDirectly = loadedViewports;
        if (DebugTools.AssertNotNull(_loadedViewportsNeverUseDirectly, null).Failed)
        {
            _loadedViewportsNeverUseDirectly = new List<WeakReference>();
        }
        if (_loadedViewportsNeverUseDirectly.Count != 0)
        {
            CompositionTarget.Rendering += OnRendering;
        }
    }

    private static void AddToLoadedViewports(WeakReference viewportRef)
    {
        if (!_loadedViewportsNeverUseDirectly.Contains(viewportRef))
        {
            _loadedViewportsNeverUseDirectly.Add(viewportRef);
            if (_loadedViewportsNeverUseDirectly.Count == 1)
            {
                CompositionTarget.Rendering += OnRendering;
            }
        }
    }

    private static void RemoveFromLoadedViewports(WeakReference viewportRef)
    {
        _loadedViewportsNeverUseDirectly.Remove(viewportRef);
        if (_loadedViewportsNeverUseDirectly.Count == 0)
        {
            CompositionTarget.Rendering -= OnRendering;
        }
    }

    private static IEnumerable<WeakReference> AllViewportsAlive => _allViewportsNeverUseDirectly.Where((WeakReference weakRef) => weakRef.IsAlive);

    private static void ReplaceAllViewports(List<WeakReference> allViewports)
    {
        _allViewportsNeverUseDirectly.Clear();
        _allViewportsNeverUseDirectly = allViewports;
        if (DebugTools.AssertNotNull(_allViewportsNeverUseDirectly, null).Failed)
        {
            _allViewportsNeverUseDirectly = new List<WeakReference>();
        }
    }

    private static WeakReference FindInAllViewports(object sender)
    {
        return _allViewportsNeverUseDirectly.SingleOrDefault((WeakReference weakRef) => weakRef.Target == sender);
    }

    private static bool _renderingEnabled = true;

    public static bool RenderingEnabled
    {
        get
        {
            return _renderingEnabled;
        }
        set
        {
            if (_renderingEnabled != value)
            {
                _renderingEnabled = value;
                if (_renderingEnabled)
                {
                    OnRendering();
                }
            }
        }
    }

    private static void OnRendering(object sender, EventArgs args)
    {
        OnRendering();
    }

    private static void OnRendering()
    {
        if (!RenderingEnabled)
        {
            return;
        }
        List<WeakReference> viewportsAlive = new List<WeakReference>();
        foreach (WeakReference loadedViewport in LoadedViewports)
        {
            RemoveOrRenderViewportRef(loadedViewport, ref viewportsAlive);
        }
        ReplaceLoadedViewports(viewportsAlive);
    }

    private static void RemoveOrRenderViewportRef(WeakReference weakReference, ref List<WeakReference> viewportsAlive)
    {
        if (weakReference.IsAlive)
        {
            viewportsAlive.Add(weakReference);
            OnRendering(weakReference.Target as Viewport3D);
        }
    }

    private static void OnRendering(Viewport3D viewport)
    {
        Matrix3D totalTransform = ViewportInfo.GetTotalTransform(viewport);
        foreach (Visual3D child in viewport.Children)
        {
            OnRendering((DependencyObject)(object)child, totalTransform);
        }
    }

    private static void OnRendering(DependencyObject visual2Or3D, Matrix3D matx)
    {
        DebugTools.DevAssert(visual2Or3D is Visual || visual2Or3D is Visual3D, null);
        if (visual2Or3D is ModelVisual3D)
        {
            ModelVisual3D modelVisual3D = visual2Or3D as ModelVisual3D;
            if (modelVisual3D.Transform != null)
            {
                matx.Prepend(modelVisual3D.Transform.Value);
            }
        }
        if (visual2Or3D is WireBase)
        {
            (visual2Or3D as WireBase).OnRendering(matx);
        }
        int childrenCount = VisualTreeHelper.GetChildrenCount(visual2Or3D);
        for (int i = 0; i < childrenCount; i++)
        {
            OnRendering(VisualTreeHelper.GetChild(visual2Or3D, i), matx);
        }
    }

    private static void OnViewportLoaded(object sender, RoutedEventArgs args)
    {
        WeakReference weakReference = FindInAllViewports(sender);
        if (DebugTools.AssertNotNull(weakReference, null).Passed)
        {
            AddToLoadedViewports(weakReference);
        }
    }

    private static void OnViewportUnloaded(object sender, RoutedEventArgs args)
    {
        WeakReference weakReference = FindInAllViewports(sender);
        if (DebugTools.AssertNotNull(weakReference, null).Passed)
        {
            RemoveFromLoadedViewports(weakReference);
        }
    }
}
