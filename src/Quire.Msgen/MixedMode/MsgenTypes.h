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
    struct AstInfo;

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

        MetaObj(void* v);

        MetaObj()
            : m_var(nullptr), m_name("anonymous")
        {
        }

    public:
        virtual int maxargs() { return 16; }
        virtual int Arity() { return 0; }
        virtual [returnvalue: MarshalAs(UnmanagedType::U1)] bool ModuleScope() { return false; }
        virtual void SetInstance(MetaObj^) {}
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

        virtual MethodInfo^ GetMethod([MarshalAs(UnmanagedType::U1)] bool may_emit) = 0;
        virtual Type^ ReturnType() = 0;
        virtual cli::array<Type^>^ MplParameterTypes() = 0;
        virtual int Arity() override = 0;
    };

    [::Quire::Mirror::MirrorOfAttribute("gen.IlAsm, msgen")]
    ref class IlAsm
    {
    private:
        static bool m_statics_initialized;
        static AstInfo* m_info;
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
        IlAsm(void*, AstInfo*, unsigned int)
        {
            throw ::Quire::Mirror::MirrorStub::NotImplemented();
        }

        static Type^ IntType();
        static Type^ DblType();
        static Type^ ObjType();
        static Type^ FcnType();
        static Type^ TagType();
        static Type^ VoidType();
        static MethodInfo^ NumValCtor();
        static cli::array<Type^>^ GetParameterTypes(MethodBase^ mb, [MarshalAs(UnmanagedType::U1)] bool is_static);
        static cli::array<Type^>^ GetParameterTypes(MethodBase^ mb);
        static Type^ TranslateType(Substring<const char> ty);
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

        void SetIlGenerator(System::Reflection::Emit::Quire::Mpl::ILGenerator^ il)
        {
            m_il = il;
        }

        System::Reflection::Emit::Quire::Mpl::ILGenerator^ GetIlGenerator()
        {
            return m_il;
        }

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

}}
