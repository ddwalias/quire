#include "Msclr.h"

using namespace System;

namespace Quire { namespace Mpl {

    using ::Mpl::Loaded;

    [Quire::Mirror::MirrorOfAttribute("Mpl.IRegion, msclr")]
    public ref class IRegion abstract
    {
    public:
        [Quire::Mirror::MirrorOfAttribute("Mpl.IRegion+Symbol, msclr")]
        value struct Symbol
        {
            String^ name;
            String^ type;
            UInt64 ordinal;
        };

        virtual array<Symbol>^ GetImports() = 0;
        virtual array<Symbol>^ GetExports() = 0;
        virtual void Evaluate(array<Object^>^ i, array<Object^>^ e) = 0;

        static IRegion^ GetRegion(Hex::Module&);
    };

    [Quire::Mirror::MirrorOfAttribute("Mpl.Region, msclr")]
    ref class Region : IRegion
    {
    public:
        typedef Run::FV<array<Object^>^, array<Object^>^> Loader;

        Region(Loader^ loader, ms::Range<const Hex::Signature> imports, ms::Range<const Hex::Signature> exports)
            : m_ldr(loader), m_imp(NewSymbols(imports)), m_exp(NewSymbols(exports))
        {
        }

        virtual array<Symbol>^ GetImports() override { return m_imp; }
        virtual array<Symbol>^ GetExports() override { return m_exp; }
        virtual void Evaluate(array<Object^>^ i, array<Object^>^ e) override { m_ldr(i, e); }

    private:
        static array<Symbol>^ NewSymbols(ms::Range<const Hex::Signature> signatures)
        {
            array<Symbol>^ symbols = gcnew array<Symbol>(static_cast<int>(signatures.size()));
            int count = symbols->Length;
            for (int i = 0; i != count; ++i)
            {
                symbols[i].name = ::Mpl::ToString(signatures[i].GetName());
                symbols[i].type = ::Mpl::ToString(signatures[i].GetType());
                symbols[i].ordinal = i;
            }
            return symbols;
        }

        Loader^ const m_ldr;
        array<Symbol>^ m_imp;
        array<Symbol>^ m_exp;
    };

    IRegion^ IRegion::GetRegion(Hex::Module& module)
    {
        Loaded* state = ::Mpl::GetModuleState(module);
        return gcnew Region(::Mpl::unpack<Region::Loader>(state->GetLoader()), state->GetImportSignatures(), state->GetExportSignatures());
    }
}}
