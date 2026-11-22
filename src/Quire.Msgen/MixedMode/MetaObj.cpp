#include "MsgenTypes.h"

namespace Quire { namespace gen {

    // Defined here so gen::ToString in MsgenPort.cpp is called rather than inlined.
    String^ IlAsm::Name(void* v)
    {
        const char* raw = m_info->Name(v);
        String^ name = ::gen::ToString(Substring<const char>(raw));
        if (m_info->Exported(v) != 0)
            return name;
        String^ hex = Convert::ToString(static_cast<int>(reinterpret_cast<std::intptr_t>(v)), 16);
        return String::Format("${0}_{1}", name, hex);
    }

    MetaObj^ IlAsm::ForeignValue(Substring<const char> fid)
    {
        return m_mod->ForeignValue(::gen::ToString(fid));
    }

    // ===== MetaObj =====

    MetaObj::MetaObj(const char* n)
        : m_var(nullptr), m_name(::gen::ToString(Substring<const char>(n)))
    {
    }

    String^ MetaObj::Name()
    {
        if (!m_name)
        {
            m_name = (m_var != nullptr) ? IlAsm::Name(m_var) : "anonymous";
        }
        return m_name;
    }

    void MetaObj::FinishClosure(IlAsm^, bool)
    {
        IlAsm::Raise("may not bake");
    }

    void MetaObj::LdVal(IlEmit^)
    {
        IlAsm::Raise("may not load");
    }

    void MetaObj::StVal(IlEmit^)
    {
        IlAsm::Raise("may not store");
    }

    void MetaObj::ApplyFunc(IlEmit^ emit, int nargs, bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters)
    {
        if (nargs == Arity())
        {
            CallFunc(emit, nargs, tail, einfo, return_type, parameters);
        }
        else
        {
            PartialApplyFunc(emit, nargs, tail, einfo, return_type, parameters);
        }
    }

    void MetaObj::PartialApplyFunc(IlEmit^ emit, int nargs, bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters)
    {
        if (nargs > 0)
        {
            MethodInfo^ apply = emit->IlAssembler()->GetApplyMethod(return_type, parameters);
            if (tail)
            {
                emit->emit_tailcall();
            }
            emit->emit_call(einfo, OpCodes::Callvirt, apply);
        }
    }

    void MetaObj::LdFuncArg(IlEmit^ emit, Type^, cli::array<Type^>^)
    {
        LdVal(emit);
    }

    void MetaObj::BegArg(IlEmit^ emit, int i, int nargs)
    {
        if (nargs > maxargs() && nargs != Arity())
        {
            BegVectorArg(emit, i);
        }
    }

    void MetaObj::BegVectorArg(IlEmit^ emit, int i)
    {
        emit->emit_instr(OpCodes::Dup);
        emit->emit_word(i);
    }

    void MetaObj::EndArg(IlEmit^ emit, int i, Type^ ty, int nargs)
    {
        if (nargs > maxargs() && nargs != Arity())
        {
            EndVectorArg(emit, i, ty);
        }
    }

    void MetaObj::EndArg(IlEmit^ emit, int i, Substring<const char> ty, int nargs)
    {
        if (nargs > maxargs() && nargs != Arity())
        {
            EndVectorArg(emit, i, ty);
        }
    }

    void MetaObj::EndVectorArg(IlEmit^ emit, int, Type^ ty)
    {
        if (ty != nullptr && ty->IsPrimitive)
        {
            emit->emit_instr(OpCodes::Box, ty);
        }
        emit->emit_instr(OpCodes::Stelem_Ref);
    }

    void MetaObj::EndVectorArg(IlEmit^ emit, int i, Substring<const char> ty)
    {
        EndVectorArg(emit, i, IlAsm::TranslateType(ty));
    }

    void MetaObj::CallFunc(IlEmit^ emit, int nargs, bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters)
    {
        PartialApplyFunc(emit, nargs, tail, einfo, return_type, parameters);
    }

    Type^ MetaObj::RepresentationType()
    {
        return IlAsm::ObjType();
    }

    void MetaObj::DeclareFunction(Hex::Tag*, ms::Range<void* const>, Type^, cli::array<Type^>^)
    {
        IlAsm::Raise("may not declare");
    }

