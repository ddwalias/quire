#include "MsgenTypes.h"

namespace Quire { namespace gen {

    void IlEmit::EmitReturn()
    {
        emit_return();
    }

    System::Reflection::Emit::Quire::Mpl::LocalBuilder^ IlEmit::DefineLocal(Type^ ty)
    {
        return (ty == IlAsm::VoidType()) ? nullptr : m_il->DeclareLocal(ty, false);
    }

    Label IlEmit::new_label()
    {
        return m_il->DefineLabel();
    }

    Label IlEmit::emit_label_ref(OpCode op)
    {
        Label lab = new_label();
        emit_instr(op, lab);
        return lab;
    }

    Label IlEmit::emit_label_ref(OpCode op, Label lab)
    {
        emit_instr(op, lab);
        return lab;
    }

    Label IlEmit::emit_label_def()
    {
        Label lab = new_label();
        m_il->MarkLabel(lab);
        m_reachable = true;
        return lab;
    }

    Label IlEmit::emit_label_def(Label lab)
    {
        m_il->MarkLabel(lab);
        m_reachable = true;
        return lab;
    }

    void IlEmit::Ldarg(int index)
    {
        switch (index)
        {
        case 0:
            m_il->Emit(OpCodes::Ldarg_0);
            break;
        case 1:
            m_il->Emit(OpCodes::Ldarg_1);
            break;
        case 2:
            m_il->Emit(OpCodes::Ldarg_2);
            break;
        case 3:
            m_il->Emit(OpCodes::Ldarg_3);
            break;
        default:
            if (index > -128 && index < 127)
                m_il->Emit(OpCodes::Ldarg_S, static_cast<unsigned char>(index));
            else
                m_il->Emit(OpCodes::Ldarg, static_cast<short>(index));
            break;
        }
    }

    void IlEmit::Starg(int index)
    {
        if (index > -128 && index < 127)
            m_il->Emit(OpCodes::Starg_S, static_cast<unsigned char>(index));
        else
            m_il->Emit(OpCodes::Starg, static_cast<short>(index));
    }

    void IlEmit::Ldloc(int index)
    {
        switch (index)
        {
        case 0:
            m_il->Emit(OpCodes::Ldloc_0);
            break;
        case 1:
            m_il->Emit(OpCodes::Ldloc_1);
            break;
        case 2:
            m_il->Emit(OpCodes::Ldloc_2);
            break;
        case 3:
            m_il->Emit(OpCodes::Ldloc_3);
            break;
        default:
            if (index > -128 && index < 127)
                m_il->Emit(OpCodes::Ldloc_S, static_cast<unsigned char>(index));
            else
                m_il->Emit(OpCodes::Ldloc, static_cast<short>(index));
            break;
        }
    }

    void IlEmit::Ldloc(System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb)
    {
        emit_instr(OpCodes::Ldloc, lb);
    }

    void IlEmit::Stloc(int index)
    {
        switch (index)
        {
        case 0:
            m_il->Emit(OpCodes::Stloc_0);
            break;
        case 1:
            m_il->Emit(OpCodes::Stloc_1);
            break;
        case 2:
            m_il->Emit(OpCodes::Stloc_2);
            break;
        case 3:
            m_il->Emit(OpCodes::Stloc_3);
            break;
        default:
            if (index > -128 && index < 127)
                m_il->Emit(OpCodes::Stloc_S, static_cast<unsigned char>(index));
            else
                m_il->Emit(OpCodes::Stloc, static_cast<short>(index));
            break;
        }
    }

    void IlEmit::Stloc(System::Reflection::Emit::Quire::Mpl::LocalBuilder^ lb)
    {
        emit_instr(OpCodes::Stloc, lb);
    }

    void IlEmit::Ldloca(int index)
    {
        if (index < 127)
            m_il->Emit(OpCodes::Ldloca_S, static_cast<unsigned char>(index));
        else
            m_il->Emit(OpCodes::Ldloca, static_cast<short>(index));
    }

    void IlEmit::Ldsfld(FieldInfo^ fi)
    {
        emit_instr(OpCodes::Ldsfld, fi);
    }

    void IlEmit::Stsfld(FieldInfo^ fi)
    {
        emit_instr(OpCodes::Stsfld, fi);
    }

    void IlEmit::Ldfld(FieldInfo^ fi)
    {
        emit_instr(OpCodes::Ldfld, fi);
    }

