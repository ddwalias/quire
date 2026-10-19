using System.Reflection;
using System.Xml.Serialization;
using Quire.PersistentDataObjects;

namespace Quire.Differential.Tests;

public class PtcPersistentDataObjectsWorksheetRegionTypeSerializerDifferentialTests
{
    private const string Contract = "Ptc.PersistentDataObjects.worksheetRegionType";

    [SkippableFact]
    public void CreateDefaultSerializer_matches_PtcCore()
    {
        Mathcad.Require();
        System.Type qType = typeof(worksheetRegionType);
        System.Type mType = Mathcad.Type("Ptc.Core", "Ptc.PersistentDataObjects.worksheetRegionType");
        MethodInfo qMethod = qType.GetMethod("CreateDefaultSerializer", BindingFlags.Public | BindingFlags.Static)!;
        MethodInfo mMethod = mType.GetMethod("CreateDefaultSerializer", BindingFlags.Public | BindingFlags.Static)!;

        foreach ((System.Type rootType, string rootName) in new[]
        {
            (typeof(string), "page-number"),
            (typeof(int), "region-value"),
        })
        {
            MethodComparison.Compare($"CreateDefaultSerializer({rootType.Name}, {rootName})", Contract,
                qMethod, null, [rootType, rootName], mMethod, null, [rootType, rootName],
                value => ShowSerializer(value, rootType), value => ShowSerializer(value, rootType));
        }

    }

    private static string ShowSerializer(object? value, System.Type rootType)
    {
        if (value is not XmlSerializer serializer)
            return Reflect.Text(value);
        using var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
        object sample = rootType == typeof(string) ? "content" : 42;
        serializer.Serialize(writer, sample);
        return writer.ToString();
    }
}
