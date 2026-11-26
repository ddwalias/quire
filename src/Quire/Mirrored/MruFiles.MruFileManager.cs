using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Quire.Controls;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.MruFiles;

public partial class MruFileManager
{
    public const int TotalInList = 25;

    public const int ItemsToDisplay = 15;

    public const int TemplatesToDisplay = 9;

    public const int NumberOfMixedDocumentsToDisplayWithNoWindchill = 10;

    public const int NumberOfMixedDocumentsToDisplayWithWindchill = 9;

    public int MaxNumberOfPinnedItems = Math.Min(15, Math.Min(9, MixedDocumentsToDisplay));

    private IMruFilePersistanceManager userConfigPersistance;

    public static readonly DependencyProperty MruListProperty = DependencyProperty.Register("MruList", typeof(ObservableCollection<MruFileItem>), typeof(MruFileManager), new PropertyMetadata((object)new ObservableCollection<MruFileItem>()));

    public static readonly DependencyProperty RegularMruListProperty = DependencyProperty.Register("RegularMruList", typeof(ObservableCollection<MruFileItem>), typeof(MruFileManager), new PropertyMetadata((object)new ObservableCollection<MruFileItem>()));

    public static readonly DependencyProperty TemplateMruListProperty = DependencyProperty.Register("TemplateMruList", typeof(ObservableCollection<MruFileItem>), typeof(MruFileManager), new PropertyMetadata((object)new ObservableCollection<MruFileItem>()));

    public static readonly DependencyProperty MixedMruListProperty = DependencyProperty.Register("MixedMruList", typeof(ObservableCollection<MruFileItem>), typeof(MruFileManager), new PropertyMetadata((object)new ObservableCollection<MruFileItem>()));

    public static int MixedDocumentsToDisplay
    {
        get
        {
            if (IntegrationCommands.Instance == null || !IntegrationCommands.Instance.WcEnable)
            {
                return 10;
            }
            return 9;
        }
    }

    public ObservableCollection<MruFileItem> MruList
    {
        get => (ObservableCollection<MruFileItem>)GetValue(MruListProperty);
        set => SetValue(MruListProperty, value);
    }

    public ObservableCollection<MruFileItem> RegularMruList
    {
        get => (ObservableCollection<MruFileItem>)GetValue(RegularMruListProperty);
        set => SetValue(RegularMruListProperty, value);
    }

    public ObservableCollection<MruFileItem> TemplateMruList
    {
        get => (ObservableCollection<MruFileItem>)GetValue(TemplateMruListProperty);
        set => SetValue(TemplateMruListProperty, value);
    }

    public ObservableCollection<MruFileItem> MixedMruList
    {
        get => (ObservableCollection<MruFileItem>)GetValue(MixedMruListProperty);
        set => SetValue(MixedMruListProperty, value);
    }

    public int NumberedItems => 9;

    public MruFileManager()
    {
        userConfigPersistance = new MruUserConfigPersistance();
        SetMruCommandsFunctionalityParameters();
    }

    private static void SetMruCommandsFunctionalityParameters()
    {
        ((RoutedUICommandWithFunctionalityParameterKey)AppCommands.MruOpenEngineeringDocWithoutCommandExtender).AddFunctionalityKeyAccordingParameter(".mctx", EnumFunctionalityKey.OpenTemplateDocumentCommand);
        ((RoutedUICommandWithFunctionalityParameterKey)AppCommands.MruOpenEngineeringDocWithoutCommandExtender).AddFunctionalityKeyAccordingParameter(".*", EnumFunctionalityKey.OpenMcdxDocumentCommand);
        ((RoutedUICommandWithFunctionalityParameterKey)AppCommands.MruNewEngineeringDocWithoutCommandExtender).AddFunctionalityKeyAccordingParameter(".mctx", EnumFunctionalityKey.NewEngineeringDocumentFromTemplateCommand);
        ((RoutedUICommandWithFunctionalityParameterKey)AppCommands.MruNewEngineeringDocWithoutCommandExtender).AddFunctionalityKeyAccordingParameter(".*", EnumFunctionalityKey.OpenMcdxDocumentCommand);
    }

    public void onPinChanged(MruFileItem mruFileItem, bool pinned)
    {
        userConfigPersistance.Reload();
        RemoveMruItem(mruFileItem);
        int indexWhereTobeAdded = GetIndexOfLastPinnedItem() + 1;
        InsertMruItemAtSpecificSpot(indexWhereTobeAdded, mruFileItem);
        UpdateMruSubLists();
    }

