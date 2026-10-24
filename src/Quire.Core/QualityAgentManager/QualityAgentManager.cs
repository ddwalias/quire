#nullable disable
using Quire.Mirror;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.QualityAgentManager;

public partial class QualityAgentManager
{
    private static string GetMathcadPrimeBitness()
    {
        string text = BuildArchitectureToBitnessConverter.Instance.Convert(global::Quire.Xml.XmlWorksheetData.Architecture, typeof(string), null, null).ToString();
        if (!string.IsNullOrEmpty(text))
        {
            return text;
        }
        return string.Empty;
    }

    private static void AddArgument(ref string commandLine, string name)
    {
        commandLine = string.Concat(commandLine, string.Format(" -{0}", name));
    }

    private static void AddArgument(ref string commandLine, string name, string data)
    {
        commandLine = string.Concat(commandLine, string.Format(" -{0} \"{1}\"", name, data));
    }

    private static string GetProdDcuPath(string mathcadPrimeInstallPath)
    {
        if (!string.IsNullOrEmpty(mathcadPrimeInstallPath))
        {
            return System.IO.Path.Combine(mathcadPrimeInstallPath, "prod_dcu.exe");
        }
        return string.Empty;
    }

    private string GetMathcadPrimeChecksum()
    {
        if (!_computeCheckSumOperationState.IsCompleted)
        {
            _computeCheckSumOperationState.Wait();
        }
        return _mathcadPrimeChecksum;
    }

    [MirrorVerified("Release IL")]
    public bool SendReport()
    {
        if (!IsReportSendingEnabled)
        {
            return false;
        }
        return SendReportHelper();
    }

    [MirrorVerified("Release IL")]
    public bool SendReport(out IQualityAgentReportData reportedData)
    {
        bool result = SendReportHelper();
        reportedData = _reportData;
        return result;
    }

    [MirrorVerified("Release IL")]
    private bool SendReportHelper()
    {
        try
        {
            string prodDcuPath = GetProdDcuPath(_mathcadPrimeInstallPath);
            if (string.IsNullOrEmpty(prodDcuPath))
            {
                return false;
            }
            string commandLine = GetCommandLine();
            if (string.IsNullOrEmpty(commandLine))
            {
                return false;
            }
            return new global::System.Diagnostics.Process
            {
                StartInfo =
                {
                    FileName = prodDcuPath,
                    Arguments = commandLine,
                    WindowStyle = global::System.Diagnostics.ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            }.Start();
        }
        catch
        {
            return false;
        }
    }

    [MirrorVerified("Release IL")]
    private void ComputeCheckSum()
    {
        if (!global::System.IO.Directory.Exists(_mathcadPrimeInstallPath))
        {
            _mathcadPrimeChecksum = "1234567890123456789012345678901234567890123456789012345678901234";
            return;
        }
        try
        {
            byte[] array;
            using (global::System.Security.Cryptography.HashAlgorithm hashAlgorithm = global::Quire.Services.AppServiceRegistry.GetService<global::Quire.IHashAlgorithmProvider>().GetDisposableHash())
            {
                global::System.Collections.Generic.List<byte> list = new global::System.Collections.Generic.List<byte>();
                AddHashesForFileExt(list, hashAlgorithm, "*.exe");
                AddHashesForFileExt(list, hashAlgorithm, "*.dll");
                hashAlgorithm.Initialize();
                array = hashAlgorithm.ComputeHash(list.ToArray());
            }
            global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            _mathcadPrimeChecksum = stringBuilder.ToString();
        }
        catch
        {
            _mathcadPrimeChecksum = "1234567890123456789012345678901234567890123456789012345678901234";
        }
    }

    [MirrorVerified("Release IL")]
    private void AddHashesForFileExt(global::System.Collections.Generic.List<byte> hashesList, global::System.Security.Cryptography.HashAlgorithm hashAlgorithm, string filesFilter)
    {
        string[] files = global::System.IO.Directory.GetFiles(_mathcadPrimeInstallPath, filesFilter);
        foreach (string path in files)
        {
            hashAlgorithm.Initialize();
            using global::System.IO.FileStream inputStream = global::System.IO.File.OpenRead(path);
            hashesList.AddRange(hashAlgorithm.ComputeHash(inputStream));
        }
    }

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    private static string GetMathcadPrimeInstallationDate() => MsiHelper.GetProductInfo(MsiHelper.MathcadCurrentReleaseProductCode, MsiHelper.InstallProperty.InstallDate);

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    public bool IsReportSendingEnabled { private get; set; }

    [MirrorVerified("Release IL")]
    private static string GetMathcadPrimeVersion() => string.Format("{0}", WpfUtils.ApplicationName).Replace(" ", "_");
}
