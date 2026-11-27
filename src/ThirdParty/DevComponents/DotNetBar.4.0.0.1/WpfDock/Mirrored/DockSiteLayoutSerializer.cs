#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace Quire.DevComponents.WpfDock;

internal partial class DockSiteLayoutSerializer
{
    public string SaveLayout(DockSite site)
    {
        XmlDocument document = new();
        XmlElement root = document.CreateElement(DockXmlElements.DockSite);
        root.SetAttribute(DockXmlElements.Version, XmlConvert.ToString(1));
        document.AppendChild(root);

        foreach (SplitPanel panel in site.SplitPanels)
        {
            if (DockSite.GetSaveLayout(panel))
            {
                SaveSplitPanelLayout(panel, root);
            }
        }

        if (site.Content is SplitPanel contentPanel && DockSite.GetSaveLayout(contentPanel))
        {
            SaveSplitPanelLayout(contentPanel, root).SetAttribute(DockXmlElements.IsDocument, XmlConvert.ToString(true));
        }

        foreach (FloatingWindow window in site.GetFloatingWindows())
        {
            if (DockSite.GetSaveLayout(window))
            {
                SaveFloatingWindowLayout(window, root);
            }
        }

        return document.InnerXml;
    }

    private void SaveFloatingWindowLayout(FloatingWindow window, XmlElement xmlParent)
    {
        if (window.Content is not DockWindowGroup group || !DockSite.GetSaveLayout(group))
        {
            return;
        }

        XmlElement element = xmlParent.OwnerDocument.CreateElement(DockXmlElements.FloatingWindow);
        xmlParent.AppendChild(element);
        SaveDockWindowGroupLayout(group, element);
        element.SetAttribute(DockXmlElements.FloatRect, ConvertRect(new Rect(window.Left, window.Top, window.Width, window.Height)));
    }

    private XmlElement SaveSplitPanelLayout(SplitPanel panel, XmlElement xmlParent)
    {
        XmlElement element = xmlParent.OwnerDocument.CreateElement(DockXmlElements.SplitPanel);
        xmlParent.AppendChild(element);

        if (panel.Parent is DockSite)
        {
            element.SetAttribute(DockXmlElements.Dock, DockSite.GetDock(panel).ToString());
            element.SetAttribute(DockXmlElements.DockSize, XmlConvert.ToString(DockSite.GetDockSize(panel)));
        }
        else if (panel.Parent is SplitPanel)
        {
            Size relativeSize = SplitPanel.GetRelativeSize(panel);
            element.SetAttribute(DockXmlElements.RelativeSize, ConvertSize(relativeSize));
        }

        element.SetAttribute(DockXmlElements.Orientation, panel.Orientation.ToString());
        foreach (UIElement child in panel.Children)
        {
            if (DockSite.GetSaveLayout(child))
            {
                if (child is DockWindowGroup)
                {
                    SaveDockWindowGroupLayout(child as DockWindowGroup, element);
                }
                else if (child is DockWindow)
                {
                    SaveDockWindowLayout(child as DockWindow, element);
                }
                else if (child is SplitPanel)
                {
                    SaveSplitPanelLayout(child as SplitPanel, element);
                }
                else if (child is FrameworkElement)
                {
                    SaveDockedControlLayout(child as FrameworkElement, element);
                }
            }
        }

        return element;
    }

    private void SaveDockedControlLayout(FrameworkElement element, XmlElement xmlSplitPanel)
    {
        if (element.Name == "" || element.Name == null)
        {
            throw new InvalidOperationException("The FrameworkElement child of SplitPanel does not have name assigned and its layout cannot be serialized.");
        }

        XmlElement xmlElement = xmlSplitPanel.OwnerDocument.CreateElement(DockXmlElements.DockedControl);
        if (element.Parent is SplitPanel)
        {
            xmlElement.SetAttribute(DockXmlElements.RelativeSize, ConvertSize(SplitPanel.GetRelativeSize(element)));
        }

        xmlElement.SetAttribute(DockXmlElements.ControlName, element.Name);
        xmlSplitPanel.AppendChild(xmlElement);
    }

