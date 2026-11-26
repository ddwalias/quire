using System;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.MruFiles;
using Quire.Wpf;

namespace Quire;

public static partial class AppCommands
{
    private static readonly Type _typeOfThis;

    public static readonly RoutedUICommand NewEngineeringDocument;

    public static readonly RoutedUICommand SaveEngineeringDocumentWithInputsOutputs;

    public static readonly RoutedUICommand SaveEngineeringDocumentWithoutInputsOutputs;

    public static readonly RoutedUICommand SaveEngineeringDocumentAs;

    public static readonly RoutedUICommand SaveEngineeringDocumentCopy;

    public static readonly RoutedUICommand SaveEngineeringDocumentAsXPS;

    public static readonly RoutedUICommand SaveEngineeringDocumentAsMCDX;

    public static readonly RoutedUICommand ShowSaveAsError;

    public static readonly RoutedUICommand OpenEngineeringDocument;

    public static readonly RoutedUICommand PrintEngineeringDocument;

    public static readonly RoutedUICommand CloseEngineeringDocument;

    public static readonly RoutedUICommandWithFunctionalityKey MoveTabCommand;

    public static readonly RoutedUICommandWithFunctionalityKey CloseTabsCommand;

    public static readonly RoutedUICommand MruOpenEngineeringDocWithoutCommandExtender;

    public static readonly RoutedUICommand MruNewEngineeringDocWithoutCommandExtender;

    public static readonly RoutedUICommand DisplayHelp;

    public static readonly RoutedUICommand DisplayTutorial;

    public static readonly RoutedUICommand DisplayAbout;

    public static readonly RoutedUICommand DisplayOptions;

    public static readonly RoutedUICommand DisplayHyperlink;

    public static readonly RoutedUICommand Exit;

    public static readonly RoutedUICommand DisplayFreemiumAboutBox;

    public static readonly RoutedUICommand DisplayLicenseStatusIfNeeded;

    public static readonly RoutedUICommand FindNext;

    public static readonly RoutedUICommand FindPrevious;

    public static readonly RoutedUICommand ReplaceAndFindNext;

    public static readonly RoutedUICommand GoToFind;

    public static readonly RoutedUICommand GoToPage;

    public static readonly RoutedUICommand WorksheetControlVisibleObjectsDebugging;

    public static readonly RoutedUICommand WorksheetControlUIDebugging;

    public static readonly RoutedUICommand ImportExcel;

    public static readonly RoutedUICommand OpenInputOutputDesignationWindow;

    public static readonly RoutedUICommand OpenFunctionBrowser;

    public static readonly RoutedUICommand LaunchConverter;

    public static readonly RoutedUICommand SaveEngineeringDocumentAsRtf;

    public static readonly RoutedUICommand SaveEngineeringDocumentAsPdf;

    public static readonly RoutedUICommand SaveEngineeringDocumentAsMCTX;

    public static readonly RoutedUICommand NewEngineeringDocumentFromExisting;

    public static readonly RoutedUICommand OpenTemplateDocument;

