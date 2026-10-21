namespace Quire;

public static partial class MsiHelper
{
    private static string GuidToString(global::System.Guid guid) => guid.ToString("B");

    public static global::System.Collections.Generic.IList<global::System.Guid> EnumRelatedProducts(global::System.Guid upgradeCode)
    {
        global::System.Collections.Generic.List<global::System.Guid> list = new global::System.Collections.Generic.List<global::System.Guid>();
        uint num = 0u;
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(39);
        while (MsiEnumRelatedProducts(GuidToString(upgradeCode), 0u, num++, stringBuilder) == 0)
        {
            list.Add(new global::System.Guid(stringBuilder.ToString()));
        }
        return list;
    }

    public static string GetProductInfo(global::System.Guid productCode, InstallProperty property)
    {
        string szProduct = GuidToString(productCode);
        string szProperty = property.ToString();
        int pcchValueBuf = 512;
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder(pcchValueBuf);
        uint num = MsiGetProductInfo(szProduct, szProperty, stringBuilder, ref pcchValueBuf);
        if (num == 234 && ++pcchValueBuf < stringBuilder.MaxCapacity)
        {
            stringBuilder.Capacity = pcchValueBuf;
            num = MsiGetProductInfo(szProduct, szProperty, stringBuilder, ref pcchValueBuf);
        }
        if (num != 0)
        {
            return null;
        }
        return stringBuilder.ToString();
    }
}
