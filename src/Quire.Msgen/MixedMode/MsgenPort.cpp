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
    MethodFunction::MethodFunction(void* f, Closure^ clo)
        : IlEmit(f, clo, nullptr), m_arity(-1)
    {
    }

    int MethodFunction::Arity()
    {
        return m_arity;
    }

    Type^ MethodFunction::ReturnType()
    {
        return m_return_type;
    }

    cli::array<Type^>^ MethodFunction::MplParameterTypes()
    {
        return m_param_types;
    }

    MethodInfo^ MethodFunction::GetMethod(bool)
    {
        return m_method;
    }

    MethodFunction^ MethodFunction::GetMethodFunction()
    {
        return this;
    }

    bool MethodFunction::LdClosureRecord(IlEmit^ emit)
    {
        if (m_clo->HasInstance())
        {
            m_clo->LdVal(emit);
            GC::KeepAlive(this);
            return true;
        }
        return false;
    }

    Type^ MethodFunction::ClosureType()
    {
        Type^ instanceType = m_clo->GetInstanceType();
        GC::KeepAlive(this);
        return instanceType;
    }

    void MethodFunction::CallFunc(IlEmit^ emit, int, bool tail, Hex::Tag* einfo, Type^, cli::array<Type^>^)
    {
        if (tail)
        {
            emit->emit_tailcall();
        }
        MethodInfo^ method = m_method;
        emit->emit_call(einfo, OpCodes::Call, method);
        GC::KeepAlive(this);
    }
    void MethodFunction::DeclareFunction(Hex::Tag*, ms::Range<void* const>, Type^ ty, cli::array<Type^>^ tys)
    {
        m_arity = tys->Length;
        m_return_type = ty;
        m_param_types = tys;
        Closure^ clo = m_clo;
        cli::array<Type^>^ actuals = MethodParameterTypes();
        String^ nm = Name();
        IlGenMethod^ ilGenMethod = clo->CreateMethod(nm, ty, actuals);
        m_method = ilGenMethod->Method();
        m_il = ilGenMethod->IlGen();
        GC::KeepAlive(this);
    }
    IlEmit^ ContinuationClo::DefineFunction(void* f)
    {
        return gcnew ContinuationFunction(f, m_parent, this);
    }

    ContinuationFunction::ContinuationFunction(void* f, MethodFunction^ e, ContinuationClo^ clo)
        : IlEmit(f, e), m_arity(0), m_return_type(nullptr), m_param_types(nullptr),
          m_params(nullptr), m_method(e), m_clo(clo)
    {
        Label label = e->new_label();
        m_label = label;
        GC::KeepAlive(this);
    }

    int ContinuationFunction::Arity()
    {
        return m_arity;
    }

    Type^ ContinuationFunction::ReturnType()
    {
        return m_return_type;
    }

    cli::array<Type^>^ ContinuationFunction::MplParameterTypes()
    {
        return m_param_types;
    }

    cli::array<MetaObj^>^ ContinuationFunction::CreateParameterEnv(ms::Range<void* const>)
    {
        return m_params;
    }

    MethodFunction^ ContinuationFunction::GetMethodFunction()
    {
        return m_method;
    }

    MethodInfo^ ContinuationFunction::GetMethod(bool)
    {
        IlAsm::Raise("not implemented");
        // Named result keeps the null on the stack across GC::KeepAlive(this), as the Release IL requires.
        MethodInfo^ result = nullptr;
        GC::KeepAlive(this);
        return result;
    }

    void ContinuationFunction::EmitProlog(IlAsm^, bool)
    {
        emit_label_def(m_label);
    }
    void IlAsm::Branch(Closure^ clo, Label lab)
    {
        m_ctxt->EmitBranch(m_emit, clo, lab);
        GC::KeepAlive(this);
    }
    void ContinuationFunction::CallFunc(IlEmit^ emit, int, bool, Hex::Tag*, Type^, cli::array<Type^>^)
    {
        for (int i = m_params->Length - 1; i >= 0; i--)
        {
            m_params[i]->StVal(emit);
        }
        IlAssembler()->Branch(m_clo, m_label);
        GC::KeepAlive(this);
    }
    Constructor::Constructor(IlAsm^ il)
        : Callable(il), m_method_function(nullptr)
    {
    }

    MethodInfo^ Constructor::GetMethod(bool may_emit)
    {
        if (!m_method && may_emit)
        {
            ImplementAsMethodFunction();
        }
        return m_method;
    }

    MethodFunction^ Constructor::ImplementAsMethodFunction()
    {
        if (m_method_function)
        {
            return m_method_function;
        }
        Closure^ closure = IlAssembler()->ModuleClosure();
        cli::array<Type^>^ a_ = MplParameterTypes();
        Type^ a_2 = ReturnType();
        String^ a_3 = IlAsm::NewName(Name());
        MethodFunction^ methodFunction = closure->NewMethodFunction(a_3, closure, a_2, a_);
        int nargs = MplParameterTypes()->Length;
        cli::array<Type^>^ parameters = MplParameterTypes();
        Type^ return_type = ReturnType();
        LdFuncArg(methodFunction, return_type, parameters);
        int num = 0;
        if (0 < Arity())
        {
            do
            {
                BegArg(methodFunction, num, nargs);
                methodFunction->Ldarg(num);
                EndArg(methodFunction, num, MplParameterTypes()[num], nargs);
                num++;
            }
            while (num < Arity());
        }
        cli::array<Type^>^ parameters2 = MplParameterTypes();
        Type^ return_type2 = ReturnType();
        int nargs2 = Arity();
        CallFunc(methodFunction, nargs2, true, nullptr, return_type2, parameters2);
        methodFunction->emit_return();
        m_method_function = methodFunction;
        m_method = methodFunction->GetMethod(false);
        GC::KeepAlive(this);
        return methodFunction;
    }

    VoidConstructor::VoidConstructor(IlAsm^ il, void* v)
        : Constructor(il)
    {
        m_var = v;
    }

    void VoidConstructor::LdVal(IlEmit^)
    {
    }

    int VoidConstructor::Arity()
    {
        return 0;
    }

    Type^ VoidConstructor::ReturnType()
    {
        Type^ result = IlAsm::VoidType();
        GC::KeepAlive(this);
        return result;
    }

    cli::array<Type^>^ VoidConstructor::MplParameterTypes()
    {
        return gcnew cli::array<Type^>(0);
    }

    cli::array<MetaObj^>^ VoidConstructor::Deconstruct(MetaObj^, ms::Range<void* const>)
    {
        return gcnew cli::array<MetaObj^>(0);
    }

    void ContinuationFunction::DeclareFunction(Hex::Tag*, ms::Range<void* const> args, Type^ ty, cli::array<Type^>^ tys)
    {
        m_arity = tys->Length;
        m_return_type = ty;
        m_param_types = tys;
        cli::array<MetaObj^>^ params = Ctxt::NewCtxtEnv(static_cast<int>(args.size()));
        cli::array<Type^>^ ptys = MplParameterTypes();
        for (unsigned int i = 0; i < args.size(); i++)
        {
            if (ptys[i] == IlAsm::VoidType())
            {
                params[i] = gcnew VoidConstructor(IlAssembler(), args[i]);
            }
            else
            {
                params[i] = MetaObj::NewMetaObj(args[i], DefineLocal(ptys[i]));
            }
        }
        m_params = params;
    }
    // Out of line so callers such as ContinuationFunction::DeclareFunction call it rather than inlining the allocation.
    cli::array<MetaObj^>^ Ctxt::NewCtxtEnv(int sz)
    {
        return gcnew cli::array<MetaObj^>(sz);
    }
    cli::array<MetaObj^>^ MethodFunction::CreateParameterEnv(ms::Range<void* const> args)
    {
        cli::array<MetaObj^>^ array = Ctxt::NewCtxtEnv(static_cast<int>(args.size()));
        int num = m_clo->ClosureArgs();
        int num2 = 0;
        cli::array<Type^>^ array2 = MplParameterTypes();
        for (unsigned int i = 0; i < args.size(); i++)
        {
            if (array2[i] == IlAsm::VoidType())
            {
                array[i] = gcnew VoidConstructor(IlAssembler(), args[i]);
            }
            else
            {
                array[i] = MetaObj::NewMetaObj(args[i], num2 + num);
                num2++;
            }
        }
        GC::KeepAlive(this);
        return array;
    }
    MethodFunction::MethodFunction(String^ f, Closure^ clo, Type^ ty, cli::array<Type^>^ tys)
        : IlEmit(f, clo, nullptr), m_arity(-1)
    {
        // Unset argument range (-1 begin and end), as the original declares before DeclareFunction.
        ms::Range<void* const> args(reinterpret_cast<void* const*>(-1), reinterpret_cast<void* const*>(-1));
        DeclareFunction(nullptr, args, ty, tys);
        GC::KeepAlive(this);
    }
    MethodFunction::MethodFunction(const char* f, Closure^ clo)
        : IlEmit(::gen::ToString(Substring<const char>(f)), clo, nullptr), m_arity(-1)
    {
        GC::KeepAlive(this);
    }
    void MethodFunction::EmitStackProbe(IlAsm^, bool probe)
    {
        if (m_tracing)
        {
            if (probe)
            {
                MethodInfo^ method = ::Quire::Mpl::Run::AppException::typeid->GetMethod("ProbeStack");
                emit_instr(OpCodes::Call, method);
            }
            else
            {
                MethodInfo^ method2 = ::Quire::Mpl::Run::AppException::typeid->GetMethod("RestoreStack");
                emit_instr(OpCodes::Call, method2);
            }
        }
    }
}}
