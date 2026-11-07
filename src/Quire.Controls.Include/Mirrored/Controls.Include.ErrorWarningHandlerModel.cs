using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire;
using Quire.Controls.Core;
using Quire.IO;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Include;

internal partial class ErrorWarningHandlerModel
{
    private ErrorsList _errorsList;
    private WarningsList _warningsList;
    private readonly ResourceDictionary _resources;
    private readonly Func<string> _getCwd;
    private string _pathIndexes;
    public const string WarningCodesSeparator = "#";

    internal string PathIndexes
    {
        set
        {
            if (value == null)
            {
                return;
            }
            _pathIndexes = value;
            FilterOutNotOwnErrorsWarnings();
        }
    }

    internal ErrorsList ErrorsList
    {
        get => _errorsList;
        set
        {
            if (_errorsList.Count > 0)
            {
                _errorsList.Clear();
            }
            _errorsList = value;
            if (_errorsList.Count > 0)
            {
                UpdateErrorStruct();
            }
        }
    }

    internal WarningsList WarningsList
    {
        get => _warningsList;
        set
        {
            if (_warningsList.Count > 0)
            {
                _warningsList.Clear();
            }
            _warningsList = value;
            if (_warningsList.Count > 0)
            {
                UpdateWarningStruct();
            }
        }
    }

    internal ErrorWarningHandlerModel(ResourceDictionary resources, Func<string> getCwd)
    {
        _resources = resources;
        _getCwd = getCwd;
        _errorsList = new ErrorsList();
        _warningsList = new WarningsList();
        _pathIndexes = null;
    }

    internal includeWorksheetErrorWarningData[] GetIncludeWorksheetErrorWarningData(IList list)
    {
        if (list.Count > 0)
        {
            return (from IncludeWorksheetErrorWarningData item in list
                select new includeWorksheetErrorWarningData
                {
                    code = item.Code,
                    fileName = GetFilePathRelativeToCwd(new Pair<string, bool>(item.FileName, item.IsPathRooted)),
                    indexes = item.PathIndexes
                }).ToArray();
        }
        return null;
    }

    internal IList SetIncludeWorksheetErrorWarningData(includeWorksheetErrorWarningData[] datas)
    {
        if (datas.Length != 0)
        {
            return (from includeWorksheetErrorWarningData item in datas
                select new IncludeWorksheetErrorWarningData
                {
                    Code = item.code,
                    FileName = item.fileName,
                    IsPathRooted = true,
                    PathIndexes = item.indexes
                }).ToList();
        }
        return null;
    }

    internal void UpdateErrors(string pathIndexes, string errorCode, Pair<string, bool> fileName)
    {
        UpdateErrorsWarnings(pathIndexes, new List<string> { errorCode }, fileName, NodeMessageAdornerType.Error);
    }

    internal void UpdateWarnings(string pathIndexes, List<string> codes, Pair<string, bool> fileName)
    {
        UpdateErrorsWarnings(pathIndexes, codes, fileName, NodeMessageAdornerType.Warning);
    }

    internal void ClearErrors()
    {
        ErrorsList.Clear();
    }

    internal void ClearWarnings()
    {
        WarningsList.Clear();
    }

    private void FilterOutNotOwnErrorsWarnings()
    {
        ValidateErrorsWarningsList(ErrorsList);
        ErrorsList.RemoveAll((ErrorOnIncludeWorksheet error) => !string.Equals(error.PathIndexes, _pathIndexes));
        ValidateErrorsWarningsList(WarningsList);
        WarningsList.RemoveAll((WarningOnIncludeWorksheet warning) => !string.Equals(warning.PathIndexes, _pathIndexes));
    }

    private void ValidateErrorsWarningsList(IEnumerable<IncludeWorksheetErrorWarningData> list)
    {
        IEnumerable<IncludeWorksheetErrorWarningData> enumerable = list.Where((IncludeWorksheetErrorWarningData error) => error.PathIndexes == null);
        DebugTools.DevAssert(enumerable.Count() == 0, null);
        enumerable.Each((IncludeWorksheetErrorWarningData error) =>
        {
            error.PathIndexes = _pathIndexes;
        });
    }