    void MetaObj::DeclareFunction(ms::Range<void* const> args, Hex::Tag* info)
    {
        int arity = static_cast<int>(args.size());
        cli::array<Type^>^ tys = IlAsm::GetParameterTypes(m_var, static_cast<unsigned long long>(arity));
        Type^ ty = IlAsm::GetReturnType(m_var, static_cast<unsigned long long>(arity));
        DeclareFunction(info, args, ty, tys);
        GC::KeepAlive(this);
    }

    Object^ MetaObj::Invoke(cli::array<Object^>^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<Object^>(nullptr));
    }

    int MetaObj::Ordinal()
    {
        IlAsm::Raise("not implemented");
        return 0;
    }

    MetaObj^ MetaObj::NewMetaObj(::Quire::Mpl::Run::Primitive^ prim)
    {
        MetaObj^ obj;
        if (prim->IsMethodPrimitive())
            obj = gcnew IlMethod(prim);
        else
            obj = prim->IsForeignPrimitive()
                ? static_cast<MetaObj^>(gcnew LateBoundMethod(prim))
                : static_cast<MetaObj^>(gcnew IlOpCode(prim));
        return obj;
    }

    MetaObj^ MetaObj::NewMetaObj(String^ nm, FieldInfo^ mi)
    {
        return gcnew TopLevelVar(nm, mi);
    }

    MetaObj^ MetaObj::NewMetaObj(String^ nm, MethodInfo^ mi)
    {
        return gcnew IlMethod(nm, mi, CallConv_Default);
    }

    MetaObj^ MetaObj::NewMetaObj(String^ nm, MethodInfo^ mi, CallConv cc)
    {
        return gcnew IlMethod(nm, mi, cc);
    }

    MetaObj^ MetaObj::NewMetaObj(FieldInfo^ f)
    {
        return gcnew StaticField(f);
    }

#pragma optimize("", off)
    // Match Mathcad's stloc.0; ldloc.0; ret spill on LocalVar factory overloads.
    MetaObj^ MetaObj::NewMetaObj(String^ nm, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb)
    {
        return gcnew LocalVar(nm, lb);
    }

    MetaObj^ MetaObj::NewMetaObj(void* v, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb)
    {
        return gcnew LocalVar(v, lb);
    }
#pragma optimize("", on)

    MetaObj^ MetaObj::NewMetaObj(void* v, MetaObj^ r, int i, Type^ unbox)
    {
        return gcnew ElementVar(v, r, i, unbox);
    }

    MetaObj^ MetaObj::NewMetaObj(void* v, MetaObj^ r, Callable^ p)
    {
        return gcnew PropertyVar(v, r, p);
    }

    MetaObj^ MetaObj::NewMetaObj(void* v, MetaObj^ r, FieldInfo^ f)
    {
        return gcnew FieldVar(v, r, f);
    }

    MetaObj^ MetaObj::NewMetaObj(String^ nm, int idx)
    {
        return gcnew ArgVar(nm, idx);
    }

    MetaObj^ MetaObj::NewMetaObj(void* v, int idx)
    {
        return gcnew ArgVar(v, idx);
    }

    MetaObj^ MetaObj::NewMetaObj(void* v, FieldInfo^ f)
    {
        return gcnew TopLevelVar(v, f);
    }

    MetaObj^ MetaObj::ModuleVar(String^ nm, MetaObj^ r, int i, Type^ unbox)
    {
        return gcnew TopLevelElementVar(nm, r, i, unbox);
    }

    MetaObj^ MetaObj::ModuleVar(void* v, MetaObj^ r, int i, Type^ unbox)
    {
        return gcnew TopLevelElementVar(v, r, i, unbox);
    }

    // ===== Callable =====

    MetaObj^ Callable::DelegateValue()
    {
        if (m_delegate != nullptr)
            return m_delegate;
        m_delegate = Clo()->DefineFunctionValue(this, ftype_Delegate);
        return m_delegate;
    }

    MetaObj^ Callable::FunctionValue()
    {
        if (m_function != nullptr)
            return m_function;
        m_function = Clo()->DefineFunctionValue(this, ftype_Function);
        return m_function;
    }