    private void SaveDockWindowLayout(DockWindow dockWindow, XmlElement xmlParent)
    {
        if (dockWindow.Name == "" || dockWindow.Name == null)
        {
            throw new InvalidOperationException("The DockWindow child of SplitPanel or DockWindowGroup does not have name assigned and its layout cannot be serialized.");
        }

        XmlElement element = xmlParent.OwnerDocument.CreateElement(DockXmlElements.DockWindow);
        if (dockWindow.Parent is SplitPanel)
        {
            element.SetAttribute(DockXmlElements.RelativeSize, ConvertSize(SplitPanel.GetRelativeSize(dockWindow)));
        }

        element.SetAttribute(DockXmlElements.ControlName, dockWindow.Name);
        if (dockWindow.Visibility != Visibility.Visible)
        {
            element.SetAttribute(DockXmlElements.Visibility, dockWindow.Visibility.ToString());
        }

        if (dockWindow.IsFloating)
        {
            FloatingWindow floatingWindow = dockWindow.GetDockSite().GetFloatingWindow(dockWindow);
            Rect floatingRect = new(floatingWindow.Left, floatingWindow.Top, floatingWindow.Width, floatingWindow.Height);
            dockWindow.FloatingRect = floatingRect;
        }

        if (dockWindow.FloatingRect != DockWindow.DefaultFloatingRect)
        {
            element.SetAttribute(DockXmlElements.FloatRect, ConvertRect(dockWindow.FloatingRect));
        }

        xmlParent.AppendChild(element);
    }

    private void SaveDockWindowGroupLayout(DockWindowGroup dockGroup, XmlElement xmlParent)
    {
        XmlElement element = xmlParent.OwnerDocument.CreateElement(DockXmlElements.DockWindowGroup);
        xmlParent.AppendChild(element);
        element.SetAttribute(DockXmlElements.RelativeSize, ConvertSize(SplitPanel.GetRelativeSize(dockGroup)));
        if (dockGroup.Visibility != Visibility.Visible)
        {
            element.SetAttribute(DockXmlElements.Visibility, dockGroup.Visibility.ToString());
        }

        foreach (object item in (IEnumerable)dockGroup.Items)
        {
            if (item is DockWindow)
            {
                SaveDockWindowLayout(item as DockWindow, element);
            }
            else if (item is FrameworkElement)
            {
                SaveDockedControlLayout(item as FrameworkElement, element);
            }
        }
    }

    public void LoadLayout(DockSite site, string layout)
    {
        XmlDocument document = new();
        document.LoadXml(layout);
        if (document.FirstChild.Name != DockXmlElements.DockSite)
        {
            throw new InvalidOperationException("Layout XML not in valid format. DockSite element missing.");
        }

        XmlElement root = document.FirstChild as XmlElement;
        if (XmlConvert.ToInt32(root.GetAttribute(DockXmlElements.Version)) > 1)
        {
            throw new InvalidOperationException("Layout XML not in supported version.");
        }

        m_IgnoreLoadLayoutElementNotFoundErrors = site.IgnoreLoadLayoutElementNotFoundErrors;
        m_DockWindows = site.GetDockWindows();
        try
        {
            foreach (XmlNode childNode in root.ChildNodes)
            {
                XmlElement child = childNode as XmlElement;
                if (child.Name == DockXmlElements.SplitPanel)
                {
                    LoadSplitPanelLayout(site, child);
                }
                else if (child.Name == DockXmlElements.FloatingWindow)
                {
                    LoadFloatingWindowLayout(site, child);
                }
            }
        }
        finally
        {
            m_DockWindows = null;
        }

        Window window = Window.GetWindow(site);
        if (window != null)
        {
            window.Activate();
        }
    }

    private void LoadFloatingWindowLayout(DockSite site, XmlElement xmlParent)
    {
        foreach (XmlNode childNode in xmlParent.ChildNodes)
        {
            XmlElement element = childNode as XmlElement;
            if (element.Name == DockXmlElements.DockWindowGroup)
            {
                DockWindowGroup group = LoadDockWindowGroupLayout(element);
                site.FloatWindow(group);
                break;
            }
        }
    }

    private DockWindowGroup LoadDockWindowGroupLayout(XmlElement element)
    {
        return LoadDockWindowGroupLayout(null, element);
    }

