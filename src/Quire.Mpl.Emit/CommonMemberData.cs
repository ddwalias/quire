#nullable enable

namespace System.Reflection.Emit.Quire.Mpl;

internal partial record CommonMemberData(
    string Name,
    MethodAttributes attributes,
    Type[]? parameters,
    Type? returnType,
    bool IsStatic,
    ILGenerator codeStore,
    LocalVariableManager localVars,
    LabelManager labelManager,
    object source)
{
    public static CommonMemberData FromMethodBuilder(MethodBuilder method)
    {
        return new CommonMemberData(method.Name, method.Attributes, method._parameters, method._returnType, method.IsStatic, method._codeStore, method._localVariableManager, method._labelManager, method);
    }

    public static CommonMemberData FromConstructorBuilder(ConstructorBuilder ctor)
    {
        return new CommonMemberData(ctor.Name, ctor.Attributes, ctor._parameters, ctor._returnType, ctor.IsStatic, ctor._codeStore, ctor._localVariableManager, ctor._labelManager, ctor);
    }
}
