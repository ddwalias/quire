namespace Quire.Controls.Worksheet;

public static partial class HeaderFooterRoleExtensions
{
    public static bool IsHeaderRole(this HeaderFooterRole role)
    {
        return role == HeaderFooterRole.Header;
    }

    public static bool IsFooterRole(this HeaderFooterRole role)
    {
        return role == HeaderFooterRole.Footer;
    }
}