    private void LoadSplitPanelLayout(DockSite site, XmlElement element)
    {
        SplitPanel panel = new();
        Dock dock = (Dock)Enum.Parse(typeof(Dock), element.GetAttribute(DockXmlElements.Dock));
        double dockSize = XmlConvert.ToDouble(element.GetAttribute(DockXmlElements.DockSize));
        Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), element.GetAttribute(DockXmlElements.Orientation));
        bool isDocument = false;
        if (element.HasAttribute(DockXmlElements.IsDocument) && XmlConvert.ToBoolean(element.GetAttribute(DockXmlElements.IsDocument)))
        {
            isDocument = true;
        }

        DockSite.SetDock(panel, dock);
        DockSite.SetDockSize(panel, dockSize);
        panel.Orientation = orientation;
        if (isDocument)
        {
            site.Content = panel;
        }
        else
        {
            site.SplitPanels.Add(panel);
        }

        LoadChildren(panel, element);
        panel.UpdateAutoVisibility();
    }

    private void LoadSplitPanelLayout(SplitPanel parent, XmlElement element)
    {
        SplitPanel panel = new();
        Size relativeSize = ConvertSize(element.GetAttribute(DockXmlElements.RelativeSize));
        SplitPanel.SetRelativeSize(panel, relativeSize);
        _ = (Orientation)Enum.Parse(typeof(Orientation), element.GetAttribute(DockXmlElements.Orientation));
        panel.Orientation = Orientation.Horizontal;
        parent.Children.Add(panel);
        LoadChildren(panel, element);
        panel.UpdateAutoVisibility();
    }

    private void LoadChildren(SplitPanel parent, XmlElement xmlParent)
    {
        foreach (XmlNode childNode in xmlParent.ChildNodes)
        {
            XmlElement child = childNode as XmlElement;
            if (child.Name == DockXmlElements.SplitPanel)
            {
                LoadSplitPanelLayout(parent, child);
            }
            else if (child.Name == DockXmlElements.DockWindowGroup)
            {
                LoadDockWindowGroupLayout(parent, child);
            }
            else if (child.Name == DockXmlElements.DockWindow)
            {
                DockWindow window = LoadDockWindowLayout(child);
                if (window != null)
                {
                    Detach(window);
                    parent.Children.Add(window);
                }
            }
            else if (child.Name == DockXmlElements.DockedControl)
            {
                FrameworkElement frameworkElement = LoadDockControlLayout(child);
                if (frameworkElement != null)
                {
                    parent.Children.Add(frameworkElement);
                }
            }
        }
    }

    private void Detach(DockWindow window)
    {
        if (window.IsAutoHide)
        {
            window.IsAutoHide = false;
        }

        if (window.Parent is DockWindowGroup)
        {
            DockWindowGroup group = window.Parent as DockWindowGroup;
            group.Items.Remove(window);
            if (group.Items.Count == 0)
            {
                Detach(group);
            }
        }
        else if (window.Parent is SplitPanel)
        {
            SplitPanel panel = window.Parent as SplitPanel;
            panel.Children.Remove(window);
            if (panel.Children.Count == 0)
            {
                Detach(panel);
            }
        }
    }

    private void Detach(DockWindowGroup group)
    {
        if (group.Parent is SplitPanel)
        {
            SplitPanel panel = group.Parent as SplitPanel;
            panel.Children.Remove(group);
            if (panel.Children.Count == 0)
            {
                Detach(panel);
            }
        }
        else if (group.Parent is FloatingWindow)
        {
            group.GetDockSite().Detach(group);
        }
    }

    private void Detach(SplitPanel panel)
    {
        if (panel.Parent is SplitPanel)
        {
            SplitPanel parent = panel.Parent as SplitPanel;
            parent.Children.Remove(panel);
            if (parent.Children.Count == 0)
            {
                Detach(parent);
            }
        }
        else if (panel.Parent is DockSite)
        {
            DockSite site = panel.Parent as DockSite;
            if (site.Content == panel)
            {
                site.Content = null;
            }
            else if (site.SplitPanels.Contains(panel))
            {
                site.SplitPanels.Remove(panel);
            }
        }
    }

    private FrameworkElement LoadDockControlLayout(XmlElement element)
    {
        string name = element.GetAttribute(DockXmlElements.ControlName);
        Size size = default;
        if (element.HasAttribute(DockXmlElements.RelativeSize))
        {
            size = ConvertSize(element.GetAttribute(DockXmlElements.RelativeSize));
        }

        FrameworkElement frameworkElement = FindDockControl(name);
        if (frameworkElement != null && !LayoutHelpers.IsEmpty(size))
        {
            SplitPanel.SetRelativeSize(frameworkElement, size);
        }

        return frameworkElement;
    }

    private FrameworkElement FindDockControl(string name)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    private DockWindow LoadDockWindowLayout(XmlElement element)
    {
        string name = element.GetAttribute(DockXmlElements.ControlName);
        Size size = default;
        if (element.HasAttribute(DockXmlElements.RelativeSize))
        {
            size = ConvertSize(element.GetAttribute(DockXmlElements.RelativeSize));
        }

        DockWindow window = FindDockWindow(name);
        if (window == null && m_IgnoreLoadLayoutElementNotFoundErrors)
        {
            return null;
        }

        if (window != null && !LayoutHelpers.IsEmpty(size))
        {
            SplitPanel.SetRelativeSize(window, size);
        }

        if (element.HasAttribute(DockXmlElements.Visibility))
        {
            window.Visibility = (Visibility)Enum.Parse(typeof(Visibility), element.GetAttribute(DockXmlElements.Visibility));
        }
        else
        {
            window.Visibility = Visibility.Visible;
        }

        if (element.HasAttribute(DockXmlElements.FloatRect))
        {
            window.FloatingRect = ConvertRect(element.GetAttribute(DockXmlElements.FloatRect));
        }

        return window;
    }

    private DockWindow FindDockWindow(string name)
    {
        foreach (DockWindow window in m_DockWindows)
        {
            if (window.Name == name)
            {
                return window;
            }
        }

        Trace.WriteLine("DockWindow named '" + name + "' could not be found while loading layout");
        return null;
    }

    private DockWindowGroup LoadDockWindowGroupLayout(SplitPanel panel, XmlElement element)
    {
        DockWindowGroup group = new();
        Size relativeSize = ConvertSize(element.GetAttribute(DockXmlElements.RelativeSize));
        if (panel != null)
        {
            SplitPanel.SetRelativeSize(group, relativeSize);
            panel.Children.Add(group);
        }

        if (element.HasAttribute(DockXmlElements.Visibility))
        {
            group.Visibility = (Visibility)Enum.Parse(typeof(Visibility), element.GetAttribute(DockXmlElements.Visibility));
        }
        else
        {
            group.Visibility = Visibility.Visible;
        }

        foreach (XmlNode childNode in element.ChildNodes)
        {
            XmlElement child = childNode as XmlElement;
            if (child.Name == DockXmlElements.DockWindow)
            {
                DockWindow window = LoadDockWindowLayout(child);
                if (window != null)
                {
                    Detach(window);
                    group.Items.Add(window);
                }
            }
            else if (child.Name == DockXmlElements.DockedControl)
            {
                FrameworkElement frameworkElement = LoadDockControlLayout(child);
                if (frameworkElement != null)
                {
                    group.Items.Add(frameworkElement);
                }
            }
        }

        if (group.SelectedItem == null)
        {
            group.SelectFirstTab();
        }

        return group;
    }

    private static Size ConvertSize(string size)
    {
        string[] parts = size.Split(',');
        return new Size(double.Parse(parts[0]), double.Parse(parts[1]));
    }

    private static string ConvertSize(Size size)
    {
        return XmlConvert.ToString(size.Width) + "," + XmlConvert.ToString(size.Height);
    }

    private static Rect ConvertRect(string rect)
    {
        string[] parts = rect.Split(',');
        return new Rect(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2]), double.Parse(parts[3]));
    }

    private static string ConvertRect(Rect rect)
    {
        return XmlConvert.ToString(rect.X) + "," + XmlConvert.ToString(rect.Y) + "," + XmlConvert.ToString(rect.Width) + "," + XmlConvert.ToString(rect.Height);
    }
}