    private void UpdateErrorsWarnings(string pathIndexes, List<string> codes, Pair<string, bool> fileName, NodeMessageAdornerType type)
    {
        if (codes.Count == 0)
        {
            return;
        }
        IList list;
        DelegateFunction3<IncludeWorksheetErrorWarningData, string, List<string>, Pair<string, bool>> delegateFunction;
        DelegateFunction2<bool, string, string> delegateFunction2;
        if (type == NodeMessageAdornerType.Error)
        {
            list = ErrorsList;
            delegateFunction = CreateError;
            delegateFunction2 = ShouldRemovePreviousError;
        }
        else if (type == NodeMessageAdornerType.Warning)
        {
            list = WarningsList;
            delegateFunction = CreateWarning;
            delegateFunction2 = delegate
            {
                return true;
            };
        }
        else
        {
            return;
        }
        IncludeWorksheetErrorWarningData includeWorksheetErrorWarningData2 = null;
        foreach (IncludeWorksheetErrorWarningData item in list)
        {
            if (item.PathIndexes == pathIndexes && (IsResetCode(codes) || delegateFunction2(codes[0], item.Code)))
            {
                includeWorksheetErrorWarningData2 = item;
            }
        }
        if (includeWorksheetErrorWarningData2 != null)
        {
            list.Remove(includeWorksheetErrorWarningData2);
        }
        if (!IsResetCode(codes) && !IsCalculationSuccedErrorCode(codes))
        {
            list.Add(delegateFunction(pathIndexes, codes, fileName));
        }
    }

    private static bool IsResetCode(IList<string> codes)
    {
        if (codes.Count == 1)
        {
            return codes[0] == IncludeWorksheetErrorWarningData.ResetCode;
        }
        return false;
    }

    private static bool IsCalculationSuccedErrorCode(IList<string> codes)
    {
        if (codes.Count == 1)
        {
            return codes[0] == "CalculationSucceededErrorCode";
        }
        return false;
    }

    private string GetFilePathRelativeToCwd(Pair<string, bool> fileName)
    {
        string first = fileName.First;
        bool second = fileName.Second;
        string text = string.Empty;
        if (second)
        {
            text = first;
        }
        else
        {
            string text2 = _getCwd();
            if (text2 != null)
            {
                text = FileUtils.EvaluateRelativePath(text2, first, stripSingleLeadingDot: true);
            }
        }
        DebugTools.DevAssert(text != null, null);
        return text;
    }

    private string FindInResource(string resourceCode, int inlineParamCount)
    {
        if (_resources.Contains(resourceCode))
        {
            return (string)_resources[resourceCode];
        }
        for (int i = 0; i < inlineParamCount; i++)
        {
            resourceCode = resourceCode + "_{" + i + "}";
        }
        return resourceCode;
    }

    private string GetErrorTextByCode(string errorCode, Pair<string, bool> fileName)
    {
        string result = "";
        string text = FindInResource(errorCode, 1);
        if (text != null)
        {
            string filePathRelativeToCwd = GetFilePathRelativeToCwd(fileName);
            result = string.Format(text, filePathRelativeToCwd);
        }
        return result;
    }

    private static bool ShouldRemovePreviousError(string newErrorCode, string oldErrorCode)
    {
        if (newErrorCode.Equals("IncludeControlCalculationErrors"))
        {
            if (newErrorCode.Equals("CalculationSucceededErrorCode") || newErrorCode.Equals("IncludeControlCalculationErrors"))
            {
                return oldErrorCode.Equals("IncludeControlCalculationErrors");
            }
            return false;
        }
        return true;
    }

    private IncludeWorksheetErrorWarningData CreateError(string indexes, List<string> codes, Pair<string, bool> fileName)
    {
        return new ErrorOnIncludeWorksheet
        {
            PathIndexes = indexes,
            FileName = fileName.First,
            Code = codes[0],
            Text = GetErrorTextByCode(codes[0], fileName),
            IsPathRooted = fileName.Second
        };
    }

