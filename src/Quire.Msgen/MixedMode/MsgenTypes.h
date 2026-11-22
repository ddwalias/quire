#pragma once

#include "MsgenCommon.h"

namespace Quire { namespace gen {

    using namespace System;
    using namespace System::Collections;
    using namespace System::Reflection;
    using namespace System::Reflection::Emit;
    using namespace System::Runtime::InteropServices;

    ref class IlAsm;
    ref class MetaObj;
    ref class Callable;
    ref class IlEmit;
    ref class Closure;
    ref class Constructor;
    ref class Ctxt;
    ref class CtxtMod;
    ref class MethodFunction;
    ref class ArgVar;
    ref class LocalVar;
    ref class FieldVar;
    ref class StaticField;
    ref class TopLevelVar;
    ref class PropertyVar;
    ref class ElementVar;
    ref class TopLevelElementVar;
    ref class IlOpCode;
    ref class IlMethod;
    ref class LateBoundMethod;

    [::Quire::Mirror::MirrorOfAttribute("gen.IlGenMethod, msgen")]
    ref class IlGenMethod
    {
    private:
        MethodInfo^ m_method;
        System::Reflection::Emit::Quire::Mpl::ILGenerator^ m_il;

    public:
        IlGenMethod(System::Reflection::Emit::Quire::Mpl::DynamicMethod^ dm)
            : m_method(dm), m_il(dm->GetILGenerator())
        {
        }

        IlGenMethod(System::Reflection::Emit::Quire::Mpl::MethodBuilder^ mb)
            : m_method(mb), m_il(mb->GetILGenerator())
        {
        }

        MethodInfo^ Method()
        {
            return m_method;
        }

        System::Reflection::Emit::Quire::Mpl::ILGenerator^ IlGen()
        {
            return m_il;
        }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.MetaObj, msgen")]
    ref class MetaObj
    {
    protected:
        void* m_var;
        String^ m_name;

        MetaObj(String^ n)
            : m_var(nullptr), m_name(n)
        {
        }

        MetaObj(const char* n);
        MetaObj(void* v);

        MetaObj()
            : m_var(nullptr), m_name("anonymous")
        {
        }

    public:
        String^ Name();

        [returnvalue: MarshalAs(UnmanagedType::U1)]
        bool IsVar(void* v)
        {
            return m_var == v;
        }

        virtual void FinishClosure(IlAsm^, [MarshalAs(UnmanagedType::U1)] bool);
        virtual void LdVal(IlEmit^ emit);
        virtual void StVal(IlEmit^ emit);
        virtual int maxargs() { return 16; }

        void ApplyFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters);
        virtual void PartialApplyFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters);
        virtual void LdFuncArg(IlEmit^ emit, Type^ return_type, cli::array<Type^>^ parameters);
        virtual void BegArg(IlEmit^ emit, int i, int nargs);
        void BegVectorArg(IlEmit^ emit, int i);
        virtual void EndArg(IlEmit^ emit, int i, Type^ ty, int nargs);
        virtual void EndArg(IlEmit^ emit, int i, Substring<const char> ty, int nargs);
        void EndVectorArg(IlEmit^ emit, int i, Type^ ty);
        void EndVectorArg(IlEmit^ emit, int i, Substring<const char> ty);
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters);
        virtual Type^ RepresentationType();
        virtual int Arity() { return 0; }
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() { return false; }
        virtual void DeclareFunction(Hex::Tag* info, ms::Range<void* const> args, Type^ ty, cli::array<Type^>^ tys);
        void DeclareFunction(ms::Range<void* const> args, Hex::Tag* info);
        virtual void SetInstance(MetaObj^) {}
        virtual Object^ Invoke(cli::array<Object^>^ args);
        virtual int Ordinal();

