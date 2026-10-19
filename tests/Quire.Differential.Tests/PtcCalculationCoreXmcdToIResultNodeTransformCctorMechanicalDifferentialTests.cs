using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace Quire.Differential.Tests;

public class PtcCalculationCoreXmcdToIResultNodeTransformCctorMechanicalDifferentialTests
{
    [SkippableFact]
    public void XmcdToIResultNodeTransform_cctor()
    {
        var quireType = typeof(global::Quire.Calculation.Core.XmcdToIResultNodeTransform);
        var mathcadType = Mathcad.Type("Ptc.Calculation.Core", "Ptc.Calculation.Core.XmcdToIResultNodeTransform");

        string Transform(Type type)
        {
            var xslt = (XslCompiledTransform)type.GetField("_resultVectorUnescapingXslt",
                BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
            var results = new List<string>();
            foreach (string xml in new[] { "<DataVectors>one</DataVectors>", "<DataVectors><V>1</V><V>2</V></DataVectors>" })
            {
                var document = new XmlDocument();
                document.LoadXml(xml);
                using var output = new StringWriter(CultureInfo.InvariantCulture);
                xslt.Transform(document, null, output);
                results.Add(output.ToString());
            }
            return string.Join("|", results);
        }

        MethodComparison.Compare("XmcdToIResultNodeTransform..cctor()", "Ptc.Calculation.Core.XmcdToIResultNodeTransform",
            quireType.TypeInitializer!, null, [], mathcadType.TypeInitializer!, null, [],
            _ => Transform(quireType), _ => Transform(mathcadType));
    }
}