    static AppCommands()
    {
        _typeOfThis = typeof(AppCommands);
        NewEngineeringDocument = ApplicationCommands.New;
        SaveEngineeringDocumentWithInputsOutputs = ApplicationCommands.Save;
        SaveEngineeringDocumentWithoutInputsOutputs = new RoutedUICommand();
        SaveEngineeringDocumentAs = ApplicationCommands.SaveAs;
        SaveEngineeringDocumentCopy = new RoutedUICommand();
        SaveEngineeringDocumentAsXPS = new RoutedUICommand();
        SaveEngineeringDocumentAsMCDX = new RoutedUICommand();
        ShowSaveAsError = new RoutedUICommand();
        OpenEngineeringDocument = ApplicationCommands.Open;
        PrintEngineeringDocument = ApplicationCommands.Print;
        CloseEngineeringDocument = ApplicationCommands.Close;
        MoveTabCommand = new RoutedUICommandWithFunctionalityKey("Move Tab", "MoveTab", _typeOfThis, EnumFunctionalityKey.MoveTab);
        CloseTabsCommand = new RoutedUICommandWithFunctionalityKey("Close Tabs", "CloseTabs", _typeOfThis, EnumFunctionalityKey.CloseTabs);
        MruOpenEngineeringDocWithoutCommandExtender = new RoutedUICommandWithFunctionalityParameterKey("Mru Open Engineering Doc Without CommandExtender", "MruOpenEngineeringDocWithoutCommandExtender", _typeOfThis, new MruFileExtensionComparer());
        MruNewEngineeringDocWithoutCommandExtender = new RoutedUICommandWithFunctionalityParameterKey("Mru New Engineering Doc Without CommandExtender", "Mru NewEngineeringDocWithoutCommandExtender", _typeOfThis, new MruFileExtensionComparer());
        DisplayHelp = new RoutedUICommand();
        DisplayTutorial = new RoutedUICommand();
        DisplayAbout = new RoutedUICommand();
        DisplayOptions = new RoutedUICommand();
        DisplayHyperlink = new RoutedUICommand();
        Exit = new RoutedUICommand();
        DisplayFreemiumAboutBox = FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox;
        DisplayLicenseStatusIfNeeded = new RoutedUICommand();
        FindNext = new RoutedUICommand();
        FindPrevious = new RoutedUICommand();
        ReplaceAndFindNext = new RoutedUICommand();
        GoToFind = new RoutedUICommand();
        GoToPage = new RoutedUICommand();
        WorksheetControlVisibleObjectsDebugging = new RoutedUICommand("Visible Objects Debugging Command", "VisibleObjectsCommand", _typeOfThis);
        WorksheetControlUIDebugging = new RoutedUICommand("WorksheetControl UI Debugging Command", "WorksheetControlUICommand", _typeOfThis);
        ImportExcel = new RoutedUICommandWithFunctionalityKey("Import Excel", "ImportExcel", _typeOfThis, EnumFunctionalityKey.FileAccessFunctionsReadExcelCommand);
        OpenInputOutputDesignationWindow = new RoutedUICommandWithFunctionalityKey("Open Input Output Designation Window", "OpenInputOutputDesignationWindow", _typeOfThis, EnumFunctionalityKey.CreoElementsProIntegrationShowAsListCommand);
        OpenFunctionBrowser = new RoutedUICommandWithFunctionalityKey("Open Function Browser", "OpenFunctionBrowser", _typeOfThis, EnumFunctionalityKey.AllFunctionsCategory);
        LaunchConverter = new RoutedUICommandWithFunctionalityKey("Launch Converter", "LaunchConverter", _typeOfThis, EnumFunctionalityKey.WorksheetConverterCommand);
        SaveEngineeringDocumentAsRtf = new RoutedUICommandWithFunctionalityKey("Save Engineering Document As RTF", "SaveEngineeringDocumentAsRtf", _typeOfThis, EnumFunctionalityKey.SaveEngineeringDocumentAsRtfCommand);
        SaveEngineeringDocumentAsPdf = new RoutedUICommandWithFunctionalityKey("Save Engineering Document As PDF", "SaveEngineeringDocumentAsPdf", _typeOfThis, EnumFunctionalityKey.SaveEngineeringDocumentAsPdfCommand);
        SaveEngineeringDocumentAsMCTX = new RoutedUICommandWithFunctionalityKey("Save Engineering Document As MCTX", "SaveEngineeringDocumentAsMCTX", _typeOfThis, EnumFunctionalityKey.SaveEngineeringDocumentAsMCTXCommand);
        NewEngineeringDocumentFromExisting = new RoutedUICommandWithFunctionalityKey("New Engineering Document From Existing", "NewEngineeringDocumentFromExisting", _typeOfThis, EnumFunctionalityKey.NewEngineeringDocumentFromTemplateCommand);
        OpenTemplateDocument = new RoutedUICommandWithFunctionalityKey("Open Template Document", "OpenTemplateDocument", _typeOfThis, EnumFunctionalityKey.OpenTemplateDocumentCommand);
        DisplayAbout.InputGestures.Add(Gestures.CtrlAltF1);
        DisplayHelp.InputGestures.Add(Gestures.F1);
        ApplicationCommands.New.InputGestures.Clear();
        ApplicationCommands.New.InputGestures.Add(Gestures.CtrlN);
        ApplicationCommands.Save.InputGestures.Clear();
        ApplicationCommands.Save.InputGestures.Add(Gestures.CtrlS);
        ApplicationCommands.Open.InputGestures.Clear();
        ApplicationCommands.Open.InputGestures.Add(Gestures.CtrlO);
        ApplicationCommands.Close.InputGestures.Clear();
        ApplicationCommands.Close.InputGestures.Add(Gestures.CtrlF4);
        ApplicationCommands.Close.InputGestures.Add(Gestures.CtrlW);
        GoToFind.InputGestures.Add(Gestures.CtrlF);
        GoToPage.InputGestures.Add(Gestures.CtrlAltG);
        FindNext.InputGestures.Add(Gestures.F3);
        ApplicationCommands.Cut.InputGestures.Add(Gestures.ShiftDelete);
        ApplicationCommands.Copy.InputGestures.Add(Gestures.CtrlInsert);
        ApplicationCommands.Paste.InputGestures.Add(Gestures.ShiftInsert);
        ApplicationCommands.Undo.InputGestures.Add(Gestures.AltBack);
        ApplicationCommands.Redo.InputGestures.Add(Gestures.AltInsert);
        CommandManager.RegisterClassInputBinding(typeof(SpiritMainWindow), new InputBinding(new InputBindingFunctionalityCommandWrapper(OpenFunctionBrowser), Gestures.F2));
    }
}
