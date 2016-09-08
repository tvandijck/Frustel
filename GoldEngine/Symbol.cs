using System.Linq;
using System.Text.RegularExpressions;

namespace GoldEngine
{
    public class Symbol
    {
        // Fields
        internal Group Group;
        private string m_name;
        private short m_tableIndex;
        private SymbolType m_type;

        // Methods
        internal Symbol()
        {
            m_name = "";
            m_type = SymbolType.Content;
            m_tableIndex = -1;
        }

        internal Symbol(string name, SymbolType type)
        {
            m_name = name;
            m_type = type;
            m_tableIndex = -1;
        }

        internal Symbol(string name, SymbolType type, short tableIndex)
        {
            m_name = name;
            m_type = type;
            m_tableIndex = tableIndex;
        }

        internal bool IsEqualTo(Symbol sym)
        {
            return ((Operators.CompareString(m_name.ToUpper(), sym.Name.ToUpper(), true) == 0) & (m_type == sym.Type));
        }

        public string LiteralFormat(string source, bool alwaysDelimit)
        {
            if (Operators.CompareString(source, "'", true) == 0)
            {
                return "''";
            }
            if (alwaysDelimit)
            {
                return ("'" + source + "'");
            }
            bool flag = false;
            for (short i = 0; (i < Enumerable.Count<char>(source)) & !flag; i = (short)(i + 1))
            {
                flag = !char.IsLetter(source[i]);
            }
            if (flag)
            {
                return ("'" + source + "'");
            }
            return source;
        }

        internal void SetTableIndex(short value)
        {
            m_tableIndex = value;
        }

        public string Text(bool alwaysDelimitTerminals = false)
        {
            switch (m_type)
            {
            case SymbolType.Nonterminal:
                return ("<" + Name + ">");

            case SymbolType.End:
            case SymbolType.Error:
                return ("(" + Name + ")");
            }
            return LiteralFormat(Name, alwaysDelimitTerminals);
        }

        public string TypeName()
        {
            switch (m_type)
            {
            case SymbolType.Nonterminal:
                return "Nonterminal";

            case SymbolType.Content:
                return "Content";

            case SymbolType.Noise:
                return "Noise";

            case SymbolType.End:
                return "End of File";

            case SymbolType.GroupStart:
                return "Lexical Group Start";

            case SymbolType.GroupEnd:
                return "Lexical Group End";

            case SymbolType.LEGACYCommentLine:
                return "Comment Line (LEGACY)";

            case SymbolType.Error:
                return "Runtime Error Symbol";
            }
            return "";
        }

        // Properties
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }

        public short TableIndex
        {
            get
            {
                return m_tableIndex;
            }
        }

        public SymbolType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                m_type = value;
            }
        }
    }
}