    void Callable::LdVal(IlEmit^ emit)
    {
        LdFunction(emit);
    }

    void Callable::LdFuncArg(IlEmit^ emit, Type^, cli::array<Type^>^ parameters)
    {
        if (Arity() != parameters->Length)
        {
            LdVal(emit);
            if (parameters->Length > maxargs())
            {
                emit->emit_word(parameters->Length);
                emit->emit_instr(OpCodes::Newarr, Object::typeid);
            }
        }
        else
        {
            LdClosure(emit);
        }
    }

    Type^ Callable::ClosureType()
    {
        return Void::typeid;
    }

    bool Callable::LdClosure(IlEmit^ emit)
    {
        return LdClosureRecord(emit);
    }

    void Callable::LdFunction(IlEmit^ emit)
    {
        MetaObj^ func = FunctionValue();
        if (func != nullptr)
            func->LdVal(emit);
        else
            EmitFunctionValue(emit);
    }

    void Callable::LdDelegate(IlEmit^ emit)
    {
        MetaObj^ del = DelegateValue();
        if (del != nullptr)
            del->LdVal(emit);
        else
            EmitDelegateValue(emit);
    }

    Object^ Callable::Invoke(cli::array<Object^>^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<Object^>(nullptr));
    }

    void Callable::EmitDelegateValue(IlEmit^ emit)
    {
        MethodInfo^ method = GetMethod(true);
        emit->emit_instr(OpCodes::Ldnull);
        emit->emit_instr(OpCodes::Ldftn, method);
        emit->emit_instr(OpCodes::Newobj, GetDelegateCtor());
    }

    void Callable::EmitFunctionValue(IlEmit^ emit)
    {
        if (Arity() <= maxargs())
        {
            LdDelegate(emit);
            bool has_closure = LdClosureRecord(emit);
            emit->emit_word(Arity() + static_cast<int>(has_closure));
            if (has_closure)
            {
                emit->emit_instr(OpCodes::Call, ::Quire::Mpl::Run::FunctionUtilities::CloCtor(ClosureType()));
            }
            else
            {
                emit->emit_instr(OpCodes::Call, ::Quire::Mpl::Run::FunctionUtilities::FcnCtor);
            }
        }
        else
        {
            MethodInfo^ method = GetMethod(true);
            emit->emit_instr(OpCodes::Ldtoken, method);
            emit->emit_word(Arity());
            ConstructorInfo^ ctor = LdClosureRecord(emit)
                ? ::Quire::Mpl::Run::FunctionUtilities::InstMethCtor(ClosureType())
                : ::Quire::Mpl::Run::FunctionUtilities::MethCtor;
            emit->emit_instr(OpCodes::Newobj, ctor);
        }
    }

    cli::array<Type^>^ Callable::DelegateParameterTypes()
    {
        if (m_delegate_params == nullptr)
        {
            m_delegate_params = Clo()->DelegateParameterTypes(MplParameterTypes());
        }
        return m_delegate_params;
    }

    cli::array<Type^>^ Callable::MethodParameterTypes()
    {
        if (m_method_params == nullptr)
        {
            m_method_params = Clo()->MethodParameterTypes(MplParameterTypes());
        }
        return m_method_params;
    }

    Type^ Callable::GetDelegateType()
    {
        return ::Quire::Mpl::Run::Delegates::ParameterTypesToDelegateType(ReturnType(), DelegateParameterTypes());
    }

    ConstructorInfo^ Callable::GetDelegateCtor()
    {
        return ::Quire::Mpl::Run::Delegates::ParameterTypesToDelegateCtor(ReturnType(), DelegateParameterTypes());
    }

    // ===== Closure =====

    MetaObj^ Closure::DefineVariable(void*)
    {
        return (IlAsm::Raise("not implemented"), static_cast<MetaObj^>(nullptr));
    }

    MetaObj^ Closure::DefineVariable(String^, Type^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<MetaObj^>(nullptr));
    }

    MetaObj^ Closure::DefineDynamicVariable(void* f, int)
    {
        return DefineVariable(f);
    }

    IlEmit^ Closure::DefineFunction(void*)
    {
        return (IlAsm::Raise("not implemented"), static_cast<IlEmit^>(nullptr));
    }

    MethodFunction^ Closure::DefineMethodFunction(String^ nm, Type^ ty, cli::array<Type^>^ tys)
    {
        return NewMethodFunction(nm, this, ty, tys);
    }

    MethodFunction^ Closure::DefineMethodFunction(void* f)
    {
        return NewMethodFunction(f, this);
    }

    MethodFunction^ Closure::NewMethodFunction(String^, Closure^, Type^, cli::array<Type^>^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<MethodFunction^>(nullptr));
    }

    MethodFunction^ Closure::NewMethodFunction(void*, Closure^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<MethodFunction^>(nullptr));
    }

    IlAsm^ Closure::IlAssembler()
    {
        return m_ilasm;
    }

    bool Closure::Static()
    {
        return !HasInstance();
    }

    Type^ Closure::GetInstanceType()
    {
        return Void::typeid;
    }

    cli::array<MetaObj^>^ Closure::Exports()
    {
        return gcnew cli::array<MetaObj^>(0);
    }

    IlGenMethod^ Closure::CreateMethod(String^, Type^, cli::array<Type^>^)
    {
        return (IlAsm::Raise("not implemented"), static_cast<IlGenMethod^>(nullptr));
    }

    MetaObj^ Closure::DefineConstant(double, double)
    {
        return (IlAsm::Raise("not implemented"), static_cast<MetaObj^>(nullptr));
    }

    System::Reflection::Emit::Quire::Mpl::ModuleBuilder^ Closure::Builder()
    {
        return (IlAsm::Raise("not implemented"), static_cast<System::Reflection::Emit::Quire::Mpl::ModuleBuilder^>(nullptr));
    }

    IlEmit^ Closure::Update()
    {
        return (IlAsm::Raise("not implemented"), static_cast<IlEmit^>(nullptr));
    }

    Delegate^ Closure::UpdateDelegate()
    {
        IlAsm::Raise("not implemented");
        return nullptr;
    }

    void Closure::EmitFunctionInitialization(Callable^)
    {
        IlAsm::Raise("not implemented");
    }

    cli::array<Type^>^ Closure::MethodParameterTypes(cli::array<Type^>^ p)
    {
        return FilterVoid(StaClosureType(), DynClosureType(), p);
    }

    cli::array<Type^>^ Closure::DelegateParameterTypes(cli::array<Type^>^ p)
    {
        return FilterVoid(nullptr, DynClosureType(), p);
    }

    MetaObj^ Closure::DfvImplementation(Callable^ func, ftype ty)
    {
        String^ name = IlAsm::NewName(func->Name());
        Type^ varType = (ty == ftype_Function) ? IlAsm::FcnType() : IlAsm::DelegateType();
        MetaObj^ result = DefineVariable(name, varType);
        QueueInitialization(func);
        return result;
    }

    void Closure::EmitInitializations()
    {
        if (m_queue != nullptr)
        {
            while (m_queue->Count > 0)
            {
                Callable^ func = static_cast<Callable^>(m_queue->Dequeue());
                EmitFunctionInitialization(func);
            }
        }
    }

    cli::array<Type^>^ Closure::FilterVoid(Type^ sta, Type^ clo, cli::array<Type^>^ p)
    {
        int len = p->Length;
        int extra = (static_cast<Object^>(sta) != nullptr ? 1 : 0) + (static_cast<Object^>(clo) != nullptr ? 1 : 0);
        int voids = 0;
        for (int i = 0; i < len; i++)
        {
            if (p[i] == IlAsm::VoidType())
                voids++;
        }
        if (voids + extra == 0)
            return p;

        int dst = 0;
        cli::array<Type^>^ result = gcnew cli::array<Type^>(extra - voids + len);
        if (sta)
        {
            result[0] = sta;
            dst = 1;
        }
        if (clo)
        {
            result[dst] = clo;
            dst++;
        }
        if (voids == 0)
        {
            p->CopyTo(result, extra);
            return result;
        }
        for (int i = 0; i < len; i++)
        {
            if (p[i] != IlAsm::VoidType())
            {
                result[dst] = p[i];
                dst++;
            }
        }
        return result;
    }

    void Closure::QueueInitialization(Callable^ func)
    {
        if (m_queue == nullptr)
        {
            m_queue = gcnew Queue();
        }
        if (func->Function() == nullptr && func->Delegate() == nullptr)
        {
            m_queue->Enqueue(func);
        }
    }

    // ===== Var subclasses =====

    void ArgVar::LdVal(IlEmit^ emit)
    {
        emit->Ldarg(m_index);
    }

    void ArgVar::StVal(IlEmit^ emit)
    {
        emit->Starg(m_index);
    }

