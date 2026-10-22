#nullable disable
#pragma warning disable

namespace Quire.QualityAgentManager;

partial interface IQualityAgentManager
{
    bool IsReportSendingEnabled { set; }
    bool SendReport() ;
    bool SendReport(out global::Quire.QualityAgentManager.IQualityAgentReportData reportedData) ;
}
