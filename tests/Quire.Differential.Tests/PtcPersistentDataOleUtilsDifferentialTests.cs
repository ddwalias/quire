#if WINDOWS
using System.Reflection;
using Quire.PersistentData;

namespace Quire.Differential.Tests;

/// <summary>Directly compares OleUtils methods with Ptc.Core. COM registration cases compare this machine's results.</summary>
public class PtcPersistentDataOleUtilsDifferentialTests
{
    private const string Contract = "Ptc.PersistentData.OleUtils";
    private const BindingFlags AllStatic = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private static System.Type Q => typeof(OleUtils);
    private static System.Type M => Mathcad.Type("Ptc.Core", "Ptc.PersistentData.OleUtils");

    [SkippableFact]
    public void OleUtils_methods_match_PtcCore()
    {
        Mathcad.Require();
        var q = Q;
        var m = M;

        MethodComparison.Compare(".cctor static GUID and conversion map initialization", Contract,
            q.TypeInitializer!, null, [], m.TypeInitializer!, null, [], _ => "void", _ => "void");
        foreach (string field in new[]
        {
            "_excel8Guid", "_excel12Guid", "_excelMacroEnabled12Guid", "_excelBinaryMacroEnabled12Guid",
            "_chartAppOlePrime5Guid", "_chartAppOlePrime6Guid", "_chartAppOlePrime7Guid", "_chartAppOlePrime8Guid",
            "_chartAppOlePrime9Guid", "_chartAppOlePrime10Guid", "_chartAppOlePrime11Guid", "_equation1Guid",
            "_equation2Guid", "_equation3Guid", "ChartAppOleGuidsConversionDict",
        })
            Assert.Equal(Reflect.Text(Reflect.Get(m, field)), Reflect.Text(Reflect.Get(q, field)));

        MethodInfo QMethod(string name, params System.Type[] parameters) => q.GetMethod(name, AllStatic, null, parameters, null)!;
        MethodInfo MMethod(string name, params System.Type[] parameters) => m.GetMethod(name, AllStatic, null, parameters, null)!;
        MethodComparison.Compare("IsMicrosoftEquationGuid known and unknown", Contract,
            QMethod("IsMicrosoftEquationGuid", typeof(Guid)), null, [Guid.Parse("0003000B-0000-0000-C000-000000000046")],
            MMethod("IsMicrosoftEquationGuid", typeof(Guid)), null, [Guid.Parse("0003000B-0000-0000-C000-000000000046")]);
        foreach (Guid guid in new[] { Guid.Empty, Guid.NewGuid(), Guid.Parse("00021700-0000-0000-C000-000000000046"), Guid.Parse("0002CE02-0000-0000-C000-000000000046") })
            MethodComparison.Compare($"IsMicrosoftEquationGuid({guid})", Contract,
                QMethod("IsMicrosoftEquationGuid", typeof(Guid)), null, [guid],
                MMethod("IsMicrosoftEquationGuid", typeof(Guid)), null, [guid]);

        foreach (Guid guid in new[]
        {
            Guid.Empty,
            Guid.Parse("00020820-0000-0000-C000-000000000046"),
            Guid.Parse("00020830-0000-0000-C000-000000000046"),
            Guid.Parse("00020832-0000-0000-C000-000000000046"),
            Guid.Parse("00020833-0000-0000-C000-000000000046"),
            Guid.NewGuid(),
        })
            MethodComparison.Compare($"IsSupportedExcelGuid({guid})", Contract,
                QMethod("IsSupportedExcelGuid", typeof(Guid)), null, [guid],
                MMethod("IsSupportedExcelGuid", typeof(Guid)), null, [guid]);

        foreach (string? progId in new string?[] { "Excel.Sheet", "Excel.Sheet.12", "NoSuchQuireOleProgId", "", null })
            MethodComparison.Compare($"GetExcelSheetGuid({Reflect.Text(progId)})", Contract,
                QMethod("GetExcelSheetGuid", typeof(string)), null, [progId],
                MMethod("GetExcelSheetGuid", typeof(string)), null, [progId], Reflect.Text, Reflect.Text);

        MethodComparison.Compare("CanCreateExcelContainer with local COM registry", Contract,
            QMethod("CanCreateExcelContainer"), null, [], MMethod("CanCreateExcelContainer"), null, []);
        MethodComparison.Compare("get_IsExcel8Installed with local COM registry", Contract,
            QMethod("get_IsExcel8Installed"), null, [], MMethod("get_IsExcel8Installed"), null, []);

        foreach (string? progId in new string?[]
        {
            null, "", "Mathcad", "Mathcad.Prime.Application", "OrgPlusWOPX", "OrgPlusWOPX.something",
            "AcroExch.Document.7", "PDFPlus.Document", "MSGraph.Chart", "PowerPoint.Application",
            "Word.Document.12", "Excel.Sheet.12", "Anything.Excel", "gibberish", ".Excel.Sheet",
        })
            MethodComparison.Compare($"IsOleAppSupportZoom({Reflect.Text(progId)})", Contract,
                QMethod("IsOleAppSupportZoom", typeof(string)), null, [progId],
                MMethod("IsOleAppSupportZoom", typeof(string)), null, [progId]);

        var qProgIdFromClsid = QMethod("ProgIDFromCLSID", typeof(Guid).MakeByRefType());
        var mProgIdFromClsid = MMethod("ProgIDFromCLSID", typeof(Guid).MakeByRefType());
        foreach (Guid clsid in new[] { Guid.NewGuid(), Guid.Parse("00020820-0000-0000-C000-000000000046") })
            MethodComparison.Compare($"ProgIDFromCLSID({clsid}) with local COM registry", Contract,
                qProgIdFromClsid, null, [clsid], mProgIdFromClsid, null, [clsid]);

        var qVersion = QMethod("IsVersionInstalled", typeof(object));
        var mVersion = MMethod("IsVersionInstalled", typeof(object));
        string ShowPersist(object? value) => value is PersistProxy persist
            ? $"IPersist:{persist.ClassId}"
            : Reflect.Text(value);
        foreach (Guid classId in new[] { Guid.NewGuid(), Guid.Parse("00020820-0000-0000-C000-000000000046") })
        {
            object qPersist = MakePersist(typeof(IPersist), classId);
            System.Type mPersistType = Mathcad.Type("Ptc.Core", "Ptc.PersistentData.IPersist");
            object mPersist = MakePersist(mPersistType, classId);
            MethodComparison.Compare($"IsVersionInstalled(IPersist {classId}) with local COM registry", Contract,
                qVersion, null, [qPersist], mVersion, null, [mPersist], ShowPersist, ShowPersist);
        }
        MethodComparison.Compare("IsVersionInstalled(non-IPersist)", Contract,
            qVersion, null, [new object()], mVersion, null, [new object()], Reflect.Text, Reflect.Text);
    }

    private static object MakePersist(System.Type interfaceType, Guid classId)
    {
        var proxy = (PersistProxy)DispatchProxy.Create(interfaceType, typeof(PersistProxy));
        proxy.ClassId = classId;
        return proxy;
    }

    public class PersistProxy : DispatchProxy
    {
        public Guid ClassId { get; set; }

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args) =>
            targetMethod?.Name == "GetClassID" ? ClassId : throw new NotSupportedException(targetMethod?.Name);
    }
}
#endif
