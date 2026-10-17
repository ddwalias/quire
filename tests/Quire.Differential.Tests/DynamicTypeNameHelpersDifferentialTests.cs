namespace Quire.Differential.Tests;

public class DynamicTypeNameHelpersDifferentialTests
{
    [SkippableFact]
    public void type_parameter_name_default_prefix() =>
        Mechanical.Compare(typeof(global::Quire.Mpl.Run.DynamicType), 0x0600040c);

    [SkippableFact]
    public void type_parameter_name_custom_prefix() =>
        Mechanical.Compare(typeof(global::Quire.Mpl.Run.DynamicType), 0x0600040d);

    [SkippableFact]
    public void field_name() =>
        Mechanical.Compare(typeof(global::Quire.Mpl.Run.DynamicType), 0x06000417);
}
