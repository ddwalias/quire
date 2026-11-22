#include "MsgenTypes.h"

bool IsDyn(System::String^ TyName)
{
    return TyName->Equals("Dv");
}

bool IsTuple(System::String^ TyName)
{
    return TyName->StartsWith("(");
}

bool IsStrVal(System::String^ TyName)
{
    return TyName->Equals("Str");
}

namespace gen {

    System::String^ ToString(Substring<const char> text)
    {
        return gcnew System::String(text.begin(), 0, static_cast<int>(text.size()), System::Text::Encoding::UTF8);
    }

}

namespace Quire { namespace gen {

    MetaObj::MetaObj(void* v)
        : m_var(v), m_name(nullptr)
    {
        GC::KeepAlive(this);
    }

    MethodInfo^ CtxtMod::GetApplyMethod(Type^ return_type, cli::array<Type^>^ parameters)
    {
        return ::Quire::Mpl::Run::FunctionUtilities::GetCallMethod(return_type, parameters);
    }

    Type^ IlAsm::IntType()
    {
        return m_int_type;
    }

    Type^ IlAsm::DblType()
    {
        return m_dbl_type;
    }

    Type^ IlAsm::ObjType()
    {
        return m_obj_type;
    }

    Type^ IlAsm::FcnType()
    {
        return m_fcn_type;
    }

    Type^ IlAsm::DelegateType()
    {
        return m_del_type;
    }

    Type^ IlAsm::TagType()
    {
        return m_tag_type;
    }

    Type^ IlAsm::VoidType()
    {
        return m_voidtype;
    }

    MethodInfo^ IlAsm::NumValCtor()
    {
        return m_num_ctor;
    }

    String^ IlAsm::FieldName(int i)
    {
        return ::Quire::Mpl::Run::DynamicType::FieldName(i);
    }

    cli::array<String^>^ IlAsm::FieldNames(int n)
    {
        return ::Quire::Mpl::Run::DynamicType::FieldNames(n);
    }

    String^ IlAsm::NewName()
    {
        return NewName("$T");
    }

    String^ IlAsm::NewName(String^ prefix)
    {
        int id = m_counter++;
        return String::Concat(prefix, id);
    }

    void IlAsm::Raise(Substring<const char> m)
    {
        Hex::OnError(Hex::Error(74) << m);
    }

    bool IlAsm::Exported(void* v)
    {
        return m_info->Exported(v) != 0;
    }

    const char* IlAsm::Name(void*** d)
    {
        return m_info->Name(d);
    }

    const char* IlAsm::Name(void** c)
    {
        return m_info->Name(c);
    }

    const char* IlAsm::ModuleName(void** c)
    {
        return m_info->ModuleName(c);
    }

    int IlAsm::Arity(void** c)
    {
        return static_cast<int>(m_info->Arity(c));
    }

    int IlAsm::GetTag(void** c)
    {
        return static_cast<int>(m_info->GetTag(c));
    }

    int IlAsm::GetSpan(void*** d)
    {
        return static_cast<int>(m_info->GetSpan(d));
    }

    void** IlAsm::GetConstructor(void*** d, int i)
    {
        return m_info->GetConstructor(d, static_cast<std::size_t>(i));
    }

    Constructor^ IlAsm::DynConstructor()
    {
        return m_dyn_cons;
    }

    Constructor^ IlAsm::KeyConstructor()
    {
        return m_key_cons;
    }

    Type^ IlAsm::GetValueType(void* v)
    {
        return TranslateType(m_info->GetType(v));
    }

    cli::array<Type^>^ IlAsm::GetConParameters(void** c)
    {
        std::size_t arity = static_cast<std::size_t>(static_cast<int>(m_info->Arity(c)));
        return GetParameterTypes(m_info->GetType(c), arity);
    }

    Type^ IlAsm::GetReturnType(void* v, unsigned long long arity)
    {
        return GetReturnType(m_info->GetType(v), arity);
    }

    cli::array<Type^>^ IlAsm::GetParameterTypes(MethodBase^ mb, bool is_static)
    {
        cli::array<ParameterInfo^>^ parameters = mb->GetParameters();
        int offset = is_static ? 0 : 1;
        int total = parameters->Length + offset;
        cli::array<Type^>^ types = gcnew cli::array<Type^>(total);
        if (offset != 0)
        {
            types[0] = mb->DeclaringType;
        }
        for (int i = offset; i < total; i++)
        {
            types[i] = parameters[i - offset]->ParameterType;
        }
        return types;
    }

    cli::array<Type^>^ IlAsm::GetParameterTypes(MethodBase^ mb)
    {
        return GetParameterTypes(mb, true);
    }

    cli::array<Type^>^ IlAsm::GetParameterTypes(Substring<const char> ty, unsigned long long arity)
    {
        cli::array<Type^>^ types = gcnew cli::array<Type^>(static_cast<int>(arity));
        for (unsigned long long i = 0; i != arity; i++)
        {
            types[static_cast<int>(i)] = (i < ty.size())
                ? TranslateType(Substring<const char>(ty[static_cast<int>(i)]))
                : ObjType();
        }
        return types;
    }

