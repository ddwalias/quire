#include "MsgenTypes.h"

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

}}
