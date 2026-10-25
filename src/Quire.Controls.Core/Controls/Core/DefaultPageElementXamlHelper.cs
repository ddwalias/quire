#nullable disable
using System;
using System.Windows;
using Quire;
using Quire.Mirror;
using Quire.Xml;

namespace Quire.Controls.Core;

public partial class DefaultPageElementXamlHelper
{
    [MirrorVerified("PtcControlsCoreDefaultPageElementXamlHelperMechanicalDifferentialTests")]
    public DefaultPageElementXamlHelper() { }

    public virtual UIElement GetPrintView(UIElement element)
    {
        UIElement uIElement;
        ISupportPrintViewport supportPrintViewport;
        Rect? printViewport;
        ICloneable cloneable;
        try
        {
            supportPrintViewport = element as ISupportPrintViewport;
            if (supportPrintViewport != null)
            {
                printViewport = supportPrintViewport.PrintViewport;
                DebugTools.DevAssert(printViewport.HasValue, null);
            }
            DocumentAttachedProperties.SetIsInPrinting(element, true);
            cloneable = element as ICloneable;
            if (cloneable != null)
            {
                uIElement = cloneable.Clone() as UIElement;
            }
            else
            {
                uIElement = (UIElement)XmlHelper.CreateXamlCopy(element);
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertionDoNotThrow(ex);
            uIElement = null;
        }
        finally
        {
            DocumentAttachedProperties.ClearIsInPrinting(element);
        }
        DebugTools.DevAssert(uIElement == null || !DocumentAttachedProperties.GetIsInPrinting(uIElement), null);
        return uIElement;
    }
}
