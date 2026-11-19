using namespace System;
using namespace System::Collections;

namespace Quire { namespace gen {

    ref class IlAsm;
    ref class MetaObj;

    [Quire::Mirror::MirrorOfAttribute("gen.ResolvedForeignId, msgen")]
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