    private void UpdateErrorStruct()
    {
        foreach (ErrorOnIncludeWorksheet errors in _errorsList)
        {
            if (!string.IsNullOrEmpty(errors.Code) && string.IsNullOrEmpty(errors.Text))
            {
                errors.Text = GetErrorTextByCode(errors.Code, new Pair<string, bool>(errors.FileName, errors.IsPathRooted));
            }
        }
    }

    private string GetWarningTextByCodes(List<string> codes, Pair<string, bool> fileName)
    {
        if (IncludeWorksheetErrorWarningData.IsCacheWarningCode(codes[0]))
        {
            return FindInResource(codes[0], 0);
        }
        return GetWarningTextOnDiffSettings(codes, fileName);
    }

    private IncludeWorksheetErrorWarningData CreateWarning(string indexes, List<string> codes, Pair<string, bool> fileName)
    {
        return new WarningOnIncludeWorksheet
        {
            Code = (IncludeWorksheetErrorWarningData.IsCacheWarningCode(codes[0]) ? codes[0] : GetWarningCodeForDiffSetting(codes)),
            PathIndexes = indexes,
            Text = GetWarningTextByCodes(codes, fileName),
            WarningCount = codes.Count,
            FileName = fileName.First,
            IsPathRooted = fileName.Second
        };
    }

    private static string GetWarningCodeForDiffSetting(List<string> codes)
    {
        string text = "IncludeControlWarningsConflict";
        List<string>.Enumerator enumerator = codes.GetEnumerator();
        while (enumerator.MoveNext())
        {
            text = text + "#" + enumerator.Current;
        }
        return text;
    }

    private static List<string> ParseCode(string code)
    {
        List<string> list = new List<string>();
        int num = 0;
        int num2 = 0;
        while (num2 >= 0)
        {
            num2 = code.IndexOf("#", num);
            list.Add((num2 > num) ? code.Substring(num, num2 - num) : code.Substring(num));
            num = num2 + 1;
        }
        if (list.Count > 1)
        {
            list.RemoveAt(0);
        }
        return list;
    }

    public static string CleanUpLegacyWarnings(string warningCode)
    {
        string result = warningCode;
        if (warningCode.Contains("IncludeControlWarningsConflictPropertyNameLegacySymbolicEngine"))
        {
            result = warningCode.Replace("#IncludeControlWarningsConflictPropertyNameLegacySymbolicEngine", string.Empty);
            result = ((result == "IncludeControlWarningsConflict") ? null : result);
        }
        return result;
    }

    private void UpdateWarningStruct()
    {
        foreach (WarningOnIncludeWorksheet warnings in _warningsList)
        {
            if (!string.IsNullOrEmpty(warnings.Code) && string.IsNullOrEmpty(warnings.Text))
            {
                List<string> list = ParseCode(warnings.Code);
                warnings.WarningCount = list.Count;
                warnings.Text = GetWarningTextByCodes(list, new Pair<string, bool>(warnings.FileName, warnings.IsPathRooted));
            }
        }
    }

    private string GetWarningTextOnDiffSettings(List<string> codes, Pair<string, bool> fileName)
    {
        if (codes.Count == 0)
        {
            return null;
        }
        int count = 0;
        string text = codes.Where(IncludeWorksheetErrorWarningData.IsConflictWarningCode).Aggregate("", (string current, string code) => current + ((count++ > 0) ? ", " : "") + FindInResource(code, 0));
        DebugTools.DevAssert(count == codes.Count, null);
        text = text + " " + ((count > 1) ? FindInResource("IncludeControlWarningsConflictSettings", 0) : FindInResource("IncludeControlWarningsConflictSetting", 0));
        string filePathRelativeToCwd = GetFilePathRelativeToCwd(fileName);
        return string.Format(FindInResource("IncludeControlWarningsConflict", 0), text, filePathRelativeToCwd);
    }
}