        static MetaObj^ NewMetaObj(::Quire::Mpl::Run::Primitive^ prim);
        static MetaObj^ NewMetaObj(String^ nm, FieldInfo^ mi);
        static MetaObj^ NewMetaObj(String^ nm, MethodInfo^ mi);
        static MetaObj^ NewMetaObj(String^ nm, MethodInfo^ mi, CallConv cc);
        static MetaObj^ NewMetaObj(FieldInfo^ f);
        static MetaObj^ NewMetaObj(String^ nm, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb);
        static MetaObj^ NewMetaObj(void* v, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb);
        static MetaObj^ NewMetaObj(void* v, MetaObj^ r, int i, Type^ unbox);
        static MetaObj^ NewMetaObj(void* v, MetaObj^ r, Callable^ p);
        static MetaObj^ NewMetaObj(void* v, MetaObj^ r, FieldInfo^ f);
        static MetaObj^ NewMetaObj(IlAsm^ il, ConstructorInfo^ ci, cli::array<Type^>^ params);
        static MetaObj^ NewMetaObj(String^ nm, int idx);
        static MetaObj^ NewMetaObj(void* v, int idx);
        static MetaObj^ NewMetaObj(void* v, FieldInfo^ f);
        static MetaObj^ ModuleVar(String^ nm, MetaObj^ r, int i, Type^ unbox);
        static MetaObj^ ModuleVar(void* v, MetaObj^ r, int i, Type^ unbox);
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.Callable, msgen")]
    ref class Callable abstract : public MetaObj
    {
    private:
        IlAsm^ m_ilasm;

    protected:
        MetaObj^ m_delegate;
        MetaObj^ m_function;
        MethodInfo^ m_method;
        cli::array<Type^>^ m_method_params;
        cli::array<Type^>^ m_delegate_params;

    public:
        Callable(Callable^ c, void* f)
            : MetaObj(f), m_ilasm(c->m_ilasm), m_delegate(c->m_delegate), m_method(c->m_method),
              m_method_params(nullptr), m_delegate_params(nullptr)
        {
        }

        Callable(IlAsm^ il)
            : MetaObj(), m_ilasm(il), m_delegate(nullptr), m_function(nullptr), m_method(nullptr),
              m_method_params(nullptr), m_delegate_params(nullptr)
        {
        }

        Callable(IlAsm^ il, void* v)
            : MetaObj(v), m_ilasm(il), m_delegate(nullptr), m_function(nullptr), m_method(nullptr),
              m_method_params(nullptr), m_delegate_params(nullptr)
        {
        }

        Callable(String^ nm, MethodInfo^ mi)
            : MetaObj(nm), m_ilasm(nullptr), m_delegate(nullptr), m_function(nullptr), m_method(mi),
              m_method_params(nullptr), m_delegate_params(nullptr)
        {
        }

        Callable(IlAsm^ il, String^ nm)
            : MetaObj(nm), m_ilasm(il), m_delegate(nullptr), m_function(nullptr), m_method(nullptr),
              m_method_params(nullptr), m_delegate_params(nullptr)
        {
        }

        // Blocked on gen::CurrAsm() / gcroot<Quire::gen::IlAsm ^> mangled name normalization.
        [::Quire::Mirror::MirrorStub]
        IlAsm^ IlAssembler()
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }

        MetaObj^ Function() { return m_function; }
        MetaObj^ Delegate() { return m_delegate; }

        virtual MetaObj^ DelegateValue();
        virtual MetaObj^ FunctionValue();
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override { return true; }
        virtual void LdVal(IlEmit^ emit) override;
        virtual void LdFuncArg(IlEmit^ emit, Type^ return_type, cli::array<Type^>^ parameters) override;

