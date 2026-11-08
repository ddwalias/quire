#nullable disable
using System.IO;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls;

public partial class MruFileItem
{
    public const int NumberedItems = 9;

    public static readonly DependencyProperty NumberedNameProperty = DependencyProperty.Register("NumberedName", typeof(string), typeof(MruFileItem));

    public static readonly DependencyProperty FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(MruFileItem));

    public static readonly DependencyProperty IsPinnedProperty = DependencyProperty.Register("IsPinned", typeof(bool), typeof(MruFileItem), new PropertyMetadata(IsPinned_Changed));

    public static readonly DependencyProperty FullPathAndFileNameProperty = DependencyProperty.Register("FullPathAndFileName", typeof(string), typeof(MruFileItem));

    public const int MaximumMruFileNameToDisplay = 27;

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public MruFileItem() { }

    public MruFileItem(string fullPathAndName)
    {
        FullPathAndFileName = Path.GetFullPath(fullPathAndName);
    }

    public MruFileItem(string fullPathAndName, bool isPinned)
        : this(fullPathAndName)
    {
        IsPinned = isPinned;
    }

    public MruFileItem(string fullPathAndName, MruFileTypes mruFileType)
        : this(fullPathAndName)
    {
        MruFileType = mruFileType;
    }

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public bool HasAccessKey => true;

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public MruFileTypes MruFileType { get; set; }

    [MirrorVerified("PtcControlsMruFileItemMechanicalDifferentialTests")]
    public event Delegate2<MruFileItem, bool> onPinChangedEvent;

    public string FullPathAndFileName
    {
        get => (string)GetValue(FullPathAndFileNameProperty);
        set => SetValue(FullPathAndFileNameProperty, value);
    }

    public bool IsPinned
    {
        get => (bool)GetValue(IsPinnedProperty);
        set => SetValue(IsPinnedProperty, value);
    }

    public string NumberedName
    {
        get => (string)GetValue(NumberedNameProperty);
        set => SetValue(NumberedNameProperty, value);
    }

    public string FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    private static void IsPinned_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is MruFileItem && (d as MruFileItem).onPinChangedEvent != null)
        {
            (d as MruFileItem).onPinChangedEvent.Invoke(d as MruFileItem, (bool)e.NewValue);
        }
    }

    public void SetNumberedName(int indexInList)
    {
        string fileName = Path.GetFileName(FullPathAndFileName);
        string displayName = fileName;
        if (fileName.Length > MaximumMruFileNameToDisplay)
        {
            displayName = "..." + fileName.Substring(fileName.Length - MaximumMruFileNameToDisplay, MaximumMruFileNameToDisplay);
        }
        NumberedName = indexInList + 1 > NumberedItems ? displayName : string.Format("_{0}. ", indexInList + 1) + displayName;
    }
}
