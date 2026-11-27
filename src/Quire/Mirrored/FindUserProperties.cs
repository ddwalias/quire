#nullable disable

using System.Collections.Generic;
using System.Windows;
using Quire.Controls.Core;

namespace Quire;

public partial class FindUserProperties
{
    public FindOptions Options
    {
        get => (FindOptions)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public FindReplaceStatus Status
    {
        get => (FindReplaceStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string FindText
    {
        get => (string)GetValue(SearchTextProperty);
        set => SetValue(SearchTextProperty, value);
    }

    public string ReplaceText
    {
        get => (string)GetValue(ReplaceTextProperty);
        set => SetValue(ReplaceTextProperty, value);
    }

    public List<Pair<int, int>> FindTextSubscriptPositions
    {
        get => (List<Pair<int, int>>)GetValue(FindTextSubscriptPositionsProperty);
        set => SetValue(FindTextSubscriptPositionsProperty, value);
    }

    public List<Pair<int, int>> ReplaceTextSubscriptPositions
    {
        get => (List<Pair<int, int>>)GetValue(ReplaceTextSubscriptPositionsProperty);
        set => SetValue(ReplaceTextSubscriptPositionsProperty, value);
    }

    public bool FindReplaceIsActive
    {
        get => (bool)GetValue(FindReplaceIsActiveProperty);
        set => SetValue(FindReplaceIsActiveProperty, value);
    }

    public bool IsSubscriptAndActive
    {
        get => (bool)GetValue(IsSubscriptAndActiveProperty);
        set => SetValue(IsSubscriptAndActiveProperty, value);
    }

    public FindUserProperties()
    {
        FindTextSubscriptPositions = new List<Pair<int, int>>();
        ReplaceTextSubscriptPositions = new List<Pair<int, int>>();
    }

    private static void OnOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FindUserProperties findUserProperties = d as FindUserProperties;
        DebugTools.DevAssert(findUserProperties != null, null);
        findUserProperties.Status = FindReplaceStatus.Empty;
        if (((findUserProperties.Options & FindOptions.FindInMath) | (findUserProperties.Options & FindOptions.FindInText)) == FindOptions.None)
        {
            FindOptions findOptions = (FindOptions)e.NewValue ^ (FindOptions)e.OldValue;
            DebugTools.DevAssert((findOptions & (FindOptions.FindInMath | FindOptions.FindInText)) != 0, null);
            findUserProperties.Options |= (FindOptions)(((findOptions & FindOptions.FindInMath) != 0) ? 2 : 1);
        }
    }

    private static void OnFindTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(d != null && d is FindUserProperties, null);
        (d as FindUserProperties).Status = FindReplaceStatus.Empty;
    }
}