#pragma inline_depth(0)
    // Match Mathcad's non-inlined base MetaObj::.ctor(String^) call in LocalVar.
    LocalVar::LocalVar(String^ nm, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ local)
        : MetaObj(nm), m_local(local)
    {
    }
#pragma inline_depth()

    LocalVar::LocalVar(void* v, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ local)
        : MetaObj(v), m_local(local)
    {
    }

    void LocalVar::LdVal(IlEmit^ emit)
    {
        if (m_local != nullptr)
        {
            emit->Ldloc(m_local);
        }
    }

    void LocalVar::StVal(IlEmit^ emit)
    {
        if (m_local != nullptr)
        {
            emit->Stloc(m_local);
        }
    }

    bool FieldVar::ModuleScope()
    {
        return m_record->ModuleScope();
    }

    void FieldVar::LdVal(IlEmit^ emit)
    {
        if (m_field)
        {
            m_record->LdVal(emit);
            emit->Ldfld(m_field);
        }
    }

    void FieldVar::StVal(IlEmit^ emit)
    {
        if (m_field)
        {
            m_record->LdVal(emit);
            emit->Stfld(m_field);
        }
    }

    void StaticField::LdVal(IlEmit^ emit)
    {
        if (m_field)
        {
            emit->Ldsfld(m_field);
        }
    }

    void TopLevelVar::StVal(IlEmit^ emit)
    {
        if (m_field)
        {
            if (m_var != nullptr && IlAsm::Exported(m_var))
            {
                emit->emit_value_assertion();
            }
            emit->Stsfld(m_field);
        }
    }

    PropertyVar::PropertyVar(void* v, MetaObj^ rec, Callable^ prop)
        : MetaObj(v), m_record(rec), m_prop_get(prop)
    {
        cli::array<Type^>^ params = gcnew cli::array<Type^>{ m_record->RepresentationType() };
        m_get_param = (prop != nullptr) ? params : nullptr;
    }

    bool PropertyVar::ModuleScope()
    {
        return m_record->ModuleScope();
    }

    void PropertyVar::LdVal(IlEmit^ emit)
    {
        m_record->LdVal(emit);
        if (m_prop_get != nullptr)
        {
            m_prop_get->CallFunc(emit, 1, false, nullptr, m_prop_get->ReturnType(), m_get_param);
        }
    }

    bool ElementVar::ModuleScope()
    {
        return m_record->ModuleScope();
    }

    void ElementVar::LdVal(IlEmit^ emit)
    {
        m_record->LdVal(emit);
        emit->emit_word(m_index);
        emit->emit_instr(OpCodes::Ldelem_Ref);
        if (m_unbox && m_unbox->IsPrimitive)
        {
            emit->emit_instr(OpCodes::Unbox, m_unbox);
            emit->emit_instr(OpCodes::Ldobj, m_unbox);
        }
    }

    void ElementVar::StVal(IlEmit^ emit)
    {
        if (m_unbox && m_unbox->IsPrimitive)
        {
            emit->emit_instr(OpCodes::Box, m_unbox);
        }
        m_record->LdVal(emit);
        emit->emit_word(m_index);
        emit->emit_call(nullptr, OpCodes::Call, ::Quire::Mpl::Run::Primitive::m_stelem_vec);
    }

    // ===== IlOpCode, IlMethod, LateBoundMethod =====

    IlOpCode::IlOpCode(::Quire::Mpl::Run::Primitive^ p)
        : Callable(p->m_name, p->m_method), m_op(p->m_opcodes), m_code(p->m_code),
          m_arity(p->m_method->GetParameters()->Length)
    {
    }

    Type^ IlOpCode::ReturnType()
    {
        return m_method->ReturnType;
    }

    cli::array<Type^>^ IlOpCode::MplParameterTypes()
    {
        return IlAsm::GetParameterTypes(m_method);
    }

    void IlOpCode::CallFunc(IlEmit^ emit, int, bool, Hex::Tag*, Type^, cli::array<Type^>^)
    {
        if (m_code != nullptr)
        {
            m_code->EmitCode(emit->GetIlGenerator());
        }
        else
        {
            for (int i = 0; i < m_op->Length; i++)
            {
                emit->emit_instr(m_op[i]);
            }
        }
    }

    void IlOpCode::BegArg(IlEmit^ emit, int i, int)
    {
        if (m_code != nullptr)
        {
            m_code->BegArg(emit->GetIlGenerator(), i);
        }
    }

    void IlOpCode::EndArg(IlEmit^ emit, int i, Substring<const char>, int)
    {
        if (m_code != nullptr)
        {
            m_code->EndArg(emit->GetIlGenerator(), i);
        }
    }

    void IlOpCode::EndArg(IlEmit^ emit, int i, Type^, int)
    {
        if (m_code != nullptr)
        {
            m_code->EndArg(emit->GetIlGenerator(), i);
        }
    }

    IlMethod::IlMethod(String^ nm, MethodInfo^ mi, CallConv cc)
        : Callable(nm, mi), m_cc(cc),
          m_arity(mi->GetParameters()->Length + ((cc == CallConv_Default) ? 0 : 1))
    {
    }

    IlMethod::IlMethod(::Quire::Mpl::Run::Primitive^ prim)
        : Callable(prim->m_name, prim->m_method), m_cc(CallConv_Default),
          m_arity(prim->m_method->GetParameters()->Length)
    {
    }

    Type^ IlMethod::ReturnType()
    {
        return m_method->ReturnType;
    }

    cli::array<Type^>^ IlMethod::MplParameterTypes()
    {
        return IlAsm::GetParameterTypes(m_method, m_cc == CallConv_Default);
    }

    void IlMethod::CallFunc(IlEmit^ emit, int, bool tail, Hex::Tag* einfo, Type^, cli::array<Type^>^)
    {
        if (tail)
        {
            emit->emit_tailcall();
        }
        MethodInfo^ method = m_method;
        OpCode op = (m_cc == CallConv_Instance) ? OpCodes::Callvirt : OpCodes::Call;
        emit->emit_call(einfo, op, method);
    }

    Object^ IlMethod::Invoke(cli::array<Object^>^ args)
    {
        if (m_method)
        {
            return m_method->Invoke(nullptr, args);
        }
        IlAsm::Raise("no compile time evaluation");
        return nullptr;
    }

    LateBoundMethod::LateBoundMethod(::Quire::Mpl::Run::Primitive^ prim)
        : Callable(prim->m_name, nullptr), m_foreign(prim->m_foreign), m_impl(nullptr)
    {
    }

    Callable^ LateBoundMethod::GetImpl()
    {
        if (m_impl != nullptr)
            return m_impl;
        m_impl = dynamic_cast<Callable^>(IlAssembler()->ForeignValue(m_foreign));
        return m_impl;
    }

    int LateBoundMethod::Arity()
    {
        return GetImpl()->Arity();
    }

    Type^ LateBoundMethod::ReturnType()
    {
        return GetImpl()->ReturnType();
    }

    cli::array<Type^>^ LateBoundMethod::MplParameterTypes()
    {
        return GetImpl()->MplParameterTypes();
    }

    void LateBoundMethod::CallFunc(IlEmit^ emit, int nargs, bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters)
    {
        GetImpl()->CallFunc(emit, nargs, tail, einfo, return_type, parameters);
    }

    MethodInfo^ LateBoundMethod::GetMethod(bool may_emit)
    {
        MethodInfo^ method = GetImpl()->GetMethod(may_emit);
        m_method = method;
        GC::KeepAlive(this);
        return method;
    }

    Object^ LateBoundMethod::Invoke(cli::array<Object^>^ args)
    {
        return GetImpl()->Invoke(args);
    }

}}
