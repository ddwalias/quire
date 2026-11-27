using System;
using System.Windows;

namespace Quire.Controls.Internal;

public partial class ScriptedTextLayoutManager
{
    protected TextLayoutItem[] _childItems;

    protected TextLayoutItem _defaultSubscriptItem;

    public TextLayoutItem[] ChildItems => _childItems;

    public Size DesiredSize
    {
        get
        {
            Rect rect = new Rect(0.0, 0.0, 0.0, 0.0);
            TextLayoutItem[] childItems = ChildItems;
            foreach (TextLayoutItem textLayoutItem in childItems)
            {
                rect.Union(textLayoutItem.Rect);
            }
            return rect.Size;
        }
    }

    public double TextBaselineOffset { get; private set; }

    public double SubscriptBaseline { get; private set; }

    public TextLayoutItem this[int i] => ChildItems[i];

    public ScriptedTextLayoutManager(TextLayoutItem[] childItems, TextLayoutItem defaultSubscriptItem)
    {
        if (defaultSubscriptItem == null)
        {
            throw new ArgumentNullException(nameof(defaultSubscriptItem));
        }

        if (defaultSubscriptItem.ItemType != TextLayoutItem.ItemTypes.Subscript)
        {
            throw new ArgumentException("item was not a subscript item", nameof(defaultSubscriptItem));
        }

        if (defaultSubscriptItem.TextBaselineOffset == 0.0 || defaultSubscriptItem.CapsHeight == 0.0)
        {
            throw new ArgumentException("default subscript item must contain values for both TextBaselineOffset & CapsHeight", nameof(defaultSubscriptItem));
        }

        _childItems = childItems;
        _defaultSubscriptItem = defaultSubscriptItem;
    }

    public void Arrange()
    {
        SetBaselines();
        double num = 0.0;
        for (int i = 0; i < ChildItems.Length; i++)
        {
            TextLayoutItem textLayoutItem = ChildItems[i];
            textLayoutItem.Rect.X = num;
            switch (textLayoutItem.ItemType)
            {
                case TextLayoutItem.ItemTypes.Normal:
                    textLayoutItem.Rect.Y = TextBaselineOffset - textLayoutItem.TextBaselineOffset;
                    break;
                case TextLayoutItem.ItemTypes.Subscript:
                    textLayoutItem.Rect.Y = SubscriptBaseline - textLayoutItem.TextBaselineOffset;
                    break;
            }
            num += textLayoutItem.Rect.Width;
        }
    }

    private void SetBaselines()
    {
        double num = -1.0;
        double baselineOffset = -1.0;
        double capsHeight = -1.0;
        TextLayoutItem[] childItems = ChildItems;
        foreach (TextLayoutItem textLayoutItem in childItems)
        {
            switch (textLayoutItem.ItemType)
            {
                case TextLayoutItem.ItemTypes.Normal:
                    if (textLayoutItem.TextBaselineOffset > num)
                    {
                        num = textLayoutItem.TextBaselineOffset;
                    }
                    break;
                case TextLayoutItem.ItemTypes.Subscript:
                    if (textLayoutItem.TextBaselineOffset > baselineOffset)
                    {
                        baselineOffset = textLayoutItem.TextBaselineOffset;
                    }
                    if (textLayoutItem.CapsHeight > capsHeight)
                    {
                        capsHeight = textLayoutItem.CapsHeight;
                    }
                    break;
            }
        }

        if (baselineOffset == -1.0)
        {
            baselineOffset = _defaultSubscriptItem.TextBaselineOffset;
            capsHeight = _defaultSubscriptItem.CapsHeight;
        }

        if (num == -1.0)
        {
            TextBaselineOffset = baselineOffset - capsHeight / 2.0;
        }
        else
        {
            TextBaselineOffset = num;
        }

        SubscriptBaseline = TextBaselineOffset + capsHeight / 2.0;
    }
}