        // Blocked on gen::CurrAsm() / gcroot<Quire::gen::IlAsm ^> mangled name normalization.
        [::Quire::Mirror::MirrorStub]
        virtual Closure^ Clo()
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }
        virtual Type^ ClosureType();
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool LdClosure(IlEmit^ emit);
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool LdClosureRecord(IlEmit^) { return false; }
        void LdFunction(IlEmit^ emit);
        void LdDelegate(IlEmit^ emit);

        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) = 0;
        virtual Type^ ReturnType() = 0;
        virtual cli::array<Type^>^ MplParameterTypes() = 0;
        virtual int Arity() override = 0;
        virtual Object^ Invoke(cli::array<Object^>^ args) override;
        virtual void EmitDelegateValue(IlEmit^ emit);
        virtual void EmitFunctionValue(IlEmit^ emit);
        cli::array<Type^>^ DelegateParameterTypes();
        cli::array<Type^>^ MethodParameterTypes();
        Type^ GetDelegateType();
        ConstructorInfo^ GetDelegateCtor();
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.Closure, msgen")]
    ref class Closure : public MetaObj
    {
    private:
        IlAsm^ m_ilasm;
        Queue^ m_queue;

        void QueueInitialization(Callable^ func);

    protected:
        MetaObj^ DfvImplementation(Callable^ func, ftype ty);
        void EmitInitializations();
        static cli::array<Type^>^ FilterVoid(Type^ sta, Type^ clo, cli::array<Type^>^ p);

    public:
        Closure(IlAsm^ il)
            : MetaObj(), m_ilasm(il), m_queue(nullptr)
        {
        }

        virtual MetaObj^ DefineVariable(void* f);
        virtual MetaObj^ DefineVariable(String^ nm, Type^ ty);
        virtual MetaObj^ DefineDynamicVariable(void* f, int ord);
        virtual IlEmit^ DefineFunction(void* f);
        MethodFunction^ DefineMethodFunction(String^ nm, Type^ ty, cli::array<Type^>^ tys);
        MethodFunction^ DefineMethodFunction(void* f);
        virtual MethodFunction^ NewMethodFunction(String^ nm, Closure^ clo, Type^ ty, cli::array<Type^>^ tys);
        virtual MethodFunction^ NewMethodFunction(void* f, Closure^ clo);
        IlAsm^ IlAssembler();
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool HasInstance() { return false; }
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override { return true; }
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool Static();
        virtual Type^ GetInstanceType();
        virtual int ClosureArgs() { return 0; }
        virtual Type^ DynClosureType() { return nullptr; }
        virtual Type^ StaClosureType() { return nullptr; }
        virtual cli::array<MetaObj^>^ Exports();
        virtual IlGenMethod^ CreateMethod(String^ nm, Type^ rtype, cli::array<Type^>^ actuals);
        virtual MetaObj^ DefineConstant(double re, double im);
        virtual System::Reflection::Emit::Quire::Mpl::ModuleBuilder^ Builder();
        virtual IlEmit^ Update();
        virtual Delegate^ UpdateDelegate();
        virtual MetaObj^ DefineFunctionValue(Callable^, ftype) { return nullptr; }
        virtual void EmitFunctionInitialization(Callable^ func);
        cli::array<Type^>^ MethodParameterTypes(cli::array<Type^>^ p);
        cli::array<Type^>^ DelegateParameterTypes(cli::array<Type^>^ p);
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlBuilderType, msgen")]
    ref class IlBuilderType : public Closure
    {
    protected:
        System::Reflection::Emit::Quire::Mpl::TypeBuilder^ m_tb;
        IlEmit^ m_cctor;

    public:
        IlBuilderType(IlAsm^ il, System::Reflection::Emit::Quire::Mpl::TypeBuilder^ tb)
            : Closure(il), m_tb(tb), m_cctor(nullptr)
        {
        }

        System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ DefineConstructor(MethodAttributes attrs, cli::array<Type^>^ parameters);
        FieldInfo^ DefineField(String^ nm, Type^ ty, FieldAttributes attrs);
        virtual IlGenMethod^ CreateMethod(String^ nm, Type^ rtype, cli::array<Type^>^ actuals) override;
        System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ TypeInitializer();
        System::Reflection::Emit::Quire::Mpl::TypeBuilder^ GetTypeBuilder() { return m_tb; }
        virtual Type^ GetInstanceType() override { return m_tb; }
        IlEmit^ Cctor();
        void CreateType();
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.Constructor, msgen")]
    ref class Constructor abstract : public Callable
    {
    private:
        MethodFunction^ m_method_function;

    public:
        Constructor(IlAsm^ il);
        Constructor(IlAsm^ il, String^ nm);
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) override;
        MethodFunction^ ImplementAsMethodFunction();
        virtual MetaObj^ FunctionValue() override;
        virtual cli::array<MetaObj^>^ Deconstruct(MetaObj^ loc, ms::Range<void* const> vs) = 0;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.VoidConstructor, msgen")]
    ref class VoidConstructor : public Constructor
    {
    public:
        VoidConstructor(IlAsm^ il, void* v);
        virtual void LdVal(IlEmit^ emit) override;
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual cli::array<MetaObj^>^ Deconstruct(MetaObj^ loc, ms::Range<void* const> vs) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ContinuationClo, msgen")]
    ref class ContinuationClo : public Closure
    {
    public:
        MethodFunction^ m_parent;

        ContinuationClo(IlAsm^ il, IlEmit^ mf);
        virtual IlEmit^ DefineFunction(void* f) override;
        virtual void LdVal(IlEmit^ emit) override;
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool HasInstance() override { return false; }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.Ctxt, msgen")]
    ref class Ctxt abstract
    {
    private:
        Ctxt^ m_parent;

    public:
        Ctxt(Ctxt^ parent)
            : m_parent(parent)
        {
        }

        static __declspec(noinline) cli::array<MetaObj^>^ NewCtxtEnv(int sz);

        virtual void EmitBranch(IlEmit^ emit, Closure^ clo, Label lab);
        Ctxt^ Parent();
        Ctxt^ Root();
        virtual MetaObj^ lookup(void* v);
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.CtxtMod, msgen")]
    ref class CtxtMod : public Ctxt
    {
    private:
        IlAsm^ const m_ilasm;
        System::Reflection::Emit::ModuleBuilder^ m_module_builder;
        Hashtable^ m_cache;
        static cli::array<wchar_t>^ m_fid_punctuation;
        const unsigned int m_compilation_options;
        cli::array<MetaObj^>^ m_env;
        int m_envsize;
        Closure^ m_clo;
        IlEmit^ m_update;
        MetaObj^ m_calldepth;

        static CtxtMod()
        {
            m_fid_punctuation = String::Intern(".:")->ToCharArray();
        }

    public:
        [::Quire::Mirror::MirrorStub]
        CtxtMod(IlAsm^ ilasm, unsigned int compilation_options, System::Reflection::Emit::Quire::Mpl::TypeBuilder^)
            : Ctxt(nullptr), m_ilasm(ilasm), m_compilation_options(compilation_options)
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }

        Closure^ ModuleClosure()
        {
            return m_clo;
        }

        MethodInfo^ GetApplyMethod(Type^ return_type, cli::array<Type^>^ parameters);

        [::Quire::Mirror::MirrorStub]
        MetaObj^ ForeignValue(String^)
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlAsm, msgen")]
    ref class IlAsm
    {
    private:
        static bool m_statics_initialized;
        static ::gen::AstInfo* m_info;
        static int m_counter;
        static Type^ m_int_type;
        static Type^ m_dbl_type;
        static Type^ m_obj_type;
        static Type^ m_fcn_type;
        static Type^ m_del_type;
        static Type^ m_tag_type;
        static Type^ m_voidtype;
        static MethodInfo^ m_num_ctor;
        static Constructor^ m_dyn_cons;
        static Constructor^ m_key_cons;
        static cli::array<MetaObj^>^ m_primitive;

        Ctxt^ m_ctxt;
        CtxtMod^ m_mod;
        void* m_handle;
        Hashtable^ m_datatypes;
        IlEmit^ m_emit;

    public:
        [::Quire::Mirror::MirrorStub]
        IlAsm(void*, ::gen::AstInfo*, unsigned int)
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }

        static Type^ IntType();
        static Type^ DblType();
        static Type^ ObjType();
        static Type^ FcnType();
        static Type^ DelegateType();
        static Type^ TagType();
        static Type^ VoidType();
        static System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ GetTypeInitializer(System::Reflection::Emit::Quire::Mpl::TypeBuilder^ tb);
        void Branch(Closure^ clo, Label lab);
        IlEmit^ Emitter();
        IlEmit^ SetEmitter(IlEmit^ emit);
        static MethodInfo^ NumValCtor();
        static String^ FieldName(int i);
        static cli::array<String^>^ FieldNames(int n);
        static String^ NewName();
        static String^ NewName(String^ prefix);
        [::Quire::Mirror::MirrorStub]
        static void Raise(String^)
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }
        static void Raise(Substring<const char> m);
        static [returnvalue: MarshalAs(UnmanagedType::U1)] bool Exported(void* v);
        static const char* Name(void*** d);
        static const char* Name(void** c);
        static String^ Name(void* v);
        static const char* ModuleName(void** c);
        static int Arity(void** c);
        static int GetTag(void** c);
        static int GetSpan(void*** d);
        static void** GetConstructor(void*** d, int i);
        static Constructor^ DynConstructor();
        static Constructor^ KeyConstructor();
        static Type^ GetValueType(void* v);
        static cli::array<Type^>^ GetConParameters(void** c);
        // Temporarily stubbed while isolating Range::drop uint64 temp spill.
        [::Quire::Mirror::MirrorStub]
        static Type^ GetReturnType(Substring<const char> ty, unsigned long long arity);
        static Type^ GetReturnType(void* v, unsigned long long arity);
        static cli::array<Type^>^ GetParameterTypes(MethodBase^ mb, [MarshalAs(UnmanagedType::U1)] bool is_static);
        static cli::array<Type^>^ GetParameterTypes(MethodBase^ mb);
        static cli::array<Type^>^ GetParameterTypes(Substring<const char> ty, unsigned long long arity);
        static cli::array<Type^>^ GetParameterTypes(void* v, unsigned long long arity);
        static Type^ TranslateType(Substring<const char> ty);

        MetaObj^ ForeignValue(String^ fid);
        MetaObj^ ForeignValue(Substring<const char> fid);
        Closure^ ModuleClosure();
        MethodInfo^ GetApplyMethod(Type^ return_type, cli::array<Type^>^ parameters);
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlEmit, msgen")]
    ref class IlEmit abstract : public Callable
    {
    protected:
        Closure^ m_clo;
        System::Reflection::Emit::Quire::Mpl::ILGenerator^ m_il;
        bool m_reachable;
        bool m_tracing;
        bool m_protected;
        int m_current_handle;
        MetaObj^ m_einfo_local;

    public:
        static const bool ProbeStack = true;

        IlEmit(void* f, IlEmit^ e)
            : Callable(e, f), m_clo(e->m_clo), m_il(e->m_il), m_reachable(true),
              m_tracing(e->m_tracing), m_protected(e->m_protected), m_einfo_local(e->m_einfo_local)
        {
        }

        IlEmit(String^ nm, Closure^ clo, System::Reflection::Emit::Quire::Mpl::ILGenerator^ il);
        IlEmit(void* v, Closure^ clo, System::Reflection::Emit::Quire::Mpl::ILGenerator^ il);

        virtual Closure^ Clo() override { return m_clo; }

        void SetIlGenerator(System::Reflection::Emit::Quire::Mpl::ILGenerator^ il)
        {
            m_il = il;
        }

        System::Reflection::Emit::Quire::Mpl::ILGenerator^ GetIlGenerator()
        {
            return m_il;
        }

        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override;
        [returnvalue: MarshalAs(UnmanagedType::U1)] bool HasInstance();
        virtual cli::array<MetaObj^>^ CreateParameterEnv(ms::Range<void* const> args);

        virtual void EmitProlog(IlAsm^, [MarshalAs(UnmanagedType::U1)] bool) {}
        virtual void EmitEpilog(IlAsm^) {}
        virtual void EmitStackProbe(IlAsm^, [MarshalAs(UnmanagedType::U1)] bool) {}
        virtual void EmitReturn();

        [returnvalue: MarshalAs(UnmanagedType::U1)]
        bool Reachable() { return m_reachable; }

        [returnvalue: MarshalAs(UnmanagedType::U1)]
        bool TracingEnabled() { return m_tracing; }

        void EnableTracing([MarshalAs(UnmanagedType::U1)] bool trace) { m_tracing = trace; }

        [returnvalue: MarshalAs(UnmanagedType::U1)]
        bool Protected() { return m_protected; }

        void SetProtected([MarshalAs(UnmanagedType::U1)] bool protect) { m_protected = protect; }

        virtual MethodFunction^ GetMethodFunction();
        System::Reflection::Emit::Quire::Mpl::LocalBuilder^ DefineLocal(void* v);
        System::Reflection::Emit::Quire::Mpl::LocalBuilder^ DefineLocal(Type^ ty);
        Label new_label();
        Label emit_label_ref(OpCode op);
        Label emit_label_ref(OpCode op, Label lab);
        Label emit_label_def();
        Label emit_label_def(Label lab);

        void Ldarg(int index);
        void Starg(int index);
        void Ldloc(int index);
        void Ldloc(System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb);
        void Stloc(int index);
        void Stloc(System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb);
        void Ldloca(int index);
        void Ldsfld(FieldInfo^ fi);
        void Stsfld(FieldInfo^ fi);
        void Ldfld(FieldInfo^ fi);
        void Stfld(FieldInfo^ fi);

        void emit_value_assertion();
        void emit_double(const double val);
        void emit_complex(const double re, const double im);
        void emit_Double(const double re);
        void emit_Int(const int i);
        void emit_word(long long val);
        void emit_word(int val);
        void emit_handle(int val);
        void emit_string(String^ s);
        void emit_return();
        [returnvalue: MarshalAs(UnmanagedType::U1)] bool emit_leave(Label lab);
        [returnvalue: MarshalAs(UnmanagedType::U1)] bool emit_branch(Label lab);
        void emit_raise(const char* msg);
        void emit_raise(String^ msg);
        void emit_rethrow();
        void emit_pattern_match_failure();
        Label emit_begin_exception_block();
        void emit_begin_catch_block(Type^ ty);
        void emit_begin_filter_block();
        void emit_end_exception_block();
        void emit_call(Hex::Tag* einfo, OpCode op, MethodInfo^ mi);
        void emit_tailcall();

        void emit_instr(OpCode op, Type^ ty)
        {
            if (m_reachable)
                m_il->Emit(op, ty);
        }

        void emit_instr(OpCode op, cli::array<Label>^ lab)
        {
            if (m_reachable)
                m_il->Emit(op, lab);
        }

        void emit_instr(OpCode op, ConstructorInfo^ ci)
        {
            if (m_reachable)
                m_il->Emit(op, ci);
        }

        void emit_instr(OpCode op, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ loc)
        {
            if (m_reachable)
                m_il->Emit(op, loc);
        }

        void emit_instr(OpCode op, MethodInfo^ mi)
        {
            if (m_reachable)
                m_il->Emit(op, mi);
        }

        void emit_instr(OpCode op, String^ s)
        {
            if (m_reachable)
                m_il->Emit(op, s);
        }

        void emit_instr(OpCode op, const char* nm);

        void emit_instr(OpCode op, FieldInfo^ fi)
        {
            if (m_reachable)
                m_il->Emit(op, fi);
        }

        void emit_instr(OpCode op, Label lab)
        {
            if (m_reachable)
                m_il->Emit(op, lab);
        }

        void emit_instr(OpCode op, double d)
        {
            if (m_reachable)
                m_il->Emit(op, d);
        }

        void emit_instr(OpCode op, int i)
        {
            if (m_reachable)
                m_il->Emit(op, i);
        }

        void emit_instr(OpCode op, short i)
        {
            if (m_reachable)
                m_il->Emit(op, i);
        }

        void emit_instr(OpCode op, unsigned char i)
        {
            if (m_reachable)
                m_il->Emit(op, i);
        }

        void emit_instr(OpCode op)
        {
            if (m_reachable)
                m_il->Emit(op);
        }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.CctorFunction, msgen")]
    ref class CctorFunction : public IlEmit
    {
    private:
        MetaObj^ m_value;
        System::Reflection::Emit::Quire::Mpl::ConstructorBuilder^ m_ctor;

    public:
        CctorFunction(IlBuilderType^ clo);
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.MethodFunction, msgen")]
    ref class MethodFunction : public IlEmit
    {
    private:
        int m_arity;
        Type^ m_return_type;
        cli::array<Type^>^ m_param_types;

    public:
        MethodFunction(void* f, Closure^ clo);
        MethodFunction(String^ f, Closure^ clo, Type^ ty, cli::array<Type^>^ tys);
        MethodFunction(const char* f, Closure^ clo);
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) override;
        virtual MethodFunction^ GetMethodFunction() override;
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool LdClosureRecord(IlEmit^ emit) override;
        virtual Type^ ClosureType() override;
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual cli::array<MetaObj^>^ CreateParameterEnv(ms::Range<void* const> args) override;
        virtual void EmitStackProbe(IlAsm^ bcasm, [MarshalAs(UnmanagedType::U1)] bool probe) override;
        virtual void DeclareFunction(Hex::Tag* info, ms::Range<void* const> args, Type^ ty, cli::array<Type^>^ tys) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ContinuationFunction, msgen")]
    ref class ContinuationFunction : public IlEmit
    {
    private:
        Label m_label;
        int m_arity;
        Type^ m_return_type;
        cli::array<Type^>^ m_param_types;
        cli::array<MetaObj^>^ m_params;
        MethodFunction^ m_method;
        ContinuationClo^ m_clo;

    public:
        ContinuationFunction(void* f, MethodFunction^ e, ContinuationClo^ clo);
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual cli::array<MetaObj^>^ CreateParameterEnv(ms::Range<void* const> args) override;
        virtual MethodFunction^ GetMethodFunction() override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) override;
        virtual void EmitProlog(IlAsm^ bcasm, [MarshalAs(UnmanagedType::U1)] bool) override;
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual void DeclareFunction(Hex::Tag* info, ms::Range<void* const> args, Type^ ty, cli::array<Type^>^ tys) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ArgVar, msgen")]
    ref class ArgVar : public MetaObj
    {
    private:
        int m_index;

    public:
        ArgVar(String^ nm, int index)
            : MetaObj(nm), m_index(index)
        {
        }

        ArgVar(void* v, int index)
            : MetaObj(v), m_index(index)
        {
        }

        virtual void LdVal(IlEmit^ emit) override;
        virtual void StVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.LocalVar, msgen")]
    ref class LocalVar : public MetaObj
    {
    private:
        System::Reflection::Emit::Quire::Mpl::LocalBuilder^ m_local;

    public:
        LocalVar(String^ nm, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ local);
        LocalVar(void* v, System::Reflection::Emit::Quire::Mpl::LocalBuilder^ local);
        virtual void LdVal(IlEmit^ emit) override;
        virtual void StVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.FieldVar, msgen")]
    ref class FieldVar : public MetaObj
    {
    private:
        MetaObj^ m_record;
        FieldInfo^ m_field;

    public:
        FieldVar(void* v, MetaObj^ rec, FieldInfo^ field)
            : MetaObj(v), m_record(rec), m_field(field)
        {
        }

        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override;
        virtual void LdVal(IlEmit^ emit) override;
        virtual void StVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.StaticField, msgen")]
    ref class StaticField : public MetaObj
    {
    protected:
        FieldInfo^ m_field;

    public:
        StaticField(FieldInfo^ field)
            : MetaObj(), m_field(field)
        {
        }

        StaticField(String^ nm, FieldInfo^ field)
            : MetaObj(nm), m_field(field)
        {
        }

        StaticField(void* v, FieldInfo^ field)
            : MetaObj(v), m_field(field)
        {
        }

        virtual void LdVal(IlEmit^ emit) override;
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override { return true; }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.TopLevelVar, msgen")]
    ref class TopLevelVar : public StaticField
    {
    public:
        TopLevelVar(String^ nm, FieldInfo^ field)
            : StaticField(nm, field)
        {
        }

        TopLevelVar(void* v, FieldInfo^ field)
            : StaticField(v, field)
        {
        }

        virtual void StVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.PropertyVar, msgen")]
    ref class PropertyVar : public MetaObj
    {
    private:
        MetaObj^ m_record;
        Callable^ m_prop_get;
        cli::array<Type^>^ m_get_param;

    public:
        PropertyVar(void* v, MetaObj^ rec, Callable^ prop);
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override;
        virtual void LdVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ElementVar, msgen")]
    ref class ElementVar : public MetaObj
    {
    protected:
        MetaObj^ m_record;
        int m_index;
        Type^ m_unbox;

    public:
        ElementVar(String^ nm, MetaObj^ rec, int index, Type^ unbox)
            : MetaObj(nm), m_record(rec), m_index(index), m_unbox(unbox)
        {
        }

        ElementVar(void* v, MetaObj^ rec, int index, Type^ unbox)
            : MetaObj(v), m_record(rec), m_index(index), m_unbox(unbox)
        {
        }

        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override;
        virtual Type^ RepresentationType() override { return m_unbox; }
        virtual void LdVal(IlEmit^ emit) override;
        virtual void StVal(IlEmit^ emit) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.TopLevelElementVar, msgen")]
    ref class TopLevelElementVar : public ElementVar
    {
    public:
        TopLevelElementVar(void* v, MetaObj^ rec, int index, Type^ unbox)
            : ElementVar(v, rec, index, unbox)
        {
        }

        TopLevelElementVar(String^ nm, MetaObj^ rec, int index, Type^ unbox)
            : ElementVar(nm, rec, index, unbox)
        {
        }

        virtual int Ordinal() override { return m_index; }
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() override { return true; }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlOpCode, msgen")]
    ref class IlOpCode : public Callable
    {
    private:
        cli::array<OpCode>^ m_op;
        ::Quire::Mpl::Run::Code^ m_code;
        int m_arity;

    public:
        IlOpCode(::Quire::Mpl::Run::Primitive^ p);
        virtual int Arity() override { return m_arity; }
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool) override { return m_method; }
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual void BegArg(IlEmit^ emit, int i, int nargs) override;
        virtual void EndArg(IlEmit^ emit, int i, Substring<const char> ty, int nargs) override;
        virtual void EndArg(IlEmit^ emit, int i, Type^ ty, int nargs) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlMethod, msgen")]
    ref class IlMethod : public Callable
    {
    private:
        CallConv m_cc;
        int m_arity;

    public:
        IlMethod(String^ nm, MethodInfo^ mi, CallConv cc);
        IlMethod(::Quire::Mpl::Run::Primitive^ prim);
        virtual int Arity() override { return m_arity; }
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool) override { return m_method; }
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual Object^ Invoke(cli::array<Object^>^ args) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.LateBoundMethod, msgen")]
    ref class LateBoundMethod : public Callable
    {
    private:
        String^ m_foreign;
        Callable^ m_impl;

    public:
        LateBoundMethod(::Quire::Mpl::Run::Primitive^ prim);
        Callable^ GetImpl();
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) override;
        virtual Object^ Invoke(cli::array<Object^>^ args) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ResolvedForeignId, msgen")]
    ref class ResolvedForeignId abstract
    {
    public:
        static char m_wildcard;

        static void InitForeignIdCache(char wildcard)
        {
            if (m_wildcard != wildcard)
            {
                m_wildcard = wildcard;
                ClearCache();
            }
        }

        virtual Object^ LoadField()
        {
            return nullptr;
        }

        static void ClearCache()
        {
            m_fv_cache = gcnew Hashtable();
            s_loaded = gcnew Hashtable(100);
        }

        virtual MetaObj^ CreateMetaObj(IlAsm^ bcasm) = 0;

    protected:
        String^ m_id;

        ResolvedForeignId(String^ id)
            : m_id(id)
        {
        }

        void CacheId()
        {
            String^ id = m_id;
            m_fv_cache->default[id] = this;
        }

    private:
        static Hashtable^ m_fv_cache;
        static Hashtable^ s_loaded;

        static int GobbleWhite(int idx, String^ s)
        {
            while (Char::IsWhiteSpace(s, idx))
            {
                idx++;
            }
            return idx;
        }

        static Type^ ArrayTypeFromElementType(Type^ element)
        {
            if (element == nullptr)
                return nullptr;
            return Array::CreateInstance(element, 0)->GetType();
        }
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ForeignField, msgen")]
    ref class ForeignField : public ResolvedForeignId
    {
    private:
        FieldInfo^ m_fi;

    public:
        ForeignField(String^ id, FieldInfo^ fi)
            : ResolvedForeignId(id), m_fi(fi)
        {
            CacheId();
        }

        virtual MetaObj^ CreateMetaObj(IlAsm^) override;
        virtual Object^ LoadField() override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ForeignMethod, msgen")]
    ref class ForeignMethod : public ResolvedForeignId
    {
    private:
        MethodInfo^ m_mi;
        String^ m_name;
        CallConv m_conv;

    public:
        ForeignMethod(String^ id, String^ name, MethodInfo^ mi, CallConv conv)
            : ResolvedForeignId(id), m_mi(mi), m_name(name), m_conv(conv)
        {
            CacheId();
        }

        ForeignMethod(String^ id, String^ name, MethodInfo^ mi)
            : ResolvedForeignId(id), m_mi(mi), m_name(name), m_conv(CallConv_Default)
        {
            CacheId();
        }

        virtual MetaObj^ CreateMetaObj(IlAsm^) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ForeignCtor, msgen")]
    ref class ForeignCtor abstract : public ResolvedForeignId
    {
    private:
        cli::array<Type^>^ m_sig;
        ConstructorInfo^ m_ci;

    public:
        ForeignCtor(String^ id, cli::array<Type^>^ sig, ConstructorInfo^ ci);
        virtual MetaObj^ CreateMetaObj(IlAsm^ bcasm) override;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.ForeignConstructor, msgen")]
    ref class ForeignConstructor : public Constructor
    {
    private:
        ConstructorInfo^ m_ctor;
        int m_arity;
        cli::array<Type^>^ m_types;

    public:
        ForeignConstructor(IlAsm^ il, String^ mn, String^ tn, ConstructorInfo^ ctor, cli::array<Type^>^ tys);
        virtual int Arity() override;
        virtual Type^ ReturnType() override;
        virtual Type^ RepresentationType() override;
        virtual cli::array<Type^>^ MplParameterTypes() override;
        virtual cli::array<MetaObj^>^ Deconstruct(MetaObj^ loc, ms::Range<void* const> vs) override;
        virtual void CallFunc(IlEmit^ emit, int nargs, [MarshalAs(UnmanagedType::U1)] bool tail, Hex::Tag* einfo, Type^ return_type, cli::array<Type^>^ parameters) override;
        virtual void LdVal(IlEmit^ emit) override;
    };

}}
