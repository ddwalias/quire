using namespace System;
using namespace System::Collections;
using namespace System::Globalization;
using namespace System::Text;
using namespace System::Runtime::InteropServices;

namespace Quire { namespace Engine {

    ref class PCons;
    ref class Reader;
    ref class ReaderMacro;

    [Quire::Mirror::MirrorOfAttribute("Engine.InternFunction, mspcons-mgd")]
    public delegate Object^ InternFunction(String^ s);

    [Quire::Mirror::MirrorOfAttribute("Engine.InternFunctionLabeled, mspcons-mgd")]
    public delegate Object^ InternFunctionLabeled(String^ s, String^ labels, String^ tag);

    [Quire::Mirror::MirrorOfAttribute("Engine.TagFunction, mspcons-mgd")]
    public delegate String^ TagFunction(String^ s);

    [Serializable]
    [Quire::Mirror::MirrorOfAttribute("Engine.ReaderException, mspcons-mgd")]
    public ref class ReaderException : public Exception
    {
    public:
        ReaderException(String^ msg)
            : Exception(msg)
        {
        }
    };



    [Serializable]
    [Quire::Mirror::MirrorOfAttribute("Engine.PCons, mspcons-mgd")]
    public ref class PCons
    {
    public:
        Object^ hd;
        PCons^ tl;

        PCons(Object^ h, PCons^ t)
            : hd(h), tl(t)
        {
        }

        String^ ToString(Hashtable^ seen)
        {
            if (seen->ContainsKey(this))
            {
                return "(%circular)";
            }
            seen->default[this] = true;
            StringBuilder^ stringBuilder = gcnew StringBuilder("(");
            PCons^ pCons = this;
            if (this != nullptr)
            {
                do
                {
                    stringBuilder->Append(_ToString(seen, pCons->hd));
                    if (pCons->tl != nullptr)
                    {
                        stringBuilder->Append(L' ');
                    }
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            stringBuilder->Append(L')');
            seen->Remove(this);
            String^ result = stringBuilder->ToString();
            GC::KeepAlive(this);
            return result;
        }

        virtual String^ ToString() override
        {
            return ToString(gcnew Hashtable());
        }

        cli::array<Object^>^ ToArray(cli::array<Object^>^ a, int offset)
        {
            int num = offset;
            PCons^ pCons = this;
            if (this != nullptr)
            {
                do
                {
                    a[num] = pCons->hd;
                    num++;
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            return a;
        }

        cli::array<Object^>^ ToArray()
        {
            const PCons^ pCons = this;
            int num = 0;
            if (this != nullptr)
            {
                do
                {
                    num++;
                    pCons = pCons->tl;
                }
                while (pCons != nullptr);
            }
            cli::array<Object^>^ array = gcnew cli::array<Object^>(num);
            int num2 = 0;
            PCons^ pCons2 = this;
            if (this != nullptr)
            {
                do
                {
                    array[num2] = pCons2->hd;
                    num2++;
                    pCons2 = pCons2->tl;
                }
                while (pCons2 != nullptr);
            }
            return array;
        }

        virtual bool Equals(Object^ A_0) override
        {
            if (this == A_0)
            {
                return true;
            }
            PCons^ other = dynamic_cast<PCons^>(A_0);
            if (other != nullptr)
            {
                PCons^ cur = this;
                // Idiomatic truthiness test (`other && cur`) preserves Mathcad's loop header instead of MSVC rotating `other != nullptr`.
                while (other && cur)
                {
                    if (other->hd && cur->hd)
                    {
                        if (!other->hd->Equals(cur->hd))
                        {
                            return false;
                        }
                    }
                    else if (other->hd || cur->hd)
                    {
                        return false;
                    }
                    other = other->tl;
                    cur = cur->tl;
                }
                return !other && !cur;
            }
            return false;
        }
        static PCons^ FromArray(cli::array<Object^>^ a, int offset)
        {
            PCons^ pCons = nullptr;
            if (a != nullptr)
            {
                int num = a->Length - 1;
                if (num >= offset)
                {
                    do
                    {
                        pCons = gcnew PCons(a[num], pCons);
                        num--;
                    }
                    while (num >= offset);
                }
            }
            return pCons;
        }

        static int length(const PCons^ c)
        {
            int num = 0;
            if (c != nullptr)
            {
                do
                {
                    num++;
                    c = c->tl;
                }
                while (c != nullptr);
            }
            return num;
        }

        static PCons^ shallowCopy(const PCons^ l)
        {
            PCons^ pCons = nullptr;
            const PCons^ pCons2 = l;
            if (l != nullptr)
            {
                do
                {
                    pCons = gcnew PCons(pCons2->hd, pCons);
                    pCons2 = pCons2->tl;
                }
                while (pCons2 != nullptr);
            }
            PCons^ pCons3 = pCons;
            PCons^ result = nullptr;
            if (pCons != nullptr)
            {
                PCons^ pCons4;
                do
                {
                    pCons4 = pCons3->tl;
                    pCons3->tl = result;
                    result = pCons3;
                    pCons3 = pCons4;
                }
                while (pCons4 != nullptr);
            }
            return result;
        }

        __declspec(noinline) static PCons^ nreverse(PCons^ a)
        {
            PCons^ result = nullptr;
            if (a != nullptr)
            {
                PCons^ pCons;
                do
                {
                    pCons = a->tl;
                    a->tl = result;
                    result = a;
                    a = pCons;
                }
                while (pCons != nullptr);
            }
            return result;
        }

        static PCons^ reverse(const PCons^ a)
        {
            PCons^ pCons = nullptr;
            const PCons^ pCons2 = a;
            if (a != nullptr)
            {
                do
                {
                    pCons = gcnew PCons(pCons2->hd, pCons);
                    pCons2 = pCons2->tl;
                }
                while (pCons2 != nullptr);
            }
            return pCons;
        }

        static String^ _ToString(Hashtable^ seen, Object^ o)
        {
            if (o != nullptr)
            {
                if (seen != nullptr)
                {
                    PCons^ pCons = dynamic_cast<PCons^>(o);
                    if (pCons != nullptr)
                    {
                        return pCons->ToString(seen);
                    }
                }
                String^ arg = dynamic_cast<String^>(o);
                if ((Object^)arg != nullptr)
                {
                    return String::Format("\"{0}\"", arg);
                }
                Double^ valueType = dynamic_cast<Double^>(o);
                if (valueType != nullptr)
                {
                    NumberFormatInfo^ invariantInfo = NumberFormatInfo::InvariantInfo;
                    String^ text = "g";
                    return valueType->ToString(text, invariantInfo);
                }
                return o->ToString();
            }
            return "()";
        }

        static String^ _ToString(Object^ o)
        {
            return _ToString(nullptr, o);
        }
    };

    [Quire::Mirror::MirrorOfAttribute("Engine.ReadFunction, mspcons-mgd")]
    public delegate Object^ ReadFunction(Reader^ r, ::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf, int ch);

    [Serializable]
    [Quire::Mirror::MirrorOfAttribute("Engine.ReaderMacro, mspcons-mgd")]
    public ref class ReaderMacro
    {
    public:
        ReadFunction^ function;
        bool is_terminating;

        ReaderMacro(ReadFunction^ f, [MarshalAs(UnmanagedType::U1)] bool t)
            : function(f), is_terminating(t)
        {
        }
    };

    [Serializable]
    [Quire::Mirror::MirrorOfAttribute("Engine.Reader, mspcons-mgd")]
    public ref class Reader : public IDisposable
    {
    private:
        InternFunction^ m_intern_func;
        InternFunctionLabeled^ m_intern_func_labeled;
        cli::array<bool>^ m_macro_places;
        System::Collections::Generic::Dictionary<int, ReaderMacro^>^ m_macro_table;

        Object^ read_symbol_or_number(::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf)
        {
            String^ labels = nullptr;
            String^ tag = nullptr;
            bool done = false;
            bool hasChars = false;
            StringBuilder^ sb = gcnew StringBuilder();
            while (!done)
            {
                int ch = t->Read();
                if (Char::IsWhiteSpace((wchar_t)ch) || ch == -1)
                {
                    done = true;
                }
                else
                {
                    switch (ch)
                    {
                    case '{':
                        if (tag == nullptr && hasChars)
                        {
                            labels = read_labels(t, tf);
                            break;
                        }
                        throw gcnew ReaderException("misplaced labels");
                    case ':':
                        if (hasChars)
                        {
                            tag = read_tag(t, tf);
                            done = true;
                            break;
                        }
                        [[fallthrough]];
                    default:
                        hasChars = true;
                        if (ch < 256 && m_macro_places[ch])
                        {
                            ReaderMacro^ macro = m_macro_table[ch];
                            if (macro != nullptr && macro->is_terminating)
                            {
                                t->Unread(ch);
                                done = true;
                                break;
                            }
                        }
                        sb->Append((wchar_t)ch);
                        break;
                    }
                }
            }
            String^ s = sb->ToString();
            if ((labels == nullptr && tag == nullptr) || m_intern_func_labeled == nullptr)
            {
                int^ value = ::Quire::Engine::StringUtils::ParseInt(s);
                if (value != nullptr)
                {
                    GC::KeepAlive(this);
                    return value;
                }
                Object^ result = m_intern_func(s);
                GC::KeepAlive(this);
                return result;
            }
            Object^ labeledResult = m_intern_func_labeled(s, labels, tag);
            GC::KeepAlive(this);
            return labeledResult;
        }

        Object^ read_string(::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf)
        {
            String^ text = nullptr;
            String^ text2 = nullptr;
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            int num = t->Read();
            if (num != 34)
            {
                do
                {
                    if (num == -1)
                    {
                        throw gcnew ReaderException("unterminated string");
                    }
                    if (num == 92)
                    {
                        stringBuilder->Append(L'\\');
                        num = t->Read();
                        if (num == -1)
                        {
                            throw gcnew ReaderException("unterminated string");
                        }
                    }
                    stringBuilder->Append((wchar_t)num);
                    num = t->Read();
                }
                while (num != 34);
            }
            int num2 = t->Read();
            if (num2 == 123)
            {
                text = read_labels(t, tf);
                int num3 = t->Read();
                if (num3 == 58)
                {
                    text2 = read_tag(t, tf);
                }
                else
                {
                    t->Unread(num3);
                }
            }
            else if (num2 == 58)
            {
                text2 = read_tag(t, tf);
            }
            else
            {
                t->Unread(num2);
            }
            if ((text == nullptr && text2 == nullptr) || m_intern_func_labeled == nullptr)
            {
                return stringBuilder->ToString();
            }
            return m_intern_func_labeled("\"" + stringBuilder->ToString() + "\"", text, text2);
        }

        String^ read_labels(::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            stringBuilder->Append(L'{');
            int num = t->Read();
            if (num != 125)
            {
                do
                {
                    switch (num)
                    {
                    case 32:
                        throw gcnew ReaderException("bad RL syntax: space in label");
                    case -1:
                        throw gcnew ReaderException("unterminated label");
                    }
                    stringBuilder->Append((wchar_t)num);
                    num = t->Read();
                }
                while (num != 125);
            }
            stringBuilder->Append((wchar_t)num);
            String^ result = stringBuilder->ToString();
            GC::KeepAlive(this);
            return result;
        }

        String^ read_tag(::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf)
        {
            StringBuilder^ stringBuilder = gcnew StringBuilder();
            int num = t->Read();
            if (!Char::IsWhiteSpace((wchar_t)num))
            {
            loop_head:
                if (num != -1)
                {
                    if (num < 256 && m_macro_places[num])
                    {
                        ReaderMacro^ readerMacro = m_macro_table[num];
                        if (readerMacro != nullptr && readerMacro->is_terminating)
                        {
                            t->Unread(num);
                            // Release IL requires the terminator branch to share the loop's exit block.
                            goto loop_end;
                        }
                    }
                    stringBuilder->Append((wchar_t)num);
                    num = t->Read();
                    if (!Char::IsWhiteSpace((wchar_t)num))
                    {
                        // Release IL requires the character test to branch back to the loop head.
                        goto loop_head;
                    }
                }
            loop_end:;
            }
            String^ text = stringBuilder->ToString();
            return ((Object^)tf != nullptr) ? tf(text) : text;
        }

    public:
        Reader(InternFunction^ f, InternFunctionLabeled^ lf)
            : m_intern_func(f),
              m_intern_func_labeled(lf),
              m_macro_places(gcnew cli::array<bool>(256)),
              m_macro_table(gcnew System::Collections::Generic::Dictionary<int, ReaderMacro^>())
        {
            ReadFunction^ f2 = gcnew ReadFunction(this, &Reader::read_list);
            add_reader_macro(40, gcnew ReaderMacro(f2, true));
            add_reader_macro(41, gcnew ReaderMacro(nullptr, true));
        }

        ~Reader()
        {
        }

        void add_reader_macro(int ch, ReadFunction^ f, [MarshalAs(UnmanagedType::U1)] bool is_terminating)
        {
            add_reader_macro(ch, gcnew ReaderMacro(f, is_terminating));
        }

        void add_reader_macro(int ch, ReaderMacro^ m)
        {
            if (ch < 0 || ch > 255)
            {
                throw gcnew ReaderException("bad reader macro index");
            }
            m_macro_places[ch] = true;
            m_macro_table->Add(ch, m);
        }

        Object^ read(::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf)
        {
            int num = eat_white(t);
            if (num == -1)
            {
                return nullptr;
            }
            if (num == 34)
            {
                return read_string(t, tf);
            }
            if (num < 256 && m_macro_places[num])
            {
                ReaderMacro^ readerMacro = m_macro_table[num];
                GC::KeepAlive(this);
                return readerMacro->function(this, t, tf, num);
            }
            t->Unread(num);
            GC::KeepAlive(this);
            return read_symbol_or_number(t, tf);
        }

        int eat_white(::Quire::Native::EngutilsMgd::PushbackReader^ t)
        {
            int num = t->Read();
            while (true)
            {
                if (!Char::IsWhiteSpace((wchar_t)num))
                {
                    if (num != 59)
                    {
                        break;
                    }
                }
                else if (num != 59)
                {
                    // Release IL requires whitespace to jump directly to the shared next-read block.
                    goto next_read;
                }
                while (num != -1 && (num = t->Read()) != 10)
                {
                }
            next_read:
                num = t->Read();
            }
            return num;
        }

        Object^ read_list(Reader^ r, ::Quire::Native::EngutilsMgd::PushbackReader^ t, TagFunction^ tf, int ch)
        {
            PCons^ pCons = nullptr;
            int num = r->eat_white(t);
            if (num != 41)
            {
                do
                {
                    t->Unread(num);
                    pCons = gcnew PCons(r->read(t, tf), pCons);
                    num = r->eat_white(t);
                }
                while (num != 41);
            }
            return PCons::nreverse(pCons);
        }
    };

}}
