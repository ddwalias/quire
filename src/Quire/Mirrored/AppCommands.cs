#nullable disable

using System;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.MruFiles;
using Quire.Wpf;

namespace Quire;

public static partial class AppCommands
{
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
