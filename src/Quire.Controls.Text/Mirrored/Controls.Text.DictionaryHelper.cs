#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Quire.Controls.Text.Properties;
using Quire.IO;

namespace Quire.Controls.Text;

public static partial class DictionaryHelper
{
    internal const int MaxDictionaryWordLength = 128;

    internal static readonly TextFromRunsStops DictionaryWordStopFlags;

    public static Uri WpfSpellerCustomDictionaryUri { get; }

    private static string WindowsSpellerCustomDictionaryPath { get; }

    public static bool IsWindowsSpellerCustomDictionaryValid => EnumerableExtensions.IsNotEmpty(WindowsSpellerCustomDictionaryPath);

    static DictionaryHelper()
    {
        DictionaryWordStopFlags = TextFromRunsStops.StopOnWhitespace | TextFromRunsStops.StopOnPunctuation;
        DebugTools.DevAssert(DictionaryWordStopFlags == (TextFromRunsStops.StopOnWhitespace | TextFromRunsStops.StopOnPunctuation));
        string dictionaryPath = FileUtils.GetLocalApplicationDataPath() + "\\SpellCheck\\MathcadDictionary.lex";
        Settings settings = Settings.Default;
        Uri uri;
        if (!Uri.TryCreate(Environment.ExpandEnvironmentVariables(settings.DictionaryPath), UriKind.Absolute, out uri) || !uri.IsFile)
        {
            DebugTools.FailAssertion(settings.DictionaryPath);
            uri = new Uri(dictionaryPath, UriKind.Absolute);
        }
        WpfSpellerCustomDictionaryUri = uri;
        if (DebugTools.IsDebug && Assembly.GetEntryAssembly()?.GetName().Name == "MathcadPrime")
        {
            string defaultPath = settings.Properties["DictionaryPath"]?.DefaultValue as string;
            if (defaultPath != null)
            {
                defaultPath = Environment.ExpandEnvironmentVariables(defaultPath);
            }
            else
            {
                defaultPath = string.Empty;
            }
            DebugTools.DevAssert(dictionaryPath.Equals(defaultPath), dictionaryPath + ", " + defaultPath);
        }
        string fileName = Path.GetFileNameWithoutExtension(WpfSpellerCustomDictionaryUri.LocalPath);
        WindowsSpellerCustomDictionaryPath = settings.UseWindowsDictionary && SystemInfo.IsWindows8Point1orGreater() && DebugTools.DevAssert(EnumerableExtensions.IsNotEmpty(fileName)).Passed
            ? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Spelling\\" + fileName + ".dic"
            : string.Empty;
        try
        {
            IList<string> lines = File.Exists(WpfSpellerCustomDictionaryUri.LocalPath)
                ? File.ReadAllLines(WpfSpellerCustomDictionaryUri.LocalPath, Encoding.UTF8).ToList()
                : new List<string>();
            SynchronizeWindowsSpellerCustomDictionary(ref lines);
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertion(ex);
        }
    }

    public static bool AddWordToDictionary(string word) => UpdateCustomDictionary(word, null);

    public static bool RemoveFromDictionary(string word) => UpdateCustomDictionary(null, word);

    private static bool UpdateCustomDictionary(string wordToAdd, string wordToRemove)
    {
        bool addValid = EnumerableExtensions.IsNotEmpty(wordToAdd);
        bool removeValid = EnumerableExtensions.IsNotEmpty(wordToRemove);
        if (DebugTools.DevAssert(addValid | removeValid).Failed)
        {
            return false;
        }
        string localPath = WpfSpellerCustomDictionaryUri.LocalPath;
        bool result;
        try
        {
            string directory = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            List<string> lines = File.Exists(localPath) ? File.ReadAllLines(localPath, Encoding.UTF8).ToList() : new List<string>();
            if (removeValid)
            {
                lines.RemoveAll(word => word.Equals(wordToRemove, StringComparison.Ordinal));
            }
            if (addValid && lines.Find(word => word.Equals(wordToAdd, StringComparison.Ordinal)) == null)
            {
                lines.Add(wordToAdd);
            }
            File.WriteAllLines(localPath, lines, Encoding.UTF8);
            IList<string> updated = lines;
            result = SynchronizeWindowsSpellerCustomDictionary(ref updated);
        }
        catch (Exception ex)
        {
            result = false;
            DebugTools.FailAssertion(ex);
        }
        return result;
    }

    internal static bool SynchronizeWindowsSpellerCustomDictionary(ref IList<string> lines)
    {
        if (DebugTools.AssertNotNull(lines).Failed)
        {
            return false;
        }
        bool result;
        try
        {
            if (EnumerableExtensions.IsNotEmpty(WindowsSpellerCustomDictionaryPath))
            {
                string directory = Path.GetDirectoryName(WindowsSpellerCustomDictionaryPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                if (lines.Count != 0)
                {
                    File.WriteAllLines(WindowsSpellerCustomDictionaryPath, lines, Encoding.Unicode);
                }
                else if (File.Exists(WindowsSpellerCustomDictionaryPath))
                {
                    File.Delete(WindowsSpellerCustomDictionaryPath);
                }
            }
            result = true;
        }
        catch (Exception ex)
        {
            result = false;
            DebugTools.FailAssertion(ex);
        }
        return result;
    }
}