    cli::array<Type^>^ IlAsm::GetParameterTypes(void* v, unsigned long long arity)
    {
        return GetParameterTypes(m_info->GetType(v), arity);
    }

    Type^ IlAsm::TranslateType(Substring<const char> ty)
    {
        if (ty.size() == 1)
        {
            switch (*ty.begin())
            {
            case 'f':
                return m_dbl_type;
            case 'i':
                return m_int_type;
            case 'v':
                return m_voidtype;
            default:
                return m_obj_type;
            }
        }
        return (ty.size() > 1) ? m_fcn_type : ObjType();
    }

    MetaObj^ IlAsm::ForeignValue(String^ fid)
    {
        return m_mod->ForeignValue(fid);
    }

    Closure^ IlAsm::ModuleClosure()
    {
        return m_mod->ModuleClosure();
    }

    MethodInfo^ IlAsm::GetApplyMethod(Type^ return_type, cli::array<Type^>^ parameters)
    {
        return m_mod->GetApplyMethod(return_type, parameters);
    }

    // Defined here so MetaObj::NewMetaObj in MetaObj.cpp is called rather than inlined.
    MetaObj^ ForeignField::CreateMetaObj(IlAsm^)
    {
        return MetaObj::NewMetaObj(m_fi);
    }

    Object^ ForeignField::LoadField()
    {
        return m_fi->GetValue(nullptr);
    }

    MetaObj^ ForeignMethod::CreateMetaObj(IlAsm^)
    {
        return MetaObj::NewMetaObj(m_name, m_mi, m_conv);
    }

    void Ctxt::EmitBranch(IlEmit^ emit, Closure^ clo, Label lab)
    {
        if (m_parent != nullptr)
        {
            m_parent->EmitBranch(emit, clo, lab);
            GC::KeepAlive(this);
        }
        else
        {
            IlAsm::Raise("call to continuation out of scope");
            GC::KeepAlive(this);
        }
    }

    System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ IlBuilderType::DefineConstructor(MethodAttributes attrs, cli::array<Type^>^ parameters)
    {
        return m_tb->DefineConstructor(attrs, CallingConventions::Standard, parameters);
    }

    FieldInfo^ IlBuilderType::DefineField(String^ nm, Type^ ty, FieldAttributes attrs)
    {
        // The original keeps `this` alive after the builder call; the Release IL shape requires that.
        FieldInfo^ result = (ty == IlAsm::VoidType()) ? nullptr : m_tb->DefineField(nm, ty, attrs);
        GC::KeepAlive(this);
        return result;
    }

    IlGenMethod^ IlBuilderType::CreateMethod(String^ nm, Type^ rtype, cli::array<Type^>^ actuals)
    {
        // Same `this` keep-alive as DefineField, before the wrapper is built.
        System::Reflection::Emit::Quire::Mpl::MethodBuilder^ mb = m_tb->DefineMethod(nm, MethodAttributes::Public | MethodAttributes::Static, rtype, actuals);
        GC::KeepAlive(this);
        return gcnew IlGenMethod(mb);
    }

    void IlBuilderType::CreateType()
    {
        if (m_cctor)
        {
            m_cctor->emit_return();
        }
        m_tb->CreateType();
    }
    System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ IlAsm::GetTypeInitializer(System::Reflection::Emit::Quire::Mpl::TypeBuilder^ tb)
    {
        return ::Quire::Mpl::Run::Rts::GetTypeInitializer(tb);
    }

    System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ IlBuilderType::TypeInitializer()
    {
        System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ ti = IlAsm::GetTypeInitializer(m_tb);
        GC::KeepAlive(this);
        return ti;
    }

    IlEmit^ IlBuilderType::Cctor()
    {
        if (!m_cctor)
        {
            m_cctor = gcnew CctorFunction(this);
        }
        return m_cctor;
    }

    CctorFunction::CctorFunction(IlBuilderType^ clo)
        : IlEmit(".cctor", clo, nullptr), m_value(nullptr), m_ctor(clo->TypeInitializer())
    {
        m_il = m_ctor->GetILGenerator();
        GC::KeepAlive(this);
    }

    int CctorFunction::Arity()
    {
        return 0;
    }

    Type^ CctorFunction::ReturnType()
    {
        Type^ result = IlAsm::VoidType();
        GC::KeepAlive(this);
        return result;
    }

    cli::array<Type^>^ CctorFunction::MplParameterTypes()
    {
        return gcnew cli::array<Type^>(0);
    }

    MethodInfo^ CctorFunction::GetMethod(bool)
    {
        IlAsm::Raise("not implemented");
        MethodInfo^ result = nullptr;
        GC::KeepAlive(this);
        return result;
    }
    ContinuationClo::ContinuationClo(IlAsm^ il, IlEmit^ mf)
        : Closure(il)
    {
        m_parent = mf->GetMethodFunction();
        GC::KeepAlive(this);
    }

    void ContinuationClo::LdVal(IlEmit^)
    {
        IlAsm::Raise("invalid continuation");
        GC::KeepAlive(this);
    }
}}