    internal void AddMRUFileItemIfNeeded(string fullPathAndName) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void InsertMruItem(string fullPathAndName, MruFileItem item, MruFileTypes type)
    {
        MruFileItem mruFileItem = item ?? new MruFileItem(fullPathAndName, type);
        if (mruFileItem.IsPinned)
        {
            return;
        }
        int num = GetIndexOfLastPinnedItem() + 1;
        if (userConfigPersistance.MruFileItemsCollection.Contains(mruFileItem))
        {
            if (userConfigPersistance.MruFileItemsCollection.IndexOf(mruFileItem) == num)
            {
                return;
            }
            RemoveItem(mruFileItem);
        }
        InsertMruItemAtSpecificSpot(num, mruFileItem);
    }

    private void InsertMruItemAtSpecificSpot(int indexWhereTobeAdded, MruFileItem mruFileItem)
    {
        MruList.Insert(indexWhereTobeAdded, mruFileItem);
        userConfigPersistance.MruFileItemsCollection.Insert(indexWhereTobeAdded, mruFileItem);
        userConfigPersistance.Save();
    }

    private int GetIndexOfLastPinnedItem()
    {
        MruFileItem mruFileItem = userConfigPersistance.MruFileItemsCollection.LastOrDefault((MruFileItem item) => item.IsPinned);
        if (mruFileItem != null)
        {
            return userConfigPersistance.MruFileItemsCollection.IndexOf(mruFileItem);
        }
        return -1;
    }

    private void RemoveMruItem(MruFileItem mruItem)
    {
        MruFileItem fileItem = userConfigPersistance.MruFileItemsCollection.First((MruFileItem item) => string.Equals(item.FullPathAndFileName, mruItem.FullPathAndFileName, StringComparison.OrdinalIgnoreCase));
        MruList.Remove(mruItem);
        RemoveItem(fileItem);
    }

    public void FixAccessKey(IEnumerable<MruFileItem> list)
    {
        list.EachWithIndex((int index, MruFileItem item) =>
        {
            item.SetNumberedName(index);
        });
    }

    public void ClearPersistence(MruFileTypes type)
    {
        if (userConfigPersistance.MruFileItemsCollection != null)
        {
            ObservableCollection<MruFileItem> items = new ObservableCollection<MruFileItem>(userConfigPersistance.MruFileItemsCollection.Where((MruFileItem item) => (item.MruFileType & type) == 0));
            userConfigPersistance.MruFileItemsCollection.Clear();
            items.Each((MruFileItem item) =>
            {
                userConfigPersistance.MruFileItemsCollection.Add(item);
            });
            userConfigPersistance.Save();
        }
        else
        {
            userConfigPersistance.MruFileItemsCollection = new MruObservableCollection();
            userConfigPersistance.Save();
        }
    }

    private void RemoveItem(MruFileItem fileItem)
    {
        userConfigPersistance.MruFileItemsCollection.Remove(fileItem);
        userConfigPersistance.Save();
    }

    private MruFileItem GetMruFileItem(string fullPathAndName)
    {
        fullPathAndName = Path.GetFullPath(fullPathAndName);
        return userConfigPersistance.MruFileItemsCollection.FirstOrDefault((MruFileItem item) => item.FullPathAndFileName == fullPathAndName);
    }

    private MruFileItem RemoveLastNotPinnedItem()
    {
        MruFileItem mruFileItem = userConfigPersistance.MruFileItemsCollection.LastOrDefault((MruFileItem item) => !item.IsPinned);
        if (mruFileItem == null)
        {
            return null;
        }
        RemoveItem(mruFileItem);
        return mruFileItem;
    }

    public void Menu_Opened(object sender, RoutedEventArgs e)
    {
        LoadMruItemsAndRegisterToPinChanged();
    }

    public void LoadMruItemsAndRegisterToPinChanged()
    {
        userConfigPersistance.Reload();
        if (userConfigPersistance.MruFileItemsCollection == null)
        {
            userConfigPersistance.MruFileItemsCollection = new MruObservableCollection();
        }
        MruList = userConfigPersistance.GetAllItems();
        UpdateMruSubLists();
        foreach (MruFileItem mru in MruList)
        {
            mru.onPinChangedEvent += onPinChanged;
        }
        userConfigPersistance.Clear();
        userConfigPersistance.Reload();
    }

    private void UpdateMruSubLists()
    {
        RegularMruList = new ObservableCollection<MruFileItem>(MruList.Where((MruFileItem item) => (item.MruFileType & MruFileTypes.RegularMruFileType) != 0).Take(ItemsToDisplay));
        MixedMruList = new ObservableCollection<MruFileItem>(MruList.Where((MruFileItem item) => (item.MruFileType & MruFileTypes.MruFileTypeThatIsBothRegularAndTemplate) != 0).Take(MixedDocumentsToDisplay));
        TemplateMruList = new ObservableCollection<MruFileItem>(MruList.Where((MruFileItem item) => (item.MruFileType & MruFileTypes.NewFromTemplatesMruFileType) != 0).Take(TemplatesToDisplay));
    }

    static MruFileManager()
    {
    }
}
