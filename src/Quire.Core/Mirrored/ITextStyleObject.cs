#nullable disable
#pragma warning disable

namespace Quire;

partial interface ITextStyleObject
{
    string Name { get; set; }
    string StyleKey { get; set; }
    string BasedOnKey { get; set; }
    global::System.Windows.TextDecorationCollection TextDecorations { get; set; }
}
