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

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    private static string GetMathcadPrimeInstallationDate() => MsiHelper.GetProductInfo(MsiHelper.MathcadCurrentReleaseProductCode, MsiHelper.InstallProperty.InstallDate);

    [MirrorVerified("PtcQualityAgentManagerQualityAgentManagerMechanicalDifferentialTests")]
    public bool IsReportSendingEnabled { private get; set; }

    [MirrorVerified("Release IL")]
    private static string GetMathcadPrimeVersion() => string.Format("{0}", WpfUtils.ApplicationName).Replace(" ", "_");
}