    void IlEmit::Stfld(FieldInfo^ fi)
    {
        emit_instr(OpCodes::Stfld, fi);
    }

    void IlEmit::emit_double(const double val)
    {
        emit_instr(OpCodes::Ldc_R8, val);
    }

    void IlEmit::emit_complex(const double re, const double im)
    {
        emit_double(re);
        emit_double(im);
        emit_instr(OpCodes::Call, IlAsm::NumValCtor());
    }

    void IlEmit::emit_Double(const double re)
    {
        emit_double(re);
        emit_instr(OpCodes::Box, IlAsm::DblType());
    }

    void IlEmit::emit_Int(const int i)
    {
        emit_word(i);
        emit_instr(OpCodes::Box, IlAsm::IntType());
    }

    void IlEmit::emit_word(long long val)
    {
        m_il->Emit(OpCodes::Ldc_I8, val);
    }

    void IlEmit::emit_word(int val)
    {
        switch (val)
        {
        case 0:
            emit_instr(OpCodes::Ldc_I4_0);
            break;
        case 1:
            emit_instr(OpCodes::Ldc_I4_1);
            break;
        case 2:
            emit_instr(OpCodes::Ldc_I4_2);
            break;
        case 3:
            emit_instr(OpCodes::Ldc_I4_3);
            break;
        case 4:
            emit_instr(OpCodes::Ldc_I4_4);
            break;
        case 5:
            emit_instr(OpCodes::Ldc_I4_5);
            break;
        case 6:
            emit_instr(OpCodes::Ldc_I4_6);
            break;
        case 7:
            emit_instr(OpCodes::Ldc_I4_7);
            break;
        case 8:
            emit_instr(OpCodes::Ldc_I4_8);
            break;
        default:
            if (val > -128 && val < 127)
                emit_instr(OpCodes::Ldc_I4_S, static_cast<unsigned char>(val));
            else
                emit_instr(OpCodes::Ldc_I4, val);
            break;
        }
        GC::KeepAlive(this);
    }

    void IlEmit::emit_handle(int val)
    {
        emit_word(val);
    }

    void IlEmit::emit_string(String^ s)
    {
        emit_instr(OpCodes::Ldstr, s);
    }

    void IlEmit::emit_return()
    {
        emit_instr(OpCodes::Ret);
        m_reachable = false;
    }

    bool IlEmit::emit_leave(Label lab)
    {
        bool prev = m_reachable;
        emit_instr(OpCodes::Leave, lab);
        m_reachable = false;
        return prev;
    }

    bool IlEmit::emit_branch(Label lab)
    {
        bool prev = m_reachable;
        emit_instr(OpCodes::Br, lab);
        m_reachable = false;
        return prev;
    }

    void IlEmit::emit_raise(const char* msg)
    {
        emit_raise(::gen::ToString(Substring<const char>(msg)));
    }

    void IlEmit::emit_raise(String^ msg)
    {
        emit_string(msg);
        emit_instr(OpCodes::Newobj, ::Quire::Mpl::Run::Rts::ExceptionCtor());
        emit_instr(OpCodes::Throw);
        m_reachable = false;
    }

    void IlEmit::emit_rethrow()
    {
        emit_instr(OpCodes::Newobj, ::Quire::Mpl::Run::Rts::RethrowCtor());
        emit_instr(OpCodes::Throw);
        m_reachable = false;
    }

    void IlEmit::emit_pattern_match_failure()
    {
        emit_raise("pattern match exception");
    }

    Label IlEmit::emit_begin_exception_block()
    {
        return m_il->BeginExceptionBlock();
    }

    void IlEmit::emit_begin_catch_block(Type^ ty)
    {
        m_il->BeginCatchBlock(ty);
        m_reachable = true;
    }

    void IlEmit::emit_begin_filter_block()
    {
        m_il->BeginExceptFilterBlock();
        m_reachable = true;
    }

    void IlEmit::emit_end_exception_block()
    {
        m_il->EndExceptionBlock();
        m_reachable = true;
    }

    void IlEmit::emit_tailcall()
    {
        if (!m_protected)
        {
            emit_instr(OpCodes::Tailcall);
        }
    }

    void IlEmit::emit_instr(OpCode op, const char* nm)
    {
        emit_instr(op, ::gen::ToString(Substring<const char>(nm)));
    }

}}
