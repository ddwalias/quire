using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Python.Runtime;

namespace Quire.Controls.InputControls;

public static partial class PythonDetector
{
    private static Version minVersion = PythonEngine.MinSupportedVersion;

    private static Version maxVersion = PythonEngine.MaxSupportedVersion;

    private const string PythonExecutable = "python.exe";

    private static string[] possibleRegistryKeys = new string[3] { "SOFTWARE\\Python\\PythonCore", "SOFTWARE\\WOW6432Node\\Python\\PythonCore", "Software\\Python\\PythonCore" };

    public static string FindPythonPath()
    {
        string text = GetPythonPathFromCommonLocations();
        if (string.IsNullOrEmpty(text))
        {
            text = GetPythonPathFromEnvironmentVariables();
        }
        if (string.IsNullOrEmpty(text))
        {
            text = GetPythonInstallationPath();
        }
        return FindPythonDllPath(text);
    }

    private static string GetPythonPathFromEnvironmentVariables()
    {
        string environmentVariable = Environment.GetEnvironmentVariable("PYTHONHOME");
        if (!string.IsNullOrEmpty(environmentVariable) && IsValidPythonDirectory(environmentVariable))
        {
            return environmentVariable;
        }
        string environmentVariable2 = Environment.GetEnvironmentVariable("PYTHONPATH");
        if (!string.IsNullOrEmpty(environmentVariable2) && IsValidPythonDirectory(environmentVariable2))
        {
            return environmentVariable2;
        }
        string environmentVariable3 = Environment.GetEnvironmentVariable("PATH");
        if (environmentVariable3 != null)
        {
            string[] array = environmentVariable3.Split(';');
            foreach (string text in array)
            {
                if (IsValidPythonDirectory(text))
                {
                    return text;
                }
            }
        }
        return null;
    }

    private static string GetPythonPathFromCommonLocations()
    {
        string[] array = new string[2]
        {
            "C:\\Python",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs\\Python\\")
        };
        Version version = null;
        string result = null;
        foreach (string path in array)
        {
            if (!Directory.Exists(path))
            {
                continue;
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string text in directories)
            {
                if (IsValidPythonDirectory(text, out var pythonVersion) && (version == null || pythonVersion > version))
                {
                    version = pythonVersion;
                    result = text;
                }
            }
        }
        return result;
    }

    private static string ExtractVersionFromFolderName(string folderName)
    {
        int num = folderName.IndexOf("Python", StringComparison.OrdinalIgnoreCase) + "Python".Length;
        if (num >= folderName.Length)
        {
            return null;
        }
        string text = folderName.Substring(num);
        if (text.Length < 2)
        {
            return null;
        }
        return $"{text[0]}.{text.Substring(1)}";
    }

    public static bool IsVersionInRange(string targetVersionString)
    {
        if (Version.TryParse(targetVersionString, out Version result) && result >= minVersion)
        {
            return result <= maxVersion;
        }
        return false;
    }

    public static bool IsVersionInRange(string targetVersionString, out Version pythonVersion)
    {
        if (Version.TryParse(targetVersionString, out pythonVersion) && pythonVersion >= minVersion)
        {
            return pythonVersion <= maxVersion;
        }
        return false;
    }

    internal static bool IsValidPythonDirectory(string path)
    {
        if (Directory.Exists(path) && File.Exists(Path.Combine(path, "python.exe")))
        {
            return IsVersionInRange(GetPythonVersionFromExecutable(Path.Combine(path, "python.exe")));
        }
        return false;
    }

    internal static bool IsValidPythonDirectory(string path, out Version pythonVersion)
    {
        pythonVersion = null;
        if (Directory.Exists(path) && File.Exists(Path.Combine(path, "python.exe")))
        {
            return IsVersionInRange(GetPythonVersionFromExecutable(Path.Combine(path, "python.exe")), out pythonVersion);
        }
        return false;
    }

    internal static string FindPythonDllPath(string pythonPath)
    {
        string majorMinorVersion = GetMajorMinorVersion(GetPythonVersionFromExecutable(Path.Combine(pythonPath, "python.exe")));
        string path = "python" + majorMinorVersion.Replace(".", "") + ".dll";
        string text = Path.Combine(pythonPath, path);
        if (File.Exists(text))
        {
            return text;
        }
        return null;
    }

    public static string GetMajorMinorVersion(string versionString)
    {
        Match match = Regex.Match(versionString, "^\\d+\\.\\d+");
        if (match.Success)
        {
            return match.Value;
        }
        return string.Empty;
    }

    private static string GetPythonVersionFromExecutable(string pythonExecutablePath)
    {
        try
        {
            using Process process = Process.Start(new ProcessStartInfo
            {
                FileName = pythonExecutablePath,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            if (process != null)
            {
                string text = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return text.Split(' ')[1].Trim();
            }
        }
        catch
        {
        }
        return string.Empty;
    }
}